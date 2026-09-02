using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoCoinSharedTypes
{
    public class VerifiedMember
    {
        private bool bIsBusiness;
        private bool bIsPrimaryVerification;
        private string sName;
        private string sSSNHash;
        private string sEIN;
        public bool IsBusiness
        {
            get { return bIsBusiness; }
            set { bIsBusiness = value; }
        }
        public bool IsPrimaryVerification
        {
            get { return bIsPrimaryVerification; }
            set { bIsPrimaryVerification = value; }
        }
        public string Name
        {
            get { return sName; }
            set { sName = value; }
        }
        public string SSNHash
        {
            get { return sSSNHash; }
            set { sSSNHash = value; }
        }
        public string EIN
        {
            get { return sEIN; }
            set { sEIN = value; }
        }
    }
}
