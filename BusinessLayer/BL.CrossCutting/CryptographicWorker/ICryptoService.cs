using System.Security.Cryptography;

namespace BL.CrossCutting.CryptographicWorker
{
    public interface ICryptoService
    {
        #region RSAKey
        RSAParameters RSAPersistPublicKeyInCSP(string ContainerName, string PublicKeyXmlString);
        //TODO remove in release version
        string RSAPersistKeyInCSPExport(string ContainerName, bool IncludePrivateParameters);
        //TODO remove in release version
        void RSAPersistKeyInCSP(string ContainerName, string KeyXmlString);
        #endregion RSAKey

        #region SignData
        string HashAndSignString(string DataToSign);
        string HashAndSignString(string DataToSign, string KeyName);
        byte[] HashAndSignBytes(byte[] DataToSign);
        byte[] HashAndSignBytes(byte[] DataToSign, string KeyName);
        bool VerifySignedHash(string DataToVerify, string SignedData);
        bool VerifySignedHash(string DataToVerify, string SignedData, string KeyName);
        bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData);
        bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, string KeyName);
        #endregion SignData
    }
}