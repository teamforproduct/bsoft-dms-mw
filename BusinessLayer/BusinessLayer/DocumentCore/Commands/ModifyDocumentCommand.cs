using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.DocumentCore.Commands
{
    internal class ModifyDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;

        public ModifyDocumentCommand(IDocumentsDbProcess documentDb)
        {
            _documentDb = documentDb;
        }

        private ModifyDocument Model
        {
            get
            {
                if (!(_param is ModifyDocument))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDocument)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if (_document.ExecutorPositionId != positionId
                || _document.IsRegistered
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            _document = _documentDb.ModifyDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _admin.VerifyAccess(_context, CommandType);
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotPerformThisOperation();
            }
            return true;
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.Description = Model.Description;
            _document.DocumentSubjectId = Model.DocumentSubjectId;
            _document.SenderAgentId = Model.SenderAgentId;
            _document.SenderAgentPersonId = Model.SenderAgentPersonId;
            _document.SenderNumber = Model.SenderNumber;
            _document.SenderDate = Model.SenderDate;
            _document.Addressee = Model.Addressee;
            _document.AccessLevel = Model.AccessLevel;
            if (_document.Accesses?.Count() > 0)
            {
                var docAcc = _document.Accesses.First();
                CommonDocumentUtilities.SetLastChange(_context, docAcc);
                docAcc.AccessLevel = Model.AccessLevel;
            }

            if (Model.Properties?.Count() > 0)
            {
                _document.Properties = Model.Properties.Select(x =>
                {
                    var item = new InternalPropertyValue
                    {
                        PropertyLinkId = x.PropertyLinkId,
                        ValueString = x.Value
                    };
                    CommonDocumentUtilities.SetLastChange(_context, item);
                    return item;
                }).ToList();

                var model = new InternalPropertyValues { Object = EnumObjects.Documents, PropertyValues = _document.Properties };

                CommonSystemUtilities.VerifyPropertyValues(_context, model, new string[] { $"{nameof(_document.DocumentTypeId)}={_document.DocumentTypeId}", $"{nameof(_document.DocumentDirection)}={_document.DocumentDirection}", $"{nameof(_document.DocumentSubjectId)}={_document.DocumentSubjectId}" });

                _document.Properties = model.PropertyValues;
            }

            CommonDocumentUtilities.VerifyDocument(_context, new FrontDocument(_document), null);    //TODO отвязаться от фронт-модели

            _documentDb.ModifyDocument(_context, _document);

            return _document.Id;
        }

    }
}