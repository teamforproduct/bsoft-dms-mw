using System;
using System.Threading.Tasks;
using System.Threading;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Обслуживает авторизационную базу:
    /// Очищает устаревшие пользовательские контексты
    /// Очищает неподтвержденные заявки на создание новых клиентов
    /// Очищает устаревшие токены
    /// </summary>
    public class AuthWorkerService
    {
        private Task _initializeThread;
        private Timer _timer_Fast;
        private Timer _timer_Slow;
        private int _FAST_TIMEOUT_MIN = 1;
        private int _SLOW_TIMEOUT_MIN = 60;

        public AuthWorkerService()
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
                _timer_Fast = new Timer(DoSomethingOften, null, _FAST_TIMEOUT_MIN*60000, Timeout.Infinite);
                _timer_Slow = new Timer(DoSomethingRarely, null, _SLOW_TIMEOUT_MIN * 60000, Timeout.Infinite);
            }
            catch (Exception ex)
            {
            }
        }

        private void DoSomethingOften(object param)
        {
            try
            {
                var contexts = DmsResolver.Current.Get<UserContexts>();
                contexts.SaveLogContextsLastUsage();
                contexts.RemoveByTimeout();
            }
            catch (Exception ex)
            {
            }
            _timer_Fast.Change(_FAST_TIMEOUT_MIN*60000, Timeout.Infinite); //start new iteration of the timer
        }


        private void DoSomethingRarely(object param)
        {
            try
            {
                var service = DmsResolver.Current.Get<WebAPIService>();
                service.DeleteOldClientRequest();
                service.DeleteOldUserContexts();
            }
            catch (Exception ex)
            {
            }
            _timer_Slow.Change(_SLOW_TIMEOUT_MIN * 60000, Timeout.Infinite); //start new iteration of the timer
        }
        public void Dispose()
        {
            try
            {
                if (_timer_Fast != null)
                {
                    _timer_Fast.Change(Timeout.Infinite, Timeout.Infinite);
                    _timer_Slow.Change(Timeout.Infinite, Timeout.Infinite);

                    _timer_Fast.Dispose();
                    _timer_Slow.Dispose();
                }
            }
            catch
            {

            }
        }
    }
}
