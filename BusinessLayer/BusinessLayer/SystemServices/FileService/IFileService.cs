using System.Threading.Tasks;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;

namespace BL.Logic.SystemServices.FileService
{
    public interface IFileService
    {
        string GetFileUri(EnumDocumentFileType fileType, int id);
        Task<FrontDocumentFile> GetFile(IContext ctx, EnumDocumentFileType fileType, int id);
        string GetMimetype(string fileExt);
    }
}