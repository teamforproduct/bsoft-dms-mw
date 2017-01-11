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
        private string GetStorePath(IContext ctx)
        {
            var sett = DmsResolver.Current.Get<ISettings>();
            return sett.GetFileStorePath(ctx);
        }

        private string GetFullDocumentFilePath(IContext ctx, FrontDocumentAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(path, SettingConstants.FILE_STORE_DOCUMENT_FOLDER, attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString(), attFile.Version.ToString());
            return path;
        }

        private string GetFullDocumentFilePath(IContext ctx, FrontTemplateAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(new[] { path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString() });
            return path;
        }

        private string GetFullDocumentFilePath(IContext ctx, InternalDocumentAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(path, SettingConstants.FILE_STORE_DOCUMENT_FOLDER, attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString(), attFile.Version.ToString());
            return path;
        }

        private string GetFullDocumentFilePath(IContext ctx, InternalTemplateAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(new[] { path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString() });
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

        public string SaveFile(IContext ctx, InternalTemplateAttachedFile attFile, bool isOverride = true)
        {
            try
            {
                var docFile = attFile as InternalDocumentAttachedFile;
                var path = docFile == null ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;
                var pdfFileName = path + "\\" + attFile.Name + ".pdf";
                var previewFile = path + "\\" + attFile.Name + ".jpg";

                if (File.Exists(localFilePath) && isOverride)
                {
                    File.Delete(localFilePath);
                }

                if (attFile.PostedFileData != null)
                    attFile.PostedFileData.SaveAs(localFilePath);
                else
                    File.WriteAllBytes(localFilePath, attFile.FileData);

                FileInfo fileInfo = new FileInfo(localFilePath);

                attFile.FileSize = fileInfo.Length;

                try
                {
                    if (attFile.Extension.ToLower() != "pdf")
                    {
                        if (PdfGenerator.CreatePdf(localFilePath, pdfFileName))
                        {
                        }
                    }
                    PdfGenerator.CreatePdfPreview(pdfFileName, previewFile);
                }
                catch
                {
                }

                //File.WriteAllBytes(localFilePath, attFile.FileContent);
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



        public bool RenameFile(IContext ctx, InternalTemplateAttachedFile attFile, string newName)
        {
            try
            {
                var docFile = attFile as InternalDocumentAttachedFile;
                var path = docFile == null ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;
                var localFilePathNew = path + "\\" + newName + "." + attFile.Extension;

                var pdfFileName = path + "\\" + attFile.Name + ".pdf";
                var pdfFilePathNew = path + "\\" + newName + ".pdf";

                var previewFileName = path + "\\" + attFile.Name + ".jpg";
                var previewFilePathNew = path + "\\" + newName + ".jpg";

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

                return true;
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot save user file", Environment.StackTrace);
                throw new CannotSaveFile(ex);
            }
        }

        public bool IsFileCorrect(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            try
            {
                var path = GetFullDocumentFilePath(ctx, docFile);

                var localFilePath = path + "\\" + docFile.Name + "." + docFile.Extension;

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

        public byte[] GetFile(IContext ctx, FrontTemplateAttachedFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile)
        {
            try
            {
                var path = GetFullDocumentFilePath(ctx, attFile);

                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                if (!File.Exists(localFilePath))
                {
                    throw new UserFileNotExists();
                }

                var fileContent = File.ReadAllBytes(localFilePath);

                attFile.FileContent = Convert.ToBase64String(fileContent);

                return fileContent;
            }
            catch (UserFileNotExists)
            {
                throw new UserFileNotExists();
            }
            catch (Exception ex)
            {
                //TODO check if file exists
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        public byte[] GetFile(IContext ctx, FrontDocumentAttachedFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile)
        {
            try
            {
                var path = GetFullDocumentFilePath(ctx, attFile);

                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                if (!File.Exists(localFilePath))
                {
                    throw new UserFileNotExists();
                }

                var fileContent = File.ReadAllBytes(localFilePath);

                attFile.FileContent = Convert.ToBase64String(fileContent);

                attFile.WasChangedExternal = attFile.Hash != FileToSha512(localFilePath);

                if (attFile.WasChangedExternal)
                {
                    throw new DocumentFileWasChangedExternally();
                }

                return fileContent;
            }
            catch (UserFileNotExists)
            {
                throw new UserFileNotExists();
            }
            catch (DocumentFileWasChangedExternally)
            {
                throw new DocumentFileWasChangedExternally();
            }
            catch (Exception ex)
            {
                //TODO check if file exists
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        public byte[] GetFile(IContext ctx, InternalTemplateAttachedFile attFile, EnumDocumentFileType fileType = EnumDocumentFileType.UserFile)
        {
            try
            {
                var docFile = attFile as InternalDocumentAttachedFile;
                var path = (docFile == null) ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                if (!File.Exists(localFilePath))
                {
                    throw new UserFileNotExists();
                }

                var fileContent = File.ReadAllBytes(localFilePath);

                attFile.FileContent = Convert.ToBase64String(fileContent);

                if (docFile == null) return fileContent;

                docFile.WasChangedExternal = docFile.Hash != FileToSha512(localFilePath);

                if (docFile.WasChangedExternal)
                {
                    throw new DocumentFileWasChangedExternally();
                }
                return fileContent;
            }
            catch (UserFileNotExists)
            {
                throw new UserFileNotExists();
            }
            catch (DocumentFileWasChangedExternally)
            {
                throw new DocumentFileWasChangedExternally();
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
                var path = GetStorePath(ctx);
                path = Path.Combine(new[] { path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, templateId.ToString() });
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

                var path = GetStorePath(ctx);
                path = Path.Combine(new[] { path, SettingConstants.FILE_STORE_DOCUMENT_FOLDER, documentId.ToString() });
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
        /// delete file
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="attFile"></param>
        public void DeleteFile(IContext ctx, InternalTemplateAttachedFile attFile)
        {
            try
            {
                var path = GetStorePath(ctx);
                path = Path.Combine(new[] { path, ((attFile is InternalDocumentAttachedFile) ? SettingConstants.FILE_STORE_DOCUMENT_FOLDER : SettingConstants.FILE_STORE_TEMPLATE_FOLDER), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString() });

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
        public void DeleteFileVersion(IContext ctx, InternalDocumentAttachedFile attFile)
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

        public void CopyFile(IContext ctx, InternalTemplateAttachedFile fromTempl, InternalTemplateAttachedFile toTempl)
        {
            try
            {
                var fromDoc = fromTempl as InternalDocumentAttachedFile;
                var toDoc = toTempl as InternalDocumentAttachedFile;
                var fromPath = (fromDoc == null)
                    ? GetFullDocumentFilePath(ctx, fromTempl)
                    : GetFullDocumentFilePath(ctx, fromDoc);
                var toPath = (toDoc == null)
                    ? GetFullDocumentFilePath(ctx, toTempl)
                    : GetFullDocumentFilePath(ctx, toDoc);

                var localFromPath = fromPath + "\\" + fromTempl.Name + "." + fromTempl.Extension;
                var localToPath = toPath + "\\" + toTempl.Name + "." + toTempl.Extension;

                if (!File.Exists(localFromPath))
                {
                    throw new UserFileNotExists();
                }

                if (!Directory.Exists(toPath))
                {
                    Directory.CreateDirectory(toPath);
                }

                File.Copy(localFromPath, localToPath, true);
                toTempl.Hash = FileToSha512(localToPath);
            }
            catch (UserFileNotExists)
            {
                throw new UserFileNotExists();
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
            var path = GetStorePath(ctx);
            var templateReportFile = string.Empty;
            var sett = DmsResolver.Current.Get<ISettings>();
            switch (reportType)
            {
                case EnumReportTypes.RegistrationCardIncomingDocument:
                    templateReportFile = sett.GetReportRegistrationCardIncomingDocument(ctx);
                    break;
                case EnumReportTypes.RegistrationCardInternalDocument:
                    templateReportFile = sett.GetReportRegistrationCardInternalDocument(ctx);
                    break;
                case EnumReportTypes.RegistrationCardOutcomingDocument:
                    templateReportFile = sett.GetReportRegistrationCardOutcomingDocument(ctx);
                    break;
                case EnumReportTypes.RegisterTransmissionDocuments:
                    templateReportFile = sett.GetReportRegisterTransmissionDocuments(ctx);
                    break;
                case EnumReportTypes.DocumentForDigitalSignature:
                    templateReportFile = sett.GetReportDocumentForDigitalSignature(ctx);
                    break;
            }

            path = Path.Combine(new[] { path, SettingConstants.FILE_STORE_TEMPLATE_REPORTS_FOLDER, templateReportFile });
            return path;
        }

    }
}