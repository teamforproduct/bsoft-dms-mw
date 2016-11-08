﻿using System;
using System.Threading.Tasks;
using System.Threading;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Очищает устаревшие пользовательские контексты из коллекции UserContexts методом RemoveByTimeout
    /// Пользовательский контекст удаляется если не используется 1 минуту
    /// </summary>
    public class UserContextsWorkerService
    {
        private Task _initializeThread;
        private Timer _timer;
        private int _USER_CONTEXT_TIMEOUT_MIN = 1;
        public UserContextsWorkerService()
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
                DmsResolver.Current.Get<UserContexts>().RemoveByTimeout();
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