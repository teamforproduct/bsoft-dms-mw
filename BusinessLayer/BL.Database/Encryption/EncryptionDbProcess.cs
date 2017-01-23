using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Model.SystemCore;
using System.Data.Entity;
using BL.Database.Encryption.Interfaces;
using BL.Model.EncryptionCore.Filters;
using BL.Model.EncryptionCore.FrontModel;
using BL.Model.EncryptionCore.InternalModel;
using System.IO;
using BL.Database.DBModel.Encryption;
using BL.Model.Exception;
using BL.CrossCutting.Transliteration;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;
using System.Web;
using System.Transactions;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.DependencyInjection;

namespace BL.Database.Encryption
{
    internal class EncryptionDbProcess : CoreDb.CoreDb, IEncryptionDbProcess
    {
        private string _ZipCerPassword { get; } = "Qk1ncZA|HzaNiQ?8*3QjCMHQqIXQMce6m0JDqup~Nh13#H6{~BG9v2@xIGFKa%Hq5T$kEDo{mYGtXWryVCoR2PtA}QSvG*zPe6%9Q{T?4T*NOM{R}LgLkThZRF$%tb#wEw@pU7${oJzbdSq{3LsM?SOH0GMnub@#N|PH$Z}8gh%c}Wpb8hf$AhMM0@4%cusABMiP}{wIl*QEtKzqS5TZnRHOt~k3uEBc9d6%vuX6UKD0@eithk%ACMkrtrfIpY";
        private string _ZipCerFileName { get; } = "certificate";
        private string _ZipCerPasswordFileName { get; } = "password.txt";

        private const string _RSAKeyXml = "<RSAKeyValue><Modulus>sBRZy9xvw7FWdb5EHd79H8f2D4+JP3yokrbKpCgFbcwCEPPZpGUj07poBM9MvrIXEIHoahIYVw3UqWCLvFFL6Cb+u3zrOTaNmCNyXdZ4H/28sskfuBtVzXjllzwEkrcJg0NfSmCbjw/9YFUYEdl1ZTUL40pN8Kuk1Wr1f/wP+wk=</Modulus><Exponent>AQAB</Exponent><P>twV17e14On7eLeKl46JRJnnXrvZp4tHj68iNFk/S8tK/uKj3b9xeTTqxI6S31xQ3mN26X54egttXNjQ7V9OUaQ==</P><Q>9kpHMG4hxQ3/q1FyPlLgNV1XPDyeGoNF1QQDZ7Te8xfWvPW1ildAYsCEJ91tZMstgJR7oojYPy7VTNDn8bndoQ==</Q><DP>SLy017Bu/eB58IaJI2TZF4+I9pIcFvcPvB9iYyGqVrMHWx5b6GsOV2ciC2ZlYec5CVnlviabPapqiLJNe2QtMQ==</DP><DQ>R++uF2Ezj+Dk2l8xpS6DulKHFlsGOuw4y10euX3E2PAPkqWZ3sxZS/67GwG74ALQSYwVCIY700iUmJk0BhCpwQ==</DQ><InverseQ>oN5Vlg5M68jAeeZRiduiyMBw/T+oZQ5zaxMlvqIhgF603xRjHTzcNaHZB9Kvn0YBnfcRx6F1PfSkJJl4rWAaDw==</InverseQ><D>AIZ3BBwquy82vlAsfNhS8frTOZWoh6d0C0f/T8EMzxiKMwm/LvXcRv/p2oXRyUnXtsVkb5iROQVCCqVOlWe6rbvUMU8P554u4t9g0y1oLJUQDbYmRBo6z0I31OTRNBb7nCI6/l01Vyq6Ju225EdOEL8EVNd/wkQXoYRbm7Mun4E=</D></RSAKeyValue>";
        private ILogger _logger;

        public EncryptionDbProcess()
        {
            _logger = DmsResolver.Current.Get<ILogger>();
        }

        private string GetTempPath()
        {
            var path = Path.GetTempPath();

            return path;
        }

        #region Certificates
        public IEnumerable<FrontEncryptionCertificate> GetCertificates(IContext ctx, FilterEncryptionCertificate filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetCertificatesQuery(dbContext, ctx, filter);

                //TODO Sort
                {
                    qry = qry.OrderByDescending(x => x.CreateDate);
                }


                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontEncryptionCertificate>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }

