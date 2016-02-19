using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Logic.SystemLogic
{
    public interface IFileStore
    {
        string SaveFile(IContext ctx, FrontFilterDocumentAttachedFile attFile, bool isOverride = true);
        byte[] GetFile(IContext ctx, FrontFilterDocumentAttachedFile attFile);
        void DeleteFile(IContext ctx, FrontFilterDocumentAttachedFile attFile);
    }
}