using System.Management;
using System.Security.Cryptography;
using System.Text;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.CryptographicWorker;

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

        public string GetProgramRegCode(LicenceInfo li)
        {
            var cpu = GetCPUKey();
            var disc = GetDiscKey("C:");

            if (string.IsNullOrEmpty(cpu) || string.IsNullOrEmpty(disc))
            {
                throw new LicenceError();
            }
            var strinfo = $"{li.FirstStart.ToString("yyyy-MM-ddTHH:mm")}+{cpu}+{disc}";
            string res, hash;
            hash = DmsResolver.Current.Get<ICryptoService>().GetHash(strinfo);
            hash = hash.ToUpper().Replace("0", "").Replace("O", "").Replace("I", "").Replace("1", "");
            int part = hash.Length / 8;
            res = hash[0].ToString() + hash[hash.Length - 1] + hash[part] + hash[hash.Length - part] + hash[2 * part] + hash[hash.Length - 2 * part] + hash[3 * part] + hash[hash.Length - 3 * part];
            return res;
        }
        public string GetLicenceRegCode(LicenceInfo li)
        {
            var clInfo = $"Name:{li.Name}/DD:{li.DateLimit}/NNOC:{li.NamedNumberOfConnections}/CNOC:{li.ConcurenteNumberOfConnections}/Fun:{li.Functionals}";

            string res, hash;

            hash = DmsResolver.Current.Get<ICryptoService>().GetHash(clInfo);

            hash = hash.ToUpper().Replace("0", "").Replace("O", "").Replace("I", "").Replace("1", "");

            int part = hash.Length / 8;

            res = hash[0].ToString() + hash[hash.Length - 1] + hash[part] + hash[hash.Length - part] + hash[2 * part] + hash[hash.Length - 2 * part] + hash[3 * part] + hash[hash.Length - 3 * part];

            return res;
        }

        public string GetRegCode(LicenceInfo li)
        {
            var progRegCode = GetProgramRegCode(li);
            //TODO удалить когда будем использовать лицензи
            progRegCode = "D85BF3EE";
            var licRegCode = GetLicenceRegCode(li);
            return progRegCode + licRegCode;
        }
    }
}