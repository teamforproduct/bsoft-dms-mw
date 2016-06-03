using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.SystemCore;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentTaskService
    {
        #region DocumentTasks
        FrontDocumentTask GetDocumentTask(IContext context, int taskId);
        IEnumerable<FrontDocumentTask> GetTasks(IContext context, FilterDocumentTask filter, UIPaging paging);
        #endregion DocumentTasks         
    }
}