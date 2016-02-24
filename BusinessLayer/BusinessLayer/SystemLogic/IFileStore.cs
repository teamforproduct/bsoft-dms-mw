using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Logic.SystemLogic
{
    public interface IFileStore
    {
        string SaveFile(IContext ctx, InternalDocumentAttachedFile attFile, bool isOverride = true);
        byte[] GetFile(IContext ctx, InternalDocumentAttachedFile attFile);
        void DeleteAllFileInDocument(IContext ctx, int documentId);
        void DeleteFile(IContext ctx, InternalDocumentAttachedFile attFile);
        void DeleteFileVersion(IContext ctx, InternalDocumentAttachedFile attFile);
    }
}