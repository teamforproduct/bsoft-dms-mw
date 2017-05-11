using BL.Model.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DMS_WebAPI.Utilities
{
    public static class CommonUtilities
    {
        public static BaseFile Convert(HttpPostedFile file)
        {
            BaseFile res = new BaseFile();
            byte[] buffer = new byte[file.ContentLength];
            file.InputStream.Read(buffer, 0, file.ContentLength);
            res.FileContent = buffer;// System.Convert.ToBase64String(buffer);
            res.Name = Path.GetFileNameWithoutExtension(file.FileName);
            res.Extension = Path.GetExtension(file.FileName).Replace(".", "");
            res.FileType = file.ContentType;
            return res;
        }
    }
}