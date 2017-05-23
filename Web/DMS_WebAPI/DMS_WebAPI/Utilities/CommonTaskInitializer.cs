using System;
using System.Collections.Generic;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.TaskManagerService;
using BL.Model.Context;

namespace DMS_WebAPI.Utilities
{
    public static class WebCommonTaskInitializer
    {
        private const int _FAST_TIMEOUT_MIN = 1;
        private const int _SLOW_TIMEOUT_MIN = 60;
        private const int _VERIFY_LICENCES_TIMEOUT_MIN = 1;

        public static void AddAuthWorker()
        {
            var taskMngr = DmsResolver.Current.Get<ITaskManager>();

            //auth worker fast
            taskMngr.AddTask(_FAST_TIMEOUT_MIN, (context, param) =>
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
            });

            //auth worker slow
            taskMngr.AddTask(_SLOW_TIMEOUT_MIN, (context, param) =>
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
            });

            //Licence worker
            taskMngr.AddTask(_VERIFY_LICENCES_TIMEOUT_MIN, (context, param) =>
            {
                try
                {
                    var webProc = DmsResolver.Current.Get<WebAPIDbProcess>();

                    var userContext = DmsResolver.Current.Get<UserContexts>();

                    var clients = webProc.GetClients(null);

                    foreach (var client in clients)
                    {
                        var db = webProc.GetClientServer(client.Id);

                        userContext.VerifyLicence(client.Id, new List<DatabaseModelForAdminContext> { db });
                    }
                }
                catch (Exception ex)
                {
                }
            });

        }
    }
}