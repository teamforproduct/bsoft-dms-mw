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

        private InternalDocumentPaperList _item;

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
            _admin.VerifyAccess(_context, CommandType);

            _item = _operationDb.ChangeDocumentPaperListPrepare(_context, Model.Id);

            //TODO Проверить поля которые нужно обновлять
            _item.Date = Model.Date;
            _item.Description = Model.Description;

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _item);
            _operationDb.ModifyDocumentPaperList(_context, _item);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifyDocumentPaperList;
    }
}