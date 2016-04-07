using System.Collections.Generic;
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

        private ModifyDocumentPapers Model
        {
            get
            {
                if (!(_param is ModifyDocumentPapers))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentPapers)_param;
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
            _document = _operationDb.AddDocumentPaperPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (Model.IsCopy)
            {
                _context.SetCurrentPosition(Model.CurrentPositionId);
            }
            else
            {
                _context.SetCurrentPosition(_document.ExecutorPositionId);
            }
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            _document.Papers = CommonDocumentUtilities.GetNewDocumentPapers(_context, Model, _document.MaxPaperOrderNumber??0);
            _operationDb.AddDocumentPapers(_context, _document.Papers);
            return null;
        }

    }
}