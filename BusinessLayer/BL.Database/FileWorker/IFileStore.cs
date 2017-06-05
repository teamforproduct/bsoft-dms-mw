using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Database.FileWorker
{
    public interface IFileStore
    {
        string SaveFile(IContext ctx, InternalTemplateFile attFile, bool isOverride = true);
        bool CreatePdfFile(IContext ctx, InternalTemplateFile attFile, bool isOverride = true);
        bool RenameFile(IContext ctx, InternalTemplateFile attFile, string newName);
        byte[] GetFile(IContext ctx, InternalTemplateFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile);
        byte[] GetFile(IContext ctx, FrontTemplateFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile);
        byte[] GetFile(IContext ctx, FrontDocumentFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile);
        void DeleteAllFileInDocument(IContext ctx, int documentId);
        void DeleteAllFileInTemplate(IContext ctx, int templateId);
        void DeleteFile(IContext ctx, InternalTemplateFile attFile);
        void DeleteFileVersion(IContext ctx, InternalDocumentFile attFile);
        void DeletePdfCopy(IContext ctx, InternalTemplateFile attFile);
        void CopyFile(IContext ctx, InternalTemplateFile fromTempl, InternalTemplateFile toTempl);
        bool IsFileCorrect(IContext ctx, InternalDocumentFile docFile);
        //IEnumerable<InternalTemplateFile> CopyFiles(IContext ctx, IEnumerable<InternalTemplateFile> fromTempl,int newDocNumber, bool newIsTemplate = false);

        string GetFullTemplateReportFilePath(IContext ctx, EnumReportTypes reportType);
    }
}