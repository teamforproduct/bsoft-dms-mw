using System.Management;
using System.Security.Cryptography;
using System.Text;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace DMS_WebAPI.Utilities
{
    public class SystemInfo
    {
        private string GetCPUKey()
        {
            ManagementClass myManagementClass = new
                        ManagementClass("Win32_Processor");
            ManagementObjectCollection myManagementCollection =
               myManagementClass.GetInstances();

            foreach (var obj in myManagementCollection)
            {
                return obj.Properties["ProcessorId"].Value.ToString();

            }
            return "";
        }

        private string GetDiscKey(string driveLetter)
        {
            try
            {
                using (var partitions = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + driveLetter +
                                                    "'} WHERE ResultClass=Win32_DiskPartition"))
                {
                    foreach (var partition in partitions.Get())
                    {
                        using (var drives = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" +
                                                                partition["DeviceID"] +
                                                                "'} WHERE ResultClass=Win32_DiskDrive"))
                        {
                            foreach (var drive in drives.Get())
                            {
                                return (string)drive["SerialNumber"];
                            }
                        }
                    }
                }
            }
            catch
            {
                return "";
            }

            // Not Found
            return "";
        }

        public string GetRegCode(LicenceInfo li)
        {
            var cpu = GetCPUKey();
            var disc = GetDiscKey("C:");
            var clInfo = $"Id:{li.ClientId}/Name:{li.Name}/Lic:{li.LicType}/DT:{li.FirstStart}/NOC:{li.NumberOfConnections}/DD:{li.DateLimit}";


            if (string.IsNullOrEmpty(cpu) || string.IsNullOrEmpty(disc) || clInfo.Length <= 27)
            {
                throw new LicenceError();
            }
            var strinfo = $"{clInfo}+{cpu}+{disc}";
            string res,hash;
            using (var md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF32.GetBytes(strinfo));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                hash = sBuilder.ToString().ToUpper().Replace("0","").Replace("O", "").Replace("I", "").Replace("1", "");
            }
            int part = hash.Length/8;
            res = hash[0].ToString() + hash[hash.Length - 1] + hash[part] + hash[hash.Length - part] + hash[2*part] + hash[hash.Length - 2*part] + hash[3*part] + hash[hash.Length - 3*part];
            return res;
        }
    }
}