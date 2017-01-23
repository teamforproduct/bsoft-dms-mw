using System.Collections.Generic;
using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.AdditionalCommands
{
    public class AddDocumentTaskCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentTask _task;

        public AddDocumentTaskCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private AddDocumentTasks Model
        {
            get
            {
                if (!(_param is AddDocumentTasks))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddDocumentTasks)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);
            _document = _operationDb.ModifyDocumentTaskPrepare(_context, null, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_document.Tasks.Any())
            {
                throw new RecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.GetDocumentTaskOrCreateNew(_context, _document, Model.Name, _context.CurrentPositionId, Model.Description);
            Document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, null, EnumEventTypes.TaskFormulation, null, Model.Name+" / "+Model.Description);
            return _operationDb.AddDocumentTasks(_context, _document).ToList(); 
        }

    }
}