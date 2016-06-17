using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Database;
using BL.Model.Exception;
using BL.Model.SystemCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    internal class Licences
    {
        private const string _RSAPublicKeyXmlByLicence = "<RSAKeyValue><Modulus>sBRZy9xvw7FWdb5EHd79H8f2D4+JP3yokrbKpCgFbcwCEPPZpGUj07poBM9MvrIXEIHoahIYVw3UqWCLvFFL6Cb+u3zrOTaNmCNyXdZ4H/28sskfuBtVzXjllzwEkrcJg0NfSmCbjw/9YFUYEdl1ZTUL40pN8Kuk1Wr1f/wP+wk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        //TODO For release
        //private const int _TrialMaxCountDocuments = 1000;
        //private const int _TrialDurationInDays = 30;

        private const int _TrialMaxCountDocuments = int.MaxValue;
        private const int _TrialDurationInDays = Int16.MaxValue;

        #region Convert
        private byte[] GetBytesByData(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        private byte[] GetBytesByBase64(string data)
        {
            return Convert.FromBase64String(data);
        }

        #endregion Convert

        public Licences()
        {

        }



        public void Verify(string regCode, LicenceInfo licence, IEnumerable<DatabaseModel> dbs)
        {
            if (!VerifyLicenceKey(regCode, licence.LicenceKey))
            {
                if (dbs?.Count() > 0)
                {
                    try
                    {
                        VerifyTrialMaxDocumentCount(licence, dbs);
                    }
                    catch (DatabaseIsNotSet)
                    {

                    }
                    catch (LicenceError)
                    {
                        throw new LicenceError();
                    }
                }
            }

            VerifyLicenceInfo(licence);
        }

        private bool VerifyLicenceKey(string regCode, string licKey)
        {
            try
            {
                if (string.IsNullOrEmpty(regCode) || string.IsNullOrEmpty(licKey))
                    return false;

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
        private void VerifyTrialMaxDocumentCount(LicenceInfo licence, IEnumerable<DatabaseModel> dbs)
        {
            //TODO Проверить количество документов у клиента
            //TODO оптимизировать
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            foreach (var db in dbs)
            {
                var ctx = new AdminContext(db);
                docProc.GetCountDocuments(ctx, licence);
            }

            if (licence.CountDocument > _TrialMaxCountDocuments || (licence.DateFirstDocument ?? DateTime.Now).AddDays(_TrialDurationInDays) < DateTime.Now)
            {
                throw new LicenceError();
            }
        }
        private void VerifyLicenceInfo(LicenceInfo licence)
        {
            if (licence.IsDateLimit
                && licence.FirstStart > DateTime.Now
                && licence.FirstStart.AddDays(licence.DateLimit.GetValueOrDefault()) > DateTime.Now)
            {
                throw new LicenceExpired();
            }

            if (licence.IsNamedLicence
                && licence.NamedNumberOfConnectionsNow > licence.NamedNumberOfConnections)
            {
                throw new LicenceExceededNumberOfRegisteredUsers();
            }

            if (licence.IsConcurenteLicence
                && licence.ConcurenteNumberOfConnectionsNow > licence.ConcurenteNumberOfConnections)
            {
                throw new LicenceExceededNumberOfConnectedUsers();
            }

            //TODO ограниченная по функционалу - вкл./выкл.
            if (licence.IsFunctionals)
            {

            }

        }
    }
}