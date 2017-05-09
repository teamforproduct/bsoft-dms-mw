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
using LinqKit;
using BL.Model.Exception;
using BL.Database.DBModel.Document;
using BL.Model.Common;
using EntityFramework.Extensions;

namespace BL.Database.Documents
{
    public class DocumentFileDbProcess : CoreDb.CoreDb, IDocumentFileDbProcess
    {
        public IEnumerable<FrontDocumentFile> GetDocumentFiles(IContext context, FilterBase filter, UIPaging paging = null)
        {
            var dbContext = context.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = CommonQueries.GetDocumentFileQuery(context, filter?.File);

                if (filter?.Document != null)
                {
                    var documentIds = CommonQueries.GetDocumentQuery(context, filter.Document, true)
                                        .Select(x => x.Id);

                    qry = qry.Where(x => documentIds.Contains(x.DocumentId));
                }

                if (filter?.Event != null)
                {
                    var eventsDocumentIds = CommonQueries.GetDocumentEventQuery(context, filter?.Event)
                                                .Select(x => x.DocumentId);

                    qry = qry.Where(x => eventsDocumentIds.Contains(x.DocumentId));
                }

                if (filter?.Wait != null)
                {
                    var waitsDocumentIds = CommonQueries.GetDocumentWaitQuery(context, filter?.Wait)
                                                .Select(x => x.DocumentId);

                    qry = qry.Where(x => waitsDocumentIds.Contains(x.DocumentId));
                }

                //TODO Sort
                qry = qry.OrderByDescending(x => x.LastChangeDate);

                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontDocumentFile>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }

                if ((paging?.IsAll ?? true) && (filter == null || filter.File == null || ((filter.File.DocumentId?.Count ?? 0) == 0 && (filter.File.FileId?.Count ?? 0) == 0)))
                {
                    throw new WrongAPIParameters();
                }

                var isNeedRegistrationFullNumber = !(filter?.File?.DocumentId?.Any() ?? false);

                var qryFE = dbContext.DocumentFilesSet.Where(x => qry.Select(y => y.Id).Contains(x.Id))
                                .OrderByDescending(x => x.LastChangeDate)
                                .Join(dbContext.DictionaryAgentsSet, o => o.LastChangeUserId, i => i.Id, (file, agent) => new FrontDocumentFile
                                {
                                    Id = file.Id,
                                    Date = file.Date,
                                    DocumentId = file.DocumentId,
                                    Type = (EnumFileTypes)file.TypeId,
                                    TypeName = file.Type.Code,
                                    IsMainVersion = file.IsMainVersion,
                                    IsDeleted = file.IsDeleted,
                                    IsWorkedOut = file.IsWorkedOut ?? true,
                                    Description = file.Description,
                                    LastChangeDate = file.LastChangeDate,
                                    LastChangeUserId = file.LastChangeUserId,
                                    LastChangeUserName = agent.Name,
                                    OrderInDocument = file.OrderNumber,
                                    Version = file.Version,
                                    WasChangedExternal = false,
                                    ExecutorPositionName = file.ExecutorPosition.Name,
                                    ExecutorPositionExecutorAgentName = file.ExecutorPositionExecutorAgent.Name 
                                        + (file.ExecutorPositionExecutorType.Suffix != null ? " (" + file.ExecutorPositionExecutorType.Suffix + ")" : null),
                                    DocumentDate = (file.Document.LinkId.HasValue || isNeedRegistrationFullNumber) ? file.Document.RegistrationDate ?? file.Document.CreateDate : (DateTime?)null,
                                    RegistrationNumber = file.Document.RegistrationNumber,
                                    RegistrationNumberPrefix = file.Document.RegistrationNumberPrefix,
                                    RegistrationNumberSuffix = file.Document.RegistrationNumberSuffix,
                                    RegistrationFullNumber = "#" + file.Document.Id,
                                    File = new BaseFile
                                    {
                                        Extension = file.Extension,
                                        FileType = file.FileType,
                                        FileSize = file.FileSize,
                                        Name = file.Name,
                                    },
                                    //Event = new FrontDocumentEvent
                                    //{
                                    //    Id = file.Event.Id,
                                    //    EventType = file.Event.EventTypeId,
                                    //    EventTypeName = file.Event.EventType.Name,
                                    //    Date = file.Event.Date,
                                    //    CreateDate = file.Event.Date != file.Event.CreateDate ? (DateTime?)file.Event.CreateDate : null,
                                    //    Task = file.Event.Task.Task,
                                    //    Description = file.Event.Description,
                                    //    AddDescription = file.Event.AddDescription,
                                    //    OnWait = file.Event.OnWait.Select(y => new FrontDocumentWait { DueDate = y.DueDate, OffEventDate = (DateTime?)y.OffEvent.Date }).FirstOrDefault(),
                                    //}
                                });

