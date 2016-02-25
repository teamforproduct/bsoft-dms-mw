using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;


namespace BL.Database.Documents
{
    public class DocumentFileDbProcess : CoreDb.CoreDb, IDocumentFileDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public DocumentFileDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        public IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return CommonQueries.GetDocumentFiles(dbContext, documentId);
            }
        }

        public IEnumerable<FrontDocumentAttachedFile> GetDocumentFileVersions(IContext ctx, int documentId, int orderNumber)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return
                    dbContext.DocumentFilesSet
                        .Where(x => x.DocumentId == documentId && x.OrderNumber == orderNumber)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new {fl = d, agName = a.Name})
                        .Select(x => new FrontDocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extension,
                            FileContent = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            FileType = x.fl.FileType,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            WasChangedExternal = false
                        }).ToList();
            }
        }

        public FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber)
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
                        .Select(x => new FrontDocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extension,
                            FileContent = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            FileType = x.fl.FileType,
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

        public FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return
                    dbContext.DocumentFilesSet
                        .Where(x => x.Id == id)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new {fl = d, agName = a.Name})
                        .Select(x => new FrontDocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extension,
                            FileContent = x.fl.Content,
                            IsAdditional = x.fl.IsAdditional,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            FileType = x.fl.FileType,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            WasChangedExternal = false
                        }).FirstOrDefault();
            }
        }

        public InternalDocument ModifyDocumentFilePrepare(IContext ctx, int documentId, int orderNumber)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var maxVer =
                    dbContext.DocumentFilesSet.Where(x => x.DocumentId == documentId && x.OrderNumber == orderNumber)
                        .Max(m => m.Version);
                if (maxVer > 0)
                {
                    var doc = CommonQueries.GetDocumentQuery(dbContext)
                                        .Where(x => x.Doc.Id == documentId && ctx.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId))
                                        .Select(x => new InternalDocument
                                        {
                                            Id = x.Doc.Id,
                                        }).FirstOrDefault();

                    if (doc == null) return null;

                    doc.DocumentFiles =  dbContext.DocumentFilesSet
                            .Where(
                                x => x.DocumentId == documentId && x.Version == maxVer && x.OrderNumber == orderNumber)
                            .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                                (d, a) => new { fl = d, agName = a.Name })
                            .Select(x => new InternalDocumentAttachedFile
                            {
                                Id = x.fl.Id,
                                Date = x.fl.Date,
                                DocumentId = x.fl.DocumentId,
                                OrderInDocument = x.fl.OrderNumber,
                                Version = x.fl.Version
                            }).ToList();
                    return doc;
                }
            }
            return null;
        }

        public FrontDocumentAttachedFile GetDocumentFileLatestVersion(IContext ctx, int documentId, int orderNumber)
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
                            .Select(x => new FrontDocumentAttachedFile
                            {
                                Id = x.fl.Id,
                                Date = x.fl.Date,
                                DocumentId = x.fl.DocumentId,
                                Extension = x.fl.Extension,
                                FileContent = x.fl.Content,
                                IsAdditional = x.fl.IsAdditional,
                                Hash = x.fl.Hash,
                                LastChangeDate = x.fl.LastChangeDate,
                                LastChangeUserId = x.fl.LastChangeUserId,
                                LastChangeUserName = x.agName,
                                Name = x.fl.Name,
                                FileType = x.fl.FileType,
                                OrderInDocument = x.fl.OrderNumber,
                                Version = x.fl.Version,
                                WasChangedExternal = false
                            }).FirstOrDefault();
                }
            }
            return null;
        }

        public InternalDocument AddDocumentFilePrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext)
                    .Where(x => x.Doc.Id == documentId && ctx.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id
                    }).FirstOrDefault();
                return doc;
            }
        }

        public int AddNewFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var fl = ModelConverter.GetDbDocumentFile(docFile);
                dbContext.DocumentFilesSet.Add(fl);
                dbContext.SaveChanges();
                return fl.Id;
            }
        }

        public void UpdateFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var fl = ModelConverter.GetDbDocumentFile(docFile);
                dbContext.DocumentFilesSet.Attach(fl);
                var entry = dbContext.Entry(fl);
                entry.Property(x => x.Name).IsModified = true;
                entry.Property(x => x.Extension).IsModified = true;
                entry.Property(x => x.FileType).IsModified = true;
                entry.Property(x => x.IsAdditional).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.Date).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public InternalDocument DeleteDocumentFilePrepare(IContext ctx, FilterDocumentFileIdentity flIdent)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext)
                    .Where(x => x.Doc.Id == flIdent.DocumentId && ctx.CurrentPositionsIdList.Contains(x.Doc.ExecutorPositionId))
                    .Select(x => new InternalDocument
                    {
                        Id = x.Doc.Id,
                    }).FirstOrDefault();
                if (doc == null) return null;

                doc.DocumentFiles =
                    dbContext.DocumentFilesSet.Where(
                        x => x.DocumentId == flIdent.DocumentId && x.OrderNumber == flIdent.OrderInDocument)
                        .Select(x => new InternalDocumentAttachedFile {Id = x.Id});
                return doc;
            }
        }

        /// <summary>
        /// Delete of hole file with all versions
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="docFile"> should be filled DocumentId and OrderInDocument fields</param>
        public void DeleteAttachedFile(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                dbContext.DocumentFilesSet.RemoveRange(
                    dbContext.DocumentFilesSet.Where(
                        x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument));
                dbContext.SaveChanges();

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