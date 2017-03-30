using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DocumentCore
{
    public class DocumentTaskService : IDocumentTaskService
    {
        private readonly IDocumentTasksDbProcess _documentDb;

        public DocumentTaskService(IDocumentTasksDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

        #region DocumentTasks
        public FrontDocumentTask GetDocumentTask(IContext context, int taskId)
        {
            return _documentDb.GetDocumentTask(context, taskId);
        }

        public IEnumerable<FrontDocumentTask> GetTasks(IContext context, FilterDocumentTask filter, UIPaging paging)
        {
            return _documentDb.GetDocumentTasks(context, filter, paging).ToList();
        }

        #endregion DocumentTasks         
    }
}