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
using System.IO.Compression;
using BL.Database.DBModel.Encryption;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Database.EncryptionWorker;
using BL.CrossCutting.DependencyInjection;

namespace BL.Database.Encryption
{
    internal class EncryptionDbProcess : CoreDb.CoreDb, IEncryptionDbProcess
    {
        private string _ZipCerPassword { get; } = "Qk1ncZA|HzaNiQ?8*3QjCMHQqIXQMce6m0JDqup~Nh13#H6{~BG9v2@xIGFKa%Hq5T$kEDo{mYGtXWryVCoR2PtA}QSvG*zPe6%9Q{T?4T*NOM{R}LgLkThZRF$%tb#wEw@pU7${oJzbdSq{3LsM?SOH0GMnub@#N|PH$Z}8gh%c}Wpb8hf$AhMM0@4%cusABMiP}{wIl*QEtKzqS5TZnRHOt~k3uEBc9d6%vuX6UKD0@eithk%ACMkrtrfIpY";
        private string _ZipCerName { get; } = "cer";
        public EncryptionDbProcess()
        {
        }

        #region Certificates
        public IEnumerable<FrontEncryptionCertificate> GetCertificates(IContext ctx, FilterEncryptionCertificate filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
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
                    ValidFromDate = x.ValidFromDate,
                    ValidToDate = x.ValidToDate,
                    IsPublic = x.IsPublic,
                    IsPrivate = x.IsPrivate,
                    AgentId = x.AgentId,
                    AgentName = x.Agent.Name,

                    Type = (EnumEncryptionCertificateTypes)x.TypeId,
                    TypeName = x.Type.Name,

                    Extension = x.Extension,

                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                });

                var res = qryFE.ToList();

