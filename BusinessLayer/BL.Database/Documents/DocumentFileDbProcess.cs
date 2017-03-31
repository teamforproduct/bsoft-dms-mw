using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.Enums;
using BL.CrossCutting.Helpers;
using BL.Model.FullTextSearch;

namespace BL.Database.Documents
{
    public class DocumentFileDbProcess : CoreDb.CoreDb, IDocumentFileDbProcess
    {
        public IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentFiles(ctx, dbContext, filter, paging);
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(
                            x =>
                                x.DocumentId == documentId && x.Version == versionNumber && x.OrderNumber == orderNumber)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new FrontDocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Extension = x.fl.Extension,
                            FileContent = x.fl.Content,
                            Type = (EnumFileTypes)x.fl.TypeId,
                            TypeName = x.fl.Type.Name,
                            Hash = x.fl.Hash,
                            FileType = x.fl.FileType,
                            FileSize = x.fl.FileSize,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            Name = x.fl.Name,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            WasChangedExternal = false,
                            ExecutorPositionName = x.fl.ExecutorPosition.Name,
                            ExecutorPositionExecutorAgentName = x.fl.ExecutorPositionExecutorAgent.Name + (x.fl.ExecutorPositionExecutorType.Suffix != null ? " (" + x.fl.ExecutorPositionExecutorType.Suffix + ")" : null),
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public InternalDocumentAttachedFile GetInternalAttachedFile(IContext ctx, int fileId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return
                    dbContext.DocumentFilesSet.Where(x => x.Id == fileId)
                        .Select(x => new InternalDocumentAttachedFile
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            PdfCreated = x.IsPdfCreated ?? false,
                            LastPdfAccess = x.LastPdfAccessDate//??DateTime.MinValue
                        }).FirstOrDefault();
            }
        }

        public InternalDocument ModifyDocumentFilePrepare(IContext ctx, int documentId, int orderNumber, int version)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, null, true, true)
                                    .Where(x => x.Id == documentId)
                                    .Select(x => new InternalDocument
                                    {
                                        Id = x.Id,
                                        ClientId = x.ClientId,
                                        EntityTypeId = x.EntityTypeId,
                                        ExecutorPositionId = x.ExecutorPositionId
                                    }).FirstOrDefault();

                if (doc == null) return null;

                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId && x.Version == version && x.OrderNumber == orderNumber)
                        .Where(x => !x.IsDeleted)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new InternalDocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            ClientId = x.fl.ClientId,
                            EntityTypeId = x.fl.EntityTypeId,
                            Date = x.fl.Date,
                            Name = x.fl.Name,
                            DocumentId = x.fl.DocumentId,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            ExecutorPositionId = x.fl.ExecutorPositionId,
                            IsWorkedOut = x.fl.IsWorkedOut,
                            Type = (EnumFileTypes)x.fl.TypeId,
                            IsMainVersion = x.fl.IsMainVersion,
                            FileSize = x.fl.FileSize,
                            Extension = x.fl.Extension,
                            FileType = x.fl.FileType,
                            Hash = x.fl.Hash,
                            Description = x.fl.Description,
                            IsDeleted = x.fl.IsDeleted,
                            FileContent = x.fl.Content
                        }).ToList();

                transaction.Complete();
                return doc;
            }
        }
        public InternalDocument RenameDocumentFilePrepare(IContext ctx, int documentId, int orderNumber)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, null, true, true)
                                    .Where(x => x.Id == documentId)
                                    .Select(x => new InternalDocument
                                    {
                                        Id = x.Id,
                                        ClientId = x.ClientId,
                                        EntityTypeId = x.EntityTypeId,
                                        ExecutorPositionId = x.ExecutorPositionId
                                    }).FirstOrDefault();

                if (doc == null) return null;

                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(
                            x => x.DocumentId == documentId && x.OrderNumber == orderNumber)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new InternalDocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            ClientId = x.fl.ClientId,
                            EntityTypeId = x.fl.EntityTypeId,
                            Date = x.fl.Date,
                            Name = x.fl.Name,
                            Extension = x.fl.Extension,
                            DocumentId = x.fl.DocumentId,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            ExecutorPositionId = x.fl.ExecutorPositionId,
                            IsWorkedOut = x.fl.IsWorkedOut,
                            IsMainVersion = x.fl.IsMainVersion,
                            Type = (EnumFileTypes)x.fl.TypeId,
                        }).ToList();
                transaction.Complete();
                return doc;
            }
        }

        public InternalDocument AddDocumentFilePrepare(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, null, true, true)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered,
                        DocumentFiles = x.Files.Where(y => y.IsMainVersion).Where(y => !y.IsDeleted)
                        .Select(y =>
                        new InternalDocumentAttachedFile
                        {
                            Id = y.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            Name = y.Name,
                            Extension = y.Extension,
                            ExecutorPositionId = y.ExecutorPositionId,
                            FileType = y.FileType,
                            Type = (EnumFileTypes)y.TypeId,
                            OrderInDocument = y.OrderNumber
                        })
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }

        public int AddNewFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                if (docFile.IsMainVersion)
                {
                    foreach (var docFileId in dbContext.DocumentFilesSet
                                            .Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument)
                                            .Select(x => x.Id).ToList())
                    {
                        var file = new DBModel.Document.DocumentFiles
                        {
                            Id = docFileId,
                            IsMainVersion = false
                        };
                        dbContext.DocumentFilesSet.Attach(file);
                        var entry = dbContext.Entry(file);
                        entry.Property(x => x.IsMainVersion).IsModified = true;
                    }

                }

                var fl = ModelConverter.GetDbDocumentFile(docFile);
                dbContext.DocumentFilesSet.Add(fl);
                if (docFile.Events != null && docFile.Events.Any(x => x.Id == 0))
                {
                    dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(docFile.Events.Where(x => x.Id == 0)).ToList());
                }
                dbContext.SaveChanges();
                docFile.Id = fl.Id;
                CommonQueries.AddFullTextCacheInfo(ctx, dbContext, fl.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
                return fl.Id;
            }
        }

        public void UpdateFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (docFile.IsMainVersion)
                {
                    foreach (var docFileId in dbContext.DocumentFilesSet
                              .Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument && x.Id != docFile.Id)
                              .Select(x => x.Id).ToList())
                    {
                        var file = new DBModel.Document.DocumentFiles
                        {
                            Id = docFileId,
                            IsMainVersion = false,
                        };
                        dbContext.DocumentFilesSet.Attach(file);
                        var entryFile = dbContext.Entry(file);
                        entryFile.Property(x => x.IsMainVersion).IsModified = true;
                    }
                }

                var fl = ModelConverter.GetDbDocumentFile(docFile);
                dbContext.DocumentFilesSet.Attach(fl);
                var entry = dbContext.Entry(fl);
                entry.Property(x => x.Name).IsModified = true;
                entry.Property(x => x.Description).IsModified = true;
                entry.Property(x => x.Extension).IsModified = true;
                entry.Property(x => x.FileType).IsModified = true;
                entry.Property(x => x.FileSize).IsModified = true;
                entry.Property(x => x.TypeId).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.Hash).IsModified = true;
                entry.Property(x => x.IsWorkedOut).IsModified = true;
                entry.Property(x => x.IsMainVersion).IsModified = true;
                //entry.Property(x => x.Date).IsModified = true;//we do not update that
                dbContext.SaveChanges();
                if (docFile.Events != null && docFile.Events.Any(x => x.Id == 0))
                {
                    var dbEvents = ModelConverter.GetDbDocumentEvents(docFile.Events.Where(x => x.Id == 0)).ToList();
                    dbContext.DocumentEventsSet.AddRange(dbEvents);
                    dbContext.SaveChanges();
                }
                CommonQueries.AddFullTextCacheInfo(ctx, dbContext, fl.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDocumentAttachedFile> GetOldPdfForAttachedFiles(IContext ctx, int pdfAge)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentFilesSet.Where(x => x.IsPdfCreated.HasValue && x.IsPdfCreated.Value
                && (!x.LastPdfAccessDate.HasValue || Math.Abs(DbFunctions.DiffDays(DateTime.Now, x.LastPdfAccessDate.Value) ?? 0) > pdfAge))
                .Select(x => new InternalDocumentAttachedFile
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    ExecutorPositionId = x.ExecutorPositionId,
                    Name = x.Name,
                    Extension = x.Extension,
                    Type = (EnumFileTypes)x.TypeId,
                    IsMainVersion = x.IsMainVersion,
                    Version = x.Version,
                    IsDeleted = x.IsDeleted,
                    PdfCreated = x.IsPdfCreated.Value,
                    LastPdfAccess = x.LastPdfAccessDate.Value
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public void UpdateFilePdfView(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var fl = ModelConverter.GetDbDocumentFile(docFile);
                dbContext.DocumentFilesSet.Attach(fl);
                var entry = dbContext.Entry(fl);
                entry.Property(x => x.IsPdfCreated).IsModified = true;
                entry.Property(x => x.LastPdfAccessDate).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void RenameFile(IContext ctx, IEnumerable<InternalDocumentAttachedFile> docFiles, IEnumerable<InternalDocumentEvent> docFileEvents)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                int? documentId = null;
                foreach (var docFile in docFiles)
                {
                    documentId = docFile.DocumentId;
                    var fl = ModelConverter.GetDbDocumentFile(docFile);
                    dbContext.DocumentFilesSet.Attach(fl);
                    var entry = dbContext.Entry(fl);
                    entry.Property(x => x.Name).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                    dbContext.SaveChanges();
                }
                //entry.Property(x => x.Date).IsModified = true;//we do not update that
                if (docFileEvents != null && docFileEvents.Any(x => x.Id == 0))
                {
                    var dbEvents = ModelConverter.GetDbDocumentEvents(docFileEvents.Where(x => x.Id == 0)).ToList();
                    dbContext.DocumentEventsSet.AddRange(dbEvents);
                    dbContext.SaveChanges();
                }
                if (documentId.HasValue)
                    CommonQueries.AddFullTextCacheInfo(ctx, dbContext, documentId.Value, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public InternalDocument DeleteDocumentFilePrepare(IContext ctx, FilterDocumentFileIdentity flIdent)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx, null, null, true, true)
                    .Where(x => x.Id == flIdent.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        IsRegistered = x.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                    }).FirstOrDefault();
                if (doc == null) return null;

                var docFileQry = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == flIdent.DocumentId && x.OrderNumber == flIdent.OrderInDocument).AsQueryable();

                if (flIdent.Version.HasValue)
                {
                    docFileQry = docFileQry.Where(x => x.Version == flIdent.Version);
                }
                doc.DocumentFiles = docFileQry
                        .Select(x => new InternalDocumentAttachedFile
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            ExecutorPositionId = x.ExecutorPositionId,
                            Name = x.Name,
                            Extension = x.Extension,
                            Type = (EnumFileTypes)x.TypeId,
                            IsMainVersion = x.IsMainVersion,
                            Version = x.Version,
                            IsDeleted = x.IsDeleted,
                        }).ToList();
                transaction.Complete();
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
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (docFile.Events != null && docFile.Events.Any(x => x.Id == 0))
                {
                    var dbEvents = ModelConverter.GetDbDocumentEvents(docFile.Events.Where(x => x.Id == 0)).ToList();
                    dbContext.DocumentEventsSet.AddRange(dbEvents);
                    dbContext.SaveChanges();
                }

                var docFileQry = dbContext.DocumentFilesSet
                                        .Where(x => x.ClientId == ctx.CurrentClientId)
                                        .Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument)
                                        .AsQueryable();

                if (docFile.Version > 0)
                {
                    docFileQry = docFileQry.Where(x => x.Version == docFile.Version);
                }

                foreach (var fileId in docFileQry.Select(x => x.Id).ToList())
                {
                    var file = new DBModel.Document.DocumentFiles
                    {
                        Id = fileId,
                        IsDeleted = true,
                    };
                    dbContext.DocumentFilesSet.Attach(file);
                    var entry = dbContext.Entry(file);

                    if (docFile.Version > 0 && docFile.IsDeleted)
                    {
                        entry.State = System.Data.Entity.EntityState.Deleted;
                    }
                    else
                    {
                        entry.Property(x => x.IsDeleted).IsModified = true;
                    }
                }
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(ctx, dbContext, docFile.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public int CheckFileForDocument(IContext ctx, int documentId, string fileName, string fileExt)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = -1;
                if (dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId).Where(x => !x.IsDeleted).Any(x => x.DocumentId == documentId && x.Name == fileName && x.Extension == fileExt))
                {
                    res = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == documentId && x.Name == fileName && x.Extension == fileExt)
                            .Where(x => !x.IsDeleted)
                            .Select(x => x.OrderNumber)
                            .First();
                }
                transaction.Complete();
                return res;
            }
        }

        public int GetNextFileOrderNumber(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId).OrderByDescending(x => x.OrderNumber).Select(x => x.OrderNumber).FirstOrDefault() + 1;
                transaction.Complete();
                return res;
            }
        }

        public int GetFileNextVersion(IContext ctx, int documentId, int fileOrder)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == documentId && x.OrderNumber == fileOrder).OrderByDescending(x => x.Version).Select(x => x.Version).FirstOrDefault() + 1;
                transaction.Complete();
                return res;
            }
        }

    }
}