using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.Documents.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore;

namespace BL.Database.Documents
{
    public class DocumentFileDbProcess : CoreDb.CoreDb, IDocumentFileDbProcess
    {
        public DocumentFileDbProcess()
        {
        }

        public IEnumerable<FrontDocumentAttachedFile> GetDocumentFiles(IContext ctx, FilterBase filter, UIPaging paging = null)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return CommonQueries.GetDocumentFiles(ctx, dbContext, filter, paging);
            }
        }

        public FrontDocumentAttachedFile GetDocumentFileVersion(IContext ctx, int documentId, int orderNumber, int versionNumber)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return
                    dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
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
                            IsAdditional = x.fl.IsAdditional,
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
                            ExecutorPositionExecutorAgentName = x.fl.ExecutorPositionExecutorAgent.Name
                        }).FirstOrDefault();
            }
        }

        public InternalDocument ModifyDocumentFilePrepare(IContext ctx, int documentId, int orderNumber, int version)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                                    .Where(x => x.Id == documentId)
                                    .Select(x => new InternalDocument
                                    {
                                        Id = x.Id,
                                        ExecutorPositionId = x.ExecutorPositionId
                                    }).FirstOrDefault();

                if (doc == null) return null;

                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(
                            x => x.DocumentId == documentId && x.Version == version && x.OrderNumber == orderNumber)
                        .Where(x => !x.IsDeleted)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new InternalDocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            Name = x.fl.Name,
                            DocumentId = x.fl.DocumentId,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            ExecutorPositionId = x.fl.ExecutorPositionId,
                            IsWorkedOut = x.fl.IsWorkedOut,
                            IsAdditional = x.fl.IsAdditional,
                            IsMainVersion = x.fl.IsMainVersion,
                            FileSize = x.fl.FileSize,
                            Extension = x.fl.Extension,
                            FileType = x.fl.FileType,
                            Hash = x.fl.Hash,
                            Description = x.fl.Description,
                            IsDeleted = x.fl.IsDeleted,
                            FileContent = x.fl.Content
                        }).ToList();
                return doc;
            }
        }
        public InternalDocument RenameDocumentFilePrepare(IContext ctx, int documentId, int orderNumber)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                                    .Where(x => x.Id == documentId)
                                    .Select(x => new InternalDocument
                                    {
                                        Id = x.Id,
                                        ExecutorPositionId = x.ExecutorPositionId
                                    }).FirstOrDefault();

                if (doc == null) return null;

                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(
                            x => x.DocumentId == documentId && x.OrderNumber == orderNumber)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new InternalDocumentAttachedFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            Name = x.fl.Name,
                            Extension = x.fl.Extension,
                            DocumentId = x.fl.DocumentId,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            ExecutorPositionId = x.fl.ExecutorPositionId,
                            IsWorkedOut = x.fl.IsWorkedOut,
                            IsMainVersion = x.fl.IsMainVersion,
                            IsAdditional = x.fl.IsAdditional,
                        }).ToList();
                return doc;
            }
        }

        public InternalDocument AddDocumentFilePrepare(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Id == documentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        ExecutorPositionId = x.ExecutorPositionId,
                        IsRegistered = x.IsRegistered,
                        DocumentFiles = x.Files.Where(y => y.IsMainVersion).Where(y => !y.IsDeleted)
                        .Select(y =>
                        new InternalDocumentAttachedFile
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Extension = y.Extension,
                            ExecutorPositionId = y.ExecutorPositionId,
                            FileType = y.FileType,
                            IsAdditional = y.IsAdditional,
                            OrderInDocument = y.OrderNumber
                        })
                    }).FirstOrDefault();

                return doc;
            }
        }

        public int AddNewFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(ctx))
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
                return fl.Id;
            }
        }

        public void UpdateFileOrVersion(IContext ctx, InternalDocumentAttachedFile docFile)
        {
            using (var dbContext = new DmsContext(ctx))
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
                entry.Property(x => x.IsAdditional).IsModified = true;
                entry.Property(x => x.LastChangeDate).IsModified = true;
                entry.Property(x => x.LastChangeUserId).IsModified = true;
                entry.Property(x => x.Hash).IsModified = true;
                entry.Property(x => x.IsWorkedOut).IsModified = true;
                entry.Property(x => x.IsMainVersion).IsModified = true;
                //entry.Property(x => x.Date).IsModified = true;//we do not update that
                if (docFile.Events != null && docFile.Events.Any(x => x.Id == 0))
                {
                    dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(docFile.Events.Where(x => x.Id == 0)).ToList());
                }
                dbContext.SaveChanges();
            }
        }
        public void RenameFile(IContext ctx, IEnumerable<InternalDocumentAttachedFile> docFiles, IEnumerable<InternalDocumentEvent> docFileEvents)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                foreach (var docFile in docFiles)
                {
                    var fl = ModelConverter.GetDbDocumentFile(docFile);
                    dbContext.DocumentFilesSet.Attach(fl);
                    var entry = dbContext.Entry(fl);
                    entry.Property(x => x.Name).IsModified = true;
                    entry.Property(x => x.LastChangeDate).IsModified = true;
                    entry.Property(x => x.LastChangeUserId).IsModified = true;
                }
                //entry.Property(x => x.Date).IsModified = true;//we do not update that
                if (docFileEvents != null && docFileEvents.Any(x => x.Id == 0))
                {
                    dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(docFileEvents.Where(x => x.Id == 0)).ToList());
                }
                dbContext.SaveChanges();
            }
        }

        public InternalDocument DeleteDocumentFilePrepare(IContext ctx, FilterDocumentFileIdentity flIdent)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var doc = CommonQueries.GetDocumentQuery(dbContext, ctx)
                    .Where(x => x.Id == flIdent.DocumentId)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Id,
                        IsRegistered = x.IsRegistered,
                        ExecutorPositionId = x.ExecutorPositionId,
                    }).FirstOrDefault();
                if (doc == null) return null;

                var docFileQry = dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == flIdent.DocumentId && x.OrderNumber == flIdent.OrderInDocument).AsQueryable();

                if (flIdent.Version.HasValue)
                {
                    docFileQry = docFileQry.Where(x => x.Version == flIdent.Version);
                }
                doc.DocumentFiles = docFileQry
                        .Select(x => new InternalDocumentAttachedFile
                        {
                            Id = x.Id,
                            ExecutorPositionId = x.ExecutorPositionId,
                            Name = x.Name,
                            Extension = x.Extension,
                            IsAdditional = x.IsAdditional,
                            IsMainVersion = x.IsMainVersion,
                            Version = x.Version,
                            IsDeleted = x.IsDeleted,
                        }).ToList();
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
            using (var dbContext = new DmsContext(ctx))
            {
                if (docFile.Events != null && docFile.Events.Any(x => x.Id == 0))
                {
                    dbContext.DocumentEventsSet.AddRange(ModelConverter.GetDbDocumentEvents(docFile.Events.Where(x => x.Id == 0)).ToList());
                }

                var docFileQry = dbContext.DocumentFilesSet
                                        .Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
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

            }
        }

        public int CheckFileForDocument(IContext ctx, int documentId, string fileName, string fileExt)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                if (dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => !x.IsDeleted).Any(x => x.DocumentId == documentId && x.Name == fileName && x.Extension == fileExt))
                {
                    return
                        dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == documentId && x.Name == fileName && x.Extension == fileExt)
                            .Where(x => !x.IsDeleted)
                            .Select(x => x.OrderNumber)
                            .First();
                }

                return -1;
            }
        }

        public int GetNextFileOrderNumber(IContext ctx, int documentId)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                        .Where(x => x.DocumentId == documentId).OrderByDescending(x => x.OrderNumber).Select(x => x.OrderNumber).FirstOrDefault() + 1;
            }
        }

        public int GetFileNextVersion(IContext ctx, int documentId, int fileOrder)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                return dbContext.DocumentFilesSet.Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId).Where(x => x.DocumentId == documentId && x.OrderNumber == fileOrder).OrderByDescending(x => x.Version).Select(x => x.Version).FirstOrDefault() + 1;
            }
        }
    }
}