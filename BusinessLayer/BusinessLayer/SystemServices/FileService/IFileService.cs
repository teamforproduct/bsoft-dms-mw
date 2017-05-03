using BL.CrossCutting.Interfaces;
using BL.Model.Enums;

namespace BL.Logic.SystemServices.FileService
{
    public interface IFileService
    {
        string GetFileUri(string serverUrl, IContext ctx, EnumDocumentFileType fileType, int id);
        byte[] GetFile(IContext ctx, EnumDocumentFileType fileType, int id);
    }
}