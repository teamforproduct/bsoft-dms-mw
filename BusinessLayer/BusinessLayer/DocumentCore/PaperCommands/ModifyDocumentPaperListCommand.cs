using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.PaperCommands
{
    public class ModifyDocumentPaperListCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        InternalDocumentPaperList _paperList;

        public ModifyDocumentPaperListCommand(IDocumentOperationsDbProcess operationDb)
        {
            _operationDb = operationDb;
        }

        private ModifyDocumentPaperLists Model
        {
            get
            {
                if (!(_param is ModifyDocumentPaperLists))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocumentPaperLists)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _paperList = _operationDb.ModifyDocumentPaperListPrepare(_context, Model.Id);
            if (_paperList==null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_paperList.SourcePositionId.Value);
            _admin.VerifyAccess(_context, CommandType);

            return true;
        }

        public override object Execute()
        {
            _paperList.Description = Model.Description;
            CommonDocumentUtilities.SetLastChange(_context, _paperList);
            _operationDb.ModifyDocumentPaperList(_context, _paperList);
            return null;
        }

    }
}