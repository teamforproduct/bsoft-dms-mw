﻿using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class SendForInformationDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IDocumentOperationsDbProcess _operationDb;

        public SendForInformationDocumentCommand(IDocumentsDbProcess documentDb, IDocumentOperationsDbProcess operationDb)
        {
            _documentDb = documentDb;
            _operationDb = operationDb;
        }

        private InternalDocumentSendList Model
        {
            get
            {
                if (!(_param is InternalDocumentSendList))
                {
                    throw new WrongParameterTypeError();
                }
                var model = (InternalDocumentSendList)_param;
                if (model.SendType != EnumSendTypes.SendForInformationExternal && model.SendType != EnumSendTypes.SendForInformation && model.SendType != EnumSendTypes.SendForConsideration)
                {
                    throw new WrongParameterTypeError();
                }
                return (InternalDocumentSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return false;
        }

        public override bool CanExecute()
        {
            _context.SetCurrentPosition(Model.SourcePositionId);
            _admin.VerifyAccess(_context, CommandType);   //TODO без позиций
            _document = _documentDb.GetBlankInternalDocumentById(_context, Model.DocumentId);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (Model.StartEventId != null || Model.CloseEventId != null)
            {
                throw new PlanPointHasAlredyBeenLaunched();
            }
            CommonDocumentUtilities.PlanDocumentPaperFromSendList(_context, _document, Model);
            return true;
        }
        public override object Execute()
        {
            if (Model.TargetPositionId.HasValue)
            {
                _document.Accesses = CommonDocumentUtilities.GetNewDocumentAccesses(_context, Model.DocumentId, Model.AccessLevel, Model.TargetPositionId.Value);
            }
            if (Model.IsAddControl)
            {
                _document.Waits=CommonDocumentUtilities.GetNewDocumentWaits(_context, Model, EnumEventTypes.ControlOn, EnumEventCorrespondentType.FromSourceToSource);
            }

            Model.CloseEvent = Model.StartEvent = CommonDocumentUtilities.GetNewDocumentEvent(_context, Model);
            CommonDocumentUtilities.SetLastChange(_context, Model);
            _document.SendLists = new List<InternalDocumentSendList> { Model };

            _operationDb.SendBySendList(_context, _document);

            return null;
        }

        private EnumEventTypes _eventType => (EnumEventTypes)Enum.Parse(typeof(EnumEventTypes), Model.SendType.ToString());

        public override EnumDocumentActions CommandType => (EnumDocumentActions)Enum.Parse(typeof(EnumDocumentActions), Model.SendType.ToString());
    }
}