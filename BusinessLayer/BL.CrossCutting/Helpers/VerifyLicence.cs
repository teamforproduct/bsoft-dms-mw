using BL.CrossCutting.CryptographicWorker;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Constants;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BL.CrossCutting.Helpers
{
    public static class VerifyLicence
    {
        public static void Verify(string regCode, LicenceInfo licence)
        {
            var cryptoService = DmsResolver.Current.Get<ICryptoService>();

            if (!cryptoService.VerifyLicenceKey(regCode, licence.LicenceKey))
            {
                throw new LicenceError();
            }

            Verify(licence);
        }

        public static void Verify(LicenceInfo licence)
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