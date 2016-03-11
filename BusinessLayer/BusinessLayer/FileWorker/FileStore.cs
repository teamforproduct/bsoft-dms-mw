using System;
using System.IO;
using System.Security.Cryptography;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.Constants;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.FileWorker
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

        private string GetFullDocumentFilePath(IContext ctx, InternalDocumentAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_DOCUEMENT_FOLDER, ctx.CurrentAgentId.ToString(), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString(), attFile.Version.ToString() });
            return path;
        }

        private string GetFullDocumentFilePath(IContext ctx, InternalTemplateAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, ctx.CurrentAgentId.ToString(), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString() });
            return path;
        }

        public string SaveFile(IContext ctx, InternalTemplateAttachedFile attFile, bool isOverride = true)
        {
            try
            {
                var docFile = attFile as InternalDocumentAttachedFile;
                var path = (docFile == null)?GetFullDocumentFilePath(ctx, attFile): GetFullDocumentFilePath(ctx, docFile);

                if (!Directory.Exists(path))
                {
                    var dir = Directory.CreateDirectory(path);
                }
                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                if (File.Exists(localFilePath) && isOverride)
                {
                    File.Delete(localFilePath);
                }

                File.WriteAllBytes(localFilePath, attFile.FileContent);
                attFile.Hash = FileToSha1(localFilePath);
                return attFile.Hash;
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot save user file", Environment.StackTrace);
                throw new CannotSaveFile(ex);
            }
        }

        public byte[] GetFile(IContext ctx, InternalTemplateAttachedFile attFile)
        {
            try
            {
                var docFile = attFile as InternalDocumentAttachedFile;
                string path = (docFile == null) ? GetFullDocumentFilePath(ctx, attFile) : GetFullDocumentFilePath(ctx, docFile);

                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                attFile.FileContent = File.ReadAllBytes(localFilePath);
                if (docFile != null)
                {
                    docFile.WasChangedExternal = docFile.Hash != FileToSha1(localFilePath);
                }
                return attFile.FileContent;
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
                var path = GetStorePath(ctx);
                path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_TEMPLATE_FOLDER, ctx.CurrentAgentId.ToString(), templateId.ToString() });

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
                var path = GetStorePath(ctx);
                path = Path.Combine(new string[] { path, SettingConstants.FILE_STORE_DOCUEMENT_FOLDER, ctx.CurrentAgentId.ToString(), documentId.ToString() });

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
                path = Path.Combine(new string[] { path, ((attFile is InternalDocumentAttachedFile)? SettingConstants.FILE_STORE_DOCUEMENT_FOLDER : SettingConstants.FILE_STORE_TEMPLATE_FOLDER), ctx.CurrentAgentId.ToString(), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString() });

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

        public void CopyFile(IContext ctx, InternalTemplateAttachedFile fromTempl,InternalTemplateAttachedFile toTempl)
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

                if (!Directory.Exists(toPath))
                {
                    var dir = Directory.CreateDirectory(toPath);
                }
                var localFromPath = fromPath + "\\" + fromTempl.Name + "." + fromTempl.Extension;
                var localToPath = toPath + "\\" + toTempl.Name + "." + toTempl.Extension;

                File.Copy(localFromPath, localToPath, true);
                toTempl.Hash = FileToSha1(localToPath);
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to one of file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        ///// <summary>
        ///// Copy all files (with versions possible) from one document or template to another
        ///// </summary>
        ///// <param name="ctx"></param>
        ///// <param name="fromTempl"></param>
        ///// <param name="newDocNumber"></param>
        ///// <param name="newIsTemplate"></param>
        //public IEnumerable<InternalTemplateAttachedFile> CopyFiles(IContext ctx, IEnumerable<InternalTemplateAttachedFile> fromTempl, int newDocNumber, bool newIsTemplate = false)
        //{
        //    try
        //    {
        //        var res = new List<InternalTemplateAttachedFile>();
        //        int newOrdNum = 1;
        //        foreach (var fl in fromTempl)
        //        {
        //            var fromDoc = fl as InternalDocumentAttachedFile;

        //            // file from document could not be copied to template
        //            if (fromDoc != null && newIsTemplate)
        //            {
        //                throw new AccessIsDenied();
        //            }

        //            if (newIsTemplate)
        //            {
        //                var newTempl = new InternalTemplateAttachedFile
        //                {
        //                    DocumentId = newDocNumber,
        //                    Extension = fl.Extension,
        //                    Name = fl.Name,
        //                    FileType = fl.FileType,
        //                    FileSize = fl.FileSize,
        //                    IsAdditional = fl.IsAdditional,
        //                    OrderInDocument = newOrdNum
        //                };
        //                CommonDocumentUtilities.SetLastChange(ctx, newTempl);
        //                CopyFile(ctx, fl, newTempl);
        //                res.Add(newTempl);
        //            }
        //            else
        //            {
        //                var newDoc = new InternalDocumentAttachedFile
        //                {
        //                    DocumentId = newDocNumber,
        //                    Extension = fl.Extension,
        //                    Name = fl.Name,
        //                    FileType = fl.FileType,
        //                    FileSize = fl.FileSize,
        //                    IsAdditional = fl.IsAdditional,
        //                    OrderInDocument = newOrdNum,
        //                    Date = DateTime.Now,
        //                    Version = 1,
        //                    WasChangedExternal = false
        //                };
        //                CommonDocumentUtilities.SetLastChange(ctx, newDoc);
        //                CopyFile(ctx, fromDoc, newDoc);
        //                res.Add(newDoc);
        //            }

        //        }
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        var log = DmsResolver.Current.Get<ILogger>();
        //        log.Error(ctx, ex, "Cannot access to one of file", Environment.StackTrace);
        //        throw new CannotAccessToFile(ex);
        //    }
        //}

        private string FileToSha1(string sourceFileName)
        {
            using (var stream = File.OpenRead(sourceFileName))
            {
                var sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

    }
}