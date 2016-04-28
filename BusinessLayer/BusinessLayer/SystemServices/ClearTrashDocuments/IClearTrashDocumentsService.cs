using System;
using BL.Logic.SystemServices.MailWorker;

namespace BL.Logic.SystemServices.ClearTrashDocuments
{
    public interface IClearTrashDocumentsService : ISystemWorkerService, IDisposable
    {
    }
}