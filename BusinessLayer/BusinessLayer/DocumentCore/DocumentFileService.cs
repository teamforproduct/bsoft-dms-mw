using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Database.Documents.Interfaces;
using BL.Database.FileWorker;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Exception;
using BL.Model.SystemCore;
using System.Transactions;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Enums;

namespace BL.Logic.DocumentCore
{
    public class DocumentFileService : IDocumentFileService
    {
        private readonly IFileStore _fStore;
        private readonly IDocumentFileDbProcess _dbProcess;
        private readonly IAdminService _adminService;

        public DocumentFileService(IFileStore fStore, IDocumentFileDbProcess dbProcess, IAdminService adminService)
        {
            _fStore = fStore;
            _dbProcess = dbProcess;
            _adminService = adminService;
        }

        public IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null)
        {
//            _adminService.VerifyAccess(ctx, EnumDocumentActions.ViewDocument);
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return _dbProcess.GetDocumentFiles(ctx, filter, paging);
            }
        }

        public FrontDocumentAttachedFile GetUserFile(IContext ctx, FilterDocumentFileIdentity fileIdent)
        {
            var fl = _dbProcess.GetDocumentFileVersion(ctx, fileIdent.DocumentId, fileIdent.OrderInDocument, fileIdent.Version ?? 0);
            if (fl == null)
            {
                throw new UnknownDocumentFile();
            }
            _fStore.GetFile(ctx, fl);
            return fl;
        }

    }
}