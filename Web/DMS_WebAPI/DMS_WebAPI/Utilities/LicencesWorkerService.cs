using System;
using System.Threading.Tasks;
using System.Threading;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Utilities
{
    public class LicencesWorkerService
    {
        private Task _initializeThread;
        private Timer _timer;
        private int _VERIFY_LICENCES_TIMEOUT_MIN = 1;

        public LicencesWorkerService()
        {
        }

        public void Initialize()
        {
            _initializeThread = Task.Factory.StartNew(InitializeServers);
        }

        protected void InitializeServers()
        {
            try
            {
                Dispose();
            }
            catch
            {
                // ignored
            }

            try
            {
                // start timer only once. Do not do it regulary in case we don't know how much time sending of email take. So we can continue sending only when previous iteration was comlete
                _timer = new Timer(VerifyLicences, null, _VERIFY_LICENCES_TIMEOUT_MIN * 60000, Timeout.Infinite);
            }
            catch (Exception ex)
            {
            }
        }

        private void VerifyLicences(object param)
        {
            try
            {
                var webProc = new WebAPIDbProcess();

                var userContext = DmsResolver.Current.Get<UserContexts>();

                var clients = webProc.GetClients(null);

                foreach (var client in clients)
                {
                    var dbs =
                        webProc.GetServersByAdmin(new BL.Model.WebAPI.Filters.FilterAdminServers
                        {
                            ClientIds = new System.Collections.Generic.List<int> {client.Id}
                        });
                    userContext.VerifyLicence(client.Id, dbs);
                }

                DmsResolver.Current.Get<UserContexts>().RemoveByTimeout();
            }
            catch (Exception ex)
            {
            }
            _timer.Change(_VERIFY_LICENCES_TIMEOUT_MIN*60000, Timeout.Infinite); //start new iteration of the timer
        }

        public void Dispose()
        {
            try
            {
                if (_timer != null)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timer.Dispose();
                }
            }
            catch
            {

            }
        }
    }
}
