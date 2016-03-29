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

        public FrontDocumentTask GetDocumentTaskById(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                //TODO: Refactoring
                var item = dbContext.DocumentTasksSet
                    .Where(x => x.Id == id)
                    .Select(x => new { Task = x })
                    .Select(x => new FrontDocumentTask
                    {
                        Id = x.Task.Id,
                        DocumentId = x.Task.DocumentId,
                        Name = x.Task.Task,
                        Description = x.Task.Description,
                        DocumentDate = x.Task.Document.RegistrationDate ?? x.Task.Document.CreateDate,
                        RegistrationFullNumber = (x.Task.Document.RegistrationNumber != null
                                           ? x.Task.Document.RegistrationNumberPrefix + x.Task.Document.RegistrationNumber +
                                             x.Task.Document.RegistrationNumberSuffix
                                           : "#" + x.Task.Document.Id),
                        DocumentDescription = x.Task.Document.Description,
                        DocumentTypeName = x.Task.Document.TemplateDocument.DocumentType.Name,
                        DocumentDirectionName = x.Task.Document.TemplateDocument.DocumentDirection.Name,

                        PositionId = x.Task.PositionId,
                        PositionExecutorAgentId = x.Task.PositionExecutorAgentId,
                        AgentId = x.Task.AgentId,

                        PositionExecutorAgentName = x.Task.PositionExecutorAgent.Name,
                        AgentName = x.Task.Agent.Name,
                        PositionName = x.Task.Position.Name,
                        PositionExecutorNowAgentName = x.Task.Position.ExecutorAgent.Name,
                        PositionExecutorAgentPhoneNumber = "SourcePositionAgentPhoneNumber", //TODO 
                    }).FirstOrDefault();

                return item;
            }
        }
        #endregion DocumentTasks         
    }
}