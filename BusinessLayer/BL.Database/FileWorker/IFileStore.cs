using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.Database.FileWorker
{
    public interface IFileStore
    {
        string SaveFile(IContext ctx, InternalTemplateAttachedFile attFile, bool isOverride = true);
        byte[] GetFile(IContext ctx, InternalTemplateAttachedFile attFile);
        byte[] GetFile(IContext ctx, FrontTemplateAttachedFile attFile);
        byte[] GetFile(IContext ctx, FrontDocumentAttachedFile attFile);
        void DeleteAllFileInDocument(IContext ctx, int documentId);
        void DeleteAllFileInTemplate(IContext ctx, int templateId);
        void DeleteFile(IContext ctx, InternalTemplateAttachedFile attFile);
        void DeleteFileVersion(IContext ctx, InternalDocumentAttachedFile attFile);
        void CopyFile(IContext ctx, InternalTemplateAttachedFile fromTempl, InternalTemplateAttachedFile toTempl);
        bool IsFileCorrect(IContext ctx, InternalDocumentAttachedFile docFile);
        //IEnumerable<InternalTemplateAttachedFile> CopyFiles(IContext ctx, IEnumerable<InternalTemplateAttachedFile> fromTempl,int newDocNumber, bool newIsTemplate = false);

        string GetFullTemplateReportFilePath(IContext ctx, EnumReportTypes reportType);
    }
}