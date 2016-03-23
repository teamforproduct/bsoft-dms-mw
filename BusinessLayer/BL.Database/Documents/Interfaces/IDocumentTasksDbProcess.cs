using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.Filters;

namespace BL.Database.Documents.Interfaces
{
    public interface IDocumentTasksDbProcess
    {
        #region DocumentTasks
        IEnumerable<FrontDocumentTask> GetDocumentTasks(IContext ctx, FilterDocumentTask filter);
        FrontDocumentTask GetDocumentTaskById(IContext ctx, int id);
        #endregion DocumentTasks  
    }
}