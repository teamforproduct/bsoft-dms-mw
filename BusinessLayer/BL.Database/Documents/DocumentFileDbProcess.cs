﻿using System;
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
                #region qry
                var qrys = CommonQueries.GetDocumentFileQueries(context, filter?.File);

                if (filter?.Document != null)
                {
                    var documentIds = CommonQueries.GetDocumentQuery(context, filter.Document).Select(x => x.Id);
                    qrys = qrys.Select(qry => { return qry.Where(x => documentIds.Contains(x.DocumentId)); }).ToList();
                }

                if (filter?.Event != null)
                {
                    var eventsDocumentIds = CommonQueries.GetDocumentEventQuery(context, filter?.Event).Select(x => x.DocumentId);
                    qrys = qrys.Select(qry => { return qry.Where(x => eventsDocumentIds.Contains(x.DocumentId)); }).ToList();
                }

                if (filter?.Wait != null)
                {
                    var waitsDocumentIds = CommonQueries.GetDocumentWaitQuery(context, filter?.Wait).Select(x => x.DocumentId);
                    qrys = qrys.Select(qry => { return qry.Where(x => waitsDocumentIds.Contains(x.DocumentId)); }).ToList();
                }

                //TODO Sort
                qrys = qrys.Select(qry => { return qry.OrderByDescending(x => x.LastChangeDate).AsQueryable(); }).ToList();
                #endregion qry   

                #region paging
                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qrys.Sum(qry => qry.Count());
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontDocumentFile>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        if (qrys.Count > 1)
                        {
                            var take1 = paging.PageSize * (paging.CurrentPage - 1) + paging.PageSize;

                            qrys = qrys.Select(qry => qry.Take(() => take1)).ToList();

                            var qryConcat = qrys.First();

                            foreach (var qry in qrys.Skip(1).ToList())
                            {
                                qryConcat = qryConcat.Concat(qry);
                            }

                            qrys.Clear();
                            qrys.Add(qryConcat);
                        }

                        //TODO Sort
                        qrys = qrys.Select(qry => { return qry.OrderByDescending(x => x.LastChangeDate).AsQueryable(); }).ToList();

                        qrys = qrys.Select(qry => qry.Skip(() => skip).Take(() => take)).ToList();
                    }
                }

                if ((paging?.IsAll ?? true) && (filter == null || filter.File == null || ((filter.File.DocumentId?.Count ?? 0) == 0 && (filter.File.FileId?.Count ?? 0) == 0)))
                {
                    throw new WrongAPIParameters();
                }
                #endregion paging

                #region filling
                IQueryable<DocumentFiles> qryRes = qrys.First(); ;

                if (qrys.Count > 1)
                {
                    foreach (var qry in qrys.Skip(1).ToList())
                    {
                        qryRes = qryRes.Concat(qry);
                    }
                }
                var isNeedRegistrationFullNumber = !(filter?.File?.DocumentId?.Any() ?? false);

                var qryFE = dbContext.DocumentFilesSet  //Without security restrictions
                    .Where(x => qryRes.Select(y => y.Id).Contains(x.Id)).OrderByDescending(x => x.LastChangeDate)
                    .Join(dbContext.DictionaryAgentsSet, o => o.LastChangeUserId, i => i.Id,
                            (file, agent) => new { file, agent, source = file.Event.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source && file.ExecutorPositionId != y.PositionId) })
                    .Select(x => new FrontDocumentFile
                    {
                        Id = x.file.Id,
                        Date = x.file.Date,
                        DocumentId = x.file.DocumentId,
                        Type = (EnumFileTypes)x.file.TypeId,
                        TypeName = "##l@FileTypes:" + ((EnumFileTypes)x.file.TypeId).ToString() + "@l##",
                        IsMainVersion = x.file.IsMainVersion,
                        IsDeleted = x.file.IsDeleted,
                        IsWorkedOut = x.file.IsWorkedOut ?? true,
                        Description = x.file.IsDeleted ? null : x.file.Description,
                        LastChangeDate = x.file.LastChangeDate,
                        LastChangeUserId = x.file.LastChangeUserId,
                        LastChangeUserName = x.agent.Name,
                        OrderInDocument = x.file.OrderNumber,
                        Version = x.file.Version,
                        WasChangedExternal = false,
                        SourcePositionId = x.source.PositionId,
                        SourcePositionName = x.source.Position.Name,
                        SourcePositionExecutorAgentName = x.source.Agent.Name + (x.source.PositionExecutorType.Suffix != null ? " (" + x.source.PositionExecutorType.Suffix + ")" : null),

                        ExecutorPositionId = x.file.ExecutorPositionId,
                        ExecutorPositionName = x.file.ExecutorPosition.Name,
                        ExecutorPositionExecutorAgentName = x.file.ExecutorPositionExecutorAgent.Name
                                        + (x.file.ExecutorPositionExecutorType.Suffix != null ? " (" + x.file.ExecutorPositionExecutorType.Suffix + ")" : null),
                        DocumentDate = (x.file.Document.LinkId.HasValue || isNeedRegistrationFullNumber) ? x.file.Document.RegistrationDate ?? x.file.Document.CreateDate : (DateTime?)null,
                        RegistrationNumber = x.file.Document.RegistrationNumber,
                        RegistrationNumberPrefix = x.file.Document.RegistrationNumberPrefix,
                        RegistrationNumberSuffix = x.file.Document.RegistrationNumberSuffix,
                        RegistrationFullNumber = "#" + x.file.Document.Id,
                        EventId = x.file.EventId,
                        PdfAcceptable = x.file.PdfAcceptable ?? false,
                        File = new BaseFile
                        {
                            Extension = x.file.IsDeleted ? null : x.file.Extension,
                            FileType = x.file.IsDeleted ? null : x.file.FileType,
                            FileSize = x.file.IsDeleted ? (long?)null : x.file.FileSize,
                            Name = x.file.IsDeleted ? "##l@General:FileHasBeenDeleted@l##" : x.file.Name,
                        },

                    });
                var res = qryFE.ToList();

                res.ForEach(x => CommonQueries.SetRegistrationFullNumber(x));
                CommonQueries.SetFileInfo(context, res, filter?.File);
                //var events = res.Select(x => x.Event).ToList();
                //CommonQueries.SetAccessGroups(context, events);
                //CommonQueries.SetWaitInfo(context, events);
                #endregion filling
                transaction.Complete();
                return res;
            }
        }

        public FrontDocumentFile GetDocumentFile(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentFileQuery(ctx, new FilterDocumentFile { FileId = new List<int> { id } })
                    .Join(dbContext.DictionaryAgentsSet, o => o.LastChangeUserId, i => i.Id,
                            (file, agent) => new { file, agent, source = file.Event.Accesses.FirstOrDefault(y => y.AccessTypeId == (int)EnumEventAccessTypes.Source && file.ExecutorPositionId != y.PositionId) })
                    .Select(x => new FrontDocumentFile
                    {
                        Id = x.file.Id,
                        Date = x.file.Date,
                        DocumentId = x.file.DocumentId,
                        Type = (EnumFileTypes)x.file.TypeId,
                        TypeName = "##l@FileTypes:" + ((EnumFileTypes)x.file.TypeId).ToString() + "@l##",
                        Hash = x.file.Hash,
                        LastChangeDate = x.file.LastChangeDate,
                        LastChangeUserId = x.file.LastChangeUserId,
                        LastChangeUserName = x.agent.Name,
                        OrderInDocument = x.file.OrderNumber,
                        Version = x.file.Version,
                        WasChangedExternal = false,
                        SourcePositionId = x.source.PositionId,
                        SourcePositionName = x.source.Position.Name,
                        SourcePositionExecutorAgentName = x.source.Agent.Name + (x.source.PositionExecutorType.Suffix != null ? " (" + x.source.PositionExecutorType.Suffix + ")" : null),

                        ExecutorPositionId = x.file.ExecutorPositionId,
                        ExecutorPositionName = x.file.ExecutorPosition.Name,
                        ExecutorPositionExecutorAgentName = x.file.ExecutorPositionExecutorAgent.Name + (x.file.ExecutorPositionExecutorType.Suffix != null ? " (" + x.file.ExecutorPositionExecutorType.Suffix + ")" : null),
                        EventId = x.file.EventId,
                        Description = x.file.IsDeleted ? null : x.file.Description,
                        File = new BaseFile
                        {
                            Extension = x.file.IsDeleted ? null : x.file.Extension,
                            FileType = x.file.IsDeleted ? null : x.file.FileType,
                            FileSize = x.file.IsDeleted ? (long?)null : x.file.FileSize,
                            Name = x.file.IsDeleted ? "##l@General:FileHasBeenDeleted@l##" : x.file.Name,
                        },
                    }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public InternalDocumentFile GetDocumentFileInternal(IContext ctx, int fileId, bool isIncludeOtherFileVersions = false)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentFileQuery(ctx, new FilterDocumentFile { FileId = new List<int> { fileId }, IsAllVersion = true })
                    .Select(x => new InternalDocumentFile
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentId = x.DocumentId,
                        OrderInDocument = x.OrderNumber,
                        Version = x.Version,
                        Hash = x.Hash,
                        ExecutorPositionId = x.ExecutorPositionId,
                        File = new BaseFile
                        {
                            Extension = x.Extension,
                            FileType = x.FileType,
                            FileSize = x.FileSize,
                            Name = x.Name,
                        },

                    }).FirstOrDefault();
                if (res != null && isIncludeOtherFileVersions)
                    res.OtherFileVersions = CommonQueries.GetDocumentFileQuery(ctx, new FilterDocumentFile
                    {
                        DocumentId = new List<int> { res.DocumentId },
                        OrderInDocument = new List<int> { res.OrderInDocument },
                        IsAllVersion = true,
                    })
                    .Where(x => x.Version != res.Version)
                    .Select(x => new InternalDocumentFile
                    {
                        Id = x.Id,
                        ClientId = x.ClientId,
                        EntityTypeId = x.EntityTypeId,
                        DocumentId = x.DocumentId,
                        OrderInDocument = x.OrderNumber,
                        Version = x.Version,
                        Hash = x.Hash,
                        ExecutorPositionId = x.ExecutorPositionId,
                        File = new BaseFile
                        {
                            Extension = x.Extension,
                            FileType = x.FileType,
                            FileSize = x.FileSize,
                            Name = x.Name,
                        },

                    }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public InternalDocument ModifyDocumentFilePrepare(IContext ctx, int fileId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentFileQuery(ctx, new FilterDocumentFile { FileId = new List<int> { fileId }, IsAllVersion = true })
                                    .Select(x => new InternalDocument
                                    {
                                        Id = x.Id,
                                        ClientId = x.ClientId,
                                        EntityTypeId = x.EntityTypeId,
                                        ExecutorPositionId = x.Document.ExecutorPositionId,
                                        DocumentFiles = new List<InternalDocumentFile>{new InternalDocumentFile
                                        {
                                            Id = x.Id,
                                            ClientId = x.ClientId,
                                            EntityTypeId = x.EntityTypeId,
                                            Date = x.Date,
                                            DocumentId = x.DocumentId,
                                            OrderInDocument = x.OrderNumber,
                                            Version = x.Version,
                                            Hash = x.Hash,
                                            EventId = x.EventId,
                                            ExecutorPositionId = x.ExecutorPositionId,
                                            IsWorkedOut = x.IsWorkedOut,
                                            Type = (EnumFileTypes)x.TypeId,
                                            IsMainVersion = x.IsMainVersion,
                                            Description = x.Description,
                                            IsDeleted = x.IsDeleted,
                                            File = new BaseFile
                                            {
                                                Name = x.Name,
                                                FileSize = x.FileSize,
                                                Extension = x.Extension,
                                                FileType = x.FileType,
                                            }
                                        } }
                                    }).FirstOrDefault();

                if (doc == null) return null;
                transaction.Complete();
                return doc;
            }
        }
        public List<int> GetDocumentFileExecutors(IContext ctx, int documentId, List<int> orderNumbers)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var exec = CommonQueries.GetDocumentFileQuery(ctx, new FilterDocumentFile { DocumentId = new List<int> { documentId }, OrderInDocument = orderNumbers })
                                    .Where(x => x.ExecutorPositionId.HasValue).Select(x => x.ExecutorPositionId.Value).ToList();
                transaction.Complete();
                return exec;
            }
        }
        public InternalDocument AddDocumentFilePrepare(IContext ctx, int documentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentQuery(ctx, new FilterDocument { DocumentId = new List<int> { documentId }, IsInWork = true })
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
                            Version = y.Version,
                            Hash = y.Hash,
                            File = new BaseFile
                            {
                                Name = y.Name,
                                Extension = y.Extension,
                                FileType = y.FileType,
                            }
                        }).ToList()
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }

        public int AddDocumentFile(IContext ctx, InternalDocumentFile docFile)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                if (docFile.Id == 0)
                {
                    if (docFile.IsMainVersion)
                    {
                        dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)  //Without security restrictions
                            .Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument)
                            .Update(x => new DocumentFiles { IsMainVersion = false });
                    }
                    var fl = ModelConverter.GetDbDocumentFile(docFile);
                    dbContext.DocumentFilesSet.Add(fl);
                    dbContext.SaveChanges();
                    docFile.Id = fl.Id;
                    docFile.EventId = fl.EventId;
                }
                else if (!docFile.IsLinkOnly)
                {
                    var fl = ModelConverter.GetDbDocumentFile(docFile);
                    dbContext.SafeAttach(fl);
                    dbContext.Entry(fl).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    docFile.Id = fl.Id;
                    docFile.EventId = fl.EventId;
                }
                if (docFile.Id > 0 && docFile.EventId.HasValue)
                {
                    dbContext.DocumentFileLinksSet.Add(new DocumentFileLinks { FileId = docFile.Id, EventId = docFile.EventId });
                    dbContext.SaveChanges();
                }
                if (docFile.DocumentId > 0)
                {
                    CommonQueries.AddFullTextCacheInfo(ctx, docFile.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                }
                transaction.Complete();
                return docFile.Id;
            }
        }

        public void ModifyDocumentFile(IContext ctx, InternalDocument doc)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var docFile = doc.DocumentFiles.First();
                if (docFile.IsMainVersionChange)
                {
                    dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)  //Without security restrictions
                        .Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument && x.Id != docFile.Id && x.IsMainVersion)
                        .Update(x => new DocumentFiles { IsMainVersion = false, LastChangeDate = docFile.LastChangeDate, LastChangeUserId = docFile.LastChangeUserId });
                }
                if (docFile.IsWorkedOutChange)
                {
                    dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)  //Without security restrictions
                        .Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument && x.Id == docFile.Id)
                        .Update(x => new DocumentFiles { IsMainVersion = docFile.IsMainVersion, IsWorkedOut = docFile.IsWorkedOut, LastChangeDate = docFile.LastChangeDate, LastChangeUserId = docFile.LastChangeUserId });
                }
                if (docFile.IsTypeChange)
                {
                    dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)  //Without security restrictions
                        .Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument && x.Id != docFile.Id)
                        .Update(x => new DocumentFiles { TypeId = (int)docFile.Type, LastChangeDate = docFile.LastChangeDate, LastChangeUserId = docFile.LastChangeUserId });
                }
                if (docFile.IsBaseChange)
                {
                    dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)  //Without security restrictions
                        .Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument && x.Id == docFile.Id)
                        .Update(x => new DocumentFiles { TypeId = (int)docFile.Type, Description = docFile.Description, LastChangeDate = docFile.LastChangeDate, LastChangeUserId = docFile.LastChangeUserId });
                }
                if (docFile.IsFileNameChange)
                {
                    dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id)  //Without security restrictions
                        .Where(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument && x.Id == docFile.Id)
                        .Update(x => new DocumentFiles { Name = docFile.File.Name, Extension = docFile.File.Extension, LastChangeDate = docFile.LastChangeDate, LastChangeUserId = docFile.LastChangeUserId });
                }
                if (doc.Events?.Any() ?? false)
                {
                    var dbEvents = ModelConverter.GetDbDocumentEvents(doc.Events);
                    dbContext.DocumentEventsSet.AddRange(dbEvents);
                    dbContext.SaveChanges();
                }
                if (doc.Accesses?.Any() ?? false)
                {
                    dbContext.DocumentAccessesSet.AddRange(CommonQueries.GetDbDocumentAccesses(ctx, doc.Accesses, doc.Id).ToList());
                    dbContext.SaveChanges();
                }
                CommonQueries.AddFullTextCacheInfo(ctx, docFile.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDocumentFile> GetOldPdfForFiles(IContext ctx, int pdfAge)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentFilesSet    //Without security restrictions
                    .Where(x => x.IsPdfCreated.HasValue && x.IsPdfCreated.Value
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
                    PdfAcceptable = x.PdfAcceptable ?? false,
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
                entry.Property(x => x.PdfAcceptable).IsModified = true;
                entry.Property(x => x.LastPdfAccessDate).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public InternalDocument DeleteDocumentFilePrepare(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentFileQuery(ctx, new FilterDocumentFile { FileId = new List<int> { id }, IsAllVersion = true, IsAllDeleted = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Document.Id,
                        ClientId = x.Document.ClientId,
                        EntityTypeId = x.Document.EntityTypeId,
                        IsRegistered = x.Document.IsRegistered,
                        ExecutorPositionId = x.Document.ExecutorPositionId,
                        DocumentFiles = new List<InternalDocumentFile> { new InternalDocumentFile
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
                            Hash= x.Hash,
                            IsDeleted = x.IsDeleted,
                            IsContentDeleted = x.IsContentDeleted,
                            EventId = x.EventId,
                            File = new BaseFile
                            {
                                Name = x.Name,
                                Extension = x.Extension,
                            }
                        } }
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }

        /// <summary>
        /// Delete of file one or all versions
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="docFile"> should be filled IsMainVersion, DocumentId and OrderInDocument fields</param>
        public void DeleteDocumentFile(IContext ctx, InternalDocumentFile docFile)
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
                ExpressionStarter<DocumentFiles> filter;
                if (docFile.IsMainVersion)
                {
                    filter = PredicateBuilder.New<DocumentFiles>(x => x.DocumentId == docFile.DocumentId && x.OrderNumber == docFile.OrderInDocument);
                }
                else
                {
                    filter = PredicateBuilder.New<DocumentFiles>(x => x.Id == docFile.Id);
                }
                dbContext.DocumentFilesSet.Where(x => !x.IsDeleted).Where(filter).Update(x =>
                    new DocumentFiles { IsDeleted = true, LastChangeDate = docFile.LastChangeDate, LastChangeUserId = docFile.LastChangeUserId });
                CommonQueries.AddFullTextCacheInfo(ctx, docFile.DocumentId, EnumObjects.Documents, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteDocumentFileFinal(IContext ctx, InternalDocumentFile docFile)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                dbContext.DocumentFilesSet.Where(x => x.Id == docFile.Id && x.IsDeleted && !x.IsContentDeleted).Update(x =>
                    new DocumentFiles { IsContentDeleted = true, IsWorkedOut = false, LastChangeDate = docFile.LastChangeDate, LastChangeUserId = docFile.LastChangeUserId });
                transaction.Complete();
            }
        }

        public void DeleteDocumentFileFinal(IContext ctx, int days)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dateNow = DateTime.UtcNow;
                var date = dateNow.AddDays(-days);
                dbContext.DocumentFilesSet.Where(x => x.LastChangeDate < date && x.IsDeleted && !x.IsContentDeleted).Update(x =>
                    new DocumentFiles { IsContentDeleted = true, IsWorkedOut = false, LastChangeDate = dateNow, LastChangeUserId = ctx.CurrentAgentId });
                transaction.Complete();
            }
        }

        public InternalDocument RestoreDocumentFilePrepare(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var doc = CommonQueries.GetDocumentFileQuery(ctx, new FilterDocumentFile { FileId = new List<int> { id }, IsAllVersion = true, IsAllDeleted = true })
                    .Select(x => new InternalDocument
                    {
                        Id = x.Document.Id,
                        ClientId = x.Document.ClientId,
                        EntityTypeId = x.Document.EntityTypeId,
                        IsRegistered = x.Document.IsRegistered,
                        ExecutorPositionId = x.Document.ExecutorPositionId,
                        DocumentFiles = new List<InternalDocumentFile> { new InternalDocumentFile
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            EntityTypeId = x.EntityTypeId,
                            DocumentId = x.DocumentId,
                            OrderInDocument = x.OrderNumber,
                            ExecutorPositionId = x.ExecutorPositionId,
                            Type = (EnumFileTypes)x.TypeId,
                            IsMainVersion = !x.Document.Files.Any(y=>y.IsMainVersion && !y.IsDeleted && y.OrderNumber == x.OrderNumber), //определяем есть ли живая основная версия файла
                            Version = x.Version,
                            Hash= x.Hash,
                            IsDeleted = x.IsDeleted,
                            IsContentDeleted = x.IsContentDeleted,
                            EventId = x.EventId,
                            File = new BaseFile
                            {
                                Name = x.Name,
                                Extension = x.Extension,
                            }
                        } }
                    }).FirstOrDefault();
                transaction.Complete();
                return doc;
            }
        }

        /// <summary>
        /// Restore file version
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="docFile"> should be filled IsMainVersion, DocumentId and OrderInDocument fields</param>
        public void RestoreDocumentFile(IContext ctx, InternalDocumentFile docFile)
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
                dbContext.DocumentFilesSet.Where(x => x.Id == docFile.Id && !x.IsContentDeleted && x.IsDeleted).Update(x =>
                   new DocumentFiles { IsDeleted = false, IsMainVersion = docFile.IsMainVersion, LastChangeDate = docFile.LastChangeDate, LastChangeUserId = docFile.LastChangeUserId });
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
                res = dbContext.DocumentFilesSet.Where(x => x.ClientId == ctx.Client.Id).Where(x => x.DocumentId == documentId && x.Name == fileName && x.Extension == fileExt) //TODO REFACKTORING
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
                var res = CommonQueries.GetDocumentFileQuery(ctx, new FilterDocumentFile { DocumentId = new List<int> { documentId }, IsAllVersion = true, IsAllDeleted = true, IsAllContentDeleted = true }, true)
                    .OrderByDescending(x => x.OrderNumber).Select(x => x.OrderNumber).FirstOrDefault() + 1;
                transaction.Complete();
                return res;
            }
        }

        public int GetFileNextVersion(IContext ctx, int documentId, int fileOrder)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = CommonQueries.GetDocumentFileQuery(ctx, new FilterDocumentFile { DocumentId = new List<int> { documentId }, IsAllVersion = true, OrderInDocument = new List<int> { fileOrder }, IsAllDeleted = true, IsAllContentDeleted = true }, true)
                    .OrderByDescending(x => x.Version).Select(x => x.Version).FirstOrDefault() + 1;
                transaction.Complete();
                return res;
            }
        }

    }
}