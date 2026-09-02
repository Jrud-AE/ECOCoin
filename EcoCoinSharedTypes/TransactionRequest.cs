using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class TransactionRequest
    {
        #region TransactionMetaData
        private Guid gTransactionID;
        private RequestType rtRequestType;
        private Guid gTransactionSignerID;
        private int iTransactionSignerKeyID;
        private string sTransactionSignerAccountVersionHash;
        private List<EcoFileStub> lsInvolvedFiles;
        private long lNOnce;
        private string sTransactionSignature;
        private LatticeEntry leTransactionLatticeEntry;
        private int iIssuedValidatorCount;
        private DateTime dtTransactionStartDate;

        public Guid TransactionID
        {
            get { return gTransactionID; }
            set { gTransactionID = value; }
        }
        public long NOnce
        {
            get { return lNOnce; }
            set { lNOnce = value; }
        }
        public RequestType RequestType
        {
            get { return rtRequestType; }
            set
            {
                rtRequestType = value;
                if (gAccountID != Guid.Empty)
                {
                    AddAccountFileToInvolvedFileListBasedOnRequestType();
                }
            }
        }
        public string TransactionSignerAccountVersionHash
        {
            get { return sTransactionSignerAccountVersionHash; }
        }
        public int TransactionSignerKeyID
        {
            get { return iTransactionSignerKeyID; }
            set { iTransactionSignerKeyID = value; }
        }
        public Guid TransactionSignerID
        {
            get { return gTransactionSignerID; }
            set
            {
                gTransactionSignerID = value;
                sTransactionSignerAccountVersionHash = GlobalFunctions.HashFileAsString(GlobalVars.AccountStoragePath + gTransactionSignerID + ".acc");
            }
        }
        public List<EcoFileStub> InvolvedFiles
        {
            get { return lsInvolvedFiles; }
            set { lsInvolvedFiles = value; }
        }
        public string TransactionSignature
        {
            get { return sTransactionSignature; }
            set { sTransactionSignature = value; }
        }
        public LatticeEntry TransactionLatticeEntry
        {
            get
            {
                return leTransactionLatticeEntry;
            }
            set
            {
                leTransactionLatticeEntry = value;
            }
        }
        public DateTime TransactionStartDate
        {
            get { return dtTransactionStartDate; }
            set { dtTransactionStartDate = value; }
        }

        private void AddAccountFileToInvolvedFileListBasedOnRequestType()
        {
            switch (rtRequestType)
            {
                case RequestType.CreateAccount:
                    AddFileToInvolvedFileList(Guid.Parse("a6479df0-445a-4376-b3ed-6dd89fc51cf9"));
                    break;
                case RequestType.AddKey:
                    AddFileToInvolvedFileList(gAccountID);
                    break;
                default:
                    AddFileToInvolvedFileList(gAccountID);
                    break;
            }
        }
        private void AddFileToInvolvedFileList(Guid LocalAccountID)
        {
            string FileName = LocalAccountID + ".acc";

            if ((lsInvolvedFiles.Find(x => x.FileName == FileName)) == null)
            {
                lsInvolvedFiles.Add(new EcoFileStub { FileName = FileName, FileHash = GlobalFunctions.HashFile(GlobalVars.AccountStoragePath + LocalAccountID + ".acc") });
            }
        }
        #endregion

        private string sAccountName;
        private Guid gAccountID;
        private string sInitialPublicKey;
        private string sNewPublicKey;
        public TransactionRequest() 
        {
            TransactionID = Guid.NewGuid();
            lsInvolvedFiles = new List<EcoFileStub>();
        }

        public Guid AccountID 
        { 
            get { return gAccountID; }
            set
            {
                gAccountID = value;
                AddAccountFileToInvolvedFileListBasedOnRequestType();
            } 
        }
        public string AccountName 
        { 
            get { return sAccountName; } 
            set { sAccountName = value; } 
        }
        public string InitialPublicKey
        {
            get { return sInitialPublicKey; }
            set { sInitialPublicKey = value; }
        }
        public string NewPublicKey
        {
            get { return sNewPublicKey; }
            set { sNewPublicKey = value; }
        }
        public int IssuedValidatorCount
        {
            get { return iIssuedValidatorCount; }
            set { iIssuedValidatorCount = value; }
        }
    }

    public class TransactionRequestEnvelope
    {
        private TransactionRequest trRequest;
        private byte[] bTransactionSignature;

        public TransactionRequest Request 
        {
            get 
            { 
                return trRequest; 
            }
            set
            {
                trRequest = value;
            }
        }
        public byte[] EnvelopeSignature 
        {
            get
            {
                return bTransactionSignature;
            }
            set
            {
                bTransactionSignature = value;
            }
        }

        public TransactionRequestEnvelope() { }
        public TransactionRequestEnvelope(TransactionRequest Request, byte[] TransactionSignature)
        {
            this.Request = Request;
            this.EnvelopeSignature = TransactionSignature;
        }
    }
}
