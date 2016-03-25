using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.PaperCommands
{
    public class ModifyDocumentPaperCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        private InternalDocumentPaper _item;

        public ModifyDocumentPaperCommand(IDocumentOperationsDbProcess operationDb)
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
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType);

            _item = _operationDb.ChangeDocumentPaperPrepare(_context, Model.Id);

            //TODO Проверить поля которые нужно обновлять
            _item.DocumentId = Model.DocumentId;
            _item.Name = Model.Name;
            _item.Description = Model.Description;
            _item.IsMain = Model.IsMain;
            _item.IsOriginal = Model.IsOriginal;
            _item.IsCopy = Model.IsCopy;
            _item.PageQuantity = Model.PageQuantity;
            _item.OrderNumber = Model.OrderNumber;
            _item.LastPaperEventId = Model.LastPaperEventId;

            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _item);
            _operationDb.ModifyDocumentPaper(_context, _item);
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.ModifyDocumentPaper;
    }
}