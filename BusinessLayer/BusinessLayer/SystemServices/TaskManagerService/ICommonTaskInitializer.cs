using System.Collections.Generic;
using BL.Model.Context;

namespace BL.Logic.SystemServices.TaskManagerService
{
    public interface ICommonTaskInitializer
    {
        void InitWorkersForClient(DatabaseModelForAdminContext dbModel);
        void RemoveWorkersForClient(int clientId);

        void InitializeAutoPlanTask(List<DatabaseModelForAdminContext> dbs);
        void InitializeClearTrashTask(List<DatabaseModelForAdminContext> dbs);
        void InitializeMailWorkerTask(List<DatabaseModelForAdminContext> dbs);
    }
}