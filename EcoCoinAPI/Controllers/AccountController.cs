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
            TR.AccountID = Guid.NewGuid();
            TR.TransactionSignerID = Guid.Parse("a6479df0-445a-4376-b3ed-6dd89fc51cf9");
            TR.AccountName = AccountName;
            TR.RequestType = RequestType.CreateAccount;
            TR.InitialPublicKey = InitialPublicKey;

            //TODO: Validate with blockchain that account creation is acceptable, then execute below code on verification

            //AccountDetails AD = AccountDetails.CreateAccount(AccountName, InitialPublicKey);

            //AD.SaveAccountToFile();

            //AccountDetailsEnvelope ADE = new AccountDetailsEnvelope();
            //ADE.AccountDetails = AD;
            //ADE.Signature = GlobalFunctions.GenerateCryptoHashForObject(AD);

            //return AD;

            PendingTransactionReceipt PTR = new PendingTransactionReceipt();
            PTR.TransactionID = TR.TransactionID;

            PendingTransactionReceiptEnvelope PTRE = new PendingTransactionReceiptEnvelope(PTR, EcoCoinSharedTypes.GlobalFunctions.GenerateCryptoHashForObject(PTR));

            return PTRE;
        }

        [HttpGet("AccountAddKey", Name = "AccountAddKey")]
        public PendingTransactionReceiptEnvelope AccountAddKey(Guid AccountId, string NewPublicKey, string TransactionSignature)
        {
            TransactionRequest TR = new TransactionRequest();
            TR.AccountID = AccountId;
            TR.TransactionSignerID = AccountId;
            TR.RequestType = RequestType.AddKey;
            TR.NewPublicKey = NewPublicKey;

            //TODO: Validate with blockchain that key addition is acceptable, then run below code upon validation

            //AccountDetails AD = new AccountDetails(AccountId);

            //AD.ApprovedKeys.Add(new KeyPair() { PublicKey = NewPublicKey });

            //AD.SaveAccountToFile();

            PendingTransactionReceipt PTR = new PendingTransactionReceipt(TR.TransactionID);

            PendingTransactionReceiptEnvelope PTRE = new PendingTransactionReceiptEnvelope(PTR, EcoCoinSharedTypes.GlobalFunctions.GenerateCryptoHashForObject(PTR));

            return PTRE;
        }
    }


}
