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

namespace BL.Database.FileWorker
{
    public class FileStore : IFileStore
    {
        private string GetStorePath(IContext ctx)
        {
            var sett = DmsResolver.Current.Get<ISettings>();
            try
            {
                return sett.GetSetting<string>(ctx, SettingConstants.FILE_STORE_PATH);
            }
            catch
            {
                sett.SaveSetting(ctx, SettingConstants.FILE_STORE_PATH, SettingConstants.FILE_STORE_DEFAULT_PATH);
                return sett.GetSetting<string>(ctx, SettingConstants.FILE_STORE_PATH);
            }
        }

        private string GetFullDocumentFilePath(IContext ctx, FrontDocumentAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_DOCUMENT_FOLDER, attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString(), attFile.Version.ToString() });
            return path;
        }

        private string GetFullDocumentFilePath(IContext ctx, FrontTemplateAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString() });
            return path;
        }


        private string GetFullDocumentFilePath(IContext ctx, InternalDocumentAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_DOCUMENT_FOLDER, attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString(), attFile.Version.ToString() });
            return path;
        }

        private string GetFullDocumentFilePath(IContext ctx, InternalTemplateAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString() });
            return path;
        }

        public string SaveFile(IContext ctx, InternalTemplateAttachedFile attFile, bool isOverride = true)
        {
            try
            {
                var docFile = attFile as InternalDocumentAttachedFile;
                var path = (docFile == null) ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                if (!Directory.Exists(path))
                {
                    var dir = Directory.CreateDirectory(path);
                }
                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                if (File.Exists(localFilePath) && isOverride)
                {
                    File.Delete(localFilePath);
                }

                attFile.PostedFileData.SaveAs(localFilePath);

                FileInfo fileInfo = new FileInfo(localFilePath);

                attFile.FileSize = fileInfo.Length;

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

        public bool IsFileCorrect(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            try
            {
                string path = GetFullDocumentFilePath(ctx, docFile);

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

        public byte[] GetFile(IContext ctx, FrontTemplateAttachedFile attFile)
        {
            try
            {
                string path = GetFullDocumentFilePath(ctx, attFile);

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

        public byte[] GetFile(IContext ctx, FrontDocumentAttachedFile attFile)
        {
            try
            {
                string path = GetFullDocumentFilePath(ctx, attFile);

                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                if (!File.Exists(localFilePath))
                {
                    throw new UserFileNotExists();
                }

                var fileContent = File.ReadAllBytes(localFilePath);

                attFile.FileContent = Convert.ToBase64String(fileContent);

                attFile.WasChangedExternal = attFile.Hash != FileToSha512(localFilePath);

                if (!attFile.WasChangedExternal)
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

        public byte[] GetFile(IContext ctx, InternalTemplateAttachedFile attFile)
        {
            try
            {
                var docFile = attFile as InternalDocumentAttachedFile;
                string path = (docFile == null) ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                if (!File.Exists(localFilePath))
                {
                    throw new UserFileNotExists();
                }

                var fileContent = File.ReadAllBytes(localFilePath);

                attFile.FileContent = Convert.ToBase64String(fileContent);

                if (docFile != null)
                {
                    docFile.WasChangedExternal = docFile.Hash != FileToSha512(localFilePath);
                    if (!docFile.WasChangedExternal)
                    {
                        throw new DocumentFileWasChangedExternally();
                    }
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
        /// <param name="attFile"></param>
        public void DeleteAllFileInTemplate(IContext ctx, int templateId)
        {
            try
            {
                //TODO CurrentAgentId
                var path = GetStorePath(ctx);
                path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, templateId.ToString() });

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
        /// <param name="attFile"></param>
        public void DeleteAllFileInDocument(IContext ctx, int documentId)
        {
            try
            {
                //TODO CurrentAgentId

                var path = GetStorePath(ctx);
                path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_DOCUMENT_FOLDER, documentId.ToString() });

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
                path = Path.Combine(new string[] { path, ((attFile is InternalDocumentAttachedFile) ? SettingConstants.FILE_STORE_DOCUMENT_FOLDER : SettingConstants.FILE_STORE_TEMPLATE_FOLDER), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString() });

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

                if (!File.Exists(fromPath))
                {
                    throw new UserFileNotExists();
                }

                if (!Directory.Exists(toPath))
                {
                    var dir = Directory.CreateDirectory(toPath);
                }
                var localFromPath = fromPath + "\\" + fromTempl.Name + "." + fromTempl.Extension;
                var localToPath = toPath + "\\" + toTempl.Name + "." + toTempl.Extension;

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

        private string FileToSha512(string sourceFileName)
        {
            using (var stream = File.OpenRead(sourceFileName))
            {
                var sha = new SHA512Managed();
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        public string GetFullTemplateReportFilePath(IContext ctx, EnumReportTypes reportType)
        {
            var path = GetStorePath(ctx);

            var templateReportFile = string.Empty;
            var sett = DmsResolver.Current.Get<ISettings>();
            try
            {
                templateReportFile = sett.GetSetting<string>(ctx, SettingConstants.FILE_STORE_TEMPLATE_REPORT_FILE + reportType);
            }
            catch
            {
                sett.SaveSetting(ctx, SettingConstants.FILE_STORE_TEMPLATE_REPORT_FILE + reportType, SettingConstants.FILE_STORE_DEFAULT_TEMPLATE_REPORT_FILE);
                templateReportFile = sett.GetSetting<string>(ctx, SettingConstants.FILE_STORE_TEMPLATE_REPORT_FILE + reportType);
            }

            path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_TEMPLATE_REPORTS_FOLDER, templateReportFile });
            return path;
        }

    }
}