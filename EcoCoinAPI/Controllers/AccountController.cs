using EcoCoinSharedTypes;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace EcoCoinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        [HttpGet("AccountRetrieve", Name = "AccountRetrieve")]
        public AccountDetailsEnvelope AccountRetrieve(Guid AccountId)
        {
            AccountDetails AD = new AccountDetails(AccountId);

            foreach (KeyPair K in AD.ApprovedKeys)
            {
                K.PrivateKey = "";
            }

            AccountDetailsEnvelope ADE = new AccountDetailsEnvelope(AD, EcoCoinSharedTypes.GlobalFunctions.GenerateCryptoHashForObject(AD));

            return ADE;
        }


        [HttpGet("AccountCreate", Name = "AccountCreate")]
        public PendingTransactionReceiptEnvelope AccountCreate(string AccountName, string InitialPublicKey)
        {
            TransactionRequest TR = new TransactionRequest();
            TR.RequestType = RequestType.CreateAccount;
            TR.AccountID = Guid.NewGuid();
            TR.TransactionSignerID = EcoCoinSharedTypes.GlobalVars.AEAccountCreationAccount;
            TR.TransactionSignerKeyID = 0;
            TR.AccountName = AccountName;
            TR.InitialPublicKey = InitialPublicKey;
            TR.NOnce = 0;
            TR.TransactionSignature = EcoCoinSharedTypes.GlobalFunctions.GenerateCryptoHashForObjectAsString(TR, TR.TransactionSignerID, TR.TransactionSignerKeyID);

            //send transaction request to blockchain for approval
            TransactionRequestEnvelope TRE = new TransactionRequestEnvelope(TR, EcoCoinSharedTypes.GlobalFunctions.GenerateCryptoHashForObject(TR));

            GlobalFunctions.SendTransactionRequestToValidators(TRE);


            //send back a pending transaction receipt to the user so they can check on the status of their request
            PendingTransactionReceipt PTR = new PendingTransactionReceipt();
            PTR.TransactionID = TR.TransactionID;

            PendingTransactionReceiptEnvelope PTRE = new PendingTransactionReceiptEnvelope(PTR, EcoCoinSharedTypes.GlobalFunctions.GenerateCryptoHashForObject(PTR));

            return PTRE;
        }


        [HttpGet("AccountAddKey", Name = "AccountAddKey")]
        public PendingTransactionReceiptEnvelope AccountAddKey(Guid AccountId, string NewPublicKey, string TransactionSignature, long NOnce, string Signature)
        {
            TransactionRequest TR = new TransactionRequest();
            TR.RequestType = RequestType.AddKey;
            TR.AccountID = AccountId;
            TR.TransactionSignerID = AccountId;
            TR.NewPublicKey = NewPublicKey;
            TR.NOnce = NOnce;
            TR.TransactionSignature = Signature;

            //send transaction request to blockchain for approval
            TransactionRequestEnvelope TRE = new TransactionRequestEnvelope(TR, EcoCoinSharedTypes.GlobalFunctions.GenerateCryptoHashForObject(TR));

            GlobalFunctions.SendTransactionRequestToValidators(TRE);


            //send back a pending transaction receipt to the user so they can check on the status of their request
            PendingTransactionReceipt PTR = new PendingTransactionReceipt(TR.TransactionID);

            PendingTransactionReceiptEnvelope PTRE = new PendingTransactionReceiptEnvelope(PTR, EcoCoinSharedTypes.GlobalFunctions.GenerateCryptoHashForObject(PTR));

            return PTRE;
        }
    }


}
