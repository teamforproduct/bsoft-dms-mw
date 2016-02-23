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
                return (int) _param;
            }
        }

        public override bool CanBeDisplayed()
        {
            //TODO ОСТАЛЬНЫЕ ПРОВЕРКИ!
            try
            {
                _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = EnumDocumentActions.StopPlan, PositionId = _context.CurrentPositionId });
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
            try
            {
                _adminDb.VerifyAccess(_context, new VerifyAccess { DocumentActionCode = EnumDocumentActions.StopPlan, PositionId = _context.CurrentPositionId });
                _document = _documentDb.PlanDocumentPrepare(_context, Model);
                if (_document == null || _document.ExecutorPositionId != _context.CurrentPositionId)
                {
                    throw new DocumentNotFoundOrUserHasNoAccess();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override object Execute()
        {
            CommonDocumentUtilities.SetLastChangeForDocument(_context, _document);

            _documentDb.StopPlanDocument(_context, Model);

            return Model;
        }

        public override EnumDocumentActions CommandType { get { return EnumDocumentActions.StopPlan; } }
    }
}