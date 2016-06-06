using System;
using System.Security.Cryptography;
using System.Text;

namespace LicenceManager.Licence
{
    public static class LicenceGenerator
    {

        private static string GetHash(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var res = BitConverter.ToString(new SHA512Managed().ComputeHash(bytes)).Replace("-", String.Empty);
            return res;
        }

        private static string GetLicenceRegCode(LicenceInfo li)
        {
            var clInfo = $"Name:{li.ClientName}/DD:{li.DateLimit}/NNOC:{li.NamedNumberOfConnections}/CNOC:{li.ConcurenteNumberOfConnections}/Fun:{li.Functionals}";

            string res, hash;

            hash = GetHash(clInfo);

            hash = hash.ToUpper().Replace("0", "").Replace("O", "").Replace("I", "").Replace("1", "");

            int part = hash.Length / 8;

            res = hash[0].ToString() + hash[hash.Length - 1] + hash[part] + hash[hash.Length - part] + hash[2 * part] + hash[hash.Length - 2 * part] + hash[3 * part] + hash[hash.Length - 3 * part];

            return res;
        }

        private static string LicenceKey(string regCode)
        {
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

            string privatKey = "<RSAKeyValue><Modulus>sBRZy9xvw7FWdb5EHd79H8f2D4+JP3yokrbKpCgFbcwCEPPZpGUj07poBM9MvrIXEIHoahIYVw3UqWCLvFFL6Cb+u3zrOTaNmCNyXdZ4H/28sskfuBtVzXjllzwEkrcJg0NfSmCbjw/9YFUYEdl1ZTUL40pN8Kuk1Wr1f/wP+wk=</Modulus><Exponent>AQAB</Exponent><P>twV17e14On7eLeKl46JRJnnXrvZp4tHj68iNFk/S8tK/uKj3b9xeTTqxI6S31xQ3mN26X54egttXNjQ7V9OUaQ==</P><Q>9kpHMG4hxQ3/q1FyPlLgNV1XPDyeGoNF1QQDZ7Te8xfWvPW1ildAYsCEJ91tZMstgJR7oojYPy7VTNDn8bndoQ==</Q><DP>SLy017Bu/eB58IaJI2TZF4+I9pIcFvcPvB9iYyGqVrMHWx5b6GsOV2ciC2ZlYec5CVnlviabPapqiLJNe2QtMQ==</DP><DQ>R++uF2Ezj+Dk2l8xpS6DulKHFlsGOuw4y10euX3E2PAPkqWZ3sxZS/67GwG74ALQSYwVCIY700iUmJk0BhCpwQ==</DQ><InverseQ>oN5Vlg5M68jAeeZRiduiyMBw/T+oZQ5zaxMlvqIhgF603xRjHTzcNaHZB9Kvn0YBnfcRx6F1PfSkJJl4rWAaDw==</InverseQ><D>AIZ3BBwquy82vlAsfNhS8frTOZWoh6d0C0f/T8EMzxiKMwm/LvXcRv/p2oXRyUnXtsVkb5iROQVCCqVOlWe6rbvUMU8P554u4t9g0y1oLJUQDbYmRBo6z0I31OTRNBb7nCI6/l01Vyq6Ju225EdOEL8EVNd/wkQXoYRbm7Mun4E=</D></RSAKeyValue>";

            RSAalg.FromXmlString(privatKey);

            byte[] dataToSign = Encoding.UTF8.GetBytes(regCode);

            var hash = RSAalg.SignData(dataToSign, new SHA512CryptoServiceProvider());

            var licKey = Convert.ToBase64String(hash);
            return licKey;
        }

        public static string CreateLicenceKey(string regCode, LicenceInfo licenceInfo)
        {
            var fullCode = regCode + "-" + GetLicenceRegCode(licenceInfo);
            return LicenceKey(fullCode);
        }
    }
}
