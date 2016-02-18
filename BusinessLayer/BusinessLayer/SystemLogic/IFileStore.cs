using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Logic.SystemLogic
{
    public interface IFileStore
    {
        string SaveFile(IContext ctx, FrontDocumentAttachedFile attFile, bool isOverride = true);
        byte[] GetFile(IContext ctx, FrontDocumentAttachedFile attFile);
        void DeleteFile(IContext ctx, FrontDocumentAttachedFile attFile);
    }
}