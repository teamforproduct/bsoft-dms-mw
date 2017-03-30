using System.Management;
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
        private string GetNetworkAdapterKey()
        {
            try
            {
                var macs = string.Empty;

                using (var partitions = new ManagementObjectSearcher("Select MACAddress,PNPDeviceID FROM Win32_NetworkAdapter WHERE MACAddress IS NOT NULL AND PNPDeviceID IS NOT NULL"))
                {
                    foreach (var partition in partitions.Get())
                    {
                        string pnp = partition["PNPDeviceID"].ToString();
                        if (pnp.Contains("PCI\\"))
                        {
                            string mac = partition["MACAddress"].ToString();
                            mac = mac.Replace(":", string.Empty);
                            macs += mac;
                        }
                    }
                }
                return macs;
            }
            catch
            {
                return "";
            }
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
            var netw = GetNetworkAdapterKey();

            var strinfo = $"{cpu}+{disc}+{netw}";
            if (string.IsNullOrEmpty(strinfo) || string.IsNullOrEmpty(cpu) || string.IsNullOrEmpty(netw))
            {
                throw new LicenceError();
            }
            strinfo = $"{li.FirstStart.ToString("yyyy-MM-ddTHH:mm")}+{strinfo}";
            string res, hash;
            hash = DmsResolver.Current.Get<ICryptoService>().GetHash(strinfo);
            hash = hash.ToUpper().Replace("0", "").Replace("O", "").Replace("I", "").Replace("1", "");
            int part = hash.Length / 8;
            res = hash[0].ToString() + hash[hash.Length - 1] + hash[part] + hash[hash.Length - part] + hash[2 * part] + hash[hash.Length - 2 * part] + hash[3 * part] + hash[hash.Length - 3 * part];
            return res;
        }

        public string GetLicenceRegCode(LicenceInfo li)
        {
            var clInfo = $"Name:{li.ClientName}/DD:{li.DateLimit}/NNOC:{li.NamedNumberOfConnections}/CNOC:{li.ConcurenteNumberOfConnections}/Fun:{li.Functionals}";

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
            //progRegCode = "D85BF3EE";       //TEST
            //progRegCode = "E99DC7C3";       //PROD       
            var licRegCode = GetLicenceRegCode(li);
            return $"{progRegCode}-{licRegCode}";
        }
    }
}