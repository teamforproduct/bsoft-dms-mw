using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.TemplateCommands
{
    public class DeleteTemplateCommand: BaseDocumentCommand
    {
        private readonly ITemplateDocumentsDbProcess _operationDb;

        public DeleteTemplateCommand(ITemplateDocumentsDbProcess operationDb)
        {
            _operationDb = operationDb;
           
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);

            if (!_operationDb.CanModifyTemplate(_context, Model))
            {
                throw new CouldNotModifyTemplateDocument();
            }
            return true;
        }

        public override object Execute()
        {
            _operationDb.DeleteTemplate(_context, Model);
            return null;
        }
       
    }
}