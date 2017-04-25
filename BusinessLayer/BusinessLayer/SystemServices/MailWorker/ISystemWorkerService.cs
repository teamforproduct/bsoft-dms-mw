using BL.Model.Context;
using System.Collections.Generic;

namespace BL.Logic.SystemServices.MailWorker
{
    public interface ISystemWorkerService
    {
        void Initialize(IEnumerable<DatabaseModelForAdminContext> dbList);
    }
}