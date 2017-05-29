using System;
using System.IO;
using System.Security.Cryptography;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Constants;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;
using BL.Model.Enums;
using PDFCreator;

namespace BL.Database.FileWorker
{
    public class FileStore : IFileStore
    {
        private string GetStorePath()
        {
            var sett = DmsResolver.Current.Get<ISettingValues>();
            return sett.GetFileStorePath();
        }

        private string GetFullDocumentFilePath(IContext ctx, FrontDocumentFile attFile)
        {
            var path = GetStorePath();
            path = Path.Combine(path, SettingConstants.FILE_STORE_DOCUMENT_FOLDER, ctx.Client.Id.ToString(), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString(), attFile.Version.ToString());
            return path;
        }

        private string GetFullDocumentFilePath(IContext ctx, FrontTemplateDocumentFile attFile)
        {
            var path = GetStorePath();
            path = Path.Combine(path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, ctx.Client.Id.ToString(), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString());
            return path;
        }

        private string GetFullDocumentFilePath(IContext ctx, InternalDocumentFile attFile)
        {
            var path = GetStorePath();
            path = Path.Combine(path, SettingConstants.FILE_STORE_DOCUMENT_FOLDER, ctx.Client.Id.ToString(), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString(), attFile.Version.ToString());
            return path;
        }

        private string GetFullDocumentFilePath(IContext ctx, InternalTemplateDocumentFile attFile)
        {
            var path = GetStorePath();
            path = Path.Combine(path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, ctx.Client.Id.ToString(), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString());
            return path;
        }

