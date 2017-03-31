using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Database;
using BL.Model.Exception;
using BL.Model.SystemCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BL.Database.DatabaseContext;
using Ninject;
using Ninject.Parameters;

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



        public object Verify(string regCode, LicenceInfo licence, IEnumerable<DatabaseModel> dbs, bool isThrowException)
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
                        if (isThrowException)
                        {
                            throw new LicenceError();
                        }
                        else
                        {
                            return new LicenceError();
                        }
                    }
                }
            }

            return VerifyLicenceInfo(licence, isThrowException);
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

            if (licence.CountDocument > _TrialMaxCountDocuments || (licence.DateFirstDocument ?? DateTime.UtcNow).AddDays(_TrialDurationInDays) < DateTime.UtcNow)
            {
                throw new LicenceError();
            }
        }
        private object VerifyLicenceInfo(LicenceInfo licence, bool isThrowException)
        {
            if (licence.IsDateLimit
                && licence.FirstStart > DateTime.UtcNow
                && licence.FirstStart.AddDays(licence.DateLimit.GetValueOrDefault()) > DateTime.UtcNow)
            {
                if (isThrowException)
                {
                    throw new LicenceExpired();
                }
                else
                {
                    return new LicenceExpired();
                }
            }

            if (licence.IsNamedLicence
                && licence.NamedNumberOfConnectionsNow > licence.NamedNumberOfConnections)
            {
                if (isThrowException)
                {
                    throw new LicenceExceededNumberOfRegisteredUsers();
                }
                else
                {
                    return new LicenceExceededNumberOfRegisteredUsers();
                }
            }

            if (licence.IsConcurenteLicence
                && licence.ConcurenteNumberOfConnectionsNow > licence.ConcurenteNumberOfConnections)
            {
                if (isThrowException)
                {
                    throw new LicenceExceededNumberOfConnectedUsers();
                }
                else
                {
                    return new LicenceExceededNumberOfConnectedUsers();
                }
            }

            //TODO ограниченная по функционалу - вкл./выкл.
            if (licence.IsFunctionals)
            {

            }

            return null;
        }
    }
}