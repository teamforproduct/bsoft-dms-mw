using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

namespace BL.Logic.DocumentCore.Interfaces
{
    public interface IDocumentFileService
    {
        IEnumerable<FrontFilterDocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId);
        IEnumerable<FrontFilterDocumentAttachedFile> GetDocumentFileVersions(IContext ctx, FilterDocumentFileIdentity fileIdent);
        FrontFilterDocumentAttachedFile GetDocumentFileVersion(IContext ctx, FilterDocumentFileIdentity fileIdent);
        FrontFilterDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id);
        FrontFilterDocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, FilterDocumentFileIdentity fileIdent);


        void DeleteDocumentFile(IContext ctx, FilterDocumentFileIdentity fileIdent);
        void DeleteDocumentFileVersion(IContext ctx, FilterDocumentFileIdentity fileIdent);
        FrontFilterDocumentAttachedFile GetUserFile(IContext ctx, FilterDocumentFileIdentity fileIdent);
        int AddUserFile(IContext ctx, ModifyDocumentFile model);
        IEnumerable<FrontFilterDocumentAttachedFile> AddUserFile(IContext ctx, ModifyDocumentFiles model);
        FrontFilterDocumentAttachedFile AddNewVersion(IContext ctx, ModifyDocumentFile model);
        FrontFilterDocumentAttachedFile UpdateCurrentFileVersion(IContext ctx, ModifyDocumentFile model);
    }
}