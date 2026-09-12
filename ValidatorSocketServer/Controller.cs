using EcoCoinSharedTypes;
using GenericDataAccessClassCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorSocketServer
{
    public class Controller
    {
        internal static List<Validator> ValidatorConnections = new List<Validator>();
        internal static List<WebServer> ServerConnections = new List<WebServer>();
        internal static List<TransactionRequestEnvelope> ActiveTransactionRequests = new List<TransactionRequestEnvelope>();
        internal static List<TransactionRequestEnvelope> CompletedTransactionRequests = new List<TransactionRequestEnvelope>();
        public ValidatorConnections ValidatorConnectionsManager;
        public WebServerBroadcastConnections WebServerConnectionsManager;

        //TODO: Add timer that ends transaction requests after 10 seconds have passed no matter if all validators have responded or not, or if it hasn't reached 100 responses yet.

        public Controller()
        {
            var Settings = (JObject.Parse(System.IO.File.ReadAllText(AppContext.BaseDirectory + "appconfig.json")));

            if (Settings["Environment"].ToString() == "TEST")
            {
                Console.WriteLine("Running in: TEST");
                EcoCoinSharedTypes.GlobalVars.EnvironmentType = EnvironmentType.Test;
            }
            else
            {
                Console.WriteLine("Running in: PROD");
                EcoCoinSharedTypes.GlobalVars.EnvironmentType = EnvironmentType.Production;
            }

            if (EcoCoinSharedTypes.GlobalVars.EnvironmentType == EnvironmentType.Production)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    EcoCoinSharedTypes.GlobalVars.AccountStoragePath = "G:/EcoCoinData/ChainData/Accounts/";
                    EcoCoinSharedTypes.GlobalVars.ECORootStoragePath = "G:/EcoCoinData/";
                }
                else
                {
                    EcoCoinSharedTypes.GlobalVars.AccountStoragePath = "C:/EcoCoinData/ChainData/Accounts/";
                    EcoCoinSharedTypes.GlobalVars.ECORootStoragePath = "C:/EcoCoinData/";
                }
                EcoCoinSharedTypes.GlobalVars.AEOfficialServerAccount = Guid.Parse("086e33a8-d884-4b6f-ac37-5afd81091807");
                EcoCoinSharedTypes.GlobalVars.AEAccountCreationAccount = Guid.Parse("a6479df0-445a-4376-b3ed-6dd89fc51cf9");
                EcoCoinSharedTypes.GlobalVars.LocalSigningKeyID = 0;
            }
            else
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    EcoCoinSharedTypes.GlobalVars.AccountStoragePath = "G:/EcoCoinDataTest/ChainData/Accounts/";
                    EcoCoinSharedTypes.GlobalVars.ECORootStoragePath = "G:/EcoCoinDataTest/";
                }
                else
                {
                    EcoCoinSharedTypes.GlobalVars.AccountStoragePath = "C:/EcoCoinDataTest/ChainData/Accounts/";
                    EcoCoinSharedTypes.GlobalVars.ECORootStoragePath = "C:/EcoCoinDataTest/";
                }
                EcoCoinSharedTypes.GlobalVars.AEOfficialServerAccount = Guid.Parse("43cfec16-6306-4a13-8b63-b6fbcd3f96af");
                EcoCoinSharedTypes.GlobalVars.AEAccountCreationAccount = Guid.Parse("54b94908-6924-4b0b-9b1b-b4818184acc1");
                EcoCoinSharedTypes.GlobalVars.LocalSigningKeyID = 0;
            }

            ValidatorConnectionsManager = new ValidatorConnections();
            WebServerConnectionsManager = new WebServerBroadcastConnections();
        }

        internal static void BroadcastTransactionRequestToValidators(TransactionRequestEnvelope TRE)
        {
            ActiveTransactionRequests.Add(TRE);

            TransactionRequestEnvelope SendTRE = GlobalFunctions.DeepCopy(TRE);

            SQLParameterCollection Params = new SQLParameterCollection();

            Params.AddParameter("TransactionRequestID", TRE.Request.TransactionID);
            Params.AddParameter("Status", TransactionStatus.Validating.ToString());

            GlobalVars.DB.DBUpdate("UPDATE TransactionRequests SET Status = @Status WHERE TransactionRequestID = @TransactionRequestID", Params);

            //if there are less than 150 validators, send to all validators
            if (ValidatorConnections.Count <= 150)
            {
                TRE.Request.IssuedValidatorCount = ValidatorConnections.Count;
                foreach (Validator V in ValidatorConnections)
                {
                    byte[] data = GlobalFunctions.SerializeObjectToByteArray(SendTRE);

                    V.Send(data);
                }
            }
            else
            {
                List<Validator> LocalFullValidatorList = new List<Validator>(ValidatorConnections);

                //if there are more than 150 validators, use diversity algorithm
                List<Validator> ValidatorsToSendTo = new List<Validator>();

                //get first random validator
                Random rand = new Random();
                int randomIndex = rand.Next(ValidatorConnections.Count);
                ValidatorsToSendTo.Add(ValidatorConnections[randomIndex]);
                LocalFullValidatorList.Remove(ValidatorConnections[randomIndex]);

                Dictionary<string, int> Countries = new Dictionary<string, int>();
                Dictionary<string, int> Regions = new Dictionary<string, int>();
                Dictionary<string, int> Cities = new Dictionary<string, int>();
                Dictionary<string, int> ISPs = new Dictionary<string, int>();
                Dictionary<bool, int> Mobile = new Dictionary<bool, int>();

                AddToStringDiversityMap(Countries, ValidatorsToSendTo[0].Country);
                AddToStringDiversityMap(Regions, ValidatorsToSendTo[0].RegionName);
                AddToStringDiversityMap(Cities, ValidatorsToSendTo[0].City);
                AddToStringDiversityMap(ISPs, ValidatorsToSendTo[0].ISP);
                AddToBoolDiversityMap(Mobile, ValidatorsToSendTo[0].Mobile);

                //get 149 more validators using diversity algorithm
                while (ValidatorsToSendTo.Count < 150)
                {
                    Validator CurrentBestValidator = null;
                    int CurrentBestScore = -1;
                    foreach (Validator V in LocalFullValidatorList)
                    {
                        int CountryCount = Countries.ContainsKey(V.Country) ? Countries[V.Country] * 3 : 0;
                        int RegionCount = Regions.ContainsKey(V.RegionName) ? Regions[V.RegionName] * 2 : 0;
                        int CityCount = Cities.ContainsKey(V.City) ? Cities[V.City] : 0;
                        int ISPCount = ISPs.ContainsKey(V.ISP) ? ISPs[V.ISP] * 2 : 0;
                        int MobileCount = Mobile.ContainsKey(V.Mobile) ? Mobile[V.Mobile] * 2 : 0;
                        int AccountIDCount = 0;
                        if (ValidatorsToSendTo.FindAll(x => x.IPAddress == V.IPAddress).Count() == 0)
                        {
                            AccountIDCount = 15;
                        }
                        //calculate diversity score
                        int DiversityScore = CountryCount + RegionCount + CityCount + ISPCount + MobileCount + AccountIDCount;
                        //if new diversity score is lower than the previous, then save it as the new best
                        if (CurrentBestScore == -1 || DiversityScore < CurrentBestScore)
                        {
                            CurrentBestScore = DiversityScore;
                            CurrentBestValidator = V;
                        }
                    }

                    ValidatorsToSendTo.Add(CurrentBestValidator);
                    LocalFullValidatorList.Remove(CurrentBestValidator);
                    AddToStringDiversityMap(Countries, CurrentBestValidator.Country);
                    AddToStringDiversityMap(Regions, CurrentBestValidator.RegionName);
                    AddToStringDiversityMap(Cities, CurrentBestValidator.City);
                    AddToStringDiversityMap(ISPs, CurrentBestValidator.ISP);
                    AddToBoolDiversityMap(Mobile, CurrentBestValidator.Mobile);
                }

                //send to those selected validators
                foreach (Validator V in ValidatorsToSendTo)
                {
                    TRE.Request.IssuedValidatorCount = ValidatorsToSendTo.Count;
                    byte[] data = GlobalFunctions.SerializeObjectToByteArray(SendTRE);
                    V.Send(data);
                }
            }
        }

        #region ValidatorDiversity
        private static void AddToStringDiversityMap(Dictionary<string, int> DiversityMap, string Key)
        {
            if (DiversityMap.ContainsKey(Key))
            {
                DiversityMap[Key]++;
            }
            else
            {
                DiversityMap.Add(Key, 1);
            }
        }

        private static void AddToBoolDiversityMap(Dictionary<bool, int> DiversityMap, bool Key)
        {
            if (DiversityMap.ContainsKey(Key))
            {
                DiversityMap[Key]++;
            }
            else
            {
                DiversityMap.Add(Key, 1);
            }
        }
        #endregion

        internal static void ProcessValidatorResponse(TransactionValidationResponseEnvelope TVRE, Validator SourceValidator)
        {
            TransactionRequestEnvelope TRE = GetActiveRequest(TVRE.ValidationResponse);
            if (TRE != null)
            {
                //Found transaction request


                Console.WriteLine("    Round trip speed: " + (DateTime.UtcNow - TRE.Request.TransactionStartDate).TotalSeconds + " Seconds");

                //Validate the response from the validator
                if (VerifyValidatorHasPrimaryVerifiedPerson(TVRE))
                {
                    if (VerifyTransactionValidationResponseSignature(TVRE))
                    {
                        //once the signature has been verified, mark the validator socket object with the validator's ID
                        SourceValidator.ValidatorID = TVRE.ValidatorID;

                        if (TRE.Request.TransactionLatticeEntry == null)
                        {
                            TRE.Request.TransactionLatticeEntry = new LatticeEntry(TRE.Request);
                        }

                        if (TVRE.ValidationResponse.Approved)
                        {
                            TRE.Request.TransactionLatticeEntry.ApprovingValidators.Add(TVRE.ValidatorID);
                        }
                        else
                        {
                            TRE.Request.TransactionLatticeEntry.DisapprovingValidators.Add(TVRE.ValidatorID);
                        }


                        SQLParameterCollection Params = new SQLParameterCollection();

                        Params.AddParameter("TransactionRequestID", TRE.Request.TransactionID);
                        Params.AddParameter("ApprovingValidators", TRE.Request.TransactionLatticeEntry.ApprovingValidators.Count);
                        Params.AddParameter("DisapprovingValidators", TRE.Request.TransactionLatticeEntry.DisapprovingValidators.Count);
                        Params.AddParameter("Status", TransactionStatus.Validating.ToString());

                        GlobalVars.DB.DBUpdate("UPDATE TransactionRequests SET ApproveValidatorCount = @ApprovingValidators, DenyValidatorCount = @DisapprovingValidators, Status = @Status WHERE TransactionRequestID = @TransactionRequestID", Params);

                        Console.WriteLine("    Approvers: " + TRE.Request.TransactionLatticeEntry.ApprovingValidators.Count.ToString() + " Deniers: " + TRE.Request.TransactionLatticeEntry.DisapprovingValidators.Count.ToString() + " Validators Issued: " + TRE.Request.IssuedValidatorCount.ToString());


                        if (TRE.Request.TransactionLatticeEntry.ApprovingValidators.Count + TRE.Request.TransactionLatticeEntry.DisapprovingValidators.Count == TRE.Request.IssuedValidatorCount || TRE.Request.TransactionLatticeEntry.ApprovingValidators.Count + TRE.Request.TransactionLatticeEntry.DisapprovingValidators.Count > 99)
                        {
                            CompleteTransactionRequest(TRE);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid signature from validator detected. Validator ID: " + TVRE.ValidatorID.ToString() + " TransactionID: " + TVRE.ValidationResponse.TransactionID.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Invalid validator detected.  No Primary verification on account.  Validator ID: " + TVRE.ValidatorID.ToString() + " TransactionID: " + TVRE.ValidationResponse.TransactionID.ToString());
                }
            }
            else
            {
                //look for the request in the completed requests
            }
        }
        internal static void CompleteTransactionRequest(TransactionRequestEnvelope TRE)
        {
            //move from active to completed lists
            ActiveTransactionRequests.Remove(TRE);
            CompletedTransactionRequests.Add(TRE);

            Guid NewAccountID = new Guid();

            Console.WriteLine("    Validation Complete for " + TRE.Request.TransactionID.ToString());

            bool isApproved = ComputeApproval(TRE.Request);

            if (isApproved)
            {
                Console.WriteLine("    Executing Transaction...");
                TransactionRequest TR = TRE.Request;

                //update accounts and write blocks to relevant lattices
                switch (TR.RequestType)
                {
                    case RequestType.CreateAccount:

                        Console.WriteLine("    Creating Account...");

                        AccountDetails AD = AccountDetails.CreateAccount(TR.AccountName, TR.InitialPublicKey);

                        NewAccountID = AD.AccountID;

                        AD.SaveAccountToFile();

                        Console.WriteLine("    Updating Block Lattice...");

                        LatticeEntry LE = TR.TransactionLatticeEntry;
                        LE.AccountID = AD.AccountID;

                        LE.AppendEntryToLatticeFile();

                        Console.WriteLine("Account Creation Complete");
                        break;
                    case RequestType.AddKey:
                        AccountDetails AD2 = new AccountDetails(TR.AccountID);

                        AD2.ApprovedKeys.Add(new KeyPair() { PublicKey = TR.NewPublicKey });

                        AD2.SaveAccountToFile();

                        LatticeEntry LE2 = TR.TransactionLatticeEntry;

                        LE2.AppendEntryToLatticeFile();
                        break;
                }

                //increase reputation for approvers
                foreach (Guid Approver in TRE.Request.TransactionLatticeEntry.ApprovingValidators)
                {
                    SQLParameterCollection Params2 = new SQLParameterCollection();
                    Params2.AddParameter("ValidatorID", Approver);

                    if (ValidatorSocketServer.GlobalVars.DB.DBSelect("SELECT COUNT(*) FROM ValidatorReputation WHERE ValidatorID = @ValidatorID", Params2).Tables[0].Rows[0][0].ToString() == "0")
                    {
                        ValidatorSocketServer.GlobalVars.DB.DBInsert("INSERT INTO ValidatorReputation (ValidatorID, SuccessfulValidations, IncorrectValidations, TotalReputation) VALUES (@ValidatorID, 1, 0, 1.1)", Params2);
                    }
                    else
                    {
                        ValidatorSocketServer.GlobalVars.DB.DBUpdate("UPDATE ValidatorReputation SET SuccessfulValidations = SuccessfulValidations + 1, TotalReputation = TotalReputation + 0.1 WHERE ValidatorID = @ValidatorID", Params2);
                    }
                }

                //decrease reputation for deniers
                foreach (Guid Denier in TRE.Request.TransactionLatticeEntry.DisapprovingValidators)
                {
                    SQLParameterCollection Params2 = new SQLParameterCollection();
                    Params2.AddParameter("ValidatorID", Denier);

                    if (ValidatorSocketServer.GlobalVars.DB.DBSelect("SELECT COUNT(*) FROM ValidatorReputation WHERE ValidatorID = @ValidatorID", Params2).Tables[0].Rows[0][0].ToString() == "0")
                    {
                        ValidatorSocketServer.GlobalVars.DB.DBInsert("INSERT INTO ValidatorReputation (ValidatorID, SuccessfulValidations, IncorrectValidations, TotalReputation) VALUES (@ValidatorID, 0, 1, -1)", Params2);
                    }
                    else
                    {
                        ValidatorSocketServer.GlobalVars.DB.DBUpdate("UPDATE ValidatorReputation SET IncorrectValidations = IncorrectValidations + 1, TotalReputation = TotalReputation - 1.0 WHERE ValidatorID = @ValidatorID", Params2);
                    }
                }

                SQLParameterCollection Params = new SQLParameterCollection();

                Params.AddParameter("TransactionRequestID", TRE.Request.TransactionID);
                Params.AddParameter("Status", TransactionStatus.Approved.ToString());
                Params.AddParameter("TransactionEndDate", DateTime.UtcNow);

                if (TR.RequestType == RequestType.CreateAccount)
                {
                    Params.AddParameter("NewAccountID", NewAccountID);
                }
                else
                {
                    Params.AddParameter("NewAccountID", null);
                }

                GlobalVars.DB.DBUpdate("UPDATE TransactionRequests SET Status = @Status, TransactionEndDate = @TransactionEndDate, NewAccountID = @NewAccountID WHERE TransactionRequestID = @TransactionRequestID", Params);
            }
            else //ties also disapprove automatically.
            {
                //increase reputation for deniers
                foreach (Guid Denier in TRE.Request.TransactionLatticeEntry.DisapprovingValidators)
                {
                    SQLParameterCollection Params2 = new SQLParameterCollection();
                    Params2.AddParameter("ValidatorID", Denier);

                    if (ValidatorSocketServer.GlobalVars.DB.DBSelect("SELECT COUNT(*) FROM ValidatorReputation WHERE ValidatorID = @ValidatorID", Params2).Tables[0].Rows[0][0].ToString() == "0")
                    {
                        ValidatorSocketServer.GlobalVars.DB.DBInsert("INSERT INTO ValidatorReputation (ValidatorID, SuccessfulValidations, IncorrectValidations, TotalReputation) VALUES (@ValidatorID, 1, 0, 1.1)", Params2);
                    }
                    else
                    {
                        ValidatorSocketServer.GlobalVars.DB.DBUpdate("UPDATE ValidatorReputation SET SuccessfulValidations = SuccessfulValidations + 1, TotalReputation = TotalReputation + 0.1 WHERE ValidatorID = @ValidatorID", Params2);
                    }
                }

                //decrease reputation for approvers
                foreach (Guid Approver in TRE.Request.TransactionLatticeEntry.ApprovingValidators)
                {
                    SQLParameterCollection Params2 = new SQLParameterCollection();
                    Params2.AddParameter("ValidatorID", Approver);

                    if (ValidatorSocketServer.GlobalVars.DB.DBSelect("SELECT COUNT(*) FROM ValidatorReputation WHERE ValidatorID = @ValidatorID", Params2).Tables[0].Rows[0][0].ToString() == "0")
                    {
                        ValidatorSocketServer.GlobalVars.DB.DBInsert("INSERT INTO ValidatorReputation (ValidatorID, SuccessfulValidations, IncorrectValidations, TotalReputation) VALUES (@ValidatorID, 0, 1, -1)", Params2);
                    }
                    else
                    {
                        ValidatorSocketServer.GlobalVars.DB.DBUpdate("UPDATE ValidatorReputation SET IncorrectValidations = IncorrectValidations + 1, TotalReputation = TotalReputation - 1.0 WHERE ValidatorID = @ValidatorID", Params2);
                    }
                }


                SQLParameterCollection Params = new SQLParameterCollection();

                Params.AddParameter("TransactionRequestID", TRE.Request.TransactionID);
                Params.AddParameter("Status", TransactionStatus.Denied.ToString());
                Params.AddParameter("TransactionEndDate", DateTime.UtcNow);

                GlobalVars.DB.DBUpdate("UPDATE TransactionRequests SET Status = @Status, TransactionEndDate = @TransactionEndDate WHERE TransactionRequestID = @TransactionRequestID", Params);
            }
        }

        private static bool ComputeApproval(TransactionRequest TR)
        {
            bool isApproved = false;
            decimal WeightedApproverTotal = 0;
            decimal WeightedDenierTotal = 0;

            foreach (Guid Approver in TR.TransactionLatticeEntry.ApprovingValidators)
            {
                Validator V = ValidatorConnections.Find(x => x.ValidatorID == Approver);
                SQLParameterCollection Params = new SQLParameterCollection();
                Params.AddParameter("ValidatorID", Approver);

                DataTable dtRep = GlobalVars.DB.DBSelect("SELECT TotalReputation FROM ValidatorReputation WHERE ValidatorID = @ValidatorID", Params).Tables[0];

                decimal ValidatorReputation = 1;

                if (dtRep.Rows.Count > 0)
                {
                     ValidatorReputation = decimal.Parse(dtRep.Rows[0]["TotalReputation"].ToString());
                }

                if (V.Hosted)
                {
                    if (ValidatorReputation > 0)
                    {
                        WeightedApproverTotal += ValidatorReputation * 0.2m;
                    }
                }
                else
                {
                    WeightedApproverTotal += ValidatorReputation;
                }
            }

            foreach (Guid Denier in TR.TransactionLatticeEntry.DisapprovingValidators)
            {
                Validator V = ValidatorConnections.Find(x => x.ValidatorID == Denier);
                SQLParameterCollection Params = new SQLParameterCollection();
                Params.AddParameter("ValidatorID", Denier);

                decimal ValidatorReputation = decimal.Parse(GlobalVars.DB.DBSelect("SELECT TotalReputation FROM ValidatorReputation WHERE ValidatorID = @ValidatorID", Params).Tables[0].Rows[0]["TotalReputation"].ToString());

                if (V.Hosted)
                {
                    if (ValidatorReputation > 0)
                    {
                        WeightedDenierTotal += ValidatorReputation * 0.2m;
                    }
                }
                else
                {
                    WeightedDenierTotal += ValidatorReputation;
                }
            }

            if (WeightedApproverTotal > WeightedDenierTotal)
            {
                isApproved = true;
            }
            else
            {
                isApproved = false;
            }

            return isApproved;
        }

        internal static bool VerifyValidatorHasPrimaryVerifiedPerson(TransactionValidationResponseEnvelope TVRE)
        {
            bool isValid = false;

            AccountDetails AD = new AccountDetails(TVRE.ValidatorID);

            if (AD.VerifiedMembers.Count > 0)
            {

                foreach (VerifiedMember VM in AD.VerifiedMembers)
                {
                    if (VM.IsPrimaryVerification)
                    {
                        isValid = true;
                        break;
                    }
                }
            }

            return isValid;
        }

        internal static bool VerifyTransactionValidationResponseSignature(TransactionValidationResponseEnvelope TVRE)
        {
            bool isValid = false;

            RSA rsa = RSA.Create();

            AccountDetails AD = new AccountDetails(TVRE.ValidatorID);


            foreach (KeyPair KP in AD.ApprovedKeys)
            {
                rsa.ImportFromPem(KP.PublicKey);

                if (rsa.VerifyData(GlobalFunctions.SerializeObjectToByteArray(TVRE.ValidationResponse), TVRE.ValidatorSignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                {
                    if (KP.Permissions.ValidatingEnabled)
                    {
                        isValid = true;
                    }
                    break;
                }
            }

            return isValid;
        }

        internal static TransactionRequestEnvelope GetActiveRequest(TransactionValidationResponse TVR)
        {
            TransactionRequestEnvelope TRE = null;

            List<TransactionRequestEnvelope> lTRE = ActiveTransactionRequests.FindAll(x => x.Request.TransactionID == TVR.TransactionID);

            if (lTRE.Count > 0)
            {
                TRE = lTRE[0];
            }

            return TRE;
        }
    }
}
