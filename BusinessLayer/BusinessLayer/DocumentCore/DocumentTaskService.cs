using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.SystemCore;

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