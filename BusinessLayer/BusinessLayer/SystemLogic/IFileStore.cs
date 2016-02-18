using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Logic.SystemLogic
{
    public interface IFileStore
    {
        string SaveFile(IContext ctx, DocumentAttachedFile attFile, bool isOverride = true);
        byte[] GetFile(IContext ctx, DocumentAttachedFile attFile);
        void DeleteFile(IContext ctx, DocumentAttachedFile attFile);
    }
}