        private string FileToSha512(string sourceFileName)
        {
            using (var stream = File.OpenRead(sourceFileName))
            {
                var sha = new SHA512Managed();
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        private string GetFilePath(string path, string fileName, string fileExt, EnumDocumentFileType fileType)
        {
            string localFilePath;
            switch (fileType)
            {
                case EnumDocumentFileType.UserFile:
                    localFilePath = path + "\\" + fileName + "." + fileExt;
                    break;
                case EnumDocumentFileType.PdfFile:
                    localFilePath = path + "\\" + fileName + ".pdf";
                    break;
                case EnumDocumentFileType.PdfPreview:
                    localFilePath = path + "\\" + fileName + ".jpg";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }

            if (File.Exists(localFilePath) || fileType != EnumDocumentFileType.UserFile) return localFilePath;

            throw new FileNotExists();
        }

        public string SaveFile(IContext ctx, InternalTemplateDocumentFile attFile, bool isOverride = true)
        {
            try
            {
                var docFile = attFile as InternalDocumentFile;
                var path = docFile == null ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var localFilePath = path + "\\" + attFile.File.FileName;

                if (File.Exists(localFilePath) && isOverride)
                {
                    File.Delete(localFilePath);
                }

                File.WriteAllBytes(localFilePath, attFile.File.FileContent);

                var fileInfo = new FileInfo(localFilePath);

                attFile.File.FileSize = fileInfo.Length;

                attFile.Hash = FileToSha512(localFilePath);
                return attFile.Hash;
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot save user file", Environment.StackTrace);
                throw new CannotSaveFile(ex);
            }
        }

        public bool CreatePdfFile(IContext ctx, InternalTemplateDocumentFile attFile, bool isOverride = true)
        {
            try
            {
                var docFile = attFile as InternalDocumentFile;
                var path = docFile == null ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                if (!Directory.Exists(path))
                {
                    return false;
                }
                var localFilePath = path + "\\" + attFile.File.FileName;
                var pdfFileName = path + "\\" + attFile.File.Name + ".pdf";
                var previewFile = path + "\\" + attFile.File.Name + ".jpg";

                if (attFile.File.Extension.ToLower() == "pdf")
                {
                    if (File.Exists(previewFile) && isOverride)
                    {
                        File.Delete(previewFile);
                    }
                }
                else
                {
                    if (!PdfGenerator.IsAcceptedFileType(localFilePath)) return false;

                    if (File.Exists(pdfFileName) && isOverride)
                    {
                        File.Delete(pdfFileName);
                    }

                    if (!PdfGenerator.CreatePdf(localFilePath, pdfFileName))
                    {
                        return false;
                    }
                }

                if (File.Exists(previewFile) && isOverride)
                {
                    File.Delete(previewFile);
                }
                attFile.LastPdfAccess = DateTime.Now;
                attFile.PdfAcceptable = true;
                attFile.PdfCreated = true;
                PdfGenerator.CreatePdfPreview(pdfFileName, previewFile);
                return true;
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot save user file", Environment.StackTrace);
                throw new CannotSaveFile(ex);
            }
        }

        public bool RenameFile(IContext ctx, InternalTemplateDocumentFile attFile, string newName)
        {
            try
            {
                var docFile = attFile as InternalDocumentFile;
                var path = docFile == null ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var localFilePath = path + "\\" + attFile.File.FileName;
                var localFilePathNew = path + "\\" + newName + "." + attFile.File.Extension;

                if (File.Exists(localFilePath))
                {
                    try
                    {
                        File.Move(localFilePath, localFilePathNew);
                    }
                    catch
                    {
                        return false;
                    }
                }

                if (attFile.PdfCreated)
                {

                    var pdfFileName = path + "\\" + attFile.File.Name + ".pdf";
                    var pdfFilePathNew = path + "\\" + newName + ".pdf";

                    var previewFileName = path + "\\" + attFile.File.Name + ".jpg";
                    var previewFilePathNew = path + "\\" + newName + ".jpg";

                    if (File.Exists(pdfFileName))
                    {
                        try
                        {
                            File.Move(pdfFileName, pdfFilePathNew);
                        }
                        catch
                        {
                            File.Move(localFilePathNew, localFilePath);
                            return false;
                        }
                    }

                    if (File.Exists(previewFileName))
                    {
                        try
                        {
                            File.Move(previewFileName, previewFilePathNew);
                        }
                        catch
                        {
                            File.Move(localFilePathNew, localFilePath);
                            File.Move(previewFilePathNew, previewFileName);
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot save user file", Environment.StackTrace);
                throw new CannotSaveFile(ex);
            }
        }

        public bool IsFileCorrect(IContext ctx, InternalDocumentFile docFile)
        {
            try
            {
                var path = GetFullDocumentFilePath(ctx, docFile);

                var localFilePath = path + "\\" + docFile.File.FileName;

                if (!File.Exists(localFilePath))
                    return false;

                return docFile.Hash == FileToSha512(localFilePath);

            }
            catch (Exception ex)
            {
                //TODO check if file exists
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new DocumentFileWasChangedExternally(ex);
            }
        }

        public byte[] GetFile(IContext ctx, FrontTemplateDocumentFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile)
        {
            try
            {
                var path = GetFullDocumentFilePath(ctx, attFile);

                var localFilePath = GetFilePath(path, attFile.File.Name, attFile.File.Extension, fileType);


                if (fileType != EnumDocumentFileType.UserFile && !File.Exists(localFilePath)) // если просят ПДФ или превью а оно не создано
                {
                    var doc = new InternalTemplateDocumentFile(attFile);
                    if (!CreatePdfFile(ctx, doc))
                    {
                        throw new FilePdfNotExists();
                    }
                }

                attFile.File.FileContent = File.ReadAllBytes(localFilePath);

                return attFile.File.FileContent;
            }
            catch (FileNotExists)
            {
                throw new FileNotExists();
            }
            catch (Exception ex)
            {
                //TODO check if file exists
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        public byte[] GetFile(IContext ctx, FrontDocumentFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile)
        {
            try
            {
                var path = GetFullDocumentFilePath(ctx, attFile);

                var localFilePath = GetFilePath(path, attFile.File.Name, attFile.File.Extension, fileType);

                if (fileType != EnumDocumentFileType.UserFile && !File.Exists(localFilePath)) // если просят ПДФ или превью а оно не создано
                {
                    var doc = new InternalDocumentFile(attFile);
                    if (!CreatePdfFile(ctx, doc))
                    {
                        throw new FilePdfNotExists();
                    }
                }

                var fileContent = File.ReadAllBytes(localFilePath);

                attFile.File.FileContent = fileContent;

                if (fileType != EnumDocumentFileType.UserFile) return fileContent;

                attFile.WasChangedExternal = attFile.Hash != FileToSha512(localFilePath);

                if (attFile.WasChangedExternal)
                {
                    throw new DocumentFileWasChangedExternally();
                }

                return fileContent;
            }
            catch (DmsExceptions ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                //TODO check if file exists
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        public byte[] GetFile(IContext ctx, InternalTemplateDocumentFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile)
        {
            try
            {
                var docFile = attFile as InternalDocumentFile;
                var path = (docFile == null) ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                var localFilePath = GetFilePath(path, attFile.File.Name, attFile.File.Extension, fileType);

                var fileContent = File.ReadAllBytes(localFilePath);

                attFile.File.FileContent = fileContent;

                if (docFile == null || fileType != EnumDocumentFileType.UserFile) return fileContent;

                docFile.WasChangedExternal = docFile.Hash != FileToSha512(localFilePath);

                if (docFile.WasChangedExternal)
                {
                    throw new DocumentFileWasChangedExternally();
                }
                return fileContent;
            }
            catch (DmsExceptions ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                //TODO check if file exists
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        /// <summary>
        /// delete all file for the specific template
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="templateId"></param>
        public void DeleteAllFileInTemplate(IContext ctx, int templateId)
        {
            try
            {
                //TODO CurrentAgentId
                var path = GetStorePath();
                path = Path.Combine(new[] { path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, ctx.Client.Id.ToString(), templateId.ToString() });
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        /// <summary>
        /// delete all file for the specific document
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="documentId"></param>
        public void DeleteAllFileInDocument(IContext ctx, int documentId)
        {
            try
            {
                //TODO CurrentAgentId

                var path = GetStorePath();
                path = Path.Combine(new[] { path, SettingConstants.FILE_STORE_DOCUMENT_FOLDER, ctx.Client.Id.ToString(), documentId.ToString() });
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        /// <summary>
        /// Delete PDF version of the file and small preview picture for that file.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="attFile"></param>
        public void DeletePdfCopy(IContext ctx, InternalTemplateDocumentFile attFile)
        {
            try
            {
                var docFile = attFile as InternalDocumentFile;
                var path = docFile == null
                    ? GetFullDocumentFilePath(ctx, attFile)
                    : GetFullDocumentFilePath(ctx, docFile);

                var localPdfFilePath = path + "\\" + attFile.File.Name + ".pdf";
                var localPreviewFilePathNew = path + "\\" + attFile.File.Name + ".jpg";

                if (File.Exists(localPdfFilePath))
                {
                    File.Delete(localPdfFilePath);
                }

                if (File.Exists(localPreviewFilePathNew))
                {
                    File.Delete(localPreviewFilePathNew);
                }
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user PDF/Preview file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        /// <summary>
        /// delete file
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="attFile"></param>
        public void DeleteFile(IContext ctx, InternalTemplateDocumentFile attFile)
        {
            try
            {
                var path = GetStorePath();
                path = Path.Combine(new[] { path, (attFile is InternalDocumentFile ? SettingConstants.FILE_STORE_DOCUMENT_FOLDER : SettingConstants.FILE_STORE_TEMPLATE_FOLDER), ctx.Client.Id.ToString(), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString() });

                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        /// <summary>
        /// delete specific file version.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="attFile"></param>
        public void DeleteFileVersion(IContext ctx, InternalDocumentFile attFile)
        {
            try
            {
                var path = GetFullDocumentFilePath(ctx, attFile);
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        public void CopyFile(IContext ctx, InternalTemplateDocumentFile fromTempl, InternalTemplateDocumentFile toTempl)
        {
            try
            {
                var fromDoc = fromTempl as InternalDocumentFile;
                var toDoc = toTempl as InternalDocumentFile;
                var fromPath = fromDoc == null ? GetFullDocumentFilePath(ctx, fromTempl) : GetFullDocumentFilePath(ctx, fromDoc);
                var toPath = toDoc == null ? GetFullDocumentFilePath(ctx, toTempl) : GetFullDocumentFilePath(ctx, toDoc);

                var localFromPath = fromPath + "\\" + fromTempl.File.FileName;
                var localToPath = toPath + "\\" + toTempl.File.FileName;

                if (!File.Exists(localFromPath))
                {
                    throw new FileNotExists();
                }

                if (!Directory.Exists(toPath))
                {
                    Directory.CreateDirectory(toPath);
                }

                foreach (var fl in Directory.GetFiles(fromPath))
                {
                    if (Path.GetFileNameWithoutExtension(fl) == fromTempl.File.Name)
                    {
                        File.Copy(fl, Path.Combine(toPath, toTempl.File.Name + Path.GetExtension(fl)), true);
                    }
                }

                toTempl.Hash = FileToSha512(localToPath);
            }
            catch (FileNotExists)
            {
                throw new FileNotExists();
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to one of file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        public string GetFullTemplateReportFilePath(IContext ctx, EnumReportTypes reportType)
        {
            var path = GetStorePath();
            var templateReportFile = string.Empty;
            var setVal = DmsResolver.Current.Get<ISettingValues>();
            switch (reportType)
            {
                case EnumReportTypes.RegistrationCardIncomingDocument:
                    templateReportFile = setVal.GetReportRegistrationCardIncomingDocument(ctx);
                    break;
                case EnumReportTypes.RegistrationCardInternalDocument:
                    templateReportFile = setVal.GetReportRegistrationCardInternalDocument(ctx);
                    break;
                case EnumReportTypes.RegistrationCardOutcomingDocument:
                    templateReportFile = setVal.GetReportRegistrationCardOutcomingDocument(ctx);
                    break;
                case EnumReportTypes.RegisterTransmissionDocuments:
                    templateReportFile = setVal.GetReportRegisterTransmissionDocuments(ctx);
                    break;
                case EnumReportTypes.DocumentForDigitalSignature:
                    templateReportFile = setVal.GetReportDocumentForDigitalSignature(ctx);
                    break;
            }

            path = Path.Combine(new[] { path, SettingConstants.FILE_STORE_TEMPLATE_REPORTS_FOLDER, ctx.Client.Id.ToString(), templateReportFile });
            return path;
        }
    }
}