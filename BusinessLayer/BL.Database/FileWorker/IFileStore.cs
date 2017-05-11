using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Database.FileWorker
{
    public interface IFileStore
    {
        string SaveFile(IContext ctx, InternalTemplateDocumentFile attFile, bool isOverride = true);
        bool CreatePdfFile(IContext ctx, InternalTemplateDocumentFile attFile, bool isOverride = true);
        bool RenameFile(IContext ctx, InternalTemplateDocumentFile attFile, string newName);
        byte[] GetFile(IContext ctx, InternalTemplateDocumentFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile);
        byte[] GetFile(IContext ctx, FrontTemplateDocumentFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile);
        byte[] GetFile(IContext ctx, FrontDocumentFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile);
        void DeleteAllFileInDocument(IContext ctx, int documentId);
        void DeleteAllFileInTemplate(IContext ctx, int templateId);
        void DeleteFile(IContext ctx, InternalTemplateDocumentFile attFile);
        void DeleteFileVersion(IContext ctx, InternalDocumentFile attFile);
        void DeletePdfCopy(IContext ctx, InternalTemplateDocumentFile attFile);
        void CopyFile(IContext ctx, InternalTemplateDocumentFile fromTempl, InternalTemplateDocumentFile toTempl);
        bool IsFileCorrect(IContext ctx, InternalDocumentFile docFile);
        //IEnumerable<InternalTemplateAttachedFile> CopyFiles(IContext ctx, IEnumerable<InternalTemplateAttachedFile> fromTempl,int newDocNumber, bool newIsTemplate = false);

        string GetFullTemplateReportFilePath(IContext ctx, EnumReportTypes reportType);
    }
}