using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.PaperCommands
{
    public class AddDocumentPaperCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        public AddDocumentPaperCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private AddDocumentPaper Model
        {
            get
            {
                if (!(_param is AddDocumentPaper))
                {
                    throw new WrongParameterTypeError();
                }
                return (AddDocumentPaper)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            /*
            if (_document.ExecutorPositionId != positionId
                )
            {
                return false;
            }
            */
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.ModifyDocumentPaperPrepare(_context, null, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (!Model.IsCopy && _document.ExecutorPositionId != _context.CurrentPositionId)
            {
                throw new CouldNotPerformOperationWithPaper();
            }
            _adminProc.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _document.Papers = CommonDocumentUtilities.GetNewDocumentPapers(_context, (int)EnumEntytiTypes.Document, Model, _document.MaxPaperOrderNumber ?? 0);
            return _operationDb.AddDocumentPapers(_context, _document.Papers).ToList();
        }

    }
}