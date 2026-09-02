using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorSocketServer
{
    internal class GlobalVars
    {
        private static object Conch = new object();

        private static GenericDataAccessClassCore.DBMC oDB;

        internal static GenericDataAccessClassCore.DBMC DB
        {
            get
            {
                if (oDB == null)
                { 
                    lock (Conch)
                    {
                        if (oDB == null)
                        {
                            EcoCoinSharedTypes.DBSecrets Secrets = new EcoCoinSharedTypes.DBSecrets();
                            Secrets.LoadSecrets();

                            oDB = new GenericDataAccessClassCore.DBMC("Server=" + Secrets.ServerAddress + ";Database=" + Secrets.Database + ";User Id=" + Secrets.Username + ";Password=" + Secrets.Password + ";", "ValidatorSocketServer", true);
                        }
                    }
                }

                return oDB;
            }
        }
    }
}
