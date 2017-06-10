using BL.CrossCutting.Helpers;
using BL.CrossCutting.Helpers.CashService;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using BL.Database.Helper;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FrontModel;
using BL.Model.Common;
using BL.Model.Constants;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontMainModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.Model.Tree;
using EntityFramework.Extensions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BL.Database.Dictionaries
{
    public class DictionariesDbProcess : CoreDb.CoreDb
    {
        private readonly ICacheService _cacheService;
        public DictionariesDbProcess(ICacheService casheService)
        {
            _cacheService = casheService;
        }

        #region [+]Agents ...

        public int AddAgent(IContext ctx, InternalDictionaryAgent model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(ctx, model);

                dbContext.DictionaryAgentsSet.Add(dbModel);
                dbContext.SaveChanges();

                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateAgent(IContext ctx, InternalDictionaryAgent agent)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(ctx, agent);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void UpdateAgentName(IContext ctx, int id, InternalDictionaryAgent agent)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(ctx, agent);

                dbContext.SafeAttach(dbModel);

                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;

                //entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                transaction.Complete();
            }

        }


        public void DeleteAgents(IContext ctx, FilterDictionaryAgent filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentsQuery(ctx, filter);
                qry.Delete();

                transaction.Complete();
            }
        }

        public void SetAgentImage(IContext ctx, InternalDictionaryAgentImage User)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentImage(ctx, User);
                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.Image).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        public IEnumerable<FrontDictionaryAgent> GetAgents(IContext ctx, FilterDictionaryAgent filter, UIPaging paging)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontDictionaryAgent>();

                var res = qry.Select(x => new FrontDictionaryAgent
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = true,
                    ResidentTypeId = x.ResidentTypeId,
                    Description = string.Empty,

                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetAgentExternalList(IContext ctx, UIPaging paging)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                //Контрагенты(Тип: Физ.лицо / Юр.лицо / Банк, ИНН / ОКПО / МФО)

                var label = Labels.Get("Agents", "Person");
                var qryPersons = GetAgentPersonsQuery(ctx, new FilterDictionaryAgentPerson { IsActive = true });
                var resPersons = qryPersons.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    Details = new List<string> { label, x.People.TaxCode ?? string.Empty }
                }).ToList();

                label = Labels.Get("Agents", "Company");
                var qryCompanies = GetAgentCompaniesQuery(ctx, new FilterDictionaryAgentCompany { IsActive = true });
                var resCompanies = qryCompanies.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    Details = new List<string> { label, x.OKPOCode ?? string.Empty }
                }).ToList();

                label = Labels.Get("Agents", "Bank");
                var qryBanks = GetAgentBanksQuery(ctx, new FilterDictionaryAgentBank { IsActive = true });
                var resBanks = qryBanks.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    Details = new List<string> { label, x.MFOCode ?? string.Empty }
                }).ToList();

                var res = resCompanies.Union(resBanks).Union(resPersons).OrderBy(x => x.Name).ToList();

                if (Paging.Set(ref res, paging) == EnumPagingResult.IsOnlyCounter) return new List<AutocompleteItem>();

                transaction.Complete();
                return res;
            }
        }


        private IQueryable<DictionaryAgents> GetAgentsQuery(IContext ctx, FilterDictionaryAgent filter)
        {
            var dbContext = ctx.DbContext as DmsContext;

            var qry = dbContext.DictionaryAgentsSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                //SetWhereExpressionIN(filter.IDs, ref qry);

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgents>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgents>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    //qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgents>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

            }

            return qry;
        }

        public bool ExistsAgents(IContext ctx, FilterDictionaryAgent filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentsQuery(ctx, filter).Any();

                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+]People ...
        public int AddAgentPeople(IContext ctx, InternalDictionaryAgentPeople people)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                FormAgentPeopleName(people);

                people.Id = AddAgent(ctx, people);

                var dbModel = DictionaryModelConverter.GetDbAgentPeople(ctx, people);

                dbContext.DictionaryAgentPeopleSet.Add(dbModel);
                dbContext.SaveChanges();

                transaction.Complete();

                return people.Id;
            }
        }

        public void UpdateAgentPeople(IContext ctx, InternalDictionaryAgentPeople people)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                FormAgentPeopleName(people);

                UpdateAgentName(ctx, people.Id, people);

                var dbModel = DictionaryModelConverter.GetDbAgentPeople(ctx, people);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.FullName).IsModified = true;
                entity.Property(x => x.LastName).IsModified = true;
                entity.Property(x => x.FirstName).IsModified = true;
                entity.Property(x => x.MiddleName).IsModified = true;
                entity.Property(x => x.BirthDate).IsModified = true;
                entity.Property(x => x.IsMale).IsModified = true;
                entity.Property(x => x.TaxCode).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        private void FormAgentPeopleName(InternalDictionaryAgentPeople people)
        {
            var F = string.Empty;
            var M = string.Empty;

            if (!string.IsNullOrEmpty(people.FirstName)) F = people.FirstName.Trim().First().ToString();
            if (!string.IsNullOrEmpty(people.MiddleName)) M = people.MiddleName.Trim().First().ToString();

            people.Name = people.LastName?.Trim();
            people.Name += string.IsNullOrEmpty(F) ? "" : (" " + F + ".");
            people.Name += string.IsNullOrEmpty(F + M) ? "" : (M + ".");

            people.FullName = (people.LastName?.Trim() + " " + people.FirstName?.Trim() + " " + people.MiddleName?.Trim())?.Trim();
        }

        public void UpdateAgentPeoplePassport(IContext ctx, InternalDictionaryAgentPeople people)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentPeople(ctx, people);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.PassportDate).IsModified = true;
                entity.Property(x => x.PassportNumber).IsModified = true;
                entity.Property(x => x.PassportSerial).IsModified = true;
                entity.Property(x => x.PassportText).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;

                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void DeleteAgentPeoples(IContext ctx, FilterDictionaryAgentPeoples filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPeoplesQuery(ctx, filter);
                qry.Delete();
                transaction.Complete();
            }
        }

        public FrontAgentPeoplePassport GetAgentPeoplePassport(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPeoplesQuery(ctx, new FilterDictionaryAgentPeoples { IDs = new List<int> { id } });

                var res = qry.Select(x => new FrontAgentPeoplePassport
                {
                    Id = x.Id,
                    PassportSerial = x.PassportSerial,
                    PassportNumber = x.PassportNumber,
                    PassportText = x.PassportText,
                    PassportDate = x.PassportDate,
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryAgentPeople> GetAgentPeoplesQuery(IContext ctx, FilterDictionaryAgentPeoples filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentPeopleSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentPeople>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentPeople>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }
        #endregion

        #region [+]Person ...
        public int AddAgentPerson(IContext ctx, InternalDictionaryAgentPerson person)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                person.Id = AddAgentPeople(ctx, person);

                var dbModel = DictionaryModelConverter.GetDbAgentPerson(ctx, person);

                dbContext.DictionaryAgentPersonsSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.AddNew);
                transaction.Complete();

                return person.Id;
            }
        }

        public void UpdateAgentPerson(IContext ctx, InternalDictionaryAgentPerson person)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentPeople(ctx, person);

                var dbModel = DictionaryModelConverter.GetDbAgentPerson(ctx, person);

                dbContext.SafeAttach(dbModel);
                //dbContext.SaveChanges();
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.UpdateFull);
                transaction.Complete();

            }
        }

        public void AddAgentPersonToCompany(IContext ctx, InternalDictionaryAgentPerson person)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentPerson(ctx, person);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                //entity.State = EntityState.Modified;
                entity.Property(x => x.AgentCompanyId).IsModified = true;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.UpdateFull);
                transaction.Complete();

            }
        }

        public void DeleteAgentPersons(IContext ctx, FilterDictionaryAgentPerson filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryAgentPersons, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }

        public IEnumerable<FrontContactPersons> GetAgentPersonsWithContacts(IContext ctx, FilterDictionaryAgentPerson filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => new FrontContactPersons
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    CompanyId = x.AgentCompanyId ?? -1,
                    FirstName = x.Agent.AgentPeople.FirstName,
                    LastName = x.Agent.AgentPeople.LastName,
                    MiddleName = x.Agent.AgentPeople.MiddleName,
                    //IsMale = x.Agent.AgentPeople.IsMale,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    Position = x.Position,

                    Contacts = x.Agent.AgentContacts.Select(y => new FrontDictionaryAgentContact
                    {
                        Id = y.Id,
                        AgentId = y.AgentId,
                        ContactType = new FrontDictionaryContactType
                        {
                            Id = y.ContactType.Id,
                            SpecCode = y.ContactType.SpecCode,
                            Code = y.ContactType.Code,
                            Name = y.ContactType.Name,
                            InputMask = y.ContactType.InputMask,
                            IsActive = y.ContactType.IsActive
                        },
                        Value = y.Contact,
                        IsActive = y.IsActive,
                        Description = y.Description
                    }).ToList(),
                }).ToList();

                transaction.Complete();

                return res;
            }
        }

        public IEnumerable<FrontMainAgentPerson> GetMainAgentPersons(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                var qry = GetAgentPersonsQuery(ctx, filter as FilterDictionaryAgentPerson);

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainAgentPerson>();

                var res = qry.Select(x => new FrontMainAgentPerson
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    FullName = x.Agent.AgentPeople.FullName,
                    TaxCode = x.Agent.AgentPeople.TaxCode,
                    IsMale = x.Agent.AgentPeople.IsMale,
                    BirthDate = x.Agent.AgentPeople.BirthDate,
                    Position = x.Position,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetAgentPersonIDs(IContext ctx, IBaseFilter filter, UISorting sorting)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(ctx, filter as FilterDictionaryAgentPerson);

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListAgentPersons(IContext ctx, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<AutocompleteItem>();

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    Details = new List<string> { x.People.TaxCode ?? string.Empty },
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDictionaryAgentPerson> GetInternalAgentPersons(IContext ctx, FilterDictionaryAgentPerson filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryAgentPerson
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    FirstName = x.Agent.AgentPeople.FirstName,
                    LastName = x.Agent.AgentPeople.LastName,
                    MiddleName = x.Agent.AgentPeople.MiddleName,
                    TaxCode = x.Agent.AgentPeople.TaxCode,
                    IsMale = x.Agent.AgentPeople.IsMale,
                    PassportSerial = x.Agent.AgentPeople.PassportSerial,
                    PassportNumber = x.Agent.AgentPeople.PassportNumber,
                    PassportText = x.Agent.AgentPeople.PassportText,
                    PassportDate = x.Agent.AgentPeople.PassportDate,
                    BirthDate = x.Agent.AgentPeople.BirthDate,
                    Position = x.Position,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    AgentCompanyId = x.AgentCompanyId,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontAgentPerson> GetAgentPerson(IContext ctx, FilterDictionaryAgentPerson filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(ctx, filter);

                var res = qry.Select(x => new FrontAgentPerson
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    FirstName = x.Agent.AgentPeople.FirstName,
                    LastName = x.Agent.AgentPeople.LastName,
                    MiddleName = x.Agent.AgentPeople.MiddleName,
                    FullName = x.Agent.AgentPeople.FullName,
                    TaxCode = x.Agent.AgentPeople.TaxCode,
                    IsMale = x.Agent.AgentPeople.IsMale,
                    BirthDate = x.Agent.AgentPeople.BirthDate,
                    Position = x.Position,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    AgentCompanyId = x.AgentCompanyId,
                    AgentCompanyName = x.AgentCompany.Agent.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public bool ExistsAgentPersons(IContext ctx, FilterDictionaryAgentPerson filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentPersonsQuery(ctx, filter).Any();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryAgentPersons> GetAgentPersonsQuery(IContext ctx, FilterDictionaryAgentPerson filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentPersonsSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            // исключаю сотрудников из списка физлиц
            qry = qry.Where(x => x.Agent.AgentEmployee == null);

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentPersons>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Список AgentCompanyId
                if (filter.CompanyIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentPersons>(false);
                    filterContains = filter.CompanyIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentCompanyId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentPersons>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                // Поиск по полному наименованию
                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentPersons>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.AgentPeople.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentPersons>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Agent.Name == filter.NameExact);
                }

                if (!string.IsNullOrEmpty(filter.FirstNameExact))
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.FirstName == filter.FirstNameExact);
                }

                if (!string.IsNullOrEmpty(filter.LastNameExact))
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.LastName == filter.LastNameExact);
                }

                // Поиск по паспортным данным
                if (!string.IsNullOrEmpty(filter.Passport))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentPersons>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Passport).Aggregate(filterContains,
                        (current, value) => current.Or(e => (e.Agent.AgentPeople.PassportSerial + "-" + e.Agent.AgentPeople.PassportNumber + " " + e.Agent.AgentPeople.PassportDate.ToString() + " " + e.Agent.AgentPeople.PassportText) == value).Expand());

                    qry = qry.Where(filterContains);

                }

                // Поиск по ИНН
                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentPersons>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TaxCode).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.AgentPeople.TaxCode.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TaxCodeExact))
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.TaxCode == filter.TaxCodeExact);
                }

                // Поиск по дате рождения
                if (filter.BirthPeriod?.HasValue ?? false)
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.BirthDate >= filter.BirthPeriod.DateBeg);
                    qry = qry.Where(x => x.Agent.AgentPeople.BirthDate <= filter.BirthPeriod.DateEnd);
                }

                // Поиск по дате рождения
                if (filter.BirthDateExact != null)
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.BirthDate == filter.BirthDateExact);
                }

                if (!string.IsNullOrEmpty(filter.PassportSerialExact))
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.PassportSerial == filter.PassportSerialExact);
                }

                if (filter.PassportNumberExact != null)
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.PassportNumber == filter.PassportNumberExact);
                }
            }

            return qry;
        }
        private void SetSort<TSource, TKey>(IQueryable<DictionaryAgentPersons> qry, UISorting sorting)
        {
            if (sorting == null) return;

            if (sorting.SortPrimary == EnumSort.NoSorting) return;

            //System.Linq.Expression<Func<DictionaryAgentPersons, string>> keySelector = x => x.Agent.Name;

            if (sorting.SortPrimaryAsc)
                //IOrderedQueryable<DictionaryAgentPersons> o
                qry = qry.OrderBy(x => x.Agent.Name);
            else
                qry = qry.OrderByDescending(x => x.Agent.Name);

            if (sorting.SortSecondary == EnumSort.NoSorting) return;

            if (sorting.SortSecondaryAsc) qry = qry.OrderBy(x => x.Agent.Name).ThenBy(x => x.Agent.Name);
            else qry = qry.OrderByDescending(x => x.Agent.Name).ThenBy(x => x.Agent.Name);

            if (sorting.SortThird == EnumSort.NoSorting) return;

            if (sorting.SortThirdAsc) qry = qry.OrderBy(x => x.Agent.Name).ThenBy(x => x.Agent.Name).ThenBy(x => x.Agent.Name);
            else qry = qry.OrderByDescending(x => x.Agent.Name).ThenBy(x => x.Agent.Name).ThenBy(x => x.Agent.Name);

        }
        #endregion

        #region [+]Employee ...
        public int AddAgentEmployee(IContext ctx, InternalDictionaryAgentEmployee employee)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                employee.Id = AddAgentPeople(ctx, employee);
                // решили, что сотрудник и пользователь всегда создаются парой, пользователь может быть деактивирован
                AddAgentUser(ctx, new InternalDictionaryAgentUser(employee));

                var dbModel = DictionaryModelConverter.GetDbAgentEmployee(ctx, employee);
                dbContext.DictionaryAgentEmployeesSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.AddNew);
                transaction.Complete();

                return employee.Id;
            }
        }

        public void UpdateAgentEmployee(IContext ctx, InternalDictionaryAgentEmployee employee)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentPeople(ctx, employee);

                var dbModel = DictionaryModelConverter.GetDbAgentEmployee(ctx, employee);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteAgentEmployees(IContext ctx, FilterDictionaryAgentEmployee filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryAgentEmployees, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }

        public int GetAgentEmployeePersonnelNumber(IContext ctx)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var tmp = dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == ctx.Client.Id).AsEnumerable();
                var res = 0;
                // Если нен ни одно сотрудника, то начинаю нумерацию с 1
                if (!tmp.Any(x => 1 == 1))
                {
                    res = 1;
                }
                else
                {
                    res = tmp.Max(y => y.PersonnelNumber) + 1;
                }

                transaction.Complete();

                return res;
            }
        }

        public FrontAgentEmployee GetAgentEmployee(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(ctx, new FilterDictionaryAgentEmployee { IDs = new List<int> { id } });

                //qry = qry.OrderBy(x => x.Agent.Name);

                //if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontDictionaryAgentEmployee>();

                var res = qry.Select(x => new FrontAgentEmployee
                {
                    Id = x.Id,
                    ImageByteArray = x.Agent.Image,
                    Name = x.Agent.Name,
                    FullName = x.Agent.AgentPeople.FullName,
                    FirstName = x.Agent.AgentPeople.FirstName,
                    LastName = x.Agent.AgentPeople.LastName,
                    MiddleName = x.Agent.AgentPeople.MiddleName,

                    IsMale = x.Agent.AgentPeople.IsMale,
                    BirthDate = x.Agent.AgentPeople.BirthDate,

                    PersonnelNumber = x.PersonnelNumber,
                    TaxCode = x.Agent.AgentPeople.TaxCode,


                    IsActive = x.IsActive,
                    Description = x.Description,

                    //PassportDate = x.Agent.AgentPeople.PassportDate,
                    //PassportSerial = x.Agent.AgentPeople.PassportSerial,
                    //PassportNumber = x.Agent.AgentPeople.PassportNumber,
                    //PassportText = x.Agent.AgentPeople.PassportText,


                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontMainAgentEmployee> GetMainAgentEmployees(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(ctx, filter as FilterDictionaryAgentEmployee);

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainAgentEmployee>();

                var now = DateTime.UtcNow;

                var res = qry.Select(x => new FrontMainAgentEmployee
                {
                    Id = x.Id,
                    ImageByteArray = x.Agent.Image,
                    IsActive = x.IsActive,
                    Description = x.Description,
                    Name = x.Agent.Name,
                    FullName = x.Agent.AgentPeople.FullName,
                    BirthDate = x.Agent.AgentPeople.BirthDate,
                    PersonnelNumber = x.PersonnelNumber,
                    TaxCode = x.Agent.AgentPeople.TaxCode,
                    PassportDate = x.Agent.AgentPeople.PassportDate,
                    PassportSerial = x.Agent.AgentPeople.PassportSerial,
                    PassportNumber = x.Agent.AgentPeople.PassportNumber,
                    PassportText = x.Agent.AgentPeople.PassportText,
                    PositionExecutors = x.PositionExecutors
                    .Where(y => y.StartDate <= now && y.EndDate >= now && y.IsActive)
                    .OrderBy(y => y.PositionExecutorTypeId).ThenBy(y => y.Position.Order).ThenBy(y => y.Position.Name)
                    .Select(y => new FrontDictionaryPositionExecutor
                    {
                        Id = y.Id,
                        PositionName = y.Position.Name,
                        DepartmentIndex = y.Position.Department.Index,
                        DepartmentName = y.Position.Department.Name,
                        PositionExecutorTypeSuffix = y.PositionExecutorType.Suffix
                    }).ToList(),
                }).ToList();


                transaction.Complete();
                return res;
            }
        }

        public List<int> GetAgentEmployeeIDs(IContext ctx, IBaseFilter filter, UISorting sorting)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(ctx, filter as FilterDictionaryAgentEmployee);

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetAgentEmployeeList(IContext ctx, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<ListItem>();

                var res = qry.Select(x => new ListItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryAgentEmployees> GetAgentEmployeesQuery(IContext ctx, FilterDictionaryAgentEmployee filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentEmployees>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Сотрудники, у которых адреса в переданном списке Id
                if (filter.AddressIDs?.Count > 0)
                {
                    // pss Нужно найти решение: просто отказаться от переменных привязки - плохо!
                    var filterContains = PredicateBuilder.New<DictionaryAgentAddresses>(false);
                    filterContains = filter.AddressIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.Agent.AgentAddresses.AsQueryable().Any(filterContains));
                    //qry = qry.Where(x => filter.AddressIDs.Contains(x.Agent.AgentAddresses. Id));
                }

                // Сотрудники, у которых контакты в переданном списке Id
                if (filter.ContactIDs?.Count > 0)
                {
                    // pss Нужно найти решение: просто отказаться от переменных привязки - плохо!
                    var filterContains = PredicateBuilder.New<DictionaryAgentContacts>(false);
                    filterContains = filter.ContactIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.Agent.AgentContacts.AsQueryable().Any(filterContains));
                    //qry = qry.Where(x => filter.AddressIDs.Contains(x.Agent.AgentAddresses. Id));
                }

                // Сотрудники, у которых должности в переданном списке Id
                if (filter.PositionIDs?.Count > 0)
                {
                    // pss Нужно найти решение: просто отказаться от переменных привязки - плохо!
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(false);
                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(x => x.PositionExecutors.AsQueryable().Any(filterContains));
                    //qry = qry.Where(x => filter.AddressIDs.Contains(x.Agent.AgentAddresses. Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentEmployees>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                // Поиск по полному наименованию
                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentEmployees>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.AgentPeople.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentEmployees>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Agent.Name == filter.NameExact);
                }

                if (!string.IsNullOrEmpty(filter.FirstNameExact))
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.FirstName == filter.FirstNameExact);
                }

                if (!string.IsNullOrEmpty(filter.LastNameExact))
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.LastName == filter.LastNameExact);
                }

                if (filter.PersonnelNumber != null)
                {
                    qry = qry.Where(x => x.Agent.AgentEmployee.PersonnelNumber == filter.PersonnelNumber);
                }

                if (!string.IsNullOrEmpty(filter.Passport))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentEmployees>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Passport).Aggregate(filterContains,
                        (current, value) => current.Or(e => (e.Agent.AgentPeople.PassportSerial + "-" + e.Agent.AgentPeople.PassportNumber + " " +
                                          e.Agent.AgentPeople.PassportDate.ToString() + " " +
                                          e.Agent.AgentPeople.PassportText) == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentEmployees>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TaxCode).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.AgentPeople.TaxCode.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TaxCodeExact))
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.TaxCode == filter.TaxCodeExact);
                }

                // Поиск по дате рождения
                if (filter.BirthPeriod != null)
                {
                    if (filter.BirthPeriod.HasValue)
                    {
                        qry = qry.Where(x => x.Agent.AgentPeople.BirthDate >= filter.BirthPeriod.DateBeg);
                        qry = qry.Where(x => x.Agent.AgentPeople.BirthDate <= filter.BirthPeriod.DateEnd);
                    }
                }

                // Поиск по дате рождения
                if (filter.BirthDateExact != null)
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.BirthDate == filter.BirthDateExact);
                }


                if (!string.IsNullOrEmpty(filter.PassportSerialExact))
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.PassportSerial == filter.PassportSerialExact);
                }

                if (filter.PassportNumberExact != null)
                {
                    qry = qry.Where(x => x.Agent.AgentPeople.PassportNumber == filter.PassportNumberExact);
                }

                if (filter.RoleIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminUserRoles>(false);
                    filter.RoleIDs.Aggregate(filterContains, (current, value) => current.Or(e => e.RoleId == value).Expand());

                    //qry = qry.Where(x => x.PositionExecutors.UserRoles.Any(y => filter.RoleIDs.Any(RoleId => y.RoleId == RoleId)));

                    qry = qry.Where(x => x.PositionExecutors.Any(y => y.UserRoles.Any(z => filter.RoleIDs.Contains(z.RoleId))));
                }
            }

            return qry;
        }

        public bool ExistsAgentEmployees(IContext ctx, FilterDictionaryAgentEmployee filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentEmployeesQuery(ctx, filter).Any();
                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+]User ...
        public int AddAgentUser(IContext ctx, InternalDictionaryAgentUser User)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(ctx, User);
                dbContext.DictionaryAgentUsersSet.Add(dbModel);
                dbContext.SaveChanges();

                transaction.Complete();
                return User.Id;
            }
        }

        public void DeleteAgentUsers(IContext ctx, FilterDictionaryAgentUsers filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentUsersQuery(ctx, filter);
                qry.Delete();
                transaction.Complete();
            }
        }



        public void SetAgentUserLastPositionChose(IContext ctx, InternalDictionaryAgentUser User)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(ctx, User);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.LastPositionChose).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void SetAgentUserLockout(IContext ctx, InternalDictionaryAgentUser User)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(ctx, User);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.IsLockout).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void SetAgentUserUserId(IContext ctx, InternalDictionaryAgentUser User)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(ctx, User);
                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.UserId).IsModified = true;
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void SetAgentUserUserName(IContext ctx, InternalDictionaryAgentUser User)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                //var u = dbContext.DictionaryAgentUsersSet.Where(x => x.Id == User.Id).Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

                var dbModel = DictionaryModelConverter.GetDbAgentUser(ctx, User);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.UserName).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }


        public InternalDictionaryAgentUser GetInternalAgentUser(IContext ctx, int id)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentUsersQuery(ctx, new FilterDictionaryAgentUsers { IDs = new List<int> { id } });

                var res = qry.Select(x => new InternalDictionaryAgentUser
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    IsLockout = x.IsLockout,
                    UserName = x.UserName,
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryAgentUsers> GetAgentUsersQuery(IContext ctx, FilterDictionaryAgentUsers filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentUsersSet.Where(x => x.Agent.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentUsers>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentUsers>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }

        public InternalDictionaryAgentImage GetInternalAgentImage(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                // Where(x => x.ClientId == ctx.Client.Id).
                var res = dbContext.DictionaryAgentsSet.Where(x => x.Id == id).Select(x => new InternalDictionaryAgentImage
                {
                    Id = x.Id,
                    Image = x.Image
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+]Address ...
        public FrontDictionaryAgentAddress GetAgentAddress(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == ctx.Client.Id).AsQueryable();

                qry = qry.Where(x => x.Id == id);

                var res = qry.Select(x => new FrontDictionaryAgentAddress
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    AddressType = new FrontAddressType { Id = x.AdressTypeId, Name = x.AddressType.Name, IsActive = x.AddressType.IsActive },
                    PostCode = x.PostCode,
                    Address = x.Address,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public int AddAgentAddress(IContext ctx, InternalDictionaryAgentAddress addr)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAddress(ctx, addr);

                dbContext.DictionaryAgentAddressesSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.AddNew);
                addr.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateAgentAddress(IContext ctx, InternalDictionaryAgentAddress addr)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAddress(ctx, addr);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteAgentAddress(IContext ctx, FilterDictionaryAgentAddress filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAddressQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryAgentAddresses, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }



        public IEnumerable<FrontDictionaryAgentAddress> GetAgentAddresses(IContext ctx, FilterDictionaryAgentAddress filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAddressQuery(ctx, filter);

                qry = qry.OrderBy(x => x.AddressType.Code).ThenBy(x => x.Address);

                var res = qry.Select(x => new FrontDictionaryAgentAddress
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    AddressType = new FrontAddressType { Id = x.AddressType.Id, Code = x.AddressType.Code, Name = x.AddressType.Name },
                    Address = x.Address,
                    PostCode = x.PostCode,
                    Description = x.Description,
                    IsActive = x.IsActive
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryAgentAddresses> GetAddressQuery(IContext ctx, FilterDictionaryAgentAddress filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAddresses>(false);
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAddresses>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAddresses>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                if (filter.AddressTypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAddresses>(false);
                    filterContains = filter.AddressTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AdressTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.PostCode))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAddresses>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.PostCode).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PostCode.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!String.IsNullOrEmpty(filter.PostCodeExact))
                {

                    qry = qry.Where(x => x.PostCode == filter.PostCodeExact);

                }

                if (!String.IsNullOrEmpty(filter.AddressExact))
                {

                    qry = qry.Where(x => x.Address == filter.AddressExact);

                }

                if (!String.IsNullOrEmpty(filter.Address))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAddresses>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Address).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Address.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }

        #endregion

        #region [+] AddressTypes ...
        public int AddAddressType(IContext ctx, InternalDictionaryAddressType addrType)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAddressType(ctx, addrType);

                dbContext.DictionaryAddressTypesSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAddressType, EnumOperationType.AddNew);
                addrType.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateAddressType(IContext ctx, InternalDictionaryAddressType addrType)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAddressType(ctx, addrType);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                // Все поля кроме SpecCode
                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.IsActive).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAddressType, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }


        public void DeleteAddressTypes(IContext ctx, FilterDictionaryAddressType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAddressTypeQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryAddressType, EnumOperationType.Delete);
                qry.Delete();
                transaction.Complete();
            }
        }

        public bool ExistsAddressTypeSpecCode(IContext ctx, int addressTypeId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == ctx.Client.Id & x.Id == addressTypeId & x.SpecCode != null).Any();
                transaction.Complete();
                return res;
            }
        }

        public InternalDictionaryAddressType GetInternalDictionaryAddressType(IContext ctx, FilterDictionaryAddressType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAddressTypeQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryAddressType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontAddressType> GetAddressTypes(IContext ctx, FilterDictionaryAddressType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAddressTypeQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontAddressType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsActive = x.IsActive
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public string GetAddressTypeSpecCode(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryAddressTypesSet.
                    Where(x => x.ClientId == ctx.Client.Id).
                    Where(x => x.Id == id).
                    AsQueryable();

                var res = qry.Select(x => x.SpecCode).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontShortListAddressType> GetShortListAddressTypes(IContext ctx, FilterDictionaryAddressType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAddressTypeQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontShortListAddressType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SpecCode = x.SpecCode,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryAddressTypes> GetAddressTypeQuery(IContext ctx, FilterDictionaryAddressType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAddressTypes>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAddressTypes>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAddressTypes>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAddressTypes>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }



                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.CodeExact))
                {
                    qry = qry.Where(x => x.Code == filter.CodeExact);
                }
                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.CodeName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAddressTypes>(true);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.CodeName).Aggregate(filterContains,
                        (current, value) => current.And(e => (e.Code + " " + e.Name).Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }
        #endregion

        #region [+]Org ...
        public int AddAgentOrg(IContext ctx, InternalDictionaryAgentOrg org)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                if (ExistsAgents(ctx, new FilterDictionaryAgent() { IDs = new List<int> { org.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской 
                    UpdateAgentName(ctx, org.Id, org);
                }
                else
                {
                    org.Id = AddAgent(ctx, org);
                };

                DictionaryCompanies dc = DictionaryModelConverter.GetDbAgentOrg(ctx, org);
                dbContext.DictionaryAgentClientCompaniesSet.Add(dc);
                dbContext.SaveChanges();

                org.Id = dc.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dc.Id, EnumObjects.DictionaryAgentClientCompanies, EnumOperationType.AddNew);
                transaction.Complete();

                return org.Id;
            }

        }

        public void UpdateAgentOrg(IContext ctx, InternalDictionaryAgentOrg org)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentName(ctx, org.Id, org);

                var dbModel = DictionaryModelConverter.GetDbAgentOrg(ctx, org);
                dbContext.SafeAttach(dbModel);
                dbContext.Entry(dbModel).State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentClientCompanies, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteAgentOrg(IContext ctx, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryAgentClientCompanies, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }

        public List<int> GetAgentOrgIDs(IContext ctx, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(ctx, filter);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public InternalDictionaryAgentOrg GetInternalAgentOrg(IContext ctx, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryAgentOrg
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Agent.Name,
                    FullName = x.FullName,
                    Description = x.Description,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryAgentClientCompany> GetAgentOrgs(IContext ctx, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => new FrontDictionaryAgentClientCompany
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Agent.Name,
                    FullName = x.FullName,
                    Description = x.Description
                }).ToList();

                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<TreeItem> GetAgentOrgsForStaffList(IContext ctx, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                var objId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new TreeItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    SearchText = x.Agent.Name,
                    ObjectId = (int)EnumObjects.DictionaryAgentClientCompanies,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = string.Empty,
                    IsActive = x.IsActive,
                    IsLeaf = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListAgentOrgs(IContext ctx, FilterDictionaryAgentOrg filter, UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);


                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    //Details = new List<string> { },
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetAgentOrgsForDIPSubordinations(IContext ctx, int positionId, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                var objId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPSubordinationsOrg
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    SearchText = x.Agent.Name,
                    ObjectId = (int)EnumObjects.DictionaryAgentClientCompanies,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = string.Empty,
                    IsActive = x.IsActive,
                    IsLeaf = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    SourcePositionId = positionId,
                    CompanyId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetAgentOrgsForDIPJournalAccess(IContext ctx, int journalId, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                var objId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPJournalAccessOrg
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    SearchText = x.Agent.Name,
                    ObjectId = (int)EnumObjects.DictionaryAgentClientCompanies,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = string.Empty,
                    IsActive = x.IsActive,
                    IsLeaf = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    JournalId = journalId,
                    CompanyId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetAgentClientCompaniesForDIPRJournalPositions(IContext ctx, int positionId, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                var objId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPRegistrationJournalPositionsClientCompany
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    SearchText = x.Agent.Name,
                    ObjectId = (int)EnumObjects.DictionaryAgentClientCompanies,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = string.Empty,
                    IsActive = x.IsActive,
                    IsLeaf = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    PositionId = positionId,
                    CompanyId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }


        private IQueryable<DictionaryCompanies> GetAgentOrgsQuery(IContext ctx, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentClientCompaniesSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryCompanies>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryCompanies>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Только компании в который есть отделы
                if (filter.DepartmentIDs?.Count > 100)
                {
                    qry = qry.Where(x => x.Departments.Any(y => filter.DepartmentIDs.Contains(y.Id)));
                }
                else if (filter.DepartmentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryDepartments>(false);
                    filterContains = filter.DepartmentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.Departments.AsQueryable().Any(filterContains));
                }


                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryCompanies>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryCompanies>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public bool ExistsAgentClientCompanies(IContext ctx, FilterDictionaryAgentOrg filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentOrgsQuery(ctx, filter).Any();
                transaction.Complete();
                return res;
            }
        }


        #endregion

        #region [+]Companies ...
        public int AddAgentCompany(IContext ctx, InternalDictionaryAgentCompany company)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                if (ExistsAgents(ctx, new FilterDictionaryAgent() { IDs = new List<int> { company.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской 
                    UpdateAgentName(ctx, company.Id, company);
                }
                else
                {
                    company.Id = AddAgent(ctx, company);
                };

                var dbModel = DictionaryModelConverter.GetDbAgentCompany(ctx, company);

                dbContext.DictionaryAgentCompaniesSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.AddNew);
                transaction.Complete();

                return company.Id;
            }
        }

        public void UpdateAgentCompany(IContext ctx, InternalDictionaryAgentCompany company)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentName(ctx, company.Id, company);

                var dbModel = DictionaryModelConverter.GetDbAgentCompany(ctx, company);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.UpdateFull);
                transaction.Complete();

            }

        }

        public void DeleteAgentCompanies(IContext ctx, FilterDictionaryAgentCompany filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryAgentCompanies, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }


        public IEnumerable<FrontAgentCompany> GetAgentCompanies(IContext ctx, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontAgentCompany>();

                var res = qry.Select(x => new FrontAgentCompany
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Name = x.Agent.Name,
                    TaxCode = x.TaxCode,
                    OKPOCode = x.OKPOCode,
                    VATCode = x.VATCode,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<FrontMainAgentCompany> GetMainAgentCompanies(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(ctx, (filter as FilterDictionaryAgentCompany));

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainAgentCompany>();

                var res = qry.Select(x => new FrontMainAgentCompany
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Name = x.Agent.Name,
                    TaxCode = x.TaxCode,
                    OKPOCode = x.OKPOCode,
                    VATCode = x.VATCode,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    ContactsPersons = x.AgentPersons.Select(
                        y => new FrontAgentPerson
                        {
                            Name = y.Agent.Name,
                            Position = y.Position,
                        }).ToList(),
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetAgentCompanyIDs(IContext ctx, IBaseFilter filter, UISorting sortin)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(ctx, (filter as FilterDictionaryAgentCompany));

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<AutocompleteItem> GetAgentCompanyList(IContext ctx, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<AutocompleteItem>();

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    Details = new List<string> { x.OKPOCode ?? string.Empty }
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public bool ExistsAgentCompanies(IContext ctx, FilterDictionaryAgentCompany filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentCompaniesQuery(ctx, filter).Any();
                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryAgentCompanies> GetAgentCompaniesQuery(IContext ctx, FilterDictionaryAgentCompany filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentCompaniesSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentCompanies>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentCompanies>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentCompanies>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentCompanies>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentCompanies>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TaxCode).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TaxCode.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
                if (!string.IsNullOrEmpty(filter.OKPOCode))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentCompanies>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.OKPOCode).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OKPOCode.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
                if (!string.IsNullOrEmpty(filter.VATCode))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentCompanies>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.VATCode).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.VATCode == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TaxCodeExact))
                {
                    qry = qry.Where(x => x.TaxCode == filter.TaxCodeExact);
                }

                if (!string.IsNullOrEmpty(filter.OKPOCodeExact))
                {
                    qry = qry.Where(x => x.OKPOCode == filter.OKPOCodeExact);
                }

                if (!string.IsNullOrEmpty(filter.VATCodeExact))
                {
                    qry = qry.Where(x => x.VATCode == filter.VATCodeExact);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.FullName == filter.NameExact);
                }
            }

            return qry;
        }

        #endregion

        #region [+]Banks ...

        public int AddAgentBank(IContext ctx, InternalDictionaryAgentBank bank)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {

                if (ExistsAgents(ctx, new FilterDictionaryAgent() { IDs = new List<int> { bank.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской
                    UpdateAgentName(ctx, bank.Id, bank);
                }
                else
                {
                    bank.Id = AddAgent(ctx, bank);
                };


                var dbModel = DictionaryModelConverter.GetDbAgentBank(ctx, bank);

                dbContext.DictionaryAgentBanksSet.Add(dbModel);
                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                transaction.Complete();

                return bank.Id;
            }
        }

        public void UpdateAgentBank(IContext ctx, InternalDictionaryAgentBank bank)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentName(ctx, bank.Id, bank);

                var dbModel = DictionaryModelConverter.GetDbAgentBank(ctx, bank);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteAgentBank(IContext ctx, FilterDictionaryAgentBank filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentBanksQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryAgentBanks, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }


        public IEnumerable<FrontAgentBank> GetAgentBanks(IContext ctx, FilterDictionaryAgentBank filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentBanksQuery(ctx, filter);

                var res = qry.Select(x => new FrontAgentBank
                {
                    Id = x.Id,
                    MFOCode = x.MFOCode,
                    Swift = x.Swift,
                    Name = x.Agent.Name,
                    FullName = x.FullName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontMainAgentBank> GetMainAgentBanks(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sortin)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentBanksQuery(ctx, (filter as FilterDictionaryAgentBank));

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainAgentBank>();

                var res = qry.Select(x => new FrontMainAgentBank
                {
                    Id = x.Id,
                    MFOCode = x.MFOCode,
                    Swift = x.Swift,
                    Name = x.Agent.Name,
                    FullName = x.FullName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetAgentBankIDs(IContext ctx, IBaseFilter filter, UISorting sorting)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentBanksQuery(ctx, (filter as FilterDictionaryAgentBank));

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListAgentBanks(IContext ctx, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentBanksQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<AutocompleteItem>();

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    Details = new List<string> { x.MFOCode ?? string.Empty },
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public bool ExistsAgentBanks(IContext ctx, FilterDictionaryAgentBank filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentBanksQuery(ctx, filter).Any();
                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryAgentBanks> GetAgentBanksQuery(IContext ctx, FilterDictionaryAgentBank filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentBanksSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentBanks>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentBanks>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentBanks>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Agent.Name == filter.NameExact);
                }

                if (!string.IsNullOrEmpty(filter.MFOCode))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentBanks>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.MFOCode).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.MFOCode.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.MFOCodeExact))
                {
                    qry = qry.Where(x => x.MFOCode == filter.MFOCodeExact);
                }
            }

            return qry;
        }

        #endregion

        #region [+]Accounts ...
        public FrontDictionaryAgentAccount GetAgentAccount(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == ctx.Client.Id).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentAccount
                {
                    Id = x.Id,
                    AccountNumber = x.AccountNumber,
                    Name = x.Name,
                    IsMain = x.IsMain,
                    AgentId = x.AgentId,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    Bank = new FrontMainAgentBank
                    {
                        Id = x.AgentBank.Id,
                        MFOCode = x.AgentBank.MFOCode,
                        Swift = x.AgentBank.Swift,
                        Name = x.AgentBank.Agent.Name
                    }

                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public int AddAgentAccount(IContext ctx, InternalDictionaryAgentAccount account)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAccount(ctx, account);

                dbContext.DictionaryAgentAccountsSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.AddNew);
                transaction.Complete();
                return account.Id;
            }
        }

        public void UpdateAgentAccount(IContext ctx, InternalDictionaryAgentAccount account)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAccount(ctx, account);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteAgentAccounts(IContext ctx, FilterDictionaryAgentAccount filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentAccountsQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryAgentAccounts, EnumOperationType.Delete);
                qry.Delete();
                transaction.Complete();
            }
        }



        public IEnumerable<InternalDictionaryAgentAccount> GetInternalAgentAccounts(IContext ctx, FilterDictionaryAgentAccount filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentAccountsQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryAgentAccount
                {
                    Id = x.Id,
                    AccountNumber = x.AccountNumber,
                    Name = x.Name,
                    IsMain = x.IsMain,
                    AgentId = x.AgentId,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryAgentAccount> GetAgentAccounts(IContext ctx, FilterDictionaryAgentAccount filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentAccountsQuery(ctx, filter);

                var res = qry.Select(x => new FrontDictionaryAgentAccount
                {
                    Id = x.Id,
                    AccountNumber = x.AccountNumber,
                    Name = x.Name,
                    IsMain = x.IsMain,
                    AgentId = x.AgentId,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    Bank = new FrontMainAgentBank
                    {
                        Id = x.AgentBank.Id,
                        MFOCode = x.AgentBank.MFOCode,
                        Swift = x.AgentBank.Swift,
                        Name = x.AgentBank.Agent.Name
                    }

                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryAgentAccounts> GetAgentAccountsQuery(IContext ctx, FilterDictionaryAgentAccount filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAccounts>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAccounts>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAccounts>(false);
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAccounts>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.AccountNumber))
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentAccounts>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.AccountNumber).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AccountNumber == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.AccountNumberExact))
                {
                    qry = qry.Where(x => x.AccountNumber == filter.AccountNumberExact);
                }



                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }
            }

            return qry;
        }

        public bool ExistsAgentAccounts(IContext ctx, FilterDictionaryAgentAccount filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentAccountsQuery(ctx, filter);

                var res = qry.Any();

                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+] ContactTypes ...
        public int AddContactType(IContext ctx, InternalDictionaryContactType model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContactType(ctx, model);

                dbContext.DictionaryContactTypesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryContactType, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }
        public void UpdateContactType(IContext ctx, InternalDictionaryContactType model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContactType(ctx, model);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                // Все поля кроме SpecCode
                entity.Property(x => x.InputMask).IsModified = true;
                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.IsActive).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryContactType, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }
        public void DeleteContactType(IContext ctx, FilterDictionaryContactType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactTypeQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryContactType, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }
        public bool ExistsContactTypeSpecCode(IContext ctx, int contactTypeId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == ctx.Client.Id & x.Id == contactTypeId & x.SpecCode != null).Any();
                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<FrontDictionaryContactType> GetContactTypes(IContext ctx, FilterDictionaryContactType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactTypeQuery(ctx, filter);

                var res = qry.Select(x => new FrontDictionaryContactType
                {
                    Id = x.Id,
                    Name = x.Name,
                    InputMask = x.InputMask,
                    Code = x.Code,
                    SpecCode = x.SpecCode,
                    IsActive = x.IsActive
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public FrontDictionaryContactType GetInternalDictionaryContactType(IContext ctx, FilterDictionaryContactType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactTypeQuery(ctx, filter);

                var res = qry.Select(x => new FrontDictionaryContactType
                {
                    Id = x.Id,
                    Name = x.Name,
                    InputMask = x.InputMask,
                    Code = x.Code,
                    SpecCode = x.SpecCode,
                    IsActive = x.IsActive
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<FrontShortListContactType> GetShortListContactTypes(IContext ctx, FilterDictionaryContactType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactTypeQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Id).ThenBy(x => x.Name);

                var res = qry.Select(x => new FrontShortListContactType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SpecCode = x.SpecCode
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        public string GetContactTypeSpecCode(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryContactTypesSet.
                    Where(x => x.ClientId == ctx.Client.Id).
                    Where(x => x.Id == id).
                    AsQueryable();

                var res = qry.Select(x => x.SpecCode).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryContactTypes> GetContactTypeQuery(IContext ctx, FilterDictionaryContactType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryContactTypes>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryContactTypes>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryContactTypes>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!String.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.New<DictionaryContactTypes>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!String.IsNullOrEmpty(filter.CodeExact))
                {
                    qry = qry.Where(x => x.Code == filter.CodeExact);
                }

                if (!string.IsNullOrEmpty(filter.CodeName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryContactTypes>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.CodeName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value) || e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

            }

            return qry;
        }
        #endregion

        #region [+]Contacts ...

        public int AddContact(IContext ctx, InternalDictionaryContact contact)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContact(ctx, contact);

                dbContext.DictionaryAgentContactsSet.Add(dbModel);
                dbContext.SaveChanges();
                contact.Id = dbModel.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryContacts, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateContact(IContext ctx, InternalDictionaryContact contact)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContact(ctx, contact);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryContacts, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }
        public void DeleteAgentContacts(IContext ctx, FilterDictionaryContact filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactsQuery(ctx, filter);

                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryContacts, EnumOperationType.Delete);

                qry.Delete();

                transaction.Complete();
            }
        }


        public IEnumerable<FrontDictionaryAgentContact> GetContacts(IContext ctx, FilterDictionaryContact filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.ContactType.Id).ThenBy(x => x.Contact);

                var res = qry.Select(x => new FrontDictionaryAgentContact
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    ContactType = new FrontDictionaryContactType
                    {
                        Id = x.ContactTypeId,
                        Name = x.ContactType.Name,
                        Code = x.ContactType.Code,
                        SpecCode = x.ContactType.SpecCode,
                        IsActive = x.ContactType.IsActive
                    },
                    Value = x.Contact,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    IsConfirmed = x.IsConfirmed
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDictionaryContact> GetInternalContacts(IContext ctx, FilterDictionaryContact filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactsQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryContact
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    Value = x.Contact,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    ContactTypeId = x.ContactTypeId,
                    IsConfirmed = x.IsConfirmed,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryAgentContacts> GetContactsQuery(IContext ctx, FilterDictionaryContact filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentContacts>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentContacts>(false);
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsAgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentContacts>(true);
                    filterContains = filter.NotContainsAgentIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.AgentId != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ContactTypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentContacts>(false);
                    filterContains = filter.ContactTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ContactTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (!string.IsNullOrEmpty(filter.Contact))
                {
                    string searchExpression = filter.Contact.Replace('-', ' ').Replace('(', ' ').Replace(')', ' ');

                    var filterContains = PredicateBuilder.New<DictionaryAgentContacts>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(searchExpression).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Contact.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!String.IsNullOrEmpty(filter.ContactExact))
                {
                    qry = qry.Where(x =>
                    x.Contact.Replace("-", "").Replace(")", "").Replace("(", "").Replace("+", "").Replace(" ", "") ==
                    filter.ContactExact.Replace("-", "").Replace(")", "").Replace("(", "").Replace("+", "").Replace(" ", ""));
                }

                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                if (filter.IsConfirmed != null)
                {
                    qry = qry.Where(x => x.IsConfirmed == filter.IsConfirmed);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentContacts>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public IEnumerable<int> GetAgentsIDByContacts(IContext ctx, IEnumerable<int> contacts)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == ctx.Client.Id).AsQueryable();

                if (contacts.Any())
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentContacts>(false);
                    filterContains = contacts.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                var res = qry.Select(x => x.AgentId).ToList();

                transaction.Complete();
                return res;
            }
        }

        public int GetContactsTypeId(IContext ctx, EnumContactTypes type)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryContactTypesSet.
                    Where(x => x.ClientId == ctx.Client.Id).
                    Where(x => x.SpecCode == type.ToString()).
                    AsQueryable();

                var res = qry.Select(x => x.Id).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] Departments ...
        public int AddDepartment(IContext ctx, InternalDictionaryDepartment department)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dd = DictionaryModelConverter.GetDbDepartments(ctx, department);
                dbContext.DictionaryDepartmentsSet.Add(dd);
                dbContext.SaveChanges();

                department.Id = dd.Id;
                CommonQueries.AddFullTextCacheInfo(ctx, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.AddNew);
                transaction.Complete();
                return dd.Id;
            }
        }

        public void UpdateDepartment(IContext ctx, InternalDictionaryDepartment department)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dd = DictionaryModelConverter.GetDbDepartments(ctx, department);
                dbContext.SafeAttach(dd);
                dbContext.Entry(dd).State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void UpdateDepartmentCode(IContext ctx, string codePreffix, string pathPrefix, FilterDictionaryDepartment filter)
        {
            if (string.IsNullOrEmpty(codePreffix)) return;

            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                qry.Update(x => new DictionaryDepartments { Code = codePreffix + "/" + x.Index, Path = pathPrefix });

                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryDepartments, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteDepartments(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryDepartments, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }

        public IEnumerable<InternalDictionaryDepartment> GetInternalDepartments(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryDepartment
                {
                    Id = x.Id,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Path = x.Path,
                    Code = x.Code,
                    Index = x.Index,
                    Name = x.Name,
                    FullName = x.FullName,
                    CompanyId = x.CompanyId,
                    ChiefPositionId = x.ChiefPositionId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryDepartment> GetDepartments(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Index).ThenBy(x => x.Name);

                var res = qry.Select(x => new FrontDictionaryDepartment
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Code = x.Code,
                    Index = x.Index,
                    Name = x.Name,
                    FullName = x.FullName,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Agent.Name,
                    ChiefPositionId = x.ChiefPositionId,
                    ChiefPositionName = x.ChiefPosition.Name,
                    ParentDepartmentName = x.ParentDepartment.Name
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetDepartmentIDs(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public string GetDepartmentPrefix(IContext ctx, int parentId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                string res = "";

                int? id = parentId;

                while (id != null)
                {

                    var qry = GetDepartmentsQuery(ctx, new FilterDictionaryDepartment() { IDs = new List<int> { id ?? 0 } });
                    var item = qry.Select(x => new FrontDictionaryDepartment() { Id = x.Id, ParentId = x.ParentId, Code = x.Code }).FirstOrDefault();

                    if (item == null) break;

                    res = item.Code + "/" + res;
                    id = item.ParentId;
                }

                transaction.Complete();

                return res;

            }
        }

        public IEnumerable<FrontDictionaryDepartmentTreeItem> GetDepartmentsForStaffList(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Index).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDictionaryDepartmentTreeItem
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SearchText = x.Name,
                    CompanyId = x.CompanyId,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsLeaf = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.Positions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryDepartmentTreeItem> GetDepartmentsForRegistrationJournals(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);



                qry = qry.OrderBy(x => x.Index).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDictionaryDepartmentTreeItem
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SearchText = x.Name,
                    CompanyId = x.CompanyId,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsLeaf = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.RegistrationJournals.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListDepartments(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Index).ThenBy(x => x.Name);

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Code + " " + x.Name,
                    Details = new List<string>
                    {
                        x.ParentDepartment.Code +" " + x.ParentDepartment.Name,
                        x.Company.Agent.Name,
                        x.ChiefPosition.Name ?? string.Empty
                    },
                }).ToList();

                transaction.Complete();
                return res;
            }

        }

        public IEnumerable<TreeItem> GetDepartmentsTree(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Index).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new TreeItem
                {
                    Id = x.Id,
                    Name = x.Code + " " + x.Name,
                    SearchText = x.Code + " " + x.Name,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetDepartmentsForDIPSubordinations(IContext ctx, int sourcePositionId, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Index).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPSubordinationsDepartment
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SearchText = x.Code + " " + x.Name,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsLeaf = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.Positions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    SourcePositionId = sourcePositionId,
                    DepartmentId = x.Id,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }



        public IEnumerable<TreeItem> GetDepartmentsForDIPJournalAccess(IContext ctx, int journalId, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Index).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPJournalAccessDepartment
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SearchText = x.Code + " " + x.Name,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsLeaf = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.Positions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    JournalId = journalId,
                    DepartmentId = x.Id,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetDepartmentsForDIPRJournalPositions(IContext ctx, int positionId, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Index).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPRegistrationJournalPositionsDepartment
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SearchText = x.Name,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsLeaf = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.RegistrationJournals.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    PositionId = positionId,
                    DepartmentId = x.Id,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsDictionaryDepartment(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(ctx, filter);

                var res = qry.Select(x => new FrontDictionaryDepartment { Id = x.Id }).Any();
                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryDepartments> GetDepartmentsQuery(IContext ctx, FilterDictionaryDepartment filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryDepartments>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryDepartments>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Отбор по родительским элементам
                if (filter.ParentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryDepartments>(false);
                    filterContains = filter.ParentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ParentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                // Условие по Name
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryDepartments>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

                // Условие по FullName
                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryDepartments>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по Code
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.New<DictionaryDepartments>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Index.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по CompanyId
                if (filter.CompanyIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryDepartments>(false);
                    filterContains = filter.CompanyIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.CompanyId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Список первичных ключей
                if (filter.JournalIDs?.Count > 100)
                {
                    qry = qry.Where(x => x.RegistrationJournals.Any(y => filter.JournalIDs.Contains(y.Id)));
                }
                else if (filter.JournalIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryRegistrationJournals>(false);
                    filterContains = filter.JournalIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.RegistrationJournals.AsQueryable().Any(filterContains));
                }


                // Условие по ChiefPositionId
                if (filter.ChiefPositionId != null)
                {
                    qry = qry.Where(x => filter.ChiefPositionId == x.ChiefPositionId);
                }

                if (filter.ExcludeDepartmentsWithoutJournals ?? false)
                {
                    // только отделы с журналами
                    qry = qry.Where(x => x.RegistrationJournals.Any());
                }

                if (filter.ExcludeDepartmentsWithoutPositions ?? false)
                {
                    // только отделы с должностями
                    qry = qry.Where(x => x.Positions.Any());
                }
            }
            return qry;
        }
        #endregion

        #region [+] DocumentDirections ...

        public IEnumerable<FrontDictionaryDocumentDirection> GetDocumentDirections(IContext ctx, FilterDictionaryDocumentDirection filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentDirectionQuery(ctx, filter);

                var module = Labels.GetEnumName<EnumDocumentDirections>();

                var res = qry.Select(x => new FrontDictionaryDocumentDirection
                {
                    Id = x.Id,
                    Code = ((EnumDocumentDirections)x.Id).ToString(),
                    Name = Labels.FirstSigns + module + Labels.Delimiter + ((EnumDocumentDirections)x.Id).ToString() + Labels.LastSigns,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryDocumentDirections> GetDocumentDirectionQuery(IContext ctx, FilterDictionaryDocumentDirection filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryDocumentDirectionsSet.AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryDocumentDirections>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryDocumentDirections>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по Name
                //if (!string.IsNullOrEmpty(filter.Name))
                //{
                //    var filterContains = PredicateBuilder.New<DictionaryDocumentDirections>(false);
                //    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                //        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                //    qry = qry.Where(filterContains);
                //}

                //if (!string.IsNullOrEmpty(filter.NameExact))
                //{
                //    qry = qry.Where(x => x.Name == filter.NameExact);
                //}

                // Условие по Code
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.New<DictionaryDocumentDirections>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.CodeExact))
                {
                    qry = qry.Where(x => x.Code == filter.CodeExact);
                }
            }
            return qry;
        }
        #endregion DictionaryDepartments

        #region [+] DocumentTypes ...
        public int AddDocumentType(IContext ctx, InternalDictionaryDocumentType docType)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbDocumentType(ctx, docType);
                dbContext.DictionaryDocumentTypesSet.Add(dbModel);
                dbContext.SaveChanges();

                docType.Id = dbModel.Id;
                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateDocumentType(IContext ctx, InternalDictionaryDocumentType docType)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbDocumentType(ctx, docType);
                dbContext.SafeAttach(dbModel);
                dbContext.Entry(dbModel).State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteDocumentType(IContext ctx, FilterDictionaryDocumentType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentTypesQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryDocumentType, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }

        public IEnumerable<InternalDictionaryDocumentType> GetInternalDictionaryDocumentTypes(IContext ctx, FilterDictionaryDocumentType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentTypesQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryDocumentType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryDocumentType> GetMainDocumentTypes(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sortin)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentTypesQuery(ctx, filter as FilterDictionaryDocumentType);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontDictionaryDocumentType>();

                var res = qry.Select(x => new FrontDictionaryDocumentType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetDocumentTypeIDs(IContext ctx, IBaseFilter filter, UISorting sortin)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentTypesQuery(ctx, filter as FilterDictionaryDocumentType);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetShortListDocumentTypes(IContext ctx, FilterDictionaryDocumentType filter, UIPaging paging)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentTypesQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<ListItem>();

                var res = qry.Select(x => new ListItem
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryDocumentTypes> GetDocumentTypesQuery(IContext ctx, FilterDictionaryDocumentType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryDocumentTypes>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryDocumentTypes>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryDocumentTypes>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }
            }

            return qry;
        }
        #endregion

        #region [+] EventTypes ...
        public IEnumerable<FrontDictionaryEventType> GetEventTypes(IContext ctx, FilterDictionaryEventType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryEventTypesSet.AsQueryable();
                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 100)
                    {
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
                    }
                    else if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryEventTypes>(false);
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryEventTypes>(true);
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    //if (!string.IsNullOrEmpty(filter.Name))
                    //{
                    //    var filterContains = PredicateBuilder.New<DictionaryEventTypes>(false);
                    //    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                    //        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    //    qry = qry.Where(filterContains);
                    //}

                    if (filter.ImportanceEventTypeIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryEventTypes>(false);
                        filterContains = filter.ImportanceEventTypeIDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.ImportanceEventTypeId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.DocumentIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DBModel.Document.DocumentEvents>(false);
                        filterContains = filter.DocumentIDs.Aggregate(filterContains, (current, value) => current.Or(e => e.DocumentId == value).Expand());

                        qry = qry.Where(x => CommonQueries.GetDocumentEventQuery(ctx, null)
                                                .Where(filterContains).Select(y => y.EventTypeId).Contains(x.Id));
                    }
                }
                var et = Labels.GetEnumName<EnumEventTypes>();
                var iet = Labels.GetEnumName<EnumImportanceEventTypes>();

                var res = qry.Select(x => new FrontDictionaryEventType
                {
                    EventType = (EnumEventTypes)x.Id,
                    Id = x.Id,
                    Code = x.Id.ToString(),
                    Name = Labels.FirstSigns + et + Labels.Delimiter + x.Id.ToString() + Labels.LastSigns,
                    ImportanceEventTypeId = x.ImportanceEventTypeId,
                    ImportanceEventTypeName = Labels.FirstSigns + iet + Labels.Delimiter + x.ImportanceEventTypeId.ToString() + Labels.LastSigns
                }).ToList();
                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] ImportanceEventTypes ...
        public IEnumerable<FrontDictionaryImportanceEventType> GetImportanceEventTypes(IContext ctx, FilterDictionaryImportanceEventType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryImportanceEventTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 100)
                    {
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
                    }
                    else if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryImportanceEventTypes>(false);
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryImportanceEventTypes>(true);
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    //if (!string.IsNullOrEmpty(filter.Name))
                    //{
                    //    var filterContains = PredicateBuilder.New<DictionaryImportanceEventTypes>(false);
                    //    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                    //        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    //    qry = qry.Where(filterContains);
                    //}

                    if (filter.DocumentIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DBModel.Document.DocumentEvents>(false);
                        filterContains = filter.DocumentIDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.DocumentId == value).Expand());
                        qry = qry.Where(x => CommonQueries.GetDocumentEventQuery(ctx, null)
                                                .Where(filterContains).Select(y => y.EventType.ImportanceEventTypeId).Contains(x.Id));
                    }
                }

                var iet = Labels.GetEnumName<EnumImportanceEventTypes>();

                var res = qry.Select(x => new FrontDictionaryImportanceEventType
                {
                    Id = x.Id,
                    Code = x.Id.ToString(),
                    Name = Labels.FirstSigns + iet + Labels.Delimiter + x.Id.ToString() + Labels.LastSigns,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] LinkTypes ...
        public IEnumerable<FrontDictionaryLinkType> GetLinkTypes(IContext ctx, FilterDictionaryLinkType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryLinkTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 100)
                    {
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
                    }
                    else if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryLinkTypes>(false);
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryLinkTypes>(true);
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    //if (!string.IsNullOrEmpty(filter.Name))
                    //{
                    //    var filterContains = PredicateBuilder.New<DictionaryLinkTypes>(false);
                    //    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                    //        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    //    qry = qry.Where(filterContains);
                    //}
                }

                var module = Labels.GetEnumName<EnumLinkTypes>();

                var res = qry.Select(x => new FrontDictionaryLinkType
                {
                    Id = x.Id,
                    Name = Labels.FirstSigns + module + Labels.Delimiter + ((EnumLinkTypes)x.Id).ToString() + Labels.LastSigns,
                    IsImportant = x.IsImportant,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] Positions ...
        public int AddPosition(IContext ctx, InternalDictionaryPosition position)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dd = DictionaryModelConverter.GetDbPosition(ctx, position);
                dbContext.DictionaryPositionsSet.Add(dd);
                dbContext.SaveChanges();
                UpdateExecutorsInPositions(ctx, new List<int> { dd.Id });
                position.Id = dd.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dd.Id, EnumObjects.DictionaryPositions, EnumOperationType.AddNew);
                transaction.Complete();
                return dd.Id;
            }
        }

        public void UpdatePosition(IContext ctx, InternalDictionaryPosition position)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbPosition(ctx, position);
                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                //pss нельзя модифицировать поля, которые проставляет вертушка
                //dbContext.Entry(dd).State = EntityState.Modified;

                entity.Property(x => x.ParentId).IsModified = true;
                entity.Property(x => x.IsActive).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.FullName).IsModified = true;
                entity.Property(x => x.DepartmentId).IsModified = true;
                entity.Property(x => x.Order).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryPositions, EnumOperationType.UpdateFull);

                UpdateExecutorsInPositions(ctx, new List<int> { position.Id });

                transaction.Complete();
            }
        }

        public void UpdatePositionOrder(IContext ctx, int positionId, int order)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbPosition(ctx, new InternalDictionaryPosition() { Id = positionId, Order = order });
                //dbContext.DictionaryPositionsSet.Where(x => x.Id == positionId).Update(x => new DictionaryPositions { Order = order });
                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Order).IsModified = true;

                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryPositions, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void UpdateExecutorsInPositions(IContext ctx, List<int> positionId = null)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.Client.Id).Where(x => x.Id >= 0);
                if (positionId?.Any() ?? false)
                    qry = qry.Where(x => positionId.Contains(x.Id));
                var posUpd = qry.Select(x => new
                {
                    x.Id,
                    oldExecutors = new { ExecutorAgentId = x.ExecutorAgentId, x.PositionExecutorTypeId, MainExecutorAgentId = x.MainExecutorAgentId },
                    newExecutors = dbContext.DictionaryPositionExecutorsSet
                        .Where(y => y.PositionId == x.Id && DateTime.UtcNow >= y.StartDate && DateTime.UtcNow <= y.EndDate
                                    && (y.PositionExecutorTypeId == (int)EnumPositionExecutionTypes.IO || y.PositionExecutorTypeId == (int)EnumPositionExecutionTypes.Personal))
                        .GroupBy(y => y.PositionId)
                        .Select(y => new
                        {
                            Executor = y.OrderByDescending(z => z.PositionExecutorTypeId).Select(z => new { z.AgentId, z.PositionExecutorTypeId }).FirstOrDefault(),
                            MainExecutor = y.OrderBy(z => z.PositionExecutorTypeId).Select(z => new { z.AgentId, z.PositionExecutorTypeId }).FirstOrDefault(z => z.PositionExecutorTypeId == (int)EnumPositionExecutionTypes.Personal),
                        })
                        .Select(y => new
                        {
                            ExecutorAgentId = (int?)y.Executor.AgentId,
                            PositionExecutorTypeId = y.Executor.PositionExecutorTypeId == (int)EnumPositionExecutionTypes.IO ? (int?)EnumPositionExecutionTypes.IO : (int?)null,
                            MainExecutorAgentId = (int?)y.MainExecutor.AgentId
                        }).FirstOrDefault() ?? new { ExecutorAgentId = (int?)null, PositionExecutorTypeId = (int?)null, MainExecutorAgentId = (int?)null }
                }).Where(x => x.newExecutors.ExecutorAgentId != x.oldExecutors.ExecutorAgentId ||
                                    x.newExecutors.PositionExecutorTypeId != x.oldExecutors.PositionExecutorTypeId ||
                                    x.newExecutors.MainExecutorAgentId != x.oldExecutors.MainExecutorAgentId)
             .ToDictionary(x => x.Id, y => y.newExecutors);
                if (posUpd.Any()) foreach (var pos in posUpd)
                    {
                        var id = pos.Key;
                        var executorAgentId = pos.Value.ExecutorAgentId;
                        var positionExecutorTypeId = pos.Value.PositionExecutorTypeId;
                        var mainExecutorAgentId = pos.Value.MainExecutorAgentId;
                        dbContext.DictionaryPositionsSet.Where(x => x.Id == id).
                            Update(x => new DictionaryPositions { ExecutorAgentId = executorAgentId, PositionExecutorTypeId = positionExecutorTypeId, MainExecutorAgentId = mainExecutorAgentId });
                    }
                transaction.Complete();
            }
        }


        public void DeletePositions(IContext ctx, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryPositions, EnumOperationType.Delete);
                qry.Delete();
                transaction.Complete();
            }
        }

        public InternalDictionaryPositionExecutorForDocument GetExecutorAgentIdByPositionId(IContext ctx, int id)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                InternalDictionaryPositionExecutorForDocument res = null;
                var qry = dbContext.DictionaryPositionsSet.AsQueryable();
                if (!ctx.IsAdmin)
                {
                    qry = qry.Where(x => x.Department.Company.ClientId == ctx.Client.Id);
                }
                res = qry.Where(x => x.Id == id)
                    .Select(x => new InternalDictionaryPositionExecutorForDocument
                    {
                        PositionId = id,
                        ExecutorAgentId = x.ExecutorAgentId,
                        ExecutorTypeId = x.PositionExecutorTypeId,
                        MainExecutorAgentId = x.MainExecutorAgentId
                    }).FirstOrDefault();
                transaction.Complete();
                if (res.MainExecutorAgentId == ctx.CurrentAgentId)
                {
                    res.ExecutorAgentId = res.MainExecutorAgentId;
                    res.ExecutorTypeId = null;
                }
                return res;
            }
        }

        public FrontDictionaryPosition GetPosition(IContext ctx, int id)
        {
            //UpdatePositionExecutor(ctx);//, new List<int> { id });
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var now = DateTime.UtcNow;
                var module = Labels.GetEnumName<EnumPositionExecutionTypes>();

                var res = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.Client.Id).Where(x => x.Id == id)
                    .Select(x => new FrontDictionaryPosition
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        ParentId = x.ParentId,
                        Name = x.Name,
                        FullName = x.FullName,
                        ExecutorAgentId = x.ExecutorAgentId,
                        ExecutorAgentName = x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : null),
                        MainExecutorAgentId = x.MainExecutorAgentId,
                        MainExecutorAgentName = x.MainExecutorAgent.Name,
                        //ParentPositionName = x.ParentPosition.Name,
                        DepartmentId = x.DepartmentId,
                        DepartmentCode = x.Department.Index,
                        DepartmentCodePath = x.Department.Code,
                        DepartmentName = x.Department.Name,
                        Order = x.Order,
                        PositionExecutors = x.PositionExecutors.
                            Where(y => now > y.StartDate).
                            Where(y => now < y.EndDate).
                            Where(y => y.IsActive == true).
                            OrderBy(y => y.PositionExecutorTypeId).ThenBy(y => y.Agent.Name).
                            Select(y => new FrontDictionaryPositionExecutor
                            {
                                Id = y.Id,
                                IsActive = y.IsActive,
                                AgentId = y.Agent.Id,
                                AgentName = y.Agent.Name,
                                PositionExecutorTypeName = Labels.FirstSigns + module + Labels.Delimiter + ((EnumPositionExecutionTypes)y.PositionExecutorTypeId).ToString() + Labels.LastSigns,
                                PositionExecutorTypeId = (EnumPositionExecutionTypes)y.PositionExecutorTypeId,
                                //StartDate = y.StartDate,
                                //EndDate = y.EndDate,
                                //AccessLevelName = y.AccessLevel.Name,
                                //Description = y.Description,
                            }),
                        ChiefDepartments = x.ChiefDepartments.Select(y => new FrontDictionaryDepartment
                        {
                            Id = y.Id,
                            IsActive = y.IsActive,
                            ParentId = y.ParentId,
                            CompanyId = y.CompanyId,
                            Name = y.Name,
                            FullName = y.FullName,
                            ChiefPositionId = y.ChiefPositionId,
                            ParentDepartmentName = y.ParentDepartment.Name,
                            CompanyName = y.Company.FullName,
                            ChiefPositionName = y.ChiefPosition.Name
                        }),
                        StandartSendLists = x.StandartSendLists.Select(y => new FrontDictionaryStandartSendList
                        {
                            Id = y.Id,
                            Name = y.Name,
                            PositionId = y.PositionId,
                            PositionName = y.Position.Name
                        })

                    }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryPosition> GetPositions(IContext ctx, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

                var filterMaxSubordinationTypeContains = PredicateBuilder.New<AdminSubordinations>(false);
                if (filter?.SubordinatedPositions?.Count() > 0)
                {
                    filterMaxSubordinationTypeContains = filter.SubordinatedPositions.Aggregate(filterMaxSubordinationTypeContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value).Expand());
                }

                var qry2 = qry.Select(x => new FrontDictionaryPosition
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Name = x.Name,
                    FullName = x.FullName,
                    Order = x.Order,
                    //ParentPositionName = x.ParentPosition.Name,
                    DepartmentId = x.DepartmentId,
                    DepartmentCode = x.Department.Index,
                    DepartmentCodePath = x.Department.Code,
                    DepartmentName = x.Department.Name,
                    CompanyName = x.Department.Company.Agent.Name,
                    ExecutorAgentId = x.ExecutorAgentId,
                    ExecutorAgentName = x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : null),
                    MaxSubordinationTypeId = x.TargetPositionSubordinations.AsQueryable()
                                                        .Where(filterMaxSubordinationTypeContains)
                                                        .Max(y => y.SubordinationTypeId)
                });

                if (filter?.SubordinatedTypeId != null)
                {
                    qry2 = qry2.Where(x => x.MaxSubordinationTypeId >= filter.SubordinatedTypeId);
                }

                var res = qry2.ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDictionaryPosition> GetInternalPositions(IContext ctx, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

                var res = qry.Select(
                    x => new InternalDictionaryPosition
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FullName = x.FullName,
                        ParentId = x.ParentId,
                        DepartmentId = x.DepartmentId,
                        ExecutorAgentId = x.ExecutorAgentId,
                        MainExecutorAgentId = x.MainExecutorAgentId,
                        Order = x.Order,
                        IsActive = x.IsActive,
                        LastChangeDate = x.LastChangeDate,
                        LastChangeUserId = x.LastChangeUserId
                    }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<int> GetPositionIDs(IContext ctx, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);
                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);
                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetPositionList(IContext ctx, FilterDictionaryPosition filter, UIPaging paging)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<ListItem>();

                var res = qry.Select(x => new FrontShortListPosition
                {
                    Id = x.Id,
                    Name = x.Name,
                    CompanyName = x.Department.Company.Agent.Name,
                    DepartmentName = x.Department.Name,
                    DepartmentCodePath = x.Department.Code,
                    ExecutorName = x.ExecutorAgent.Name,
                    ExecutorTypeSuffix = x.ExecutorType.Suffix
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetPositionsTree(IContext ctx, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);
                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

                string objId = ((int)EnumObjects.DictionaryPositions).ToString();
                string parObjId = ((int)EnumObjects.DictionaryDepartments).ToString();

                var res = qry.Select(x => new TreeItem
                {
                    Id = x.Id,
                    Name = x.Name + x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? x.ExecutorType.Suffix : null),
                    SearchText = x.Name + " " + x.ExecutorAgent.Name + " " + x.ExecutorType.Suffix,
                    ObjectId = (int)EnumObjects.DictionaryPositions,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = x.DepartmentId.ToString() + "_" + parObjId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListPositionsExecutor(IContext ctx, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.ExecutorAgent.Name);

                var vacant = Labels.Get("Message", "PositionIsVacant");

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.ExecutorAgentId.HasValue ? x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : null) : vacant,
                    Details = new List<string>
                    {
                        x.Name,
                        x.Department.Code + " " + x.Department.Name,
                    },
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListPositions(IContext ctx, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                var vacant = Labels.Get("Message", "PositionIsVacant");

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Details = new List<string>
                    {
                        x.Department.Code + " " + x.Department.Name,
                        x.ExecutorAgentId.HasValue ? x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : null) : vacant
                    },
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDIPSubordinationsPosition> GetPositionsForStaffList(IContext ctx, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

                string objId = ((int)EnumObjects.DictionaryPositions).ToString();
                string parObjId = ((int)EnumObjects.DictionaryDepartments).ToString();

                var res = qry.Select(x => new FrontDIPSubordinationsPosition
                {
                    Id = x.Id,
                    Name = x.Name,
                    SearchText = x.Name,
                    ObjectId = (int)EnumObjects.DictionaryPositions,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = x.DepartmentId.ToString() + "_" + parObjId,
                    IsActive = x.IsActive,
                    IsLeaf = !(x.PositionExecutors.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),// || x.ChildPositions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                    Order = x.Order
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<FrontDIPRegistrationJournalPositions> GetPositionsForDIPRegistrationJournals(IContext ctx, int registrationJournalId, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

                var res = qry.Select(x => new FrontDIPRegistrationJournalPositions
                {
                    Id = x.Id,
                    PositionName = x.Name,
                    IsViewing = x.PositionRegistrationJournals.Where(y => y.PositionId == x.Id & y.RegJournalId == registrationJournalId & y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.View).Any(),
                    IsRegistration = x.PositionRegistrationJournals.Where(y => y.PositionId == x.Id & y.RegJournalId == registrationJournalId & y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.Registration).Any(),
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<FrontDIPSubordinationsPosition> GetPositionsForDIPSubordinations(IContext ctx, int sourcePositionId, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

                string objId = ((int)EnumObjects.DictionaryPositions).ToString();
                string parObjId = ((int)EnumObjects.DictionaryDepartments).ToString();

                var res = qry.Select(x => new FrontDIPSubordinationsPosition
                {
                    Id = x.Id,
                    Name = x.Name,
                    SearchText = string.Concat(x.Name, " ", x.ExecutorAgent.Name),
                    ObjectId = (int)EnumObjects.DictionaryPositions,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = x.DepartmentId.ToString() + "_" + parObjId,
                    IsActive = x.IsActive,
                    IsLeaf = true,
                    ExecutorName = x.ExecutorAgent.Name,
                    ExecutorTypeSuffix = x.ExecutorType.Suffix,
                    IsInforming = (x.TargetPositionSubordinations
                        .Any(y => y.TargetPositionId == x.Id && y.SourcePositionId == sourcePositionId && y.SubordinationTypeId == (int)EnumSubordinationTypes.Informing) ? 1 : 0),
                    IsExecution = (x.TargetPositionSubordinations
                        .Any(y => y.TargetPositionId == x.Id && y.SourcePositionId == sourcePositionId && y.SubordinationTypeId == (int)EnumSubordinationTypes.Execution) ? 1 : 0),
                    SourcePositionId = sourcePositionId,
                    TargetPositionId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDIPJournalAccessPosition> GetPositionsForDIPJournalAccess(IContext ctx, int journalId, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

                string objId = ((int)EnumObjects.DictionaryPositions).ToString();
                string parObjId = ((int)EnumObjects.DictionaryDepartments).ToString();

                var res = qry.Select(x => new FrontDIPJournalAccessPosition
                {
                    Id = x.Id,
                    Name = x.Name,
                    SearchText = string.Concat(x.Name, " ", x.ExecutorAgent.Name),
                    ObjectId = (int)EnumObjects.DictionaryPositions,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = x.DepartmentId.ToString() + "_" + parObjId,
                    IsActive = x.IsActive,
                    IsLeaf = true,
                    ExecutorName = x.ExecutorAgent.Name,
                    ExecutorTypeSuffix = x.ExecutorType.Suffix,
                    IsView = (x.PositionRegistrationJournals
                        .Where(y => y.PositionId == x.Id)
                        .Where(y => y.RegJournalId == journalId)
                        .Where(y => y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.View)
                        .Any() ? 1 : 0),
                    IsRegistration = (x.PositionRegistrationJournals
                        .Where(y => y.PositionId == x.Id)
                        .Where(y => y.RegJournalId == journalId)
                        .Where(y => y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.Registration)
                        .Any() ? 1 : 0),
                    JournalId = journalId,
                    PositionId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<SortPositoin> GetPositionsForSort(IContext ctx, FilterDictionaryPosition filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

                var res = qry.Select(x => new SortPositoin
                {
                    Id = x.Id,
                    OldOrder = x.Order,
                    NewOrder = x.Order
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryPositions> GetPositionsQuery(IContext ctx, FilterDictionaryPosition filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // по вышестоящим отделам
                if (filter.ParentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(false);
                    filterContains = filter.ParentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ParentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // по отделам
                if (filter.DepartmentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(false);
                    filterContains = filter.DepartmentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // по компаниям
                if (filter.CompanyIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(false);
                    filterContains = filter.CompanyIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Department.CompanyId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по IsActive
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.And(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по полному имени
                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.And(e => e.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по полному имени
                if (!string.IsNullOrEmpty(filter.NameDepartmentExecutor))
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(true);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.NameDepartmentExecutor).Aggregate(filterContains,
                        (current, value) => current.And(e => (e.Name + " " + e.Department.Code + " " + e.Department.Name + " " + e.ExecutorAgent.Name).Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filter.DocumentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DBModel.Document.DocumentEventAccesses>(false);
                    filterContains = filter.DocumentIDs.Aggregate(filterContains, (current, value) => current.Or(e => e.DocumentId == value).Expand());
                    qry = qry.Where(x => dbContext.DocumentEventAccessesSet.AsQueryable().Where(y => y.ClientId == ctx.Client.Id).Where(filterContains).Any(y => y.PositionId == x.Id));
                }

                if (filter.RoleIDs?.Count > 0)
                {
                    qry = qry.Where(x => x.PositionRoles.Any(y => filter.RoleIDs.Any(RoleId => y.RoleId == RoleId)));
                }

                if (filter.OrderMore.HasValue)
                {
                    qry = qry.Where(x => x.Order > filter.OrderMore);
                }

                if (filter.OrderLess.HasValue)
                {
                    qry = qry.Where(x => x.Order < filter.OrderLess);
                }

                // по отделам
                if (filter.Orders?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(false);
                    filterContains = filter.Orders.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Order == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IsHideVacated ?? false)
                {
                    qry = qry.Where(x => x.ExecutorAgentId.HasValue);
                }
            }

            return qry;
        }

        #endregion

        #region [+] PositionExecutors ...
        public int AddExecutor(IContext ctx, InternalDictionaryPositionExecutor executor)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                DictionaryPositionExecutors dc = DictionaryModelConverter.GetDbExecutor(ctx, executor);
                dbContext.DictionaryPositionExecutorsSet.Add(dc);
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(ctx, dc.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.AddNew);
                executor.Id = dc.Id;
                UpdateExecutorsInPositions(ctx, new List<int> { dc.PositionId });
                _cacheService.RefreshKey(ctx, SettingConstants.DICT_POSITION_EXECUTOR_CASHE_KEY);
                transaction.Complete();
                return dc.Id;
            }
        }

        public void UpdateExecutor(IContext ctx, InternalDictionaryPositionExecutor executor)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                DictionaryPositionExecutors drj = DictionaryModelConverter.GetDbExecutor(ctx, executor);
                dbContext.SafeAttach(drj);
                dbContext.Entry(drj).State = EntityState.Modified;
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(ctx, drj.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.UpdateFull);
                UpdateExecutorsInPositions(ctx, new List<int> { executor.PositionId });
                _cacheService.RefreshKey(ctx, SettingConstants.DICT_POSITION_EXECUTOR_CASHE_KEY);
                transaction.Complete();
            }

        }


        public void DeleteExecutors(IContext ctx, FilterDictionaryPositionExecutor filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(ctx, filter);
                var list = qry.Select(x => x.Id).ToList();
                qry.Delete();

                UpdateExecutorsInPositions(ctx, list);
                CommonQueries.AddFullTextCacheInfo(ctx, list, EnumObjects.DictionaryPositionExecutors, EnumOperationType.Delete);
                _cacheService.RefreshKey(ctx, SettingConstants.DICT_POSITION_EXECUTOR_CASHE_KEY);
                transaction.Complete();
            }

        }

        public IEnumerable<FrontDictionaryPositionExecutor> GetPositionExecutors(IContext ctx, FilterDictionaryPositionExecutor filter, EnumSortPositionExecutors sort = EnumSortPositionExecutors.StartDate_PositionExecutorType)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(ctx, filter);

                switch (sort)
                {
                    case EnumSortPositionExecutors.StartDate_PositionExecutorType:
                        // DMS-367 qry = qry.OrderBy(x => x.Position.Order).ThenBy(x => x.PositionExecutorType.Id).ThenBy(x => x.Agent.Name);
                        qry = qry.OrderByDescending(x => x.StartDate).ThenBy(x => x.PositionExecutorTypeId);
                        break;
                    case EnumSortPositionExecutors.PositionExecutorType_ExecutorName:
                        qry = qry.OrderBy(x => x.PositionExecutorTypeId).ThenBy(x => x.Agent.Name);
                        break;
                }


                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);
                var al = Labels.GetEnumName<EnumAccessLevels>();
                var pet = Labels.GetEnumName<EnumPositionExecutionTypes>();

                var res = qry.Select(x => new FrontDictionaryPositionExecutor
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    AgentId = x.AgentId,
                    PositionId = x.PositionId,
                    PositionExecutorTypeId = (EnumPositionExecutionTypes)x.PositionExecutorTypeId,
                    AccessLevelId = (EnumAccessLevels)x.AccessLevelId,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate > maxDateTime ? (DateTime?)null : x.EndDate,
                    AgentName = x.Agent.Name,
                    PositionName = x.Position.Name,
                    PositionFullName = x.Position.FullName,
                    DepartmentIndex = x.Position.Department.Index,
                    DepartmentName = x.Position.Department.Name,
                    AccessLevelName = Labels.FirstSigns + al + Labels.Delimiter + ((EnumAccessLevels)x.AccessLevelId).ToString() + Labels.LastSigns,
                    PositionExecutorTypeName = Labels.FirstSigns + pet + Labels.Delimiter + ((EnumPositionExecutionTypes)x.PositionExecutorTypeId).ToString() + Labels.LastSigns,
                    PositionExecutorTypeSuffix = x.PositionExecutorType.Suffix,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDictionaryPositionExecutor> GetInternalPositionExecutors(IContext ctx, FilterDictionaryPositionExecutor filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Position.Order).ThenBy(x => x.PositionExecutorTypeId).ThenBy(x => x.Agent.Name);

                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);

                var res = qry.Select(x => new InternalDictionaryPositionExecutor
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    AgentId = x.AgentId,
                    PositionId = x.PositionId,
                    PositionExecutorTypeId = x.PositionExecutorTypeId,
                    AccessLevelId = x.AccessLevelId,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate > maxDateTime ? (DateTime?)null : x.EndDate,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetPositionExecutorsIDs(IContext ctx, FilterDictionaryPositionExecutor filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(ctx, filter);
                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<AutocompleteItem> GetShortListPositionExecutors(IContext ctx, FilterDictionaryPositionExecutor filter, UIPaging paging)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<AutocompleteItem>();

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.PositionId,
                    Name = x.Agent.Name + (x.PositionExecutorType.Suffix != null ? " (" + x.PositionExecutorType.Suffix + ")" : null),
                    Details = new List<string> { x.Position.Name ?? string.Empty, x.Position.Department.Code + " " + x.Position.Department.Name },
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<TreeItem> GetPositionExecutorsForTree(IContext ctx, FilterDictionaryPositionExecutor filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Position.Order).ThenBy(x => x.PositionExecutorTypeId).ThenBy(x => x.Agent.Name);

                string objId = ((int)EnumObjects.DictionaryPositionExecutors).ToString();
                string parObjId = ((int)EnumObjects.DictionaryPositions).ToString();

                var res = qry.Select(x => new TreeItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    SearchText = x.Agent.Name,
                    ObjectId = (int)EnumObjects.DictionaryPositionExecutors,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = x.PositionId.ToString() + "_" + parObjId,
                    IsActive = x.IsActive,
                    IsLeaf = true,
                    Description = ((EnumPositionExecutionTypes)x.PositionExecutorType.Id).ToString()
                }).ToList();

                transaction.Complete();
                return res;
            }
        }



        public IEnumerable<FrontDictionaryPositionExecutorExtended> GetPositionExecutorsDIPUserRoles(IContext ctx, FilterDictionaryPositionExecutor filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Position.Order).ThenBy(x => x.PositionExecutorTypeId).ThenBy(x => x.Agent.Name);

                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);

                var res = qry.Select(x => new FrontDictionaryPositionExecutorExtended
                {
                    AssignmentId = x.Id,
                    IsActive = x.IsActive,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate > maxDateTime ? (DateTime?)null : x.EndDate,
                    PositionName = x.Position.Name,
                    PositionExecutorSuffix = x.Position.ExecutorType.Suffix,
                    ExecutorName = x.Position.ExecutorAgent.Name,
                    DepartmentCodeName = x.Position.Department.Code + " " + x.Position.Department.Name,
                    PositionRoles = x.Position.PositionRoles
                        .Where(y => y.PositionId == x.PositionId)
                        .OrderBy(y => y.Role.Name)
                        .Select(y => new FrontAdminPositionRole
                        {
                            Id = y.RoleId,
                            RoleName = y.Role.Name,
                            RoleId = y.RoleId,
                        }).ToList()

                }).ToList();

                transaction.Complete();
                return res;

            }
        }

        private IQueryable<DictionaryPositionExecutors> GetPositionExecutorsQuery(IContext ctx, FilterDictionaryPositionExecutor filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryPositionExecutorsSet.Where(x => x.Position.Department.Company.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(false);
                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filter.AgentIDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.AgentIDs.Contains(x.AgentId));
                }
                else if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(false);
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка сотрудников
                if (filter.NotContainsAgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(true);
                    filterContains = filter.NotContainsAgentIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.AgentId != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка должностей
                if (filter.NotContainsPositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(true);
                    filterContains = filter.NotContainsPositionIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.PositionId != value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filter.PositionExecutorTypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(false);
                    filterContains = filter.PositionExecutorTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionExecutorTypeId == (int)value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                //if (filter.StartDate.HasValue & filter.EndDate.HasValue)
                //{
                //    qry = qry.Where(x =>
                //    x.StartDate <= filter.EndDate && x.EndDate >= filter.StartDate);
                //}

                if (filter.StartDate.HasValue)
                {
                    qry = qry.Where(x => x.EndDate >= filter.StartDate);
                }

                if (filter.EndDate.HasValue)
                {
                    qry = qry.Where(x => x.StartDate <= filter.EndDate);
                }

                if (filter.ExistExecutorAgentInPositions.HasValue && filter.ExistExecutorAgentInPositions.Value)
                {
                    qry = qry.Where(x => x.Position.ExecutorAgentId.HasValue);
                }

                if (filter.AccessLevelIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutors>(false);
                    filterContains = filter.AccessLevelIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AccessLevelId == (int)value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsPositionExecutor(IContext ctx, FilterDictionaryPositionExecutor filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(ctx, filter);

                var res = qry.Any();

                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+] PositionExecutorTypes ...
        public IEnumerable<InternalDictionaryPositionExecutorType> GetInternalDictionaryPositionExecutorType(IContext ctx, FilterDictionaryPositionExecutorType filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorTypesQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryPositionExecutorType
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    //                    Code = ((EnumPositionExecutionTypes)x.Id).ToString(),
                    //                    Name = x.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryPositionExecutorType> GetPositionExecutorTypes(IContext ctx, FilterDictionaryPositionExecutorType filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorTypesQuery(ctx, filter);

                var pet = Labels.GetEnumName<EnumPositionExecutionTypes>();

                var res = qry.Select(x => new FrontDictionaryPositionExecutorType
                {
                    Id = x.Id,
                    Code = ((EnumPositionExecutionTypes)x.Id).ToString(),
                    Name = Labels.FirstSigns + pet + Labels.Delimiter + ((EnumPositionExecutionTypes)x.Id).ToString() + Labels.LastSigns,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryPositionExecutorTypes> GetPositionExecutorTypesQuery(IContext ctx, FilterDictionaryPositionExecutorType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryPositionExecutorTypesSet.AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutorTypes>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositionExecutorTypes>(true);

                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                // Поиск по наименованию
                //if (!string.IsNullOrEmpty(filter.Name))
                //{
                //    var filterContains = PredicateBuilder.New<DictionaryPositionExecutorTypes>(false);
                //    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                //        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                //    qry = qry.Where(filterContains);
                //}

                // Поиск по наименованию
                //if (!string.IsNullOrEmpty(filter.Code))
                //{
                //    var filterContains = PredicateBuilder.New<DictionaryPositionExecutorTypes>(false);
                //    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                //        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                //    qry = qry.Where(filterContains);
                //}

                if (filter.Period?.HasValue == true && filter.PositionId != null)
                {

                    // достаю всех исполнителей переданной должности в указанный срок
                    var executors = GetPositionExecutors(ctx, new FilterDictionaryPositionExecutor()
                    {
                        StartDate = filter.Period.DateBeg,
                        EndDate = filter.Period.DateEnd,
                        PositionIDs = new List<int> { filter.PositionId ?? -1 }
                    });

                    if (filter.NotContainsIDs == null) filter.NotContainsIDs = new List<int>();

                    // вычитаю Personal и IO если они уже есть
                    filter.NotContainsIDs.AddRange(executors.Where(x => x.PositionExecutorTypeId == EnumPositionExecutionTypes.Personal || x.PositionExecutorTypeId == EnumPositionExecutionTypes.IO).Select(x => (int)x.PositionExecutorTypeId));

                }

            }
            return qry;
        }

        #endregion

        #region [+] RegistrationJournals ...
        public int AddRegistrationJournal(IContext ctx, InternalDictionaryRegistrationJournal regJournal)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbRegistrationJournal(ctx, regJournal);
                dbContext.DictionaryRegistrationJournalsSet.Add(dbModel);
                dbContext.SaveChanges();
                regJournal.Id = dbModel.Id;
                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateRegistrationJournal(IContext ctx, InternalDictionaryRegistrationJournal regJournal)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbRegistrationJournal(ctx, regJournal);
                dbContext.SafeAttach(dbModel);
                dbContext.Entry(dbModel).State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteRegistrationJournals(IContext ctx, FilterDictionaryRegistrationJournal filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }

        public IEnumerable<InternalDictionaryRegistrationJournal> GetInternalRegistrationJournals(IContext ctx, FilterDictionaryRegistrationJournal filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                var incom = EnumDocumentDirections.Incoming.GetHashCode().ToString();
                var outcom = EnumDocumentDirections.Outcoming.GetHashCode().ToString();
                var intern = EnumDocumentDirections.Internal.GetHashCode().ToString();

                var res = qry.Select(x => new InternalDictionaryRegistrationJournal
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Name,
                    DepartmentId = x.DepartmentId,
                    Index = x.Index,
                    NumerationPrefixFormula = x.NumerationPrefixFormula,
                    PrefixFormula = x.PrefixFormula,
                    SuffixFormula = x.SuffixFormula,
                    IsIncoming = x.DirectionCodes.Contains(incom),
                    IsOutcoming = x.DirectionCodes.Contains(outcom),
                    IsInternal = x.DirectionCodes.Contains(intern),
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                transaction.Complete();
                return res;
            }
        }



        public IEnumerable<FrontDictionaryRegistrationJournal> GetRegistrationJournals(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(ctx, filter as FilterDictionaryRegistrationJournal);

                qry = qry.OrderBy(x => x.Index).ThenBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontDictionaryRegistrationJournal>();

                var incom = EnumDocumentDirections.Incoming.GetHashCode().ToString();
                var outcom = EnumDocumentDirections.Outcoming.GetHashCode().ToString();
                var intern = EnumDocumentDirections.Internal.GetHashCode().ToString();

                var res = qry.Select(x => new FrontDictionaryRegistrationJournal
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Name,
                    DepartmentId = x.DepartmentId,
                    Index = x.Index,
                    PrefixFormula = x.PrefixFormula,
                    SuffixFormula = x.SuffixFormula,
                    NumerationPrefixFormula = x.NumerationPrefixFormula,
                    IsIncoming = x.DirectionCodes.Contains(incom),
                    IsOutcoming = x.DirectionCodes.Contains(outcom),
                    IsInternal = x.DirectionCodes.Contains(intern),
                    DepartmentName = x.Department.Name
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetRegistrationJournalIDs(IContext ctx, IBaseFilter filter, UISorting sorting)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(ctx, filter as FilterDictionaryRegistrationJournal);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        // Использовался для дерева журналов, как конечне элементы
        public IEnumerable<TreeItem> GetRegistrationJournalsForRegistrationJournals(IContext ctx, FilterDictionaryRegistrationJournal filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                string objId = ((int)EnumObjects.DictionaryRegistrationJournals).ToString();
                string parObjId = ((int)EnumObjects.DictionaryDepartments).ToString();

                var res = qry.Select(x => new TreeItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    SearchText = x.Name,
                    ObjectId = (int)EnumObjects.DictionaryRegistrationJournals,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = x.DepartmentId.ToString() + "_" + parObjId,
                    IsActive = x.IsActive,
                    IsLeaf = true,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListRegistrationJournals(IContext ctx, FilterDictionaryRegistrationJournal filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Index + " " + x.Name,
                    Details = new List<string> { x.Department.Index + " " + x.Department.Name }
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetRegistrationJournalsForDIPRJournalPositions(IContext ctx, int positionId, FilterDictionaryRegistrationJournal filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                string objId = ((int)EnumObjects.DictionaryRegistrationJournals).ToString();
                string parObjId = ((int)EnumObjects.DictionaryDepartments).ToString();

                var res = qry.Select(x => new FrontDIPRegistrationJournalPositionsJournal
                {
                    Id = x.Id,
                    Name = x.Name,
                    SearchText = string.Concat(x.Name, " ", x.Index),
                    ObjectId = (int)EnumObjects.DictionaryRegistrationJournals,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = x.DepartmentId.ToString() + "_" + parObjId,
                    IsActive = x.IsActive,
                    IsLeaf = true,
                    Index = x.Index,
                    IsViewing = (x.PositionsRegistrationJournal
                        .Where(y => y.RegJournalId == x.Id)
                        .Where(y => y.PositionId == positionId)
                        .Where(y => y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.View)
                        .Any() ? 1 : 0),
                    IsRegistration = (x.PositionsRegistrationJournal
                        .Where(y => y.RegJournalId == x.Id)
                        .Where(y => y.PositionId == positionId)
                        .Where(y => y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.Registration)
                        .Any() ? 1 : 0),
                    PositionId = positionId,
                    RegistrationJournalId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsDictionaryRegistrationJournal(IContext ctx, FilterDictionaryRegistrationJournal filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(ctx, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryRegistrationJournals> GetRegistrationJournalsQuery(IContext ctx, FilterDictionaryRegistrationJournal filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryRegistrationJournals>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryRegistrationJournals>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryRegistrationJournals>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

                // Условие по Index
                if (!string.IsNullOrEmpty(filter.Index))
                {
                    var filterContains = PredicateBuilder.New<DictionaryRegistrationJournals>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Index).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Index == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.IndexExact))
                {
                    qry = qry.Where(x => x.Index == filter.IndexExact);
                }

                // Условие по DepartmentIDs
                if (filter.DepartmentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryRegistrationJournals>(false);
                    filterContains = filter.DepartmentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.CompanyIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryRegistrationJournals>(false);
                    filterContains = filter.CompanyIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Department.CompanyId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // журналы отдела в котором работает должность
                if (filter.DepartmentByPositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryPositions>(false);
                    filterContains = filter.DepartmentByPositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.Department.Positions.AsQueryable().Any(filterContains));
                }

                if (filter.DocumentDirection.HasValue)
                {
                    qry = qry.Where(x => x.DirectionCodes.Contains(((int)filter.DocumentDirection).ToString()));
                }

                if (filter.PositionIdsAccessForRegistration?.Count > 0)
                {
                    var filterPositionsIdList = PredicateBuilder.New<AdminRegistrationJournalPositions>(false);
                    filterPositionsIdList = filter.PositionIdsAccessForRegistration.Aggregate(filterPositionsIdList, (current, value) => current.Or(e => e.PositionId == value).Expand());
                    qry = qry.Where(x => dbContext.AdminRegistrationJournalPositionsSet
                                                .Where(filterPositionsIdList).Where(y => y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.Registration)
                                                .Select(y => y.RegJournalId).Contains(x.Id));
                }

            }

            return qry;
        }

        #endregion

        #region [+] ResultTypes ...
        public IEnumerable<FrontDictionaryResultType> GetResultTypes(IContext ctx, FilterDictionaryResultType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryResultTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 100)
                    {
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
                    }
                    else if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryResultTypes>(false);
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }
                    else
                    {
                        qry = qry.Where(x => x.Id >= 0);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryResultTypes>(true);
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }


                    // Поиск по наименованию
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.New<DictionaryResultTypes>(false);
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                        qry = qry.Where(filterContains);
                    }
                }

                var res = qry.Select(x => new FrontDictionaryResultType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsExecute = x.IsExecute,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }


        #endregion

        #region [+] SendTypes ...

        public IEnumerable<FrontDictionarySendType> GetSendTypes(IContext ctx, FilterDictionarySendType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionarySendTypesSet.AsQueryable();

                qry = qry.OrderBy(x => x.Order);

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 100)
                    {
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
                    }
                    else if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionarySendTypes>(false);
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionarySendTypes>(true);
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.New<DictionarySendTypes>(false);
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.Codes?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionarySendTypes>(false);
                        filterContains = filter.Codes.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Code == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                }

                var res = qry.Select(x => new FrontDictionarySendType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsImportant = x.IsImportant,
                    SubordinationType = (EnumSubordinationTypes)x.SubordinationTypeId,
                    SubordinationTypeName = x.SubordinationType.Name,
                    IsExternal = x.Id == (int)EnumSendTypes.SendForInformationExternal,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] StageTypes ...
        public IEnumerable<ListItem> GetStageTypes(IContext ctx, FilterDictionaryStageType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryStageTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 100)
                    {
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
                    }
                    else if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryStageTypes>(false);
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionaryStageTypes>(true);
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.New<DictionaryStageTypes>(false);
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                        qry = qry.Where(filterContains);
                    }
                }

                qry = qry.OrderBy(x => x.Code);

                var res = qry.Select(x => new ListItem
                {
                    Id = x.Id,
                    //Code = x.Code,
                    Name = x.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] StandartSendListContents ...

        private IQueryable<DictionaryStandartSendListContents> GetStandartSendListContentsQuery(IContext ctx, FilterDictionaryStandartSendListContent filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendListContents>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Список первичных ключей
                if (filter.StandartSendListId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendListContents>(false);
                    filterContains = filter.StandartSendListId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.StandartSendListId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendListContents>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.SendTypeId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendListContents>(false);
                    filterContains = filter.SendTypeId.Aggregate(filterContains,
                        (current, value) => current.Or(e => (EnumSendTypes)e.SendTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TargetPositionId.HasValue)
                {
                    qry = qry.Where(x => x.TargetPositionId == filter.TargetPositionId);
                }

                if (filter.TargetAgentId.HasValue)
                {
                    qry = qry.Where(x => x.TargetAgentId == filter.TargetAgentId);
                }

                if (!string.IsNullOrEmpty(filter.Task))
                {
                    qry = CommonFilterUtilites.GetWhereExpressions(filter.Task)
                        .Aggregate(qry, (current, task) => current.Where(x => x.Task.Contains(task)));
                }

                if (!string.IsNullOrEmpty(filter.TaskExact))
                {
                    qry = qry.Where(x => x.Task == filter.TaskExact);
                }


                if (!string.IsNullOrEmpty(filter.SendTypeName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendListContents>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.SendTypeName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SendType.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TargetPositionName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendListContents>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TargetPositionName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPosition.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TargetAgentName))
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendListContents>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TargetAgentName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetAgent.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public IEnumerable<FrontDictionaryStandartSendListContent> GetStandartSendListContents(IContext ctx, FilterDictionaryStandartSendListContent filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListContentsQuery(ctx, filter);

                var module = Labels.GetEnumName<EnumAccessLevels>();

                var res = qry.Select(x => new FrontDictionaryStandartSendListContent
                {
                    Id = x.Id,
                    StandartSendListId = x.StandartSendListId,
                    Stage = x.Stage,
                    SendTypeId = x.SendTypeId,
                    TargetPositionId = x.TargetPositionId,
                    TargetAgentId = x.TargetAgentId,
                    Task = x.Task,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevelId = x.AccessLevelId,
                    SendTypeName = x.SendType.Name,
                    TargetPositionName = x.TargetPosition.Name,

                    TargetExecutorName = x.TargetPosition.ExecutorAgent.Name ?? x.TargetAgent.Name,
                    ImageByteArray = x.TargetPosition.ExecutorAgent.Image,
                    TargetExecutorTypeSuffix = x.TargetPosition.ExecutorType.Suffix,

                    TargetDepartmentIndex = x.TargetPosition.Department.Index,
                    TargetDepartmentName = x.TargetPosition.Department.Name,


                    AccessLevelName = Labels.FirstSigns + module + Labels.Delimiter + ((EnumAccessLevels)x.AccessLevelId).ToString() + Labels.LastSigns,
                    SendTypeIsExternal = x.SendTypeId == 45
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDictionaryStandartSendListContent> GetInternalStandartSendListContents(IContext ctx, FilterDictionaryStandartSendListContent filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListContentsQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryStandartSendListContent
                {
                    Id = x.Id,
                    TargetPositionId = x.TargetPositionId,
                    TargetAgentId = x.TargetAgentId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }


        public int AddStandartSendListContent(IContext ctx, InternalDictionaryStandartSendListContent content)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendListContent(ctx, content);
                dbContext.DictionaryStandartSendListContentsSet.Add(dbModel);
                dbContext.SaveChanges();
                content.Id = dbModel.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateStandartSendListContent(IContext ctx, InternalDictionaryStandartSendListContent content)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendListContent(ctx, content);
                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteStandartSendListContents(IContext ctx, FilterDictionaryStandartSendListContent filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListContentsQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryStandartSendListContent, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }


        #endregion

        #region [+] StandartSendLists ...
        private IQueryable<DictionaryStandartSendLists> GetStandartSendListQuery(IContext ctx, FilterDictionaryStandartSendList filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendLists>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendLists>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendLists>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {

                    qry = qry.Where(x => x.Name == filter.NameExact);

                }

                if (filter.PositionIDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.PositionIDs.Contains(x.PositionId ?? 0));
                }
                else if (filter.PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendLists>(false);
                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PositionDepartmentsIDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.PositionDepartmentsIDs.Contains(x.Position.DepartmentId));
                }
                else if (filter.PositionDepartmentsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryStandartSendLists>(false);
                    filterContains = filter.PositionDepartmentsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Position.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filter.AgentId != null)
                {
                    var now = DateTime.UtcNow;
                    qry = qry.Where(x => x.Position.PositionExecutors.AsQueryable().Any(y => y.IsActive && y.StartDate < now && y.EndDate > now && y.AgentId == filter.AgentId));
                }
            }

            return qry;
        }

        public IEnumerable<FrontMainDictionaryStandartSendList> GetMainStandartSendLists(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sortin)
        {
            var sendLists = GetStandartSendLists(ctx, filter as FilterDictionaryStandartSendList, paging);

            var res = sendLists.GroupBy(x => new { x.PositionId, x.PositionName, x.PositionExecutorName, x.PositionExecutorTypeSuffix, x.DepartmentIndex, x.DepartmentName })
                 .OrderBy(x => x.Key.PositionName)
                 .Select(x => new FrontMainDictionaryStandartSendList()
                 {
                     Id = x.Key.PositionId ?? -1,
                     Name = x.Key.PositionName,
                     ExecutorName = x.Key.PositionExecutorName,
                     ExecutorTypeSuffix = x.Key.PositionExecutorTypeSuffix,
                     DepartmentIndex = x.Key.DepartmentIndex,
                     DepartmentName = x.Key.DepartmentName,
                     SendLists = x.OrderBy(y => y.Name).ToList()
                 });

            return res;

        }

        public List<int> GetStandartSendListIDs(IContext ctx, IBaseFilter filter, UISorting sortin)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListQuery(ctx, filter as FilterDictionaryStandartSendList);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }

        }

        public IEnumerable<AutocompleteItem> GetStandartSendListsShortList(IContext ctx, FilterDictionaryStandartSendList filter, UIPaging paging)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<AutocompleteItem>();

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    //Details = new List<string> { string.Empty }
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryStandartSendList> GetStandartSendLists(IContext ctx, FilterDictionaryStandartSendList filter, UIPaging paging)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontDictionaryStandartSendList>();

                var res = qry.Select(x => new FrontDictionaryStandartSendList
                {
                    Id = x.Id,
                    Name = x.Name,
                    PositionId = x.PositionId,
                    PositionName = x.Position.Name,
                    PositionExecutorName = x.Position.ExecutorAgent.Name,
                    PositionExecutorTypeSuffix = x.Position.ExecutorType.Suffix,
                    DepartmentIndex = x.Position.Department.Index,
                    DepartmentName = x.Position.Department.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public int AddStandartSendList(IContext ctx, InternalDictionaryStandartSendList model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendList(ctx, model);

                dbContext.DictionaryStandartSendListsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateStandartSendList(IContext ctx, InternalDictionaryStandartSendList model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendList(ctx, model);

                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteStandartSendList(IContext ctx, FilterDictionaryStandartSendList filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryStandartSendLists, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }


        #endregion

        #region [+] SubordinationTypes ...

        public IEnumerable<ListItem> GetSubordinationTypes(IContext ctx, FilterDictionarySubordinationType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionarySubordinationTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 100)
                    {
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
                    }
                    else if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionarySubordinationTypes>(false);
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.New<DictionarySubordinationTypes>(true);
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.New<DictionarySubordinationTypes>(false);
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                        qry = qry.Where(filterContains);
                    }
                }

                var res = qry.Select(x => new ListItem
                {
                    Id = x.Id,
                    //Code = x.Code,
                    Name = x.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+] Tags ...

        private IQueryable<DictionaryTags> GetTagsQuery(IContext ctx, FilterDictionaryTag filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryTagsSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            if (filter != null)
            {
                if (!ctx.IsAdmin)
                {
                    var filterContains = PredicateBuilder.New<DictionaryTags>(false);
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value || !e.PositionId.HasValue).Expand());

                    qry = qry.Where(filterContains);
                }

                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryTags>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryTags>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

            }

            return qry;

        }

        public IEnumerable<InternalDictionaryTag> GetInternalTags(IContext ctx, FilterDictionaryTag filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, filter);

                var res = qry.Select(x => new InternalDictionaryTag
                {
                    Id = x.Id,
                    Name = x.Name,
                    PositionId = x.PositionId,
                    Color = x.Color,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontTag> GetTag(IContext ctx, FilterDictionaryTag filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, filter);

                var res = qry.Select(x => new FrontTag
                {
                    Id = x.Id,
                    Name = x.Name,
                    Color = x.Color,
                    IsActive = x.IsActive,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetTagList(IContext ctx, FilterDictionaryTag filter, UIPaging paging)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<ListItem>();

                var res = qry.Select(x => new ListItem
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontMainTag> GetMainTags(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sortin)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, filter as FilterDictionaryTag);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontMainTag>();

                var res = qry.Select(x => new FrontMainTag
                {
                    Id = x.Id,
                    Name = x.Name,
                    PositionId = x.PositionId,
                    IsSystem = !x.PositionId.HasValue,
                    Color = x.Color,
                    PositionName = x.Position.Name,
                    IsActive = x.IsActive,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeUserName = dbContext.DictionaryAgentsSet.FirstOrDefault(y => y.Id == x.LastChangeUserId).Name,
                    DocCount = dbContext.DocumentTagsSet.Count(z => z.TagId == x.Id)
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetTagIDs(IContext ctx, IBaseFilter filter, UISorting sortin)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, filter as FilterDictionaryTag);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }
        public int DocsWithTagCount(IContext ctx, int tagId)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentTagsSet.Count(y => y.TagId == tagId);
                transaction.Complete();
                return res;
            }
        }

        public int AddTag(IContext ctx, InternalDictionaryTag model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbTag(ctx, model);

                dbContext.DictionaryTagsSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryTag, EnumOperationType.AddNew);

                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }
        public void UpdateTag(IContext ctx, InternalDictionaryTag model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbTag(ctx, model);

                dbContext.DictionaryTagsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.UpdateFull);
                transaction.Complete();
            }

        }

        public void DeleteTags(IContext ctx, FilterDictionaryTag filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryTag, EnumOperationType.Delete);
                qry.Delete();

                transaction.Complete();
            }
        }

        #endregion

        // pss перенести в AdminCore
        #region [+] AdminAccessLevels ...
        public IEnumerable<FrontAdminAccessLevel> GetAdminAccessLevels(IContext ctx, FilterAdminAccessLevel filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAccessLevelsQuery(ctx, filter);

                var module = Labels.GetEnumName<EnumAccessLevels>();

                var res = qry.Select(x => new FrontAdminAccessLevel
                {
                    Id = x.Id,
                    Code = ((EnumAccessLevels)x.Id).ToString(),
                    Name = Labels.FirstSigns + module + Labels.Delimiter + ((EnumAccessLevels)x.Id).ToString() + Labels.LastSigns,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        private IQueryable<AdminAccessLevels> GetAccessLevelsQuery(IContext ctx, FilterAdminAccessLevel filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.AdminAccessLevelsSet.AsQueryable();

            // Список первичных ключей
            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminAccessLevels>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<AdminAccessLevels>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Поиск но Code
                //if (!string.IsNullOrEmpty(filter.Code))
                //{
                //    var filterContains = PredicateBuilder.New<AdminAccessLevels>(false);
                //    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                //        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                //    qry = qry.Where(filterContains);
                //}

                //if (!string.IsNullOrEmpty(filter.CodeExact))
                //{
                //    qry = qry.Where(x => filter.Code.Equals(x.Code, StringComparison.OrdinalIgnoreCase));
                //}

            }

            return qry;
        }

        #endregion

        #region [+] CustomDictionaryTypes ...
        public int AddCustomDictionaryType(IContext ctx, InternalCustomDictionaryType model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionaryType(ctx, model);
                dbContext.CustomDictionaryTypesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.CustomDictionaryTypes, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateCustomDictionaryType(IContext ctx, InternalCustomDictionaryType model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionaryType(ctx, model);
                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.CustomDictionaryTypes, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteCustomDictionaryType(IContext ctx, FilterCustomDictionaryType filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryTypesQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.CustomDictionaryTypes, EnumOperationType.Delete);
                qry.Delete();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalCustomDictionaryType> GetInternalCustomDictionaryTypes(IContext ctx, FilterCustomDictionaryType filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryTypesQuery(ctx, filter);

                var res = qry.Select(x => new InternalCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontCustomDictionaryType> GetCustomDictionaryTypes(IContext ctx, FilterCustomDictionaryType filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryTypesQuery(ctx, filter);

                var res = qry.Select(x => new FrontCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontCustomDictionaryType> GetMainCustomDictionaryTypes(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryTypesQuery(ctx, filter as FilterCustomDictionaryType);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontCustomDictionaryType>();

                var res = qry.Select(x => new FrontCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetCustomDictionaryTypeIDs(IContext ctx, IBaseFilter filter, UISorting sorting)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryTypesQuery(ctx, filter as FilterCustomDictionaryType);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<CustomDictionaryTypes> GetCustomDictionaryTypesQuery(IContext ctx, FilterCustomDictionaryType filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == ctx.Client.Id).AsQueryable();

            // Список первичных ключей
            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<CustomDictionaryTypes>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<CustomDictionaryTypes>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Поиск но Code
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.New<CustomDictionaryTypes>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.CodeExact))
                {
                    qry = qry.Where(x => filter.Code.Equals(x.Code, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<CustomDictionaryTypes>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => filter.NameExact.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                }
            }

            return qry;
        }
        #endregion

        #region [+] CustomDictionaries ...
        public int AddCustomDictionary(IContext ctx, InternalCustomDictionary model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionary(ctx, model);
                dbContext.CustomDictionariesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.CustomDictionaries, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateCustomDictionary(IContext ctx, InternalCustomDictionary model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionary(ctx, model);
                dbContext.SafeAttach(dbModel);
                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCacheInfo(ctx, dbModel.Id, EnumObjects.CustomDictionaries, EnumOperationType.UpdateFull);
                transaction.Complete();
            }
        }

        public void DeleteCustomDictionaries(IContext ctx, FilterCustomDictionary filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryQuery(ctx, filter);
                CommonQueries.AddFullTextCacheInfo(ctx, qry.Select(x => x.Id).ToList(), EnumObjects.CustomDictionaries, EnumOperationType.Delete);
                qry.Delete();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalCustomDictionary> GetInternalCustomDictionarys(IContext ctx, FilterCustomDictionary filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryQuery(ctx, filter);

                var res = qry.Select(x => new InternalCustomDictionary
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    DictionaryTypeId = x.DictionaryTypeId
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontCustomDictionary> GetMainCustomDictionaries(IContext ctx, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryQuery(ctx, filter as FilterCustomDictionary);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontCustomDictionary>();

                var res = qry.Select(x => new FrontCustomDictionary
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    TypeId = x.DictionaryTypeId
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetCustomDictionarieIDs(IContext ctx, IBaseFilter filter, UISorting sorting)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryQuery(ctx, filter as FilterCustomDictionary);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<CustomDictionaries> GetCustomDictionaryQuery(IContext ctx, FilterCustomDictionary filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == ctx.Client.Id).AsQueryable();

            // Список первичных ключей
            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 100)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<CustomDictionaries>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TypeId.HasValue)
                {
                    qry = qry.Where(x => x.DictionaryTypeId == filter.TypeId);
                }


                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<CustomDictionaries>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                // Поиск по Code
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.New<CustomDictionaries>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.New<CustomDictionaries>(false);
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.CodeExact))
                {
                    qry = qry.Where(x => x.Code == filter.CodeExact);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

            }
            return qry;
        }

        #endregion


        #region [+] AgentFavourites ...
        public int AddAgentFavourite(IContext ctx, InternalAgentFavourite model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentFavorite(ctx, model);
                dbContext.DictionaryAgentFavoritesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void AddAgentFavourites(IContext ctx, IEnumerable<InternalAgentFavourite> list)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentFavorites(ctx, list);
                dbContext.DictionaryAgentFavoritesSet.AddRange(dbModel);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdateAgentFavourite(IContext ctx, InternalAgentFavourite model)
        {
            var dbContext = ctx.DbContext as DmsContext;
            using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentFavorite(ctx, model);
                dbContext.SafeAttach(dbModel);
                dbContext.Entry(dbModel).State = EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteAgentFavourite(IContext ctx, FilterAgentFavourite filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentFavouriteQuery(ctx, filter);
                qry.Delete();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalAgentFavourite> GetInternalAgentFavourite(IContext ctx, FilterAgentFavourite filter)
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentFavouriteQuery(ctx, filter);

                qry = qry.OrderByDescending(x => x.Date).ThenBy(x => x.Id);

                var res = qry.Select(x => new InternalAgentFavourite
                {
                    Id = x.Id,
                    Module = x.Module,
                    Feature = x.Feature,
                    ObjectId = x.ObjectId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        private IQueryable<DictionaryAgentFavorites> GetAgentFavouriteQuery(IContext ctx, FilterAgentFavourite filter)
        {
            var dbContext = ctx.DbContext as DmsContext;
            var qry = dbContext.DictionaryAgentFavoritesSet.Where(x => x.Agent.ClientId == ctx.Client.Id).AsQueryable();

            // Список первичных ключей
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentFavorites>(false);
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentFavorites>(true);
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentFavorites>(false);
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ObjectIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.New<DictionaryAgentFavorites>(false);
                    filterContains = filter.ObjectIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ObjectId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (!string.IsNullOrEmpty(filter.ModuleExact))
                {
                    qry = qry.Where(x => filter.ModuleExact.Equals(x.Module, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(filter.FeatureExact))
                {
                    qry = qry.Where(x => filter.FeatureExact.Equals(x.Feature, StringComparison.OrdinalIgnoreCase));
                }

            }
            return qry;
        }

        #endregion

        public IEnumerable<int> GetFavouriteList(IContext ctx, string module, string feature)
        {
            var list = GetInternalAgentFavourite(ctx, new FilterAgentFavourite
            {
                AgentIDs = new List<int> { ctx.CurrentAgentId },
                ModuleExact = module,
                FeatureExact = feature
            });

            return list.Select(x => x.ObjectId).ToList();
        }

    }
}