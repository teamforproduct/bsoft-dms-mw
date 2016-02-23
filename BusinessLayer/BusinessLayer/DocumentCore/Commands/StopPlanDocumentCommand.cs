using BL.CrossCutting.Common;
using BL.Database.Admins.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DocumentCore.Commands
{
    public class StopPlanDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;
        private readonly IAdminsDbProcess _adminDb;

        public StopPlanDocumentCommand(IDocumentsDbProcess documentDb, IAdminsDbProcess adminDb)
        {
            _documentDb = documentDb;
            _adminDb = adminDb;
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

        public override bool CanBeDisplayed()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            try
            {
                _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });
                if (_document == null || _document.ExecutorPositionId != _context.CurrentPositionId)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool CanExecute()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            _document = _documentDb.ChangeIsLaunchPlanDocumentPrepare(_context, Model);
            if (_document == null)
            {
                throw new DocumentNotFoundOrUserHasNoAccess();
            }
            if (!_document.IsLaunchPlan)
            {
                throw new CouldNotChangeAttributeLaunchPlan();
            }
            _context.SetCurrentPosition(_document.ExecutorPositionId);
            _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = CommandType.ToString() });
            return true;

        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChange(_context, _document);
            _document.IsLaunchPlan = false;
            _documentDb.ChangeIsLaunchPlanDocument(_context, _document);

            return Model;
        }

        public override EnumDocumentActions CommandType => EnumDocumentActions.StopPlan;
    }
}