using System.Collections.Generic;
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

        private InternalDocumentPaper _item;

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
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType);

            _item = new InternalDocumentPaper
            {
                DocumentId = Model.DocumentId,
                Name = Model.Name,
                Description = Model.Description,
                IsMain = Model.IsMain,
                IsOriginal=Model.IsOriginal,
                IsCopy = Model.IsCopy,
                PageQuantity = Model.PageQuantity,
                OrderNumber = Model.OrderNumber,
                LastPaperEventId = Model.LastPaperEventId
            };

            return true;
        }

        public override object Execute()
        {

            CommonDocumentUtilities.SetLastChange(_context, _item);
            _operationDb.AddDocumentPapers(_context, new List<InternalDocumentPaper> { _item });
            return null;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.AddDocumentPaper;
    }
}