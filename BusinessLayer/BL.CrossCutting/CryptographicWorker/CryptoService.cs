using BL.Model.Constants;
using BL.Model.Exception;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BL.CrossCutting.CryptographicWorker
{
    internal class CryptoService : ICryptoService
    {

        private const string _RSAPublicKeyXmlByLicence = "<RSAKeyValue><Modulus>sBRZy9xvw7FWdb5EHd79H8f2D4+JP3yokrbKpCgFbcwCEPPZpGUj07poBM9MvrIXEIHoahIYVw3UqWCLvFFL6Cb+u3zrOTaNmCNyXdZ4H/28sskfuBtVzXjllzwEkrcJg0NfSmCbjw/9YFUYEdl1ZTUL40pN8Kuk1Wr1f/wP+wk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        #region Convert
        private byte[] GetBytesByData(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        private string GetStringByData(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", String.Empty);
        }

        private byte[] GetBytesByBase64(string data)
        {
            return Convert.FromBase64String(data);
        }

        private string GetStringByBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        #endregion Convert

        #region RSAKey

        //TODO
        public RSAParameters RSAPersistPublicKeyInCSP(string ContainerName, string PublicKeyXmlString)
        {
            try
            {
                // Create a new instance of CspParameters.  Pass
                // 13 to specify a DSA container or 1 to specify
                // an RSA container.  The default is 1.
                CspParameters cspParams = new CspParameters();

                cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

                // Specify the container name using the passed variable.
                cspParams.KeyContainerName = ContainerName;

                //Create a new instance of RSACryptoServiceProvider to generate
                //a new key pair.  Pass the CspParameters class to persist the 
                //key in the container.  The PersistKeyInCsp property is true by 
                //default, allowing the key to be persisted. 
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(cspParams);

                RSAalg.FromXmlString(PublicKeyXmlString);

                return RSAalg.ExportParameters(false);
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicError();
            }
        }

        //TODO remove in release version
        public string RSAPersistKeyInCSPExport(string ContainerName, bool IncludePrivateParameters)
        {
            //try
            //{
            // Create a new instance of CspParameters.  Pass
            // 13 to specify a DSA container or 1 to specify
            // an RSA container.  The default is 1.
            CspParameters cspParams = new CspParameters();

            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

            // Specify the container name using the passed variable.
            cspParams.KeyContainerName = ContainerName;

            //Create a new instance of RSACryptoServiceProvider to generate
            //a new key pair.  Pass the CspParameters class to persist the 
            //key in the container.  The PersistKeyInCsp property is true by 
            //default, allowing the key to be persisted. 
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(cspParams);

            return RSAalg.ToXmlString(IncludePrivateParameters);
            //}
            //catch (CryptographicException ex)
            //{
            //    throw new CryptographicError();
            //}
        }

        //TODO remove in release version
        public void RSAPersistKeyInCSP(string ContainerName, string KeyXmlString)
        {
            //try
            //{
            // Create a new instance of CspParameters.  Pass
            // 13 to specify a DSA container or 1 to specify
            // an RSA container.  The default is 1.
            CspParameters cspParams = new CspParameters();

            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

            // Specify the container name using the passed variable.
            cspParams.KeyContainerName = ContainerName;

            //Create a new instance of RSACryptoServiceProvider to generate
            //a new key pair.  Pass the CspParameters class to persist the 
            //key in the container.  The PersistKeyInCsp property is true by 
            //default, allowing the key to be persisted. 
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(cspParams);

            RSAalg.FromXmlString(KeyXmlString);
            //}
            //catch (CryptographicException ex)
            //{
            //    throw new CryptographicError();
            //}
        }

        private RSAParameters RSAPersistKeyInCSP(string ContainerName, bool IncludePrivateParameters)
        {
            try
            {
                // Create a new instance of CspParameters.  Pass
                // 13 to specify a DSA container or 1 to specify
                // an RSA container.  The default is 1.
                CspParameters cspParams = new CspParameters();

                cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

                // Specify the container name using the passed variable.
                cspParams.KeyContainerName = ContainerName;

                //Create a new instance of RSACryptoServiceProvider to generate
                //a new key pair.  Pass the CspParameters class to persist the 
                //key in the container.  The PersistKeyInCsp property is true by 
                //default, allowing the key to be persisted. 
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(cspParams);

                return RSAalg.ExportParameters(IncludePrivateParameters);
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicError();
            }
        }

        //private void RSADeleteKeyInCSP(string ContainerName)
        //{
        //    try
        //    {
        //        // Create a new instance of CspParameters.  Pass
        //        // 13 to specify a DSA container or 1 to specify
        //        // an RSA container.  The default is 1.
        //        CspParameters cspParams = new CspParameters();

        //        cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

        //        // Specify the container name using the passed variable.
        //        cspParams.KeyContainerName = ContainerName;

        //        //Create a new instance of RSACryptoServiceProvider. 
        //        //Pass the CspParameters class to use the 
        //        //key in the container.
        //        RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(cspParams);

        //        //Explicitly set the PersistKeyInCsp property to false
        //        //to delete the key entry in the container.
        //        RSAalg.PersistKeyInCsp = false;

        //        //Call Clear to release resources and delete the key from the container.
        //        RSAalg.Clear();
        //    }
        //    catch (CryptographicException ex)
        //    {
        //    }
        //}

        #endregion RSAKey

        #region SignData
        /// <summary>
        /// Получение подписи с помощью ключа приложения
        /// </summary>
        /// <param name="DataToSign"></param>
        /// <returns></returns>
        public string HashAndSignString(string DataToSign)
        {
            return HashAndSignString(DataToSign, SettingConstants.CRYPTO_LOCAL_SIGNDATA_KEY_NAME);
        }

        /// <summary>
        /// Получение подписи для указанного имени ключа
        /// </summary>
        /// <param name="DataToSign"></param>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public string HashAndSignString(string DataToSign, string KeyName)
        {
            byte[] dataToSign = GetBytesByData(DataToSign);

            RSAParameters key = RSAPersistKeyInCSP(KeyName, true);

            byte[] hash = HashAndSignBytes(dataToSign, key);

            return GetStringByBase64(hash);
        }

        /// <summary>
        /// Получение подписи с помощью ключа приложения
        /// </summary>
        /// <param name="DataToSign"></param>
        /// <returns></returns>
        public byte[] HashAndSignBytes(byte[] DataToSign)
        {
            return HashAndSignBytes(DataToSign, SettingConstants.CRYPTO_LOCAL_SIGNDATA_KEY_NAME);
        }

        /// <summary>
        /// Получение подписи для указанного имени ключа
        /// </summary>
        /// <param name="DataToSign"></param>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public byte[] HashAndSignBytes(byte[] DataToSign, string KeyName)
        {
            RSAParameters key = RSAPersistKeyInCSP(KeyName, true);

            return HashAndSignBytes(DataToSign, key);
        }

        /// <summary>
        /// Получение подписи для определенного ключа
        /// </summary>
        /// <param name="DataToSign"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private byte[] HashAndSignBytes(byte[] DataToSign, RSAParameters Key)
        {
            try
            {
                // Create a new instance of RSACryptoServiceProvider using the 
                // key from RSAParameters.  
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.ImportParameters(Key);

                // Hash and sign the data. Pass a new instance of SHA512CryptoServiceProvider
                // to specify the use of SHA1 for hashing.
                return RSAalg.SignData(DataToSign, new SHA512CryptoServiceProvider());
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        /// <summary>
        /// Проверка подписи с помощью ключа приложения
        /// </summary>
        /// <param name="DataToVerify"></param>
        /// <param name="SignedData"></param>
        /// <returns></returns>
        public bool VerifySignedHash(string DataToVerify, string SignedData)
        {
            return VerifySignedHash(DataToVerify, SignedData, SettingConstants.CRYPTO_LOCAL_SIGNDATA_KEY_NAME);
        }

        /// <summary>
        /// Проверка подписи для указанного имени ключа
        /// </summary>
        /// <param name="DataToVerify"></param>
        /// <param name="SignedData"></param>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public bool VerifySignedHash(string DataToVerify, string SignedData, string KeyName)
        {
            byte[] dataToVerify = GetBytesByData(DataToVerify);
            byte[] signedData = GetBytesByBase64(SignedData);

            RSAParameters key = RSAPersistKeyInCSP(KeyName, false);

            return VerifySignedHash(dataToVerify, signedData, key);
        }

        /// <summary>
        /// Проверка подписи с помощью ключа приложения
        /// </summary>
        /// <param name="DataToVerify"></param>
        /// <param name="SignedData"></param>
        /// <returns></returns>
        public bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData)
        {
            return VerifySignedHash(DataToVerify, SignedData, SettingConstants.CRYPTO_LOCAL_SIGNDATA_KEY_NAME);
        }

        /// <summary>
        /// Проверка подписи для указанного имени ключа
        /// </summary>
        /// <param name="DataToVerify"></param>
        /// <param name="SignedData"></param>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, string KeyName)
        {
            RSAParameters key = RSAPersistKeyInCSP(KeyName, false);

            return VerifySignedHash(DataToVerify, SignedData, key);
        }

        /// <summary>
        /// Проверка подписи для определенного ключа
        /// </summary>
        /// <param name="DataToVerify"></param>
        /// <param name="SignedData"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, RSAParameters Key)
        {
            try
            {
                // Create a new instance of RSACryptoServiceProvider using the 
                // key from RSAParameters.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.ImportParameters(Key);

                // Verify the data using the signature.  Pass a new instance of SHA512CryptoServiceProvider
                // to specify the use of SHA1 for hashing.
                return RSAalg.VerifyData(DataToVerify, new SHA512CryptoServiceProvider(), SignedData);

            }
            catch (CryptographicException ex)
            {
                return false;
            }
        }
        #endregion SignData

        #region HashData

        public string GetHash(byte[] data)
        {
            return GetStringByData(new SHA512Managed().ComputeHash(data));
        }

        public string GetHash(Stream data)
        {
            return GetStringByData(new SHA512Managed().ComputeHash(data));
        }

        public string GetHash(string data)
        {
            return GetHash(GetBytesByData(data));
        }

        public bool VerifyHash(byte[] data, string hash)
        {
            if (string.IsNullOrEmpty(hash))
                return false;

            return hash.Equals(this.GetHash(data));
        }

        public bool VerifyHash(Stream data, string hash)
        {
            if (string.IsNullOrEmpty(hash))
                return false;

            return hash.Equals(GetHash(data));
        }

        public bool VerifyHash(string data, string hash)
        {
            if (string.IsNullOrEmpty(hash))
                return false;

            return hash.Equals(GetHash(data));
        }

        #endregion HashData

        #region Licence
        public bool VerifyLicenceKey(string regCode, string licKey)
        {
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.FromXmlString(_RSAPublicKeyXmlByLicence);

                byte[] dataToVerify = GetBytesByData(regCode);
                byte[] signedData = GetBytesByBase64(licKey);

                return RSAalg.VerifyData(dataToVerify, new SHA512CryptoServiceProvider(), signedData);

            }
            catch (CryptographicException ex)
            {
                return false;
            }
        }
        #endregion Licence
    }
}