using BL.Model.Exception;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BL.CrossCutting.Helpers.Crypto
{
    internal class CryptoService : ICryptoService
    {
        public CryptoService()
        {
        }

        #region RSAKey
        public RSAParameters RSAPersistPublicKeyInCSP(string ContainerName, string PublicKeyXmlString)
        {
            try
            {
                // Create a new instance of CspParameters.  Pass
                // 13 to specify a DSA container or 1 to specify
                // an RSA container.  The default is 1.
                CspParameters cspParams = new CspParameters();

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

        private RSAParameters RSAPersistKeyInCSP(string ContainerName, bool IncludePrivateParameters)
        {
            try
            {
                // Create a new instance of CspParameters.  Pass
                // 13 to specify a DSA container or 1 to specify
                // an RSA container.  The default is 1.
                CspParameters cspParams = new CspParameters();

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

        private void RSADeleteKeyInCSP(string ContainerName)
        {
            try
            {
                // Create a new instance of CspParameters.  Pass
                // 13 to specify a DSA container or 1 to specify
                // an RSA container.  The default is 1.
                CspParameters cspParams = new CspParameters();

                // Specify the container name using the passed variable.
                cspParams.KeyContainerName = ContainerName;

                //Create a new instance of RSACryptoServiceProvider. 
                //Pass the CspParameters class to use the 
                //key in the container.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider(cspParams);

                //Explicitly set the PersistKeyInCsp property to false
                //to delete the key entry in the container.
                RSAalg.PersistKeyInCsp = false;

                //Call Clear to release resources and delete the key from the container.
                RSAalg.Clear();
            }
            catch (CryptographicException ex)
            {
            }
        }

        #endregion RSAKey

        #region SignData
        public string HashAndSignBytes(string DataToSign, string KeyName)
        {
            byte[] dataToSign = Encoding.Default.GetBytes(DataToSign);

            RSAParameters key = RSAPersistKeyInCSP(KeyName, true);

            byte[] hash = HashAndSignBytes(dataToSign, key);

            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }

        public byte[] HashAndSignBytes(byte[] DataToSign, string KeyName)
        {
            RSAParameters key = RSAPersistKeyInCSP(KeyName, true);

            return HashAndSignBytes(DataToSign, key);
        }

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

        public bool VerifySignedHash(string DataToVerify, string SignedData, string KeyName)
        {
            byte[] dataToVerify = Encoding.Default.GetBytes(DataToVerify);
            byte[] signedData = Encoding.Default.GetBytes(SignedData);

            RSAParameters key = RSAPersistKeyInCSP(KeyName, false);

            return VerifySignedHash(dataToVerify, signedData, key);
        }

        public bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, string KeyName)
        {
            RSAParameters key = RSAPersistKeyInCSP(KeyName, false);

            return VerifySignedHash(DataToVerify, SignedData, key);
        }

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
    }
}