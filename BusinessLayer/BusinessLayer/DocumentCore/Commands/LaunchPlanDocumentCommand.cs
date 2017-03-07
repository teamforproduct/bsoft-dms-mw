using BL.Logic.Common;
using BL.Database.Documents.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using System.Linq;

namespace BL.Logic.DocumentCore.Commands
{
    public class LaunchPlanDocumentCommand : BaseDocumentCommand
    {
        private readonly IDocumentsDbProcess _documentDb;

        public LaunchPlanDocumentCommand(IDocumentsDbProcess documentDb)
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
                return (int) _param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            if ((_document.Accesses?.Count() ?? 0) != 0 && !_document.Accesses.Any(x => x.PositionId == positionId && x.IsInWork))
                return false;
            if (_document.ExecutorPositionId != positionId
                || _document.IsLaunchPlan
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
            _context.SetCurrentPosition(_document.ExecutorPositionId);
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
            _document.IsLaunchPlan = true;
            _document.Events = CommonDocumentUtilities.GetNewDocumentEvents(_context, (int)EnumEntytiTypes.Document, _document.Id, EnumEventTypes.LaunchPlan);
            _documentDb.ChangeIsLaunchPlanDocument(_context, _document);

            DmsResolver.Current.Get<IAutoPlanService>().ManualRunAutoPlan(_context, null, _document.Id);

            return Model;
        }

    }
}