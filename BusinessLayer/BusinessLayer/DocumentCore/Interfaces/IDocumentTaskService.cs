using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.Filters;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentTaskService
    {
        #region DocumentTasks
        FrontDocumentTask GetDocumentTask(IContext context, int taskId);
        IEnumerable<FrontDocumentTask> GetTasks(IContext context, FilterDocumentTask filter);
        #endregion DocumentTasks         
    }
}