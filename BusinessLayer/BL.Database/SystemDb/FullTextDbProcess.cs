using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using EntityFramework.Extensions;

namespace BL.Database.SystemDb
{
    public class FullTextDbProcess : IFullTextDbProcess
    {
        public int GetCurrentMaxCasheId(IContext ctx)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.FullTextIndexCashSet.Any() ? dbContext.FullTextIndexCashSet.Max(x => x.Id) : 0;
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FullTextIndexItem> FullTextIndexToUpdate(IContext ctx, int maxIdValue)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.Database.CommandTimeout = 0;
                //Add deleted item to  process processing full text index
                res.AddRange(dbContext.FullTextIndexCashSet.Where(x=>x.Id<=maxIdValue).Select(x => new FullTextIndexItem
                {
                    Id = x.Id,
                    ObjectType = (EnumObjects)x.ObjectType,
                    OperationType = (EnumOperationType)x.OperationType,
                    ClientId = ctx.CurrentClientId,
                    ObjectId = x.ObjectId,
                    ObjectText = ""
                }).ToList());
                transaction.Complete();
            }
            return res;
        }

        private delegate List<FullTextQueryPrepare> DFullTextIndexItemQuery(IContext ctx, DmsContext dbContext, EnamFilterType filterType = EnamFilterType.Main);

        #region Filling FullTextIndexItemQuery
        private static readonly Dictionary<EnumObjects, DFullTextIndexItemQuery> FullTextIndexItemQuery =
        new Dictionary<EnumObjects, DFullTextIndexItemQuery>
        {
            #region Documents
            { EnumObjects.Documents, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DocumentsSet
                            .Where(x => x.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.TemplateDocumentDocumentTypeId:
                        qry = qry.Where(x=>x.Main.TemplateDocument.DocumentType!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TemplateDocument.DocumentTypeId});
                        break;
                    case EnamFilterType.ExecutorPositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.ExecutorPositionExecutorAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.ExecutorPositionExecutorAgentId});
                        break;
                    case EnamFilterType.RegistrationJournalId:
                        qry = qry.Where(x=>x.Main.RegistrationJournal!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.RegistrationJournalId.Value});
                        break;
                    case EnamFilterType.RegistrationJournalDepartmentId:
                        qry = qry.Where(x=>x.Main.RegistrationJournal.Department!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.RegistrationJournal.DepartmentId});
                        break;
                    case EnamFilterType.SenderAgentId:
                        qry = qry.Where(x=>x.Main.SenderAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.SenderAgentId.Value});
                        break;
                    case EnamFilterType.SenderAgentPersonId:
                        qry = qry.Where(x=>x.Main.SenderAgentPerson!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.SenderAgentPersonId.Value});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Documents); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId,ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.Documents,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.Documents,
                    ObjectText = (x.Main.RegistrationNumber != null ? (x.Main.RegistrationNumberPrefix ?? "") + x.Main.RegistrationNumber + (x.Main.RegistrationNumberSuffix ?? "") : "#" + x.Main.Id) + " "
                                + x.Main.RegistrationJournal.Name + " " + x.Main.RegistrationJournal.Department.Name + " "
                                + x.Main.TemplateDocument.DocumentType.Name + " " + x.Main.Description + " "
                                + x.Main.ExecutorPositionExecutorAgent.Name + " "
                                + x.Main.SenderAgent.Name + " " + x.Main.SenderAgentPerson.Agent.Name + " " + x.Main.SenderNumber + " "+ x.Main.Addressee
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DocumentEvents, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DocumentEventsSet
                            .Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.Slave:
                        break;
                    case EnamFilterType.SourcePositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.SourcePositionExecutorAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.SourcePositionExecutorAgentId.Value});
                        break;
                    case EnamFilterType.TargetPositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.TargetPositionExecutorAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetPositionExecutorAgentId.Value});
                        break;
                    case EnamFilterType.SourceAgentId:
                        qry = qry.Where(x=>x.Main.SourceAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.SourceAgentId.Value});
                        break;
                    case EnamFilterType.TargetAgentId:
                        qry = qry.Where(x=>x.Main.TargetAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetAgentId.Value});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Documents); var featureId = Features.GetId(Features.Events);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DocumentEvents,
                    ParentObjectId = x.Main.DocumentId, ParentObjectType = EnumObjects.Documents,
                    Access = x.Main.Task.TaskAccesses.Where(y=>x.Main.IsAvailableWithinTask).Select(y=>y.PositionId)
                                .Concat(new List<int> { x.Main.TargetPositionId??0, x.Main.SourcePositionId??0 }).Distinct().ToList(),
                    ObjectText = x.Main.Description + " " + x.Main.AddDescription + " " + x.Main.Task.Task + " "
                    + x.Main.SourcePositionExecutorAgent.Name + " "+ x.Main.SourceAgent.Name + " "
                    + x.Main.TargetPositionExecutorAgent.Name + " "+ x.Main.TargetAgent.Name + " "

                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DocumentSendLists, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DocumentSendListsSet
                            .Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.Slave:
                        break;
                    case EnamFilterType.SourcePositionId:
                        qry = qry.Where(x=>x.Main.SourcePosition!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.SourcePositionId});
                        break;
                    case EnamFilterType.TargetPositionId:
                        qry = qry.Where(x=>x.Main.TargetPosition!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetPositionId.Value});
                        break;
                    case EnamFilterType.SourcePositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.SourcePosition.ExecutorAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.SourcePosition.ExecutorAgentId.Value});
                        break;
                    case EnamFilterType.TargetPositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.TargetPosition.ExecutorAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetPosition.ExecutorAgentId.Value});
                        break;
                    case EnamFilterType.SourceAgentId:
                        qry = qry.Where(x=>x.Main.SourceAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.SourceAgentId});
                        break;
                    case EnamFilterType.TargetAgentId:
                        qry = qry.Where(x=>x.Main.TargetAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetAgentId.Value});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Documents); var featureId = Features.GetId(Features.Plan);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DocumentSendLists,
                    ParentObjectId = x.Main.DocumentId, ParentObjectType = EnumObjects.Documents,
                    Access = new List<int> { x.Main.TargetPositionId??0, x.Main.SourcePositionId }.Where(y=>!x.Main.IsInitial).Distinct().ToList(),
                    ObjectText = x.Main.Description + " " + x.Main.Task.Task + " "
                    + x.Main.SourcePosition.Name + " "+ x.Main.SourcePosition.ExecutorAgent.Name + " "+ x.Main.SourceAgent.Name + " "
                    + x.Main.TargetPosition.Name + " "+ x.Main.TargetPosition.ExecutorAgent.Name + " "+ x.Main.TargetAgent.Name + " "
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DocumentFiles, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DocumentFilesSet
                            .Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Where(x=>x.TypeId == (int)EnumFileTypes.Main || x.TypeId == (int)EnumFileTypes.Additional)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.Slave:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Documents); var featureId = Features.GetId(Features.Files);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DocumentFiles,
                    ParentObjectId = x.Main.DocumentId, ParentObjectType = EnumObjects.Documents,
                    Access = new List<int> { x.Main.ExecutorPositionId}.Where(y=> x.Main.TypeId == (int)EnumFileTypes.Additional).Distinct().ToList(),
                    ObjectText = x.Main.Name + " " + x.Main.Extension + " " + x.Main.Description
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DocumentSubscriptions, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DocumentSubscriptionsSet
                            .Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.Slave:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Documents); var featureId = Features.GetId(Features.Signs);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DocumentSubscriptions,
                    ParentObjectId = x.Main.DocumentId, ParentObjectType = EnumObjects.Documents,
                    ObjectText = x.Main.Description
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DocumentWaits, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DocumentWaitsSet
                            .Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.Slave:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Documents); var featureId = Features.GetId(Features.Waits);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DocumentWaits,
                    ParentObjectId = x.Main.DocumentId, ParentObjectType = EnumObjects.Documents,
                    ObjectText = x.Main.TargetDescription
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DocumentTags, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DocumentTagsSet
                            .Where(x => x.Document.TemplateDocument.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.Slave:
                        break;
                    case EnamFilterType.TagId:
                        qry = qry.Where(x=>x.Main.Tag!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TagId});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Documents); var featureId = Features.GetId(Features.Tags);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DocumentTags,
                    ParentObjectId = x.Main.DocumentId, ParentObjectType = EnumObjects.Documents,
                    ObjectText = x.Main.Tag.Name
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            #endregion Documents

            #region TemplateDocuments
            { EnumObjects.TemplateDocument, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.TemplateDocumentsSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.TemplateDocumentDocumentTypeId:
                        qry = qry.Where(x=>x.Main.DocumentType!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.DocumentTypeId});
                        break;
                    case EnamFilterType.RegistrationJournalId:
                        qry = qry.Where(x=>x.Main.RegistrationJournal!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.RegistrationJournalId.Value});
                        break;
                    case EnamFilterType.RegistrationJournalDepartmentId:
                        qry = qry.Where(x=>x.Main.RegistrationJournal.Department!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.RegistrationJournal.DepartmentId});
                        break;
                    case EnamFilterType.SenderAgentId:
                        qry = qry.Where(x=>x.Main.SenderAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.SenderAgentId.Value});
                        break;
                    case EnamFilterType.SenderAgentPersonId:
                        qry = qry.Where(x=>x.Main.SenderAgentPerson!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.SenderAgentPersonId.Value});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Templates); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId,ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.TemplateDocument,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.TemplateDocument,
                    Access = x.Main.Accesses.Select(y=>y.PositionId.Value).ToList(),
                    ObjectText = x.Main.Name + " " + x.Main.RegistrationJournal.Name + " " + x.Main.RegistrationJournal.Department.Name + " "
                                + x.Main.DocumentType.Name + " " + x.Main.Description + " "
                                + x.Main.SenderAgent.Name + " " + x.Main.SenderAgentPerson.Agent.Name + x.Main.Addressee
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.TemplateDocumentAttachedFiles, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.TemplateDocumentFilesSet
                            .Where(x => x.Document.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.Slave:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Templates); var featureId = Features.GetId(Features.Files);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.TemplateDocumentAttachedFiles,
                    ParentObjectId = x.Main.DocumentId, ParentObjectType = EnumObjects.TemplateDocument,
                    ObjectText = x.Main.Name + " " + x.Main.Extention + " " + x.Main.Description
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.TemplateDocumentTask, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.TemplateDocumentTasksSet
                            .Where(x => x.Document.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.Slave:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Templates); var featureId = Features.GetId(Features.Tasks);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.TemplateDocumentTask,
                    ParentObjectId = x.Main.DocumentId, ParentObjectType = EnumObjects.TemplateDocument,
                    ObjectText = x.Main.Task + " " + x.Main.Description
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.TemplateDocumentSendList, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.TemplateDocumentSendListsSet
                            .Where(x => x.Document.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.Slave:
                        break;
                    case EnamFilterType.TargetPositionId:
                        qry = qry.Where(x=>x.Main.TargetPosition!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetPositionId.Value});
                        break;
                    case EnamFilterType.TargetPositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.TargetPosition.ExecutorAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetPosition.ExecutorAgentId.Value});
                        break;
                    case EnamFilterType.TargetAgentId:
                        qry = qry.Where(x=>x.Main.TargetAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetAgentId.Value});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Templates); var featureId = Features.GetId(Features.Plan);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId,FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DocumentSendLists,
                    ParentObjectId = x.Main.DocumentId, ParentObjectType = EnumObjects.Documents,
                    ObjectText = x.Main.Description + " " + x.Main.Task.Task + " "
                    + x.Main.TargetPosition.Name + " "+ x.Main.TargetPosition.ExecutorAgent.Name + " "+ x.Main.TargetAgent.Name + " "
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            #endregion TemplateDocuments

            #region Complex Dictionary Info
            { EnumObjects.DictionaryAgentClientCompanies, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryAgentClientCompaniesSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Org); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentClientCompanies,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryAgentClientCompanies,
                    ObjectText = x.Main.Agent.Name + " " + x.Main.FullName+ " " + x.Main.Description
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryDepartments, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryDepartmentsSet
                            .Where(x => x.Company.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Department); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryDepartments,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryDepartments,
                    ObjectText = x.Main.Name + " " + x.Main.FullName+ " " + x.Main.Code
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryPositions, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryPositionsSet
                            .Where(x => x.Department.Company.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Position); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryPositions,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryPositions,
                    ObjectText = x.Main.Name + " " + x.Main.FullName
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryAgentEmployees, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryAgentEmployeesSet
                            .Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Employee); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentEmployees,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryAgentEmployees,
                    ObjectText = x.Main.PersonnelNumber + " " + x.Main.Description + " " + x.Main.Agent.Name + " "
                    + x.Main.Agent.AgentPeople.FullName + " " + x.Main.Agent.AgentPeople.BirthDate + " " + " " + x.Main.Agent.AgentPeople.TaxCode
                    + x.Main.Agent.AgentPeople.PassportDate + " " + x.Main.Agent.AgentPeople.PassportNumber + " " + x.Main.Agent.AgentPeople.PassportSerial + " " + x.Main.Agent.AgentPeople.PassportText
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryAgentCompanies, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryAgentCompaniesSet
                            .Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Company); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentCompanies,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryAgentCompanies,
                    ObjectText = x.Main.Agent.Name + " " + x.Main.FullName + " " + x.Main.OKPOCode + " " + x.Main.Description + " " + x.Main.TaxCode + " " + x.Main.VATCode
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryAgentPeople, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryAgentPersonsSet
                            .Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Person); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentPeople,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryAgentPeople,
                    ObjectText = x.Main.Agent.Name + " " + " " + x.Main.Description + " "
                         + x.Main.Agent.AgentPeople.FullName + x.Main.Agent.AgentPeople.TaxCode + " " + x.Main.Agent.AgentPeople.BirthDate + " "
                        + x.Main.Agent.AgentPeople.PassportNumber + " " + x.Main.Agent.AgentPeople.PassportSerial + " " + x.Main.Agent.AgentPeople.PassportText
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryAgentBanks, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryAgentBanksSet
                            .Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Bank); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentBanks,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryAgentBanks,
                    ObjectText = x.Main.Agent.Name + " " + x.Main.FullName + " " + x.Main.Description + " " + x.Main.MFOCode + " " + x.Main.Swift
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryStandartSendLists, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryStandartSendListsSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.PositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.Position.ExecutorAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.Position.ExecutorAgentId.Value});
                        break;
                    case EnamFilterType.PositionId:
                        qry = qry.Where(x=>x.Main.Position!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.PositionId.Value});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.SendList); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryStandartSendLists,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryStandartSendLists,
                    ObjectText = x.Main.Name + " " + x.Main.Position.Name + " "+ x.Main.Position.ExecutorAgent.Name
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.CustomDictionaryTypes, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.CustomDictionaryTypesSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.CustomDictionaries); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.CustomDictionaryTypes,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.CustomDictionaryTypes,
                    ObjectText = x.Main.Code + " " + x.Main.Name + " " + x.Main.Description
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.AdminRoles, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.AdminRolesSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Role); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.AdminRoles,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.AdminRoles,
                    ObjectText = x.Main.Name + " " + x.Main.Description
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            #endregion Complex Dictionary Info

            #region Complex Dictionary Details
            { EnumObjects.DictionaryAgentAddresses, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryAgentAddressesSet
                            .Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                {
                    var moduleId = Modules.GetId(Modules.Employee); var featureId = Features.GetId(Features.Addresses);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentEmployee != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentAddresses,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentEmployees,
                        ObjectText = x.Main.Address + " " + x.Main.PostCode + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Person); var featureId = Features.GetId(Features.Addresses);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentPerson != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentAddresses,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentPeople,
                        ObjectText = x.Main.Address + " " + x.Main.PostCode + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Bank); var featureId = Features.GetId(Features.Addresses);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentBank != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentAddresses,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentBanks,
                        ObjectText = x.Main.Address + " " + x.Main.PostCode + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Company); var featureId = Features.GetId(Features.Addresses);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentCompany != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentAddresses,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentCompanies,
                        ObjectText = x.Main.Address + " " + x.Main.PostCode + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Org); var featureId = Features.GetId(Features.Addresses);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentOrg != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentAddresses,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentClientCompanies,
                        ObjectText = x.Main.Address + " " + x.Main.PostCode + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                return res;
            } },
            { EnumObjects.DictionaryContacts, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryAgentContactsSet
                            .Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                {
                    var moduleId = Modules.GetId(Modules.Employee); var featureId = Features.GetId(Features.Contacts);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentEmployee != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryContacts,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentEmployees,
                        ObjectText = x.Main.Contact + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Person); var featureId = Features.GetId(Features.Contacts);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentPerson != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryContacts,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentPeople,
                        ObjectText = x.Main.Contact + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Bank); var featureId = Features.GetId(Features.Contacts);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentBank != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryContacts,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentBanks,
                        ObjectText = x.Main.Contact + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Company); var featureId = Features.GetId(Features.Contacts);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentCompany != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryContacts,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentCompanies,
                        ObjectText = x.Main.Contact + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Org); var featureId = Features.GetId(Features.Contacts);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentOrg != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryContacts,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentClientCompanies,
                        ObjectText = x.Main.Contact + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                return res;
            } },
            { EnumObjects.DictionaryAgentPersons, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryAgentPersonsSet
                            .Where(x => x.Agent.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.PeopleId:
                        qry = qry.Where(x=>x.Main.People!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.Id});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                {
                    var moduleId = Modules.GetId(Modules.Company); var featureId = Features.GetId(Features.Contacts);
                    var qryRes= qry.Where(x=>x.Main.Agent.AgentCompany != null)
                    .Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAgentPersons,
                        ParentObjectId = x.Main.AgentCompanyId.Value, ParentObjectType = EnumObjects.DictionaryAgentCompanies,
                        ObjectText = x.Main.People.Agent.Name + x.Main.People.FullName + x.Main.Position + " " + x.Main.Description
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                return res;
            } },
            { EnumObjects.DictionaryStandartSendListContent, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryStandartSendListContentsSet
                            .Where(x => x.StandartSendList.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.TargetPositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.TargetPosition.ExecutorAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetPosition.ExecutorAgentId.Value});
                        break;
                    case EnamFilterType.TargetPositionId:
                        qry = qry.Where(x=>x.Main.TargetPosition!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetPositionId.Value});
                        break;
                    case EnamFilterType.TargetAgentId:
                        qry = qry.Where(x=>x.Main.TargetAgent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.TargetAgentId.Value});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.SendList); var featureId = Features.GetId(Features.Contents);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryStandartSendLists,
                    ParentObjectId = x.Main.StandartSendListId, ParentObjectType = EnumObjects.DictionaryStandartSendLists,
                    ObjectText = x.Main.Description + " " + x.Main.Task + " " + x.Main.TargetPosition.Name + " "+ x.Main.TargetPosition.ExecutorAgent.Name
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.CustomDictionaries, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.CustomDictionariesSet
                            .Where(x => x.CustomDictionaryType.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.CustomDictionaries); var featureId = Features.GetId(Features.Contents);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.CustomDictionaries,
                    ParentObjectId = x.Main.DictionaryTypeId, ParentObjectType = EnumObjects.CustomDictionaryTypes,
                    ObjectText = x.Main.Code + " " + x.Main.Name + " " + x.Main.Description
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.AdminUserRoles, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.AdminUserRolesSet
                            .Where(x => x.Role.ClientId == ctx.CurrentClientId)
                            //TODO разрулить текущие/все назначения
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.RoleId:
                        qry = qry.Where(x=>x.Main.Role!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.RoleId});
                        break;
                    case EnamFilterType.PositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.PositionExecutor.Agent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.PositionExecutor.AgentId});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                {
                    var moduleId = Modules.GetId(Modules.Role); var featureId = Features.GetId(Features.Employees);
                    var qryRes= qry.Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.AdminUserRoles,
                        ParentObjectId = x.Main.RoleId, ParentObjectType = EnumObjects.AdminRoles,
                        DateFrom = x.Main.PositionExecutor.StartDate, DateTo = x.Main.PositionExecutor.EndDate,
                        ObjectText = x.Main.PositionExecutor.Agent.Name
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Employee); var featureId = Features.GetId(Features.Roles);
                    var qryRes= qry.Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.AdminUserRoles,
                        ParentObjectId = x.Main.PositionExecutor.AgentId, ParentObjectType = EnumObjects.DictionaryAgentEmployees,
                        DateFrom = x.Main.PositionExecutor.StartDate, DateTo = x.Main.PositionExecutor.EndDate,
                        ObjectText = x.Main.Role.Name
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }
                return res;
            } },
            { EnumObjects.AdminPositionRoles, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.AdminPositionRolesSet
                            .Where(x => x.Role.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.RoleId:
                        qry = qry.Where(x=>x.Main.Role!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.RoleId});
                        break;
                    case EnamFilterType.PositionId:
                        qry = qry.Where(x=>x.Main.Position!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.PositionId});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                {
                    var moduleId = Modules.GetId(Modules.Role); var featureId = Features.GetId(Features.Positions);
                    var qryRes= qry.Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.AdminPositionRoles,
                        ParentObjectId = x.Main.RoleId, ParentObjectType = EnumObjects.AdminRoles,
                        ObjectText = x.Main.Position.Name
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Position); var featureId = Features.GetId(Features.Roles);
                    var qryRes= qry.Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.AdminPositionRoles,
                        ParentObjectId = x.Main.PositionId, ParentObjectType = EnumObjects.DictionaryPositions,
                        ObjectText = x.Main.Role.Name
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }
                return res;
            } },
            { EnumObjects.DictionaryPositionExecutors, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryPositionExecutorsSet
                            .Where(x => x.Position.Department.Company.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.PositionId:
                        qry = qry.Where(x=>x.Main.Position!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.PositionId});
                        break;
                    case EnamFilterType.PositionExecutorAgentId:
                        qry = qry.Where(x=>x.Main.Agent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.AgentId});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                {
                    var moduleId = Modules.GetId(Modules.Employee); var featureId = Features.GetId(Features.Positions);
                    var qryRes= qry.Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryPositionExecutors,
                        ParentObjectId = x.Main.AgentId, ParentObjectType = EnumObjects.DictionaryAgentEmployees,
                        DateFrom = x.Main.StartDate, DateTo = x.Main.EndDate,
                        ObjectText = x.Main.Position.Name
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Position); var featureId = Features.GetId(Features.Employees);
                    var qryRes= qry.Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryPositionExecutors,
                        ParentObjectId = x.Main.PositionId, ParentObjectType = EnumObjects.DictionaryPositions,
                        DateFrom = x.Main.StartDate, DateTo = x.Main.EndDate,
                        ObjectText = x.Main.Agent.Name
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }
                return res;
            } },
            { EnumObjects.AdminRegistrationJournalPositions, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.AdminRegistrationJournalPositionsSet
                            .Where(x => x.RegistrationJournal.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});
                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.RegistrationJournalId:
                        qry = qry.Where(x=>x.Main.RegistrationJournal!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.RegJournalId });
                        break;
                    case EnamFilterType.PositionId:
                        qry = qry.Where(x=>x.Main.Position!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.PositionId});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                {
                    var moduleId = Modules.GetId(Modules.Journal); var featureId = Features.GetId(Features.Positions);
                    var qryRes= qry.Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.AdminRegistrationJournalPositions,
                        ParentObjectId = x.Main.RegJournalId, ParentObjectType = EnumObjects.DictionaryRegistrationJournals,
                        ObjectText = x.Main.Position.Name
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                {
                    var moduleId = Modules.GetId(Modules.Position); var featureId = Features.GetId(Features.Journals);
                    var qryRes= qry.Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.AdminRegistrationJournalPositions,
                        ParentObjectId = x.Main.PositionId, ParentObjectType = EnumObjects.DictionaryPositions,
                        ObjectText = x.Main.RegistrationJournal.Name
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }
                return res;
            } },
            { EnumObjects.AdminEmployeeDepartments, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.AdminEmployeeDepartmentsSet
                            .Where(x => x.Department.Company.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.EmployeeId:
                        qry = qry.Where(x=>x.Main.Employee.Agent!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.EmployeeId});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                {
                    var moduleId = Modules.GetId(Modules.Department); var featureId = Features.GetId(Features.Admins);
                    var qryRes= qry.Select(x => new FullTextIndexItem
                    {
                        ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                        ObjectId = x.Main.Id, ObjectType = EnumObjects.AdminEmployeeDepartments,
                        ParentObjectId = x.Main.DepartmentId, ParentObjectType = EnumObjects.DictionaryDepartments,
                        ObjectText = x.Main.Employee.Agent.Name
                    });
                    res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                }

                return res;
            } },
            #endregion Complex Dictionary Details

            #region Simple Dictionary
            { EnumObjects.DictionaryAddressType, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryAddressTypesSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.AddressType); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryAddressType,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryAddressType,
                    ObjectText = x.Main.Code + " " + x.Main.Name
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryContactType, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryContactTypesSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.ContactType); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryContactType,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryContactType,
                    ObjectText = x.Main.Code + " " + x.Main.Name
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryDocumentType, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryDocumentTypesSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.DocumentType); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryDocumentType,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryDocumentType,
                    ObjectText = x.Main.Name
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryRegistrationJournals, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryRegistrationJournalsSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    case EnamFilterType.DepartmentId:
                        qry = qry.Where(x=>x.Main.Department!=null).Select(x=>new { Main = x.Main, FilterId = x.Main.DepartmentId});
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Journal); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryRegistrationJournals,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryRegistrationJournals,
                    ObjectText =  x.Main.Index + " " + x.Main.Name + " " + x.Main.Department.FullName + " " + x.Main.Department.Name
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            { EnumObjects.DictionaryTag, (ctx,dbContext,filterType) =>
            {
                var res = new List<FullTextQueryPrepare>();
                var qry = dbContext.DictionaryTagsSet
                            .Where(x => x.ClientId == ctx.CurrentClientId)
                            .Select(x=>new { Main = x, FilterId = 0});

                switch (filterType)
                {
                    case EnamFilterType.Main:
                        break;
                    default:
                        throw new WrongParameterTypeError();
                }

                var moduleId = Modules.GetId(Modules.Tags); var featureId = Features.GetId(Features.Info);
                var qryRes= qry.Select(x => new FullTextIndexItem
                {
                    ClientId = ctx.CurrentClientId, FilterId = x.FilterId, ModuleId = moduleId, FeatureId = featureId,
                    ObjectId = x.Main.Id, ObjectType = EnumObjects.DictionaryTag,
                    ParentObjectId = x.Main.Id, ParentObjectType = EnumObjects.DictionaryTag,
                    ObjectText = x.Main.Name
                });
                res.Add(new FullTextQueryPrepare { Query = qryRes, FilterType = filterType});
                return res;
            } },
            #endregion Simple Dictionary
        };
        #endregion Filling FullTextIndexItemQuery

        #region Filling FullTextDeepUpdateParams
        private static readonly Dictionary<EnumObjects, List<FullTextDeepUpdateItemQuery>> FullTextDeepUpdateParams = new Dictionary<EnumObjects, List<FullTextDeepUpdateItemQuery>>
        {
            {EnumObjects.Documents, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.Slave },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.Slave },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentTags, FilterType = EnamFilterType.Slave },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentFiles, FilterType = EnamFilterType.Slave },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSubscriptions, FilterType = EnamFilterType.Slave },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentWaits, FilterType = EnamFilterType.Slave },
            }
            },
            {EnumObjects.DictionaryDepartments, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DictionaryRegistrationJournals, FilterType = EnamFilterType.DepartmentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.Documents, FilterType = EnamFilterType.RegistrationJournalDepartmentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocument, FilterType = EnamFilterType.RegistrationJournalDepartmentId },
                }
            },
            {EnumObjects.DictionaryPositions, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.SourcePositionId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.TargetPositionId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocumentSendList, FilterType = EnamFilterType.TargetPositionId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DictionaryStandartSendLists, FilterType = EnamFilterType.PositionId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DictionaryStandartSendListContent, FilterType = EnamFilterType.TargetPositionId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.AdminPositionRoles, FilterType = EnamFilterType.PositionId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DictionaryPositionExecutors, FilterType = EnamFilterType.PositionId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.AdminRegistrationJournalPositions, FilterType = EnamFilterType.PositionId },
                }
            },
            {EnumObjects.DictionaryAgentEmployees, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.Documents, FilterType = EnamFilterType.ExecutorPositionExecutorAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.SourcePositionExecutorAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.TargetPositionExecutorAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.SourcePositionExecutorAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.TargetPositionExecutorAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocumentSendList, FilterType = EnamFilterType.TargetPositionExecutorAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.SourceAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.SourceAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocumentSendList, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DictionaryStandartSendLists, FilterType = EnamFilterType.PositionExecutorAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DictionaryStandartSendListContent, FilterType = EnamFilterType.TargetPositionExecutorAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.AdminUserRoles, FilterType = EnamFilterType.PositionExecutorAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.AdminEmployeeDepartments, FilterType = EnamFilterType.EmployeeId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DictionaryPositionExecutors, FilterType = EnamFilterType.PositionExecutorAgentId },
               }
            },
            {EnumObjects.DictionaryAgentCompanies, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.Documents, FilterType = EnamFilterType.SenderAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocument, FilterType = EnamFilterType.SenderAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.SourceAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.SourceAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocumentSendList, FilterType = EnamFilterType.TargetAgentId },
                }
            },
            {EnumObjects.DictionaryAgentPeople, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.SourceAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.SourceAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocumentSendList, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DictionaryAgentPersons, FilterType = EnamFilterType.PeopleId },
            }
            },
            {EnumObjects.DictionaryAgentBanks, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.SourceAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentEvents, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.SourceAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentSendLists, FilterType = EnamFilterType.TargetAgentId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocumentSendList, FilterType = EnamFilterType.TargetAgentId },
                }
            },
            {EnumObjects.DictionaryAgentPersons, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.Documents, FilterType = EnamFilterType.SenderAgentPersonId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocument, FilterType = EnamFilterType.SenderAgentPersonId },
                }
            },
            {EnumObjects.AdminRoles, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.AdminUserRoles, FilterType = EnamFilterType.RoleId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.AdminPositionRoles, FilterType = EnamFilterType.RoleId },
                }
            },
            { EnumObjects.DictionaryDocumentType, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.Documents, FilterType = EnamFilterType.TemplateDocumentDocumentTypeId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocument, FilterType = EnamFilterType.TemplateDocumentDocumentTypeId },
                }
            },
            { EnumObjects.DictionaryRegistrationJournals, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.Documents, FilterType = EnamFilterType.RegistrationJournalId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.TemplateDocument, FilterType = EnamFilterType.RegistrationJournalId },
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.AdminRegistrationJournalPositions, FilterType = EnamFilterType.RegistrationJournalId },
                }
            },
            { EnumObjects.DictionaryTag, new List<FullTextDeepUpdateItemQuery>
                {
                    new FullTextDeepUpdateItemQuery { ObjectType = EnumObjects.DocumentTags, FilterType = EnamFilterType.TagId },
                }
            },
        };
        #endregion Filling FullTextDeepUpdateParams

        public List<EnumObjects> ObjectToReindex()
        {
            return FullTextIndexItemQuery.Keys.ToList();
        }

        public List<int> GetItemsToUpdateCount(IContext ctx, EnumObjects objectType, bool isDeepUpdate)
        {
            var res = new List<int>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qrys = FullTextIndexItemQuery[objectType](ctx, dbContext);

                if (isDeepUpdate)
                    foreach (var item in FullTextDeepUpdateParams[objectType])
                        qrys.AddRange(FullTextIndexItemQuery[item.ObjectType](ctx, dbContext, item.FilterType));

                res.AddRange(qrys.Select(qry => qry.Query.Count()));
                transaction.Complete();
            }
            return res;
        }

        public IEnumerable<FullTextIndexItem> GetItemsToReindex(IContext ctx, EnumObjects objectType, int? itemCount, int? offset)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qrys = FullTextIndexItemQuery[objectType](ctx, dbContext);

                foreach (var qry in qrys)
                {
                    qry.Query = qry.Query.OrderBy(x => x.ObjectId);
                    if (offset.HasValue)
                    {
                        qry.Query = qry.Query.Skip(() => offset.Value);
                    }
                    if (itemCount.HasValue)
                    {
                        qry.Query = qry.Query.Take(() => itemCount.Value);
                    }
                    res.AddRange(qry.Query.ToList());
                }

                transaction.Complete();
            }
            return res;
        }

        // перепостраивает поисковый индекс для указанного документа
        public IEnumerable<FullTextIndexItem> FullTextIndexPrepareNew(IContext ctx, EnumObjects objectType, bool isDeepUpdate, bool IsDirectFilter, int? idBeg, int? idEnd)
        {
            var res = new List<FullTextIndexItem>();
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {

                var qrys = FullTextIndexItemQuery[objectType](ctx, dbContext);

                if (isDeepUpdate)
                    foreach (var item in FullTextDeepUpdateParams[objectType])
                        qrys.AddRange(FullTextIndexItemQuery[item.ObjectType](ctx, dbContext, item.FilterType));

                foreach (var qry in qrys)
                {
                    if (qry.FilterType == EnamFilterType.Main)
                    {
                        qry.Query = IsDirectFilter 
                            ? qry.Query.Where(x => (!idBeg.HasValue || x.ObjectId >= idBeg.Value) && (!idEnd.HasValue || x.ObjectId <= idEnd.Value)) 
                            : qry.Query.Where(x => dbContext.FullTextIndexCashSet.Where(y => (!idBeg.HasValue || y.Id >= idBeg.Value) && (!idEnd.HasValue || y.Id <= idEnd.Value)).Select(y => y.ObjectId).Contains(x.ObjectId));
                    }
                    else if (qry.FilterType == EnamFilterType.Slave)
                    {
                        qry.Query = IsDirectFilter 
                            ? qry.Query.Where(x => (!idBeg.HasValue || x.ParentObjectId >= idBeg.Value) && (!idEnd.HasValue || x.ObjectId <= idEnd.Value)) 
                            : qry.Query.Where(x => dbContext.FullTextIndexCashSet.Where(y => (!idBeg.HasValue || y.Id >= idBeg.Value) && (!idEnd.HasValue || y.Id <= idEnd.Value)).Select(y => y.ObjectId).Contains(x.ParentObjectId));
                    }
                    else
                    {
                        qry.Query = IsDirectFilter 
                            ? qry.Query.Where(x => (!idBeg.HasValue || x.FilterId >= idBeg.Value) && (!idEnd.HasValue || x.ObjectId <= idEnd.Value)) 
                            : qry.Query.Where(x => x.FilterId != 0 && dbContext.FullTextIndexCashSet.Where(y => (!idBeg.HasValue || y.Id >= idBeg.Value) && (!idEnd.HasValue || y.Id <= idEnd.Value)).Select(y => y.ObjectId).Contains(x.FilterId));
                    }
                    res.AddRange(qry.Query.ToList());
                }
                transaction.Complete();
            }
            return res;
        }

        public void FullTextIndexDeleteProcessed(IContext ctx, IEnumerable<int> processedIds, bool deleteSimilarObject = false)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                if (processedIds.Count() < 50)
                {
                    dbContext.FullTextIndexCashSet.RemoveRange(
                        dbContext.FullTextIndexCashSet.Where(x => processedIds.Contains(x.Id)));
                }
                else
                {
                    while (processedIds.Any())
                    {
                        var currIds = processedIds.Take(50).ToList();
                        dbContext.FullTextIndexCashSet.RemoveRange(dbContext.FullTextIndexCashSet.Where(x => currIds.Contains(x.Id)));
                        processedIds = processedIds.Except(currIds);

                    }
                }
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void FullTextIndexDeleteCash(IContext ctx, int deleteBis)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.FullTextIndexCashSet.Where(x => x.Id <= deleteBis).Delete();
                transaction.Complete();
            }
        }


    }
}