using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DocumentCore.InternalModel;
using System.Collections.Generic;

namespace BL.Logic.DocumentCore.Commands
{
    public class StopPlanDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;

        public StopPlanDocumentCommand(IDocumentsDbProcess documentDb)
        {
            _documentDb = documentDb;
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
            if ((_document.ExecutorPositionId != positionId && !_context.IsAdmin)
                || !_document.IsLaunchPlan
                )
            {
                return false;
            }

            return true;
        }

        public override bool CanExecute()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            _document = _documentDb.ChangeIsLaunchPlanDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (_context.IsAdmin)
            {
                _context.SetCurrentPosition((int)EnumSystemPositions.AdminPosition);
            }
            else
            {
                _context.SetCurrentPosition(_document.ExecutorPositionId);
            }
            _admin.VerifyAccess(_context, CommandType);
            if (!CanBeDisplayed(_context.CurrentPositionId))
            {
                throw new CouldNotChangeAttributeLaunchPlan();
            }

            return true;

        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.IsLaunchPlan = false;
            var ev = CommonDocumentUtilities.GetNewDocumentEvent(_context, _document.Id, EnumEventTypes.StopPlan, null,null,null,null,false, _document.ExecutorPositionId);
            _document.Events = new List<InternalDocumentEvent> { ev };
            _documentDb.ChangeIsLaunchPlanDocument(_context, _document);
            return Model;
        }

    }
}