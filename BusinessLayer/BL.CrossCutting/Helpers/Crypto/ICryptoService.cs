using System.Security.Cryptography;

namespace BL.CrossCutting.Helpers.Crypto
{
    public interface ICryptoService
    {
        #region RSAKey
        RSAParameters RSAPersistPublicKeyInCSP(string ContainerName, string PublicKeyXmlString);
        #endregion RSAKey

        #region SignData
        string HashAndSignBytes(string DataToSign, string KeyName);
        byte[] HashAndSignBytes(byte[] DataToSign, string KeyName);
        bool VerifySignedHash(string DataToVerify, string SignedData, string KeyName);
        bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, string KeyName);
        #endregion SignData
    }
}