using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace EcoCoinSharedTypes
{
    public class KeyPair
    {
        private string sPrivateKey;
        private string sPublicKey;

        private KeyPairPermissions kppPermissions;

        public KeyPairPermissions Permissions
        {
            get { return kppPermissions; }
            set { kppPermissions = value; }
        }

        public KeyPair()
        {
            sPrivateKey = "";
            sPublicKey = "";
            kppPermissions = new KeyPairPermissions();
        }

        public KeyPair(string PrivateKey, string PublicKey, KeyPairPermissions Permissions)
        {
            this.sPrivateKey = PrivateKey;
            this.sPublicKey = PublicKey;
            this.kppPermissions = Permissions;
        }

        public static KeyPair CreateKey()
        {
            KeyPair Key = new KeyPair();

            RSA TKey = RSA.Create(4096);

            Key.sPrivateKey = TKey.ExportPkcs8PrivateKeyPem();
            Key.sPublicKey = TKey.ExportSubjectPublicKeyInfoPem();
            
            return Key;
        }

        public string PrivateKey
        {
            get { return sPrivateKey; }
            set { sPrivateKey = value; }
        }

        public string PublicKey
        { 
            get { return sPublicKey; } 
            set { sPublicKey = value; }
        }


    }

    public class KeyPairPermissions
    {
        #region Permissions
        private bool bAlterAccountSettingsPermission;
        private bool bKeyCreationPermission;
        private bool bKeyDeletePermission;
        private bool bKeyPermissionModifyPermission;

        public KeyPairPermissions()
        {
            bAlterAccountSettingsPermission = false;
            bKeyCreationPermission = false;
            bKeyDeletePermission = false;
            bKeyPermissionModifyPermission = false;
        }


        public bool AlterAccountSettingsPermission
        {
            get { return bAlterAccountSettingsPermission; }
            set { bAlterAccountSettingsPermission = value; }
        }

        public bool KeyCreationPermission
        {
            get { return bKeyCreationPermission; }
            set { bKeyCreationPermission = value; }
        }

        public bool KeyDeletePermission
        {
            get { return bKeyDeletePermission; }
            set { bKeyDeletePermission = value; }
        }

        public bool KeyPermissionModifyPermission
        {
            get { return bKeyPermissionModifyPermission; }
            set { bKeyPermissionModifyPermission = value; }
        }
        #endregion
    }
}
