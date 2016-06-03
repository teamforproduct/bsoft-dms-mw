using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.SystemCore;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentTasksDbProcess
    {
        #region DocumentTasks
        IEnumerable<FrontDocumentTask> GetDocumentTasks(IContext ctx, FilterDocumentTask filter, UIPaging paging);
        FrontDocumentTask GetDocumentTask(IContext ctx, int id);
        #endregion DocumentTasks  
    }
}