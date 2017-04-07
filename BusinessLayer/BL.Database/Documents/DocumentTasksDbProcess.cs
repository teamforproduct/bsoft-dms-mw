using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.SystemCore;
using BL.CrossCutting.Helpers;

namespace BL.Database.Documents
{
    public class DocumentTasksDbProcess : IDocumentTasksDbProcess
    {
        public DocumentTasksDbProcess()
        {
        }
        #region DocumentTasks

        public IEnumerable<FrontDocumentTask> GetDocumentTasks(IContext ctx, FilterDocumentTask filter, UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentTasks(ctx, filter, paging);
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentTask GetDocumentTask(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentTasks(ctx, new FilterDocumentTask { Id = new List<int> { id } }, null).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }
        #endregion DocumentTasks         
    }
}