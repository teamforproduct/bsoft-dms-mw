using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentAdditional;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;


namespace BL.Database.Documents
{
    public class DocumentFileDbProcess : CoreDb.CoreDb, IDocumentFileDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public DocumentFileDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        public IEnumerable<DocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var sq = dbContext.DocumentFilesSet
                    .Where(x => x.DocumentId == documentId)
                    .GroupBy(g => new {g.DocumentId, g.OrderNumber})
                    .Select(
                        x => new {DocId = x.Key.DocumentId, OrdId = x.Key.OrderNumber, MaxVers = x.Max(s => s.Version)});

                return
                    sq.Join(dbContext.DocumentFilesSet, sub => new {sub.DocId, sub.OrdId, VerId = sub.MaxVers},
                        fl => new {DocId = fl.DocumentId, OrdId = fl.OrderNumber, VerId = fl.Version},
                        (s, f) => new {fl = f})
                        .Join(dbContext.DictionaryAgentsSet, df => df.fl.LastChangeUserId, da => da.Id,
                            (d, a) => new {d.fl, agName = a.Name})
                        .Select(x => new DocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extension,
                            FileData = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            WasChangedExternal = false
                        }).ToList();
            }
        }

        public IEnumerable<DocumentAttachedFile> GetDocumentFileVersions(IContext ctx, int documentId, int orderNumber)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return
                    dbContext.DocumentFilesSet
                        .Where(x => x.DocumentId == documentId && x.OrderNumber == orderNumber)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new {fl = d, agName = a.Name})
                        .Select(x => new DocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extension,
                            FileData = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            WasChangedExternal = false
                        }).ToList();
            }
        }

        public DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return
                    dbContext.DocumentFilesSet
                        .Where(
                            x =>
                                x.DocumentId == documentId && x.Version == versionNumber && x.OrderNumber == orderNumber)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new {fl = d, agName = a.Name})
                        .Select(x => new DocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extension,
                            FileData = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            WasChangedExternal = false
                        }).FirstOrDefault();
            }
        }

        public DocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return
                    dbContext.DocumentFilesSet
                        .Where(x => x.Id == id)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new {fl = d, agName = a.Name})
                        .Select(x => new DocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extension,
                            FileData = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            WasChangedExternal = false
                        }).FirstOrDefault();
            }
        }

        public DocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, int documentId, int orderNumber)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var maxVer =
                    dbContext.DocumentFilesSet.Where(x => x.DocumentId == documentId && x.OrderNumber == orderNumber)
                        .Max(m => m.Version);
                if (maxVer > 0)
                {
                    return
                        dbContext.DocumentFilesSet
                            .Where(
                                x => x.DocumentId == documentId && x.Version == maxVer && x.OrderNumber == orderNumber)
                            .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                                (d, a) => new {fl = d, agName = a.Name})
                            .Select(x => new DocumentAttachedFile
                            {
                                Id = x.fl.Id,
                                Date = x.fl.Date,
                                DocumentId = x.fl.DocumentId,
                                Extension = x.fl.Extension,
                                FileData = x.fl.Content,
                                IsAdditional = x.fl.IsAdditional,
                                Hash = x.fl.Hash,
                                LastChangeDate = x.fl.LastChangeDate,
                                LastChangeUserId = x.fl.LastChangeUserId,
                                LastChangeUserName = x.agName,
                                Name = x.fl.Name,
                                OrderInDocument = x.fl.OrderNumber,
                                Version = x.fl.Version,
                                WasChangedExternal = false
                            }).FirstOrDefault();
                }
            }
            return null;
        }

        public int AddNewFileOrVersion(IContext ctx, DocumentAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var fl = new DBModel.Document.DocumentFiles
                {
                    Date = docFile.Date,
                    Content = null,
                    DocumentId = docFile.DocumentId,
                    Extension = docFile.Extension,
                    Hash = docFile.Hash,
                    IsAdditional = docFile.IsAdditional,
                    LastChangeDate = docFile.LastChangeDate,
                    LastChangeUserId = docFile.LastChangeUserId,
                    Name = docFile.Name,
                    OrderNumber = docFile.OrderInDocument,
                    Version = docFile.Version
                };
                dbContext.DocumentFilesSet.Add(fl);
                dbContext.SaveChanges();
                return fl.Id;
            }
        }

        public void UpdateFileOrVersion(IContext ctx, DocumentAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var fl =
                    dbContext.DocumentFilesSet.FirstOrDefault(x => x.Id == docFile.Id ||
                                                                   (x.DocumentId == docFile.DocumentId &&
                                                                    x.OrderNumber == docFile.OrderInDocument &&
                                                                    x.Version == docFile.Version));
                if (fl != null)
                {
                    fl.DocumentId = docFile.DocumentId;
                    fl.Extension = docFile.Extension;
                    fl.Hash = docFile.Hash;
                    fl.IsAdditional = docFile.IsAdditional;
                    fl.LastChangeDate = docFile.LastChangeDate;
                    fl.LastChangeUserId = docFile.LastChangeUserId;
                    fl.Name = docFile.Name;
                }
                dbContext.SaveChanges();
            }
        }

        public void DeleteAttachedFile(IContext ctx, DocumentAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var fl =
                    dbContext.DocumentFilesSet.FirstOrDefault(x => x.Id == docFile.Id ||
                                                                   (x.DocumentId == docFile.DocumentId &&
                                                                    x.OrderNumber == docFile.OrderInDocument &&
                                                                    x.Version == docFile.Version));
                if (fl != null)
                {
                    dbContext.DocumentFilesSet.Remove(fl);
                    dbContext.SaveChanges();
                }
            }
        }

        public int GetNextFileOrderNumber(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                if (dbContext.DocumentFilesSet.Any(x => x.DocumentId == documentId))
                {
                    return dbContext.DocumentFilesSet.Where(x => x.DocumentId == documentId).Max(x => x.OrderNumber) + 1;
                }
            }
            return 1;
        }

        public int GetFileNextVersion(IContext ctx, int documentId, int fileOrder)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                if (dbContext.DocumentFilesSet.Any(x => x.DocumentId == documentId && x.OrderNumber == fileOrder))
                {
                    return
                        dbContext.DocumentFilesSet.Where(x => x.DocumentId == documentId && x.OrderNumber == fileOrder)
                            .Max(x => x.Version) + 1;
                }
            }
            return 1;
        }
    }
}