using BL.Logic.Context;
using BL.Model.Database;
using System;
using System.Collections.Generic;
using System.Web;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.Exception;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace DMS_WebAPI.Utilities
{
    public class UserContextWorkerService
    {
        private Task _initializeThread;
        private Timer _timer;
        private int _USER_CONTEXT_TIMEOUT_MIN = 1;
        public UserContextWorkerService()
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
                _timer = new Timer(ClearUserContext, null, _USER_CONTEXT_TIMEOUT_MIN * 60000, Timeout.Infinite);
            }
            catch (Exception ex)
            {
            }
        }

        private void ClearUserContext(object param)
       {
            try
            {
                DmsResolver.Current.Get<UserContext>().RemoveByTimeout();
            }
            catch (Exception ex)
            {
            }
            _timer.Change(_USER_CONTEXT_TIMEOUT_MIN * 60000, Timeout.Infinite);//start new iteration of the timer
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
