using System;
using System.IO;
using System.Security.Cryptography;

namespace BL.Logic.System
{
    public class FileStore
    {
        private readonly string _webAddress;
        private readonly string _credName;
        private readonly string _credPassword;

        //public FileStoreApi(IConfig config, IContextProvider contextProvider)
        //{
        //    IConfig config1 = config;
        //    IContextProvider contextProvider1 = contextProvider;
        //    _webAddress = config1.GetSetting<string>(contextProvider1.Current.Database, "FileStoreUrl");
        //    _credName = config1.GetSetting<string>(contextProvider1.Current.Database, "FileStoreUserName");
        //    _credPassword = config1.GetSetting<string>(contextProvider1.Current.Database, "FileStorePassword");
        //}

        //public void SaveFile(string localFilePath, out string webPath)
        //{
        //    var date = DateTime.Now;
        //    var hash = FileToSha1(localFilePath);
        //    var minutes = date.Minute > 30 ? "30" : "00";
        //    var time = date.Hour.ToString("00") + "-" + minutes;
        //    webPath = String.Format(@"{0}/{1}/{2}/{3}/{4}", date.Year, date.Month, date.Day, time, hash);
        //    string destinationFilePath = _webAddress + webPath;
        //    var client = new WebDAVClient(new WebDAVClientCredential(_credName, _credPassword));
        //    client.Put(destinationFilePath, File.ReadAllBytes(localFilePath));
        //}

        public string FileToSha1(string sourceFileName)
        {
            using (var stream = File.OpenRead(sourceFileName))
            {
                var sha = new SHA256Managed();
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

        //public byte[] DownloadFile(string downloadUrl)
        //{
        //    var client = new WebDAVClient(new WebDAVClientCredential(_credName, _credPassword));
        //    return client.Get(_webAddress + downloadUrl);
        //}
    }
}