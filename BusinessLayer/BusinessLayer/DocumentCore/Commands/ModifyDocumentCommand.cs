using System.Linq;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;
using System;
using System.Collections.Generic;

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
                throw new CouldNotPerformOperation();
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
            _document.AccessLevel = (EnumDocumentAccesses)Model.AccessLevelId;
            if (_document.Accesses?.Count() > 0)
            {
                var docAcc = _document.Accesses.First();
                CommonDocumentUtilities.SetLastChange(_context, docAcc);
                docAcc.AccessLevel = (EnumDocumentAccesses)Model.AccessLevelId;
            }

            if (Model.Properties != null)
            {
                _document.Properties = CommonDocumentUtilities.GetNewPropertyValues(Model.Properties).ToList();
                CommonDocumentUtilities.SetLastChange(_context, _document.Properties);

                var model = new InternalPropertyValues { Object = EnumObjects.Documents, PropertyValues = _document.Properties };

                CommonSystemUtilities.VerifyPropertyValues(_context, model, CommonDocumentUtilities.GetFilterTemplateByDocument(_document).ToArray());

                _document.Properties = model.PropertyValues;
            }
            else
            {
                var model = new InternalPropertyValues { Object = EnumObjects.Documents, PropertyValues = new List<InternalPropertyValue>() };

                CommonSystemUtilities.VerifyPropertyValues(_context, model, CommonDocumentUtilities.GetFilterTemplateByDocument(_document).ToArray());
            }

            CommonDocumentUtilities.VerifyDocument(_context, new FrontDocument(_document), null);    //TODO отвязаться от фронт-модели

            _documentDb.ModifyDocument(_context, _document);

            return _document.Id;
        }

    }
}