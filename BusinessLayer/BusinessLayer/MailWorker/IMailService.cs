using System.Collections.Generic;
using BL.Model.Database;

namespace BL.Logic.MailWorker
{
    public interface IMailService
    {
        void Initialize(IEnumerable<DatabaseModel> dbList);
    }
}