                return res;
            }
        }

        public void AddCertificate(IContext ctx, InternalEncryptionCertificate item)
        {
            using (var memoryStream = new MemoryStream())
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                zip.Password = _ZipCerPassword;
                zip.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;

                Ionic.Zip.ZipEntry file;
                if (item.PostedFileData!=null)
                {
                    file = zip.AddEntry($"{_ZipCerName}.{item.Extension}", item.PostedFileData.InputStream);
                }
                else
                {
                    file = zip.AddEntry($"{_ZipCerName}.{item.Extension}", item.Certificate);
                }

                file.Password = _ZipCerPassword;
                file.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                zip.Save(memoryStream);
                item.Certificate = memoryStream.ToArray();
            }

            using (var dbContext = new DmsContext(ctx))
            {
                var itemDb = ModelConverter.GetDbEncryptionCertificate(item);

                dbContext.EncryptionCertificatesSet.Add(itemDb);
                dbContext.SaveChanges();

                item.Id = itemDb.Id;
            }
        }

        public InternalEncryptionCertificate ModifyCertificatePrepare(IContext ctx, int itemId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetCertificatesQuery(dbContext, ctx, new FilterEncryptionCertificate { CertificateId = new List<int> { itemId } });

                var item = qry.Select(x => new InternalEncryptionCertificate
                {
                    Id = x.Id,
                }).FirstOrDefault();

                return item;
            }
        }

        public void ModifyCertificate(IContext ctx, InternalEncryptionCertificate item)
        {
            using (var dbContext = new DmsContext(ctx))
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

            }
        }

        public void DeleteCertificate(IContext ctx, int itemId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetCertificatesQuery(dbContext, ctx, new FilterEncryptionCertificate { CertificateId = new List<int> { itemId } });

                dbContext.EncryptionCertificatesSet.RemoveRange(qry);
                dbContext.SaveChanges();
            }
        }

        public InternalEncryptionCertificate ExportEncryptionCertificatePrepare(IContext ctx, int itemId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetCertificatesQuery(dbContext, ctx, new FilterEncryptionCertificate { CertificateId = new List<int> { itemId }, IsPublic = true });

                var item = qry.Select(x => new InternalEncryptionCertificate
                {
                    Id = x.Id,
                    IsPublic = x.IsPublic,
                    IsPrivate = x.IsPrivate,
                    Type = (EnumEncryptionCertificateTypes)x.TypeId,
                }).FirstOrDefault();

                return item;
            }
        }

        public FrontEncryptionCertificate ExportEncryptionCertificate(IContext ctx, int itemId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetCertificatesQuery(dbContext, ctx, new FilterEncryptionCertificate { CertificateId = new List<int> { itemId } });

                var itemDb = qry.Select(x => new InternalEncryptionCertificate
                {
                    Id = x.Id,
                    Name = x.Name,
                    Certificate = x.Certificate,
                    Extension = x.Extension,
                    IsPublic = x.IsPublic,
                    IsPrivate = x.IsPrivate,
                    ValidFromDate = x.ValidFromDate,
                    ValidToDate = x.ValidToDate,
                    CreateDate = x.CreateDate,
                    Type = (EnumEncryptionCertificateTypes)x.TypeId
                }).FirstOrDefault();

                if (itemDb == null)
                {
                    throw new EncryptionCertificateWasNotFound();
                }
                else if (!(itemDb.IsPublic && ((itemDb.IsPrivate && new List<EnumEncryptionCertificateTypes> { EnumEncryptionCertificateTypes.RSA }.Contains(itemDb.Type)) || !itemDb.IsPrivate)))
                {
                    throw new EncryptionCertificatePrivateKeyСanNotBeExported();
                }

                var item = new FrontEncryptionCertificate(itemDb);

                using (var memoryStream = new MemoryStream(itemDb.Certificate))
                using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(memoryStream))
                using (var streamReader = new MemoryStream())
                {
                    zip.Password = _ZipCerPassword;
                    zip.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                    var file = zip[$"{_ZipCerName}.{itemDb.Extension}"];
                    file.Password = _ZipCerPassword;
                    file.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                    file.Extract(streamReader);

                    itemDb.Certificate = streamReader.ToArray();

                    if (itemDb.IsPrivate && itemDb.Type == EnumEncryptionCertificateTypes.RSA)
                    {
                        var egk = DmsResolver.Current.Get<IEncryptionGeneratorKey>();
                        itemDb.Certificate = egk.GetPublicKey(ctx, itemDb);
                    }

                    item.Content = System.Convert.ToBase64String(itemDb.Certificate);
                }

                return item;
            }
        }
        #endregion

        #region CertificateTypes
        public IEnumerable<FrontEncryptionCertificateType> GetCertificateTypes(IContext ctx, FilterEncryptionCertificateType filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetCertificateTypesQuery(dbContext, ctx, filter);

                //TODO Sort
                {
                    qry = qry.OrderByDescending(x => x.LastChangeDate);
                }


                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontEncryptionCertificateType>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }

                var qryFE = qry.Select(x => new FrontEncryptionCertificateType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                });

                var res = qryFE.ToList();

                return res;
            }
        }

        public void AddCertificateType(IContext ctx, InternalEncryptionCertificateType item)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var itemDb = ModelConverter.GetDbEncryptionCertificateType(item);

                dbContext.EncryptionCertificateTypesSet.Add(itemDb);
                dbContext.SaveChanges();

                item.Id = itemDb.Id;
            }
        }

        public InternalEncryptionCertificateType ModifyCertificateTypePrepare(IContext ctx, int itemId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetCertificateTypesQuery(dbContext, ctx, new FilterEncryptionCertificateType { TypeId = new List<int> { itemId } });

                var item = qry.Select(x => new InternalEncryptionCertificateType
                {
                    Id = x.Id,
                }).FirstOrDefault();

                return item;
            }
        }

        public void ModifyCertificateType(IContext ctx, InternalEncryptionCertificateType item)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var itemDb = new EncryptionCertificateTypes
                {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    LastChangeUserId = item.LastChangeUserId,
                    LastChangeDate = item.LastChangeDate,
                };

                dbContext.EncryptionCertificateTypesSet.Attach(itemDb);

                var entry = dbContext.Entry(itemDb);
                entry.Property(x => x.Code).IsModified = true;
                entry.Property(x => x.Name).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();

            }
        }

        public void DeleteCertificateType(IContext ctx, int itemId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = CommonQueries.GetCertificateTypesQuery(dbContext, ctx, new FilterEncryptionCertificateType { TypeId = new List<int> { itemId } });

                dbContext.EncryptionCertificateTypesSet.RemoveRange(qry);
                dbContext.SaveChanges();
            }
        }
        #endregion
    }
}