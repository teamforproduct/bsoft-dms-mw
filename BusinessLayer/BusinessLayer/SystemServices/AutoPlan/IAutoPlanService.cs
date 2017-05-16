using BL.CrossCutting.Interfaces;

namespace BL.Logic.SystemServices.AutoPlan
{
    public interface IAutoPlanService
    {
        bool AutoPlanTask(IContext admCtx, int? sendListId = null, int? documentId = null);
        bool ManualRunAutoPlan(IContext userContext, int? sendListId = null, int? documentId = null);
    }
}