                var res = qryFE.ToList();

                if (res.Any(x => x.IsMainVersion))
                {
                    var filterContains = PredicateBuilder.New<DocumentFiles>(false);
                    filterContains = res.Where(x => x.IsMainVersion).Select(x => new { x.DocumentId, x.OrderInDocument }).ToList()
                                        .Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value.DocumentId && e.OrderNumber == value.OrderInDocument).Expand());

                    var isNotAllWorkedOut = dbContext.DocumentFilesSet.Where(filterContains)
                                .Where(x => !x.IsDeleted)
                                .GroupBy(x => new { x.DocumentId, x.OrderNumber })
                                .Select(x => new
                                {
                                    DocumentId = x.Key.DocumentId,
                                    OrderNumber = x.Key.OrderNumber,
                                    IsNotAllWorkedOut = x.Any(y => y.IsWorkedOut == false)
                                }).ToList();
                    res.ForEach(x => x.IsNotAllWorkedOut = isNotAllWorkedOut.FirstOrDefault(y => y.DocumentId == x.DocumentId && y.OrderNumber == x.OrderInDocument)?.IsNotAllWorkedOut ?? false);
                }

                res.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));
                //var events = res.Select(x => x.Event).ToList();
                //CommonQueries.SetAccessGroups(context, events);
                //CommonQueries.SetWaitInfo(context, events);
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentFile GetDocumentFileVersion(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)
                        .Where(x => x.Id == id)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new FrontDocumentFile
                        {
                            Id = x.fl.Id,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            Type = (EnumFileTypes)x.fl.TypeId,
                            TypeName = x.fl.Type.Code,
                            Hash = x.fl.Hash,
                            LastChangeDate = x.fl.LastChangeDate,
                            LastChangeUserId = x.fl.LastChangeUserId,
                            LastChangeUserName = x.agName,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            WasChangedExternal = false,
                            ExecutorPositionName = x.fl.ExecutorPosition.Name,
                            ExecutorPositionExecutorAgentName = x.fl.ExecutorPositionExecutorAgent.Name + (x.fl.ExecutorPositionExecutorType.Suffix != null ? " (" + x.fl.ExecutorPositionExecutorType.Suffix + ")" : null),
                            File = new BaseFile
                            {
                                Extension = x.fl.Extension,
                                FileType = x.fl.FileType,
                                FileSize = x.fl.FileSize,
                                Name = x.fl.Name,
                            }
                        }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public InternalDocumentFile GetInternalAttachedFile(IContext ctx, int fileId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentFilesSet.Where(x => x.Id == fileId)
                    .Select(x => new InternalDocumentFile
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        PdfCreated = x.IsPdfCreated ?? false,
                        LastPdfAccess = x.LastPdfAccessDate //??DateTime.MinValue
                    }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public InternalDocument ModifyDocumentFilePrepare(IContext ctx, int documentId, int orderNumber, int version)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(ctx, null, null, true, true)
                                    .Where(x => x.Id == documentId)
                                    .Select(x => new InternalDocument
                                    {
                                        Id = x.Id,
                                        ClientId = x.ClientId,
                                        EntityTypeId = x.EntityTypeId,
                                        ExecutorPositionId = x.ExecutorPositionId
                                    }).FirstOrDefault();

                if (doc == null) return null;

                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)
                        .Where(x => x.DocumentId == documentId && x.Version == version && x.OrderNumber == orderNumber)
                        .Where(x => !x.IsDeleted)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new InternalDocumentFile
                        {
                            Id = x.fl.Id,
                            ClientId = x.fl.ClientId,
                            EntityTypeId = x.fl.EntityTypeId,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            ExecutorPositionId = x.fl.ExecutorPositionId,
                            IsWorkedOut = x.fl.IsWorkedOut,
                            Type = (EnumFileTypes)x.fl.TypeId,
                            IsMainVersion = x.fl.IsMainVersion,
                            Hash = x.fl.Hash,
                            Description = x.fl.Description,
                            IsDeleted = x.fl.IsDeleted,
                            File = new BaseFile
                            {
                                Name = x.fl.Name,
                                FileSize = x.fl.FileSize,
                                Extension = x.fl.Extension,
                                FileType = x.fl.FileType,
                            }

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
                var doc = CommonQueries.GetDocumentQuery(ctx, null, null, true, true)
                                    .Where(x => x.Id == documentId)
                                    .Select(x => new InternalDocument
                                    {
                                        Id = x.Id,
                                        ClientId = x.ClientId,
                                        EntityTypeId = x.EntityTypeId,
                                        ExecutorPositionId = x.ExecutorPositionId
                                    }).FirstOrDefault();

                if (doc == null) return null;

                doc.DocumentFiles = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)
                        .Where(
                            x => x.DocumentId == documentId && x.OrderNumber == orderNumber)
                        .Join(dbContext.DictionaryAgentsSet, df => df.LastChangeUserId, da => da.Id,
                            (d, a) => new { fl = d, agName = a.Name })
                        .Select(x => new InternalDocumentFile
                        {
                            Id = x.fl.Id,
                            ClientId = x.fl.ClientId,
                            EntityTypeId = x.fl.EntityTypeId,
                            Date = x.fl.Date,
                            DocumentId = x.fl.DocumentId,
                            OrderInDocument = x.fl.OrderNumber,
                            Version = x.fl.Version,
                            ExecutorPositionId = x.fl.ExecutorPositionId,
                            IsWorkedOut = x.fl.IsWorkedOut,
                            IsMainVersion = x.fl.IsMainVersion,
                            Type = (EnumFileTypes)x.fl.TypeId,
                            File = new BaseFile
                            {
                                Name = x.fl.Name,
                                Extension = x.fl.Extension,
                            }
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
                var doc = CommonQueries.GetDocumentQuery(ctx, null, null, true, true)
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
                        new InternalDocumentFile
                        {
                            Id = y.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            ExecutorPositionId = y.ExecutorPositionId,
                            Type = (EnumFileTypes)y.TypeId,
                            OrderInDocument = y.OrderNumber,
                            File = new BaseFile
                            {
                                Name = y.Name,
                                Extension = y.Extension,
                                FileType = y.FileType,
                            }
                        })
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }

        public int AddNewFileOrVersion(IContext ctx, InternalDocumentFile docFile)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                if (docFile.IsMainVersion)
                {
                    dbContext.DocumentFilesSet.Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument)
                        .Update(x => new DocumentFiles { IsMainVersion = false });
                }
                var fl = ModelConverter.GetDbDocumentFile(docFile);
                dbContext.DocumentFilesSet.Add(fl);
                dbContext.SaveChanges();
                docFile.Id = fl.Id;
                CommonQueries.AddFullTextCacheInfo(ctx, fl.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
                return fl.Id;
            }
        }

        public void UpdateFileOrVersion(IContext ctx, InternalDocumentFile docFile)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (docFile.IsMainVersion)
                {
                    dbContext.DocumentFilesSet.Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument && x.Id != docFile.Id)
                        .Update(x => new DocumentFiles { IsMainVersion = false });
                }
                var fl = ModelConverter.GetDbDocumentFile(docFile);
                dbContext.SafeAttach(fl);
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
                if (docFile.Event != null)
                {
                    var dbEvent = ModelConverter.GetDbDocumentEvent(docFile.Event);
                    dbContext.DocumentEventsSet.Add(dbEvent);
                    dbContext.SaveChanges();
                }
                CommonQueries.AddFullTextCacheInfo(ctx, fl.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDocumentFile> GetOldPdfForAttachedFiles(IContext ctx, int pdfAge)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentFilesSet.Where(x => x.IsPdfCreated.HasValue && x.IsPdfCreated.Value
                && (!x.LastPdfAccessDate.HasValue || Math.Abs(DbFunctions.DiffDays(DateTime.Now, x.LastPdfAccessDate.Value) ?? 0) > pdfAge))
                .Select(x => new InternalDocumentFile
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    EntityTypeId = x.EntityTypeId,
                    ExecutorPositionId = x.ExecutorPositionId,
                    Type = (EnumFileTypes)x.TypeId,
                    IsMainVersion = x.IsMainVersion,
                    Version = x.Version,
                    IsDeleted = x.IsDeleted,
                    PdfCreated = x.IsPdfCreated.Value,
                    LastPdfAccess = x.LastPdfAccessDate.Value,
                    File = new BaseFile
                    {
                        Name = x.Name,
                        Extension = x.Extension,
                    }
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public void UpdateFilePdfView(IContext ctx, InternalDocumentFile docFile)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var fl = ModelConverter.GetDbDocumentFile(docFile);
                dbContext.SafeAttach(fl);
                var entry = dbContext.Entry(fl);
                entry.Property(x => x.IsPdfCreated).IsModified = true;
                entry.Property(x => x.LastPdfAccessDate).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void RenameFile(IContext ctx, IEnumerable<InternalDocumentFile> docFiles, IEnumerable<InternalDocumentEvent> docFileEvents)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                int? documentId = null;
                foreach (var docFile in docFiles)
                {
                    documentId = docFile.DocumentId;
                    var fl = ModelConverter.GetDbDocumentFile(docFile);
                    dbContext.SafeAttach(fl);
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
                    CommonQueries.AddFullTextCacheInfo(ctx, documentId.Value, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public InternalDocument DeleteDocumentFilePrepare(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)
                    .Where(x => x.Id == id)
                    .Select(x => new InternalDocument
                    {
                        Id = x.Document.Id,
                        ClientId = x.Document.ClientId,
                        EntityTypeId = x.Document.EntityTypeId,
                        IsRegistered = x.Document.IsRegistered,
                        ExecutorPositionId = x.Document.ExecutorPositionId,
                        FileOrderNumber = x.OrderNumber,
                    }).FirstOrDefault();
                if (doc == null) return null;

                var docFileQry = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)
                        .Where(x => x.DocumentId == doc.Id && x.OrderNumber == doc.FileOrderNumber).AsQueryable();

                //if (flIdent.Version.HasValue)
                //{
                //    docFileQry = docFileQry.Where(x => x.Version == flIdent.Version);
                //}
                doc.DocumentFiles = docFileQry
                        .Select(x => new InternalDocumentFile
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.DocumentId,
                            OrderInDocument = x.OrderNumber,
                            ExecutorPositionId = x.ExecutorPositionId,
                            Type = (EnumFileTypes)x.TypeId,
                            IsMainVersion = x.IsMainVersion,
                            Version = x.Version,
                            IsDeleted = x.IsDeleted,
                            File = new BaseFile
                            {
                                Name = x.Name,
                                Extension = x.Extension,
                            }
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
        public void DeleteAttachedFile(IContext ctx, InternalDocumentFile docFile)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (docFile.Event != null)
                {
                    var dbEvent = ModelConverter.GetDbDocumentEvent(docFile.Event);
                    dbContext.DocumentEventsSet.Add(dbEvent);
                    dbContext.SaveChanges();
                }

                var docFileQry = dbContext.DocumentFilesSet
                                        .Where(x => x.ClientId == ctx.Client.Id)
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
                    dbContext.SafeAttach(file);
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
                CommonQueries.AddFullTextCacheInfo(ctx, docFile.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public int CheckFileForDocument(IContext ctx, int documentId, string fileName, string fileExt)
        {
            var res = 0;
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                res = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id).Where(x => x.DocumentId == documentId && x.Name == fileName && x.Extension == fileExt)
                            .Where(x => !x.IsDeleted)
                            .Select(x => x.OrderNumber)
                            .FirstOrDefault();
                transaction.Complete();
            }
            return res;
        }

        public int GetNextFileOrderNumber(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)
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
                var res = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id).Where(x => x.DocumentId == documentId && x.OrderNumber == fileOrder).OrderByDescending(x => x.Version).Select(x => x.Version).FirstOrDefault() + 1;
                transaction.Complete();
                return res;
            }
        }

    }
}