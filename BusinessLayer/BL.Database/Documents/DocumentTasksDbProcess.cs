using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.DocumentCore.Filters;

namespace BL.Database.Documents
{
    public class DocumentTasksDbProcess : IDocumentTasksDbProcess
    {
        public DocumentTasksDbProcess()
        {
        }
        #region DocumentTasks

        public IEnumerable<FrontDocumentTask> GetDocumentTasks(IContext ctx, FilterDocumentTask filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentTasks(dbContext, filter);
            }
        }

        public FrontDocumentTask GetDocumentTask(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentTasks(dbContext, new FilterDocumentTask { Id = new List<int> { id } }).FirstOrDefault();
            }
        }
        #endregion DocumentTasks         
    }
}