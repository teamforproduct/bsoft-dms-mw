using System.Collections.Generic;
using BL.Model.Context;

namespace BL.Logic.SystemServices.TaskManagerService
{
    public interface ICommonTaskInitializer
    {
        void InitializeAutoPlanTask(List<DatabaseModelForAdminContext> dbs);
        void InitializeClearTrashTask(List<DatabaseModelForAdminContext> dbs);
        void InitializeMailWorkerTask(List<DatabaseModelForAdminContext> dbs);
    }
}