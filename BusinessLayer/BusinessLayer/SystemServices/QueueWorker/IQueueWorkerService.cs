using System;
using BL.CrossCutting.Context;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemServices.MailWorker;

namespace BL.Logic.SystemServices.QueueWorker
{
    public interface IQueueWorkerService : ISystemWorkerService, IDisposable
    {
        void AddNewClient(AdminContext ctx);

        void RemoveWorkersForClient(int clientId);
        void AddNewTask(IContext ctx, ICommand command);
        void AddNewTask(IContext ctx, Action command);
        void AddNewTask(IContext ctx, QueueTask command);
    }
}