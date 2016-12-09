using System.Linq;
using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.Commands
{
    public class VerifySigningDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentOperationsDbProcess _operationDb;

        //private InternalDocumentWait _docWait;

        public VerifySigningDocumentCommand(IDocumentOperationsDbProcess operationDb)
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
            if (_document.Accesses?.Count() != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            return true;
        }

        public override bool CanExecute()
        {
            _document = _operationDb.SelfAffixSigningDocumentPrepare(_context, Model);
            //_docWait = _document?.Waits.FirstOrDefault();
            //throw new CouldNotPerformOperation();
            //_operationDb.ControlOffSendListPrepare(_context, _document);
            //_operationDb.ControlOffSubscriptionPrepare(_context, _document);

            //if (Model.CurrentPositionId.HasValue)
            //{
            //    _context.SetCurrentPosition(Model.CurrentPositionId.Value);
            //}
            //else
            //{
            //    _context.SetCurrentPosition(_document.ExecutorPositionId);
            //}

            //_admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            var isUseCertificateSign = GetIsUseCertificateSign();
            
            _operationDb.VerifySigningDocument(_context, Model, GetIsUseInternalSign(), isUseCertificateSign);
            return _document.Id;
        }

    }
}