                var qryFE = qry.Select(x => new FrontEncryptionCertificate
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreateDate = x.CreateDate,
                    NotBefore = x.NotBefore,
                    NotAfter = x.NotAfter,
                    AgentId = x.AgentId,
                    AgentName = x.Agent.Name,
                    IsRememberPassword = x.IsRememberPassword,

                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                });

                var res = qryFE.ToList();
                transaction.Complete();
                return res;
            }
        }

        private InternalEncryptionCertificate GetCertificate(IContext ctx, int certificateId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetCertificatesQuery(dbContext, ctx, new FilterEncryptionCertificate { CertificateId = new List<int> { certificateId } });

                var item = qry.Select(x => new InternalEncryptionCertificate
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreateDate = x.CreateDate,
                    NotBefore = x.NotBefore,
                    NotAfter = x.NotAfter,
                    AgentId = x.AgentId,
                    CertificateZip = x.Certificate,
                    Thumbprint = x.Thumbprint,
                    IsRememberPassword = x.IsRememberPassword,

                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                }).FirstOrDefault();

                if (item == null)
                    throw new EncryptionCertificateWasNotFound();
                transaction.Complete();
                return item;
            }
        }

        public void AddCertificate(IContext ctx, InternalEncryptionCertificate item)
        {
            ReadDetailsAboutCertificate(item);

            ZipCertificate(item);

            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var itemDb = ModelConverter.GetDbEncryptionCertificate(item);

                dbContext.EncryptionCertificatesSet.Add(itemDb);
                dbContext.SaveChanges();
                transaction.Complete();
                item.Id = itemDb.Id;
            }
        }

        public InternalEncryptionCertificate ModifyCertificatePrepare(IContext ctx, int itemId, int? agentId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var filter = new FilterEncryptionCertificate { CertificateId = new List<int> { itemId } };
                if (ctx.IsAdmin && agentId.HasValue) filter.AgentId = new List<int> { agentId.Value };
                var qry = CommonQueries.GetCertificatesQuery(dbContext, ctx, filter);

                var item = qry.Select(x => new InternalEncryptionCertificate
                {
                    Id = x.Id,
                }).FirstOrDefault();
                transaction.Complete();
                return item;
            }
        }

        public void ModifyCertificate(IContext ctx, InternalEncryptionCertificate item)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var itemDb = new EncryptionCertificates
                {
                    Id = item.Id,
                    Name = item.Name,
                    LastChangeUserId = item.LastChangeUserId,
                    LastChangeDate = item.LastChangeDate,
                };

                dbContext.EncryptionCertificatesSet.Attach(itemDb);

                var entry = dbContext.Entry(itemDb);
                entry.Property(x => x.Name).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteCertificate(IContext ctx, int itemId)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetCertificatesQuery(dbContext, ctx, new FilterEncryptionCertificate { CertificateId = new List<int> { itemId } });

                dbContext.EncryptionCertificatesSet.RemoveRange(qry);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        #endregion

        #region Utilities

        private void UnZipCertificate(InternalEncryptionCertificate item)
        {
            if (item == null || item.CertificateZip == null)
            {
                throw new EncryptionCertificateWasNotFound();
            }

            using (var memoryStream = new MemoryStream(item.CertificateZip))
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(memoryStream))
            {
                using (var streamReader = new MemoryStream())
                {
                    zip.Password = _ZipCerPassword;
                    zip.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                    var file = zip[$"{_ZipCerFileName}.pfx"];
                    file.Password = _ZipCerPassword;
                    file.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                    file.Extract(streamReader);

                    item.Certificate = streamReader.ToArray();
                }

                if (item.IsRememberPassword && zip.ContainsEntry($"{_ZipCerPasswordFileName}.txt"))
                    using (var streamReader = new MemoryStream())
                    {
                        zip.Password = _ZipCerPassword;
                        zip.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                        var file = zip[$"{_ZipCerPasswordFileName}.txt"];
                        file.Password = _ZipCerPassword;
                        file.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                        file.Extract(streamReader);

                        streamReader.Position = 0;

                        TextReader tr = new StreamReader(streamReader);
                        item.Password = tr.ReadLine();
                    }
            }
        }

        private void ZipCertificate(InternalEncryptionCertificate item)
        {
            if (item == null || item.Certificate == null)
            {
                throw new EncryptionCertificateWasNotFound();
            }

            using (var memoryStream = new MemoryStream())
            using (var cerPasswordFile = new MemoryStream())
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                zip.Password = _ZipCerPassword;
                zip.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;

                Ionic.Zip.ZipEntry file;
                file = zip.AddEntry($"{_ZipCerFileName}.pfx", item.Certificate);

                file.Password = _ZipCerPassword;
                file.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;

                if (item.IsRememberPassword && !string.IsNullOrEmpty(item.Password))
                {
                    using (TextWriter tw = new StreamWriter(cerPasswordFile))
                    {
                        tw.WriteLine(item.Password);
                        tw.Flush();
                    }

                    file = zip.AddEntry($"{_ZipCerPasswordFileName}.txt", cerPasswordFile.ToArray());

                    file.Password = _ZipCerPassword;
                    file.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                }

                zip.Save(memoryStream);
                item.CertificateZip = memoryStream.ToArray();
            }
        }

        private void ReadDetailsAboutCertificate(InternalEncryptionCertificate item)
        {
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " ReadDetailsAboutCertificate begin " + item.Certificate.Count().ToString(), @"C:\sign.log");

            var file = Path.Combine(GetTempPath(), "DMS-CERTIFICATE-" + Guid.NewGuid() + ".tmp");
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " ReadDetailsAboutCertificate " + file, @"C:\sign.log");
            try
            {
                //Если в X509Certificate2 передавать byte[] он все ровно его сохранит на диск и не факт что удалит
                using (FileStream fs = File.Create(file))
                {
                    fs.Write(item.Certificate, 0, item.Certificate.Length);
                    fs.Flush();
                }
                FileLogger.AppendTextToFile("File on disc "+File.Exists(file), @"C:\sign.log");
                FileLogger.AppendTextToFile(DateTime.Now.ToString() + " ReadDetailsAboutCertificate FileStream Write ", @"C:\sign.log");
                var certificate = new X509Certificate2(file, item.Password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                FileLogger.AppendTextToFile(DateTime.Now.ToString() + " ReadDetailsAboutCertificate new X509Certificate2 ", @"C:\sign.log");

                item.Thumbprint = certificate.Thumbprint;
                item.NotBefore = certificate.NotBefore;
                item.NotAfter = certificate.NotAfter;
            }
            catch (CryptographicException e)
            {
                FileLogger.AppendTextToFile(DateTime.Now.ToString() + " ReadDetailsAboutCertificate1 " + e.Message, @"C:\sign.log", e);

                throw e;

            }
            catch (Exception e)
            {
                FileLogger.AppendTextToFile(DateTime.Now.ToString() + " ReadDetailsAboutCertificate2 " + e.Message, @"C:\sign.log", e);

                throw e;
            }
            finally
            {
                File.Delete(file);
            }
        }

        private void AddCertificateInWindowsCertificateStores(InternalEncryptionCertificate item)
        {
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + "AddCertificateInWindowsCertificateStores begin", @"C:\sign.log");

            var file = Path.Combine(GetTempPath(), "DMS-CERTIFICATE-" + Guid.NewGuid() + ".tmp");
            try
            {

                //Если в X509Certificate2 передавать byte[] он все ровно его сохранит на диск и не факт что удалит
                File.WriteAllBytes(file, item.Certificate);

                var certificate = new X509Certificate2(file, item.Password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

                var store = new X509Store(StoreName.TrustedPeople, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(certificate);
                store.Close();
            }
            catch (CryptographicException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                File.Delete(file);
            }
            FileLogger.AppendTextToFile(DateTime.Now.ToString() +" AddCertificateInWindowsCertificateStores end ", @"C:\sign.log");

        }

        #region Convert
        private byte[] GetBytesByData(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        private string GetStringByData(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", String.Empty);
        }

        private byte[] GetBytesByBase64(string data)
        {
            return Convert.FromBase64String(data);
        }

        private string GetStringByBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        #endregion Convert

        #region функции CryptoExts.dll

        //получить сообщение об ошибке в модуле CryptoExts.dll, 
        //ошибки в Java выводятся в окно (клиентское приложение) и в лог-файл (лог для серверного) (см. silentMode для управления)
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetMessageError();

        //старт Java JVM
        //installDir - абсолютный путь к CryptoExts.dll
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StartJVM(char[] installDir);

        //получить список валидных сертификатов пользователя как CN (ФИО для физ.лиц), разделитель "|"
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetCNString();

        //получить число валидных сертификатов пользователя
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetCertCount();

        //получить список дат окончания валидных сертификатов пользователя, разделитель "|"
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetDateToAsString();

        //выбрать файл pfx
        //возвращает 0, если успешно (и записывает путь к pfx в конфиг файл, при следующем запуске выбирать не надо, а только ввести ПИН)
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SelectPfxPathInt();

        //очистить в файле конфигурации запись о выбранном раньше файле pfx, после чего его сертификат не отображается в списке сертификатов
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ClearPfxProperties();

        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ViewCert(char[] index);

        //выбрать документ PDF для подписи
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SelectPdfFile();

        //показать/открыть выбранный PDF
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ViewPdf(char[] fileName);

        //подписать выбранный PDF, возвращает путь+имя подписанного (в той же папке, но к имени добавляется *.sign.pdf)
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SignFilePdf(char[] data);

        //проверить подпись(и) PDF, подписанного в этом сеансе (контрольная проверка)
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VerifyPdf();

        //получить число подписей PDF, вызывать после проверки подписи
        //иначе вернет -1, для не подписанного вернет 0
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSignCountPdf();

        //получить ссертификат подписи PDF, заданного индекса подписи (от 0); возвращает сертификат в Base64
        //вызывать после проверки подписи
        //иначе вернет пустую строку, как для не подписанного
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetSignCertPdf(char[] index);

        //загрузить PDF документ с заданного URL, возвращает путь+имя загруженного
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Download(char[] data);

        //установить режим вывода сообщений во всплывающих окнах
        // true - подавить об успешном завершении, выводить только об ошибках
        // true - выводить и об успешном завершении, и об ошибках
        // null - не выводить никаких сообщений во всплывающих окнах (для операций сервера)
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SetSilentMode(char[] mode);

        //подписать данные (заданы в Base64) в формате PKCS#7
        //возвращает - подпись в Base64
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetSignP7(char[] data);

        //проверить подпись данных (заданы в Base64) в формате PKCS#7
        //возвращает - true/false
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SignVerify(char[] data);

        //получить число подписей PKCS#7, вызывать после проверки подписи
        //иначе вернет -1, для не подписанного вернет 0
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetSignCountP7();

        //получить ссертификат подписи PKCS#7, заданного индекса подписи (от 0); возвращает сертификат в Base64
        //вызывать после проверки подписи
        //иначе вернет пустую строку, как для не подписанного
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetSignCertP7(char[] index);

        //подписать данные (заданы локальным файлом, путь абсолюный) в формате PKCS#7
        //возвращает - подпись в Base64
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetSignP7File(char[] data);

        //проверить подпись данных (заданы в локальным файлом, путь абсолюный) в формате PKCS#7
        //возвращает - true/false
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SignVerifyFile(char[] data);

        //подписать PDF (задан локальным файлом, путь абсолюный) 
        //возвращает - абсолютный путь подписанного PDF
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SignFilePdfPath(char[] data);
        //проверить подпись(и) PDF, подписанного в этом сеансе (контрольная проверка)
        [DllImport("CryptoExts.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SignFilePdfVerify(char[] data);

        public static string StringFromNativeUtf8(IntPtr nativeUtf8)
        {
            //Native UTF-8 -> Windows-1251
            int len = 0;
            while (Marshal.ReadByte(nativeUtf8, len) != 0) ++len;
            if (len > 0)
            {
                byte[] buffer = new byte[len];
                Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer);
            }
            return "";
        }

        private void InitJVM()
        {
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " InitJVM begin ", @"C:\sign.log");

            string installDir = string.Empty;
            if (IntPtr.Size == 4)
            {
                // 32-bit
                installDir = Path.Combine(HttpContext.Current.Server.MapPath("~/"), "App_Data", "CryptoExts", "x86", "CryptoExts.dll");
            }
            else if (IntPtr.Size == 8)
            {
                // 64-bit
                installDir = Path.Combine(HttpContext.Current.Server.MapPath("~/"), "App_Data", "CryptoExts", "x64", "CryptoExts.dll");
            }
            try
            {

                installDir += '\0';

                var isStart = StartJVM(installDir.ToCharArray());
                if (isStart)
                {
                    IntPtr cnstring = GetCNString();
                    string[] certs = StringFromNativeUtf8(cnstring).Split('|');
                    FileLogger.AppendTextToFile(DateTime.Now.ToString() + " СЕРТИФИКАТЫ " + StringFromNativeUtf8(cnstring), @"C:\sign.log");

                    SetSilentMode("null".ToCharArray());//не выводить никаких сообщений во всплывающих окнах (для операций сервера)
                }
            }
            catch (Exception ex)
            {
                try
                {
                    StartJVM(installDir.ToCharArray());

                    SetSilentMode("null".ToCharArray());//не выводить никаких сообщений во всплывающих окнах (для операций сервера)

                    IntPtr cnstring = GetCNString();
                    string[] certs = StringFromNativeUtf8(cnstring).Split('|');

                }
                catch (Exception ex2)
                {
                    throw ex2;
                }
            }
        }

        #endregion

        #endregion

        #region CertificateSign

        public string GetCertificateSign(IContext ctx, int certificateId, string certificatePassword, string dataToSign)
        {
            //return dataToSign;
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " GetCertificateSign begin ", @"C:\sign.log");
            var res = string.Empty;

            try
            {
                var certificate = GetCertificate(ctx, certificateId);
                certificate.Password = certificatePassword;

                UnZipCertificate(certificate);
                ReadDetailsAboutCertificate(certificate);
                AddCertificateInWindowsCertificateStores(certificate);

                if ((certificate.NotBefore.HasValue && certificate.NotBefore > DateTime.UtcNow)
                    || (certificate.NotAfter.HasValue && certificate.NotAfter < DateTime.UtcNow))
                    throw new EncryptionCertificateHasExpired();

                InitJVM();

                //BL.CrossCutting.Helpers.Logger.SaveToFile("E GetCertificateSign " + dataToSign);
                dataToSign = dataToSign.ToTransliteration();
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(dataToSign));

                string data = "{\"indata\":\"" + base64 + "\",\"inindex\":" + certificate.Thumbprint.ToLower() + ",\"intspUrl\":\"http://cesaris.itsway.kiev.ua/tsa/srv/\",\"incrl\":true,\"inisDetached\":true}";

                data += '\0';

                IntPtr signed = GetSignP7(data.ToCharArray());
                res = StringFromNativeUtf8(signed);

                //BL.CrossCutting.Helpers.Logger.SaveToFile("E GetCertificateSign SIGN " + res);

                if (string.IsNullOrEmpty(res))
                {
                    throw new EncryptionCertificateWasNotFound();
                }

                res = res.Replace("\r\n", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

                data = "{ \"data\":\"" + base64 + "\",\"sign\":\"" + res + "\"}";

                data += '\0';

                var res2 = SignVerify(data.ToCharArray());

                if (!res2)
                    throw new EncryptionCertificateDigitalSignatureFailed();
            }
            catch (EncryptionCertificateWasNotFound ex)
            {
                throw ex;
            }
            catch (EncryptionCertificateHasExpired ex)
            {
                throw ex;
            }
            catch (EncryptionCertificateDigitalSignatureFailed ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " GetCertificateSign end ", @"C:\sign.log");
            return res;
        }

        public bool VerifyCertificateSign(IContext ctx, string dataToSign, string sign)
        {
            //return true;
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " VerifyCertificateSign begin ", @"C:\sign.log");
            var res = false;
            try
            {
                InitJVM();
                dataToSign = dataToSign.ToTransliteration();

                sign = sign.Replace("\r\n", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

                var dataToSignBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(dataToSign));

                string data = "{ \"data\":\"" + dataToSignBase64 + "\",\"sign\":\"" + sign + "\"}";

                data += '\0';

                res = SignVerify(data.ToCharArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " VerifyCertificateSign end ", @"C:\sign.log");
            return res;
        }

        public byte[] GetCertificateSignPdf(IContext ctx, int certificateId, string certificatePassword, byte[] pdf)
        {
            //return pdf;
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " GetCertificateSignPdf begin ", @"C:\sign.log");
            byte[] res = null;

            try
            {
                var certificate = GetCertificate(ctx, certificateId);
                certificate.Password = certificatePassword;

                UnZipCertificate(certificate);
                FileLogger.AppendTextToFile(DateTime.Now.ToString() + " GetCertificateSignPdf UnZipCertificate ", @"C:\sign.log");

                ReadDetailsAboutCertificate(certificate);
                FileLogger.AppendTextToFile(DateTime.Now.ToString() + " GetCertificateSignPdf ReadDetailsAboutCertificate ", @"C:\sign.log");

                AddCertificateInWindowsCertificateStores(certificate);

                if ((certificate.NotBefore.HasValue && certificate.NotBefore > DateTime.UtcNow)
                    || (certificate.NotAfter.HasValue && certificate.NotAfter < DateTime.UtcNow))
                    throw new EncryptionCertificateHasExpired();

                InitJVM();
                var file = Path.Combine(GetTempPath(), "DMS-PDF-" + Guid.NewGuid() + ".pdf");
                //var file = "c://tmp//DMS.PDF";
                var signedPdf = string.Empty;
                try
                {
                    File.WriteAllBytes(file, pdf);
                    file = file.Replace("\\", "/");

                    //silentMode = null - подавить все сообщения
                    string data = "{\"setSilentMode\":null,\"inindex\":" + certificate.Thumbprint.ToLower() + ",\"intsaUrl\":\"http://cesaris.itsway.kiev.ua/tsa/srv/\",\"incrl\":true,\"file\":\"" + file + "\"}";

                    data += '\0';

                    IntPtr ptrpdf = SignFilePdfPath(data.ToCharArray());
                    signedPdf = StringFromNativeUtf8(ptrpdf);

                    if (string.IsNullOrEmpty(signedPdf))
                    {
                        throw new EncryptionCertificateWasNotFound();
                    }

                    using (var signedPdfFileStream = new StreamReader(signedPdf))
                    using (var ms = new MemoryStream())
                    {
                        signedPdfFileStream.BaseStream.CopyTo(ms);
                        res = ms.ToArray();
                    }
                }
                catch (CryptographicException e)
                {
                    throw e;
                }
                catch (EncryptionCertificateHasExpired e)
                {
                    throw e;
                }
                catch (EncryptionCertificateWasNotFound e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    File.Delete(file);
                    if (!string.IsNullOrEmpty(signedPdf) && File.Exists(signedPdf))
                    {
                        File.Delete(signedPdf);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " GetCertificateSignPdf end ", @"C:\sign.log");

            return res;
        }

        public bool VerifyCertificateSignPdf(byte[] pdf)
        {
            //return true;
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " VerifyCertificateSignPdf begin ", @"C:\sign.log");
            int res = -1;

            try
            {
                InitJVM();
                var file = Path.Combine(GetTempPath(), "DMS-SIGN-" + Guid.NewGuid() + ".pdf");
                try
                {
                    File.WriteAllBytes(file, pdf);

                    file = file.Replace("\\", "/").ToLower();

                    file += '\0';

                    var fileArray = file.ToCharArray();
                    var tmp = string.Empty;
                    foreach (var tFileArray in fileArray)
                    {
                        tmp += tFileArray;
                    }

                    res = SignFilePdfVerify(file.ToCharArray());
                }
                catch (CryptographicException e)
                {
                    throw e;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {

            }
            FileLogger.AppendTextToFile(DateTime.Now.ToString() + " VerifyCertificateSignPdf end ", @"C:\sign.log");
            return res == 0;
        }
        #endregion

        #region InternalSign
        public string GetInternalSign(string dataToSign)
        {
            byte[] dataToSignBytes = GetBytesByData(dataToSign);

            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

            RSAalg.FromXmlString(_RSAKeyXml);

            var sign = RSAalg.SignData(dataToSignBytes, new SHA512CryptoServiceProvider());

            return GetStringByBase64(sign);
        }

        public bool VerifyInternalSign(string dataToVerify, string signedData)
        {
            if (string.IsNullOrEmpty(signedData)) return false;

            byte[] dataToVerifyBytes = GetBytesByData(dataToVerify);
            byte[] signedDataBytes = GetBytesByBase64(signedData);

            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

            RSAalg.FromXmlString(_RSAKeyXml);

            var res = RSAalg.VerifyData(dataToVerifyBytes, new SHA512CryptoServiceProvider(), signedDataBytes);

            return res;
        }
        #endregion
    }
}