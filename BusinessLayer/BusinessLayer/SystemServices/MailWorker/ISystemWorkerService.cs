using System.Collections.Generic;
using BL.Model.Database;

namespace BL.Logic.SystemServices.MailWorker
{
    public interface ISystemWorkerService
    {
        void Initialize(IEnumerable<DatabaseModel> dbList);
    }
}