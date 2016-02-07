using System;
using System.IO;
using System.Security.Cryptography;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentAdditional;
using BL.Model.Exception;

namespace BL.Logic.SystemLogic
{
    public class FileStore : IFileStore
    {
        private const string _FILE_STORE_PATH = "IRF_DMS_FILESTORE_PATH";
        private const string _FILE_STORE_DEFAULT_PATH = @"c:\IRF_DMS_FILESTORE";

        private string GetStorePath(IContext ctx)
        {
            var sett = DmsResolver.Current.Get<ISettings>();
            try
            {
                return sett.GetSetting<string>(ctx, _FILE_STORE_PATH);
            }
            catch
            {
                sett.SaveSetting(ctx, _FILE_STORE_PATH, _FILE_STORE_DEFAULT_PATH);
                return sett.GetSetting<string>(ctx, _FILE_STORE_PATH);
            }
        }

        private string GetFullFilePath(IContext ctx, DocumentAttachedFile attFile)
        {
            var path = GetStorePath(ctx);
            path = Path.Combine(new string[] { path, ctx.CurrentAgentId.ToString(), attFile.DocumentId.ToString(), attFile.OrderInDocument.ToString(), attFile.Version.ToString() });
            return path;
        }

        public string SaveFile(IContext ctx, DocumentAttachedFile attFile, bool isOverride = true)
        {
            try
            {
                var path = GetFullFilePath(ctx, attFile);
                var dir = Directory.CreateDirectory(path);
                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                if (File.Exists(localFilePath) && isOverride)
                {
                    File.Delete(localFilePath);
                }

                File.WriteAllBytes(localFilePath, attFile.FileData);
                attFile.Hash = FileToSha1(localFilePath);
                return attFile.Hash;
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot save user file", Environment.StackTrace);
                throw new CannotSaveFile(ex);
            }

            //var date = DateTime.Now;
            //var hash = FileToSha1(localFilePath);
            //var minutes = date.Minute > 30 ? "30" : "00";
            //var time = date.Hour.ToString("00") + "-" + minutes;
            //webPath = String.Format(@"{0}/{1}/{2}/{3}/{4}", date.Year, date.Month, date.Day, time, hash);
        }

        public byte[] GetFile(IContext ctx, DocumentAttachedFile attFile)
        {
            try
            {
                var path = GetFullFilePath(ctx, attFile);
                var localFilePath = path + "\\" + attFile.Name + "." + attFile.Extension;

                attFile.FileData = File.ReadAllBytes(localFilePath);
                attFile.WasChangedExternal = attFile.Hash != FileToSha1(localFilePath);

                return attFile.FileData;
            }
            catch (Exception ex)
            {
                //TODO check if file exists
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

        public void DeleteFile(IContext ctx, DocumentAttachedFile attFile)
        {
            try
            {
                var path = GetFullFilePath(ctx, attFile);
                Directory.Delete(path);
            }
            catch (Exception ex)
            {
                var log = DmsResolver.Current.Get<ILogger>();
                log.Error(ctx, ex, "Cannot access to user file", Environment.StackTrace);
                throw new CannotAccessToFile(ex);
            }
        }

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