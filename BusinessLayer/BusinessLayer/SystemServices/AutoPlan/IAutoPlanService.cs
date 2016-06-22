using System;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemServices.MailWorker;

namespace BL.Logic.SystemServices.AutoPlan
{
    public interface IAutoPlanService : ISystemWorkerService, IDisposable
    {
        bool ManualRunAutoPlan(IContext userContext, int? sendListId = null, int? documentId = null);
    }
}