using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using BL.Database.Dictionaries.Interfaces;
using BL.Database.Helper;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FrontModel;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontMainModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
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
    public class DictionariesDbProcess : CoreDb.CoreDb, IDictionariesDbProcess
    {
        public DictionariesDbProcess()
        {
        }

        //        private TransactionScope GetTransaction() => new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted });

        #region [+]Agents ...


        //public IEnumerable<EnumDictionaryAgentTypes> GetAgentRoles(IContext context, int id)
        //{
        //    using (var dbContext = new DmsContext(context))
        //    {
        //        List<EnumDictionaryAgentTypes> list = new List<EnumDictionaryAgentTypes>();
        //        var agent = GetAgent(context, id);
        //        if (agent.IsBank) { list.Add(EnumDictionaryAgentTypes.isBank); }
        //        if (agent.IsEmployee) { list.Add(EnumDictionaryAgentTypes.isEmployee); }
        //        if (agent.IsIndividual) { list.Add(EnumDictionaryAgentTypes.isIndividual); }
        //        if (agent.IsCompany) { list.Add(EnumDictionaryAgentTypes.isCompany); }

        //        return list;
        //    }
        //}

        //public void UpdateAgentRole(IContext context, int id, EnumDictionaryAgentTypes role)
        //{
        //    using (var dbContext = new DmsContext(context))
        //    {
        //        var agent = GetAgent(context, id);
        //        var dbModel = new DictionaryAgents
        //        {
        //            ClientId = context.CurrentClientId,
        //            Id = id,
        //            Name = agent.Name,
        //            ResidentTypeId = agent.ResidentTypeId,
        //            IsBank = (role == EnumDictionaryAgentTypes.isBank ? !agent.IsBank : agent.IsBank),
        //            IsCompany = (role == EnumDictionaryAgentTypes.isCompany ? !agent.IsCompany : agent.IsCompany),
        //            IsEmployee = (role == EnumDictionaryAgentTypes.isEmployee ? !agent.IsEmployee : agent.IsEmployee),
        //            IsIndividual = (role == EnumDictionaryAgentTypes.isIndividual ? !agent.IsIndividual : agent.IsIndividual),
        //            Description = agent.Description,
        //            LastChangeDate = DateTime.UtcNow,
        //            LastChangeUserId = context.CurrentAgentId,
        //            IsActive = agent.IsActive
        //        };
        //        dbContext.DictionaryAgentsSet.Attach(dbModel);
        //        var entity = dbContext.Entry(dbModel);
        //        entity.State = System.Data.Entity.EntityState.Modified;
        //        dbContext.SaveChanges();
        //    }
        //}

        public int AddAgent(IContext context, InternalDictionaryAgent model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(context, model);

                dbContext.DictionaryAgentsSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgents, EnumOperationType.AddNew);
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateAgent(IContext context, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(context, agent);

                dbContext.DictionaryAgentsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, agent.Id, EnumObjects.DictionaryAgents, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void UpdateAgentName(IContext context, int id, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(context, agent);

                dbContext.DictionaryAgentsSet.Attach(dbModel);

                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;

                //entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, id, EnumObjects.DictionaryAgents, EnumOperationType.Update);
                transaction.Complete();
            }

        }


        public void DeleteAgent(IContext context, int agentId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                var ddt = dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == agentId);

                dbContext.DictionaryAgentAddressesSet.RemoveRange(
                dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.AgentId == agentId)
                );
                dbContext.DictionaryAgentContactsSet.RemoveRange(
                dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.AgentId == agentId)
                );
                dbContext.DictionaryAgentAccountsSet.RemoveRange(
                dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.AgentId == agentId)
                );
                dbContext.DictionaryAgentsSet.Remove(ddt);
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCashInfo(dbContext, agentId, EnumObjects.DictionaryAgents, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        /// <summary>
        /// Удаляет агента если нет выносок
        /// </summary>
        /// <param name="context"></param>
        /// <param name="agent"></param>
        public void DeleteAgentIfNoAny(IContext context, List<int> list)
        {
            foreach (int agentId in list)
            {
                if (ExistsAgentClientCompanies(context, new FilterDictionaryAgentOrg() { IDs = new List<int>() { agentId } })) return;


                if (ExistsAgentEmployees(context, new FilterDictionaryAgentEmployee() { IDs = new List<int>() { agentId } })) return;
                //if (ExistsAgentUsers(context, new FilterDictionaryAgent() { IDs = new List<int>() { agentId } })) return;
                //if (ExistsAgentPeople(context, new FilterDictionaryAgentPerson() { IDs = new List<int>() { agentId } })) return;
                if (ExistsAgentPersons(context, new FilterDictionaryAgentPerson() { IDs = new List<int>() { agentId } })) return;

                if (ExistsAgentBanks(context, new FilterDictionaryAgentBank() { IDs = new List<int>() { agentId } })) return;

                if (ExistsAgentCompanies(context, new FilterDictionaryAgentCompany() { IDs = new List<int>() { agentId } })) return;

                DeleteAgent(context, agentId);
            }
        }

        public void SetAgentImage(IContext context, InternalDictionaryAgentImage User)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentImage(context, User);
                dbContext.DictionaryAgentsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.Image).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        public IEnumerable<FrontDictionaryAgent> GetAgents(IContext context, FilterDictionaryAgent filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontDictionaryAgent>();

                var res = qry.Select(x => new FrontDictionaryAgent
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = true,
                    ResidentTypeId = x.ResidentTypeId,
                    Description = string.Empty,
                    //Contacts = x.AgentContacts.Select(y => new FrontDictionaryContact
                    //{
                    //    Id = y.Id,
                    //    AgentId = y.AgentId,
                    //    Value = y.Contact,
                    //    IsActive = y.IsActive,
                    //    Description = y.Description,
                    //    ContactType = new FrontDictionaryContactType
                    //    {
                    //        Id = y.ContactType.Id,
                    //        Name = y.ContactType.Name,
                    //        Code = y.ContactType.Code,
                    //        InputMask = y.ContactType.InputMask,
                    //        IsActive = y.ContactType.IsActive
                    //    }
                    //}),
                    //Addresses = x.AgentAddresses.Select(z => new FrontDictionaryAgentAddress
                    //{
                    //    Id = z.Id,
                    //    AgentId = z.AgentId,
                    //    AddressType = new FrontDictionaryAddressType
                    //    {
                    //        Id = z.AddressType.Id,
                    //        Name = z.AddressType.Name,
                    //        IsActive = z.AddressType.IsActive
                    //    },
                    //    PostCode = z.PostCode,
                    //    Address = z.Address,
                    //    IsActive = z.IsActive,
                    //    Description = z.Description
                    //})

                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        //TODO Как можно упроцедурить PredicateBuilder??? Нужно value передавать в func
        private System.Linq.Expressions.Expression<Func<T, bool>> GetWhereExpressionIN<T>(List<int> list, System.Linq.Expressions.Expression<Func<T, bool>> func)
        {
            var filterContains = PredicateBuilder.False<T>();
            filterContains = list.Aggregate(filterContains,
                (current, value) => current.Or(func).Expand());

            return filterContains;
        }

        public IQueryable<DictionaryAgents> GetAgentsQuery(IContext context, DmsContext dbContext, FilterDictionaryAgent filter)
        {
            var qry = dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    //var filterContains = PredicateBuilder.False<DictionaryAgents>();
                    //filterContains = filter.IDs.Aggregate(filterContains,
                    //    (current, value) => current.Or(e => e.Id == value).Expand());

                    //qry = qry.Where(GetWhereExpressionIN<DictionaryAgents>(filter.IDs, e => e.Id == value));
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAgents>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgents>();
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

        public bool ExistsAgents(IContext context, FilterDictionaryAgent filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentsQuery(context, dbContext, filter).Any();

                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+]People ...
        public int AddAgentPeople(IContext context, InternalDictionaryAgentPeople people)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                people.Id = AddAgent(context, people);

                var dbModel = DictionaryModelConverter.GetDbAgentPeople(context, people);

                dbContext.DictionaryAgentPeopleSet.Add(dbModel);
                dbContext.SaveChanges();

                transaction.Complete();

                return people.Id;
            }
        }

        public void UpdateAgentPeople(IContext context, InternalDictionaryAgentPeople people)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentName(context, people.Id, people);

                var dbModel = DictionaryModelConverter.GetDbAgentPeople(context, people);

                dbContext.DictionaryAgentPeopleSet.Attach(dbModel);
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

        public void UpdateAgentPeoplePassport(IContext context, InternalDictionaryAgentPeople people)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentName(context, people.Id, people);

                var dbModel = DictionaryModelConverter.GetDbAgentPeople(context, people);

                dbContext.DictionaryAgentPeopleSet.Attach(dbModel);
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

        public void DeleteAgentPeople(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.DictionaryAgentPeopleSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryAgentPeopleSet.Remove(ddt);
                dbContext.SaveChanges();


                DeleteAgentIfNoAny(context, new List<int>() { id });

                transaction.Complete();
            }
        }

        public FrontAgentPeoplePassport GetAgentPeoplePassport(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryAgentPeopleSet
                    .Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == id)
                    .AsQueryable();

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
        #endregion

        #region [+]Person ...
        public int AddAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                person.Id = AddAgentPeople(context, person);

                var dbModel = DictionaryModelConverter.GetDbAgentPerson(context, person);

                dbContext.DictionaryAgentPersonsSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.AddNew);
                transaction.Complete();

                return person.Id;
            }
        }

        public void UpdateAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentPeople(context, person);

                var dbModel = DictionaryModelConverter.GetDbAgentPerson(context, person);

                dbContext.DictionaryAgentPersonsSet.Attach(dbModel);
                dbContext.SaveChanges();
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.Update);
                transaction.Complete();

            }
        }

        public void DeleteAgentPerson(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.DictionaryAgentPersonsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryAgentPersonsSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, id, EnumObjects.DictionaryAgentPersons, EnumOperationType.Delete);

                DeleteAgentPeople(context, id);

                transaction.Complete();
            }
        }

        public IEnumerable<FrontContactPersons> GetAgentPersonsWithContacts(IContext context, FilterDictionaryAgentPerson filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(context, dbContext, filter);

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

        public IEnumerable<FrontMainAgentPerson> GetMainAgentPersons(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                var qry = GetAgentPersonsQuery(context, dbContext, filter as FilterDictionaryAgentPerson);

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

        public List<int> GetAgentPersonIDs(IContext context, IBaseFilter filter, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(context, dbContext, filter as FilterDictionaryAgentPerson);

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(context, dbContext, filter);

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

        public IEnumerable<InternalDictionaryAgentPerson> GetInternalAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(context, dbContext, filter);

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

        public IEnumerable<FrontAgentPerson> GetAgentPerson(IContext context, FilterDictionaryAgentPerson filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentPersonsQuery(context, dbContext, filter);

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

        public bool ExistsAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentPersonsQuery(context, dbContext, filter).Any();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryAgentPersons> GetAgentPersonsQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentPerson filter)
        {
            var qry = dbContext.DictionaryAgentPersonsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            // исключаю сотрудников из списка физлиц
            qry = qry.Where(x => x.Agent.AgentEmployee == null);

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    //var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
                    //filterContains = filter.IDs.Aggregate(filterContains,
                    //    (current, value) => current.Or(e => e.Id == value).Expand());

                    //qry = qry.Where(filterContains);

                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Список AgentCompanyId
                if (filter.CompanyIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
                    filterContains = filter.CompanyIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentCompanyId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAgentPersons>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.AgentPeople.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Passport).Aggregate(filterContains,
                        (current, value) => current.Or(e => (e.Agent.AgentPeople.PassportSerial + "-" + e.Agent.AgentPeople.PassportNumber + " " + e.Agent.AgentPeople.PassportDate.ToString() + " " + e.Agent.AgentPeople.PassportText) == value).Expand());

                    qry = qry.Where(filterContains);

                }

                // Поиск по ИНН
                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
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
        public int AddAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                employee.Id = AddAgentPeople(context, employee);
                // решили, что сотрудник и пользователь всегда создаются парой, пользователь может быть деактивирован
                AddAgentUser(context, new InternalDictionaryAgentUser(employee));

                var dbModel = DictionaryModelConverter.GetDbAgentEmployee(context, employee);
                dbContext.DictionaryAgentEmployeesSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.AddNew);
                transaction.Complete();

                return employee.Id;
            }
        }

        public void UpdateAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentPeople(context, employee);

                var dbModel = DictionaryModelConverter.GetDbAgentEmployee(context, employee);

                dbContext.DictionaryAgentEmployeesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteAgentEmployee(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                var dbModel = dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryAgentEmployeesSet.Remove(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.Delete);
                dbContext.SaveChanges();

                DeleteAgentUser(context, id);
                DeleteAgentPeople(context, id);

                transaction.Complete();
            }
        }

        public int GetAgentEmployeePersonnelNumber(IContext context)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var tmp = dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == context.CurrentClientId).AsEnumerable();
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

        public FrontAgentEmployee GetAgentEmployee(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(context, dbContext, new FilterDictionaryAgentEmployee { IDs = new List<int> { id } });

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

                    LanguageId = x.Agent.AgentUser.LanguageId,
                    LanguageCode = x.Agent.AgentUser.Language.Code,
                    LanguageName = x.Agent.AgentUser.Language.Name,
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

        public IEnumerable<FrontMainAgentEmployee> GetMainAgentEmployees(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(context, dbContext, filter as FilterDictionaryAgentEmployee);

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
                        DepartmentName = y.Position.Department.Name,
                        PositionExecutorTypeSuffix = y.PositionExecutorType.Suffix
                    }).ToList(),
                }).ToList();


                transaction.Complete();
                return res;
            }
        }

        public List<int> GetAgentEmployeeIDs(IContext context, IBaseFilter filter, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(context, dbContext, filter as FilterDictionaryAgentEmployee);

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetAgentEmployeeList(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(context, dbContext, filter);

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

        public IQueryable<DictionaryAgentEmployees> GetAgentEmployeesQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentEmployee filter)
        {
            var qry = dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    // pss Нужно найти решение: просто отказаться от переменных привязки - плохо!
                    //var filterContains = PredicateBuilder.False<DictionaryAgentEmployees>();
                    //filterContains = filter.IDs.Aggregate(filterContains,
                    //    (current, value) => current.Or(e => e.Id == value).Expand());

                    //                    qry = qry.Where(filterContains);
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }


                // Сотрудники, у которых адреса в переданном списке Id
                if (filter.AddressIDs?.Count > 0)
                {
                    // pss Нужно найти решение: просто отказаться от переменных привязки - плохо!
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
                    filterContains = filter.AddressIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.Agent.AgentAddresses.AsQueryable().Any(filterContains));
                    //qry = qry.Where(x => filter.AddressIDs.Contains(x.Agent.AgentAddresses. Id));
                }

                // Сотрудники, у которых контакты в переданном списке Id
                if (filter.ContactIDs?.Count > 0)
                {
                    // pss Нужно найти решение: просто отказаться от переменных привязки - плохо!
                    var filterContains = PredicateBuilder.False<DictionaryAgentContacts>();
                    filterContains = filter.ContactIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.Agent.AgentContacts.AsQueryable().Any(filterContains));
                    //qry = qry.Where(x => filter.AddressIDs.Contains(x.Agent.AgentAddresses. Id));
                }

                // Сотрудники, у которых должности в переданном списке Id
                if (filter.PositionIDs?.Count > 0)
                {
                    // pss Нужно найти решение: просто отказаться от переменных привязки - плохо!
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(x => x.PositionExecutors.AsQueryable().Any(filterContains));
                    //qry = qry.Where(x => filter.AddressIDs.Contains(x.Agent.AgentAddresses. Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAgentEmployees>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentEmployees>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.AgentPeople.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentEmployees>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentEmployees>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Passport).Aggregate(filterContains,
                        (current, value) => current.Or(e => (e.Agent.AgentPeople.PassportSerial + "-" + e.Agent.AgentPeople.PassportNumber + " " +
                                          e.Agent.AgentPeople.PassportDate.ToString() + " " +
                                          e.Agent.AgentPeople.PassportText) == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentEmployees>();
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
                    var filterContains = PredicateBuilder.False<AdminUserRoles>();
                    filterContains = filter.RoleIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.RoleId == value).Expand());

                    //qry = qry.Where(x => x.PositionExecutors.UserRoles.Any(y => filter.RoleIDs.Any(RoleId => y.RoleId == RoleId)));

                    qry = qry.Where(x => x.PositionExecutors.Any(y => y.UserRoles.Any(z => filter.RoleIDs.Contains(z.RoleId))));
                }
            }

            return qry;
        }

        public bool ExistsAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentEmployeesQuery(context, dbContext, filter).Any();
                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+]User ...
        public int AddAgentUser(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(context, User);
                dbContext.DictionaryAgentUsersSet.Add(dbModel);
                dbContext.SaveChanges();

                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentUsers, EnumOperationType.AddNew);
                transaction.Complete();
                return User.Id;
            }
        }

        public void UpdateAgentUser(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(context, User);

                dbContext.DictionaryAgentUsersSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentUsers, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteAgentUser(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = dbContext.DictionaryAgentUsersSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryAgentUsersSet.Remove(dbModel);
                dbContext.SaveChanges();
                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentUsers, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public void SetAgentUserLanguage(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(context, User);

                dbContext.DictionaryAgentUsersSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.LanguageId).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void SetAgentUserLastPositionChose(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(context, User);

                dbContext.DictionaryAgentUsersSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.LastPositionChose).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void SetAgentUserUserId(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                //var u = dbContext.DictionaryAgentUsersSet.Where(x => x.Id == User.Id).Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                var dbModel = DictionaryModelConverter.GetDbAgentUser(context, User);

                dbContext.DictionaryAgentUsersSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.UserId).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        public FrontDictionaryAgentUser GetAgentUser(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                // Where(x => x.ClientId == context.CurrentClientId).
                var res = dbContext.DictionaryAgentUsersSet.Where(x => x.Id == id).Select(x => new FrontDictionaryAgentUser
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId,
                    LanguageName = x.Language.Code,
                    IsActive = x.Agent.AgentEmployee.IsActive,
                    IsSendEMail = false, //TODO
                    Name = x.Agent.Name,
                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public InternalDictionaryAgentUser GetInternalAgentUser(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DictionaryAgentUsersSet.Where(x => x.Id == id).Select(x => new InternalDictionaryAgentUser
                {
                    Id = x.Id,
                    UserId = x.UserId
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public InternalDictionaryAgentImage GetInternalAgentImage(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                // Where(x => x.ClientId == context.CurrentClientId).
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
        public FrontDictionaryAgentAddress GetAgentAddress(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

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

        public void UpdateAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAddress(context, addr);

                dbContext.DictionaryAgentAddressesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteAgentAddress(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryAgentAddressesSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public int AddAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAddress(context, addr);

                dbContext.DictionaryAgentAddressesSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.AddNew);
                addr.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public IEnumerable<FrontDictionaryAgentAddress> GetAgentAddresses(IContext context, FilterDictionaryAgentAddress filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();


                qry = qry.OrderBy(x => x.AddressType.Code).ThenBy(x => x.Address);

                // Список первичных ключей
                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAgentAddresses>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
                    filterContains = filter.AddressTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AdressTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.PostCode))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Address).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Address.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

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

        public IEnumerable<int> GetAgentsIDByAddress(IContext context, IEnumerable<int> addresses)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                if (addresses.Any())
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
                    filterContains = addresses.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                var res = qry.Select(x => x.AgentId).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] AddressTypes ...
        public int AddAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAddressType(context, addrType);

                dbContext.DictionaryAddressTypesSet.Add(dbModel);
                dbContext.SaveChanges();

                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAddressType, EnumOperationType.AddNew);
                addrType.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAddressType(context, addrType);

                dbContext.DictionaryAddressTypesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                // Все поля кроме SpecCode
                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.IsActive).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAddressType, EnumOperationType.Update);
                transaction.Complete();
            }
        }


        public void DeleteAddressType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryAddressTypesSet.Remove(ddt);
                //CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAddressType, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public bool ExistsAddressTypeSpecCode(IContext context, int addressTypeId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId & x.Id == addressTypeId & x.SpecCode != null).Any();
                transaction.Complete();
                return res;
            }
        }

        public InternalDictionaryAddressType GetInternalDictionaryAddressType(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAddressTypeQuery(context, dbContext, filter);

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

        public IEnumerable<FrontAddressType> GetAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAddressTypeQuery(context, dbContext, filter);

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

        public string GetAddressTypeSpecCode(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryAddressTypesSet.
                    Where(x => x.ClientId == context.CurrentClientId).
                    Where(x => x.Id == id).
                    AsQueryable();

                var res = qry.Select(x => x.SpecCode).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontShortListAddressType> GetShortListAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAddressTypeQuery(context, dbContext, filter);

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

        private static IQueryable<DictionaryAddressTypes> GetAddressTypeQuery(IContext context, DmsContext dbContext, FilterDictionaryAddressType filter)
        {
            var qry = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAddressTypes>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAddressTypes>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAddressTypes>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAddressTypes>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.CodeExact))
                {
                    qry = qry.Where(x => x.Code == filter.CodeExact);
                }

            }

            return qry;
        }
        #endregion

        #region [+]Org ...
        public int AddAgentOrg(IContext context, InternalDictionaryAgentOrg org)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                if (ExistsAgents(context, new FilterDictionaryAgent() { IDs = new List<int> { org.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской 
                    UpdateAgentName(context, org.Id, org);
                }
                else
                {
                    org.Id = AddAgent(context, org);
                };

                DictionaryCompanies dc = DictionaryModelConverter.GetDbAgentOrg(context, org);
                dbContext.DictionaryAgentClientCompaniesSet.Add(dc);
                dbContext.SaveChanges();

                org.Id = dc.Id;

                CommonQueries.AddFullTextCashInfo(dbContext, dc.Id, EnumObjects.DictionaryAgentClientCompanies, EnumOperationType.AddNew);
                transaction.Complete();

                return org.Id;
            }

        }

        public void UpdateAgentOrg(IContext context, InternalDictionaryAgentOrg org)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentName(context, org.Id, org);

                DictionaryCompanies drj = DictionaryModelConverter.GetDbAgentOrg(context, org);
                dbContext.DictionaryAgentClientCompaniesSet.Attach(drj);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryAgentClientCompanies, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteAgentOrg(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var departments = GetDepartmentIDs(context, new FilterDictionaryDepartment() { CompanyIDs = list });

                if (departments.Count > 0) DeleteDepartments(context, departments);

                dbContext.DictionaryAgentClientCompaniesSet.
                Where(x => x.ClientId == context.CurrentClientId).
                Where(x => list.Contains(x.Id)).
                Delete();

                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryAgentClientCompanies, EnumOperationType.Delete);

                DeleteAgentIfNoAny(context, list);

                transaction.Complete();
            }
        }

        public InternalDictionaryAgentOrg GetInternalAgentOrg(IContext context, FilterDictionaryAgentOrg filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(context, dbContext, filter);

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

        public IEnumerable<FrontDictionaryAgentClientCompany> GetAgentOrgs(IContext context, FilterDictionaryAgentOrg filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(context, dbContext, filter);

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


        public IEnumerable<TreeItem> GetAgentOrgsForStaffList(IContext context, FilterDictionaryAgentOrg filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(context, dbContext, filter);

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
                    IsList = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetShortListAgentOrgs(IContext context, FilterDictionaryAgentOrg filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(context, dbContext, filter);

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
                    IsList = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetAgentOrgsForDIPSubordinations(IContext context, int positionId, FilterDictionaryAgentOrg filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(context, dbContext, filter);

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
                    IsList = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    SourcePositionId = positionId,
                    CompanyId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetAgentOrgsForDIPJournalAccess(IContext context, int journalId, FilterDictionaryAgentOrg filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(context, dbContext, filter);

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
                    IsList = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    JournalId = journalId,
                    CompanyId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetAgentClientCompaniesForDIPRJournalPositions(IContext context, int positionId, FilterDictionaryAgentOrg filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentOrgsQuery(context, dbContext, filter);

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
                    IsList = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    PositionId = positionId,
                    CompanyId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }


        private IQueryable<DictionaryCompanies> GetAgentOrgsQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentOrg filter)
        {
            var qry = dbContext.DictionaryAgentClientCompaniesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryCompanies>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryCompanies>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Только компании в который есть отделы
                if (filter.DepartmentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDepartments>();
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
                    var filterContains = PredicateBuilder.False<DictionaryCompanies>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    var filterContains = PredicateBuilder.False<DictionaryCompanies>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public bool ExistsAgentClientCompanies(IContext context, FilterDictionaryAgentOrg filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentOrgsQuery(context, dbContext, filter).Any();
                transaction.Complete();
                return res;
            }
        }


        #endregion

        #region [+]Companies ...
        public int AddAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                if (ExistsAgents(context, new FilterDictionaryAgent() { IDs = new List<int> { company.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской 
                    UpdateAgentName(context, company.Id, company);
                }
                else
                {
                    company.Id = AddAgent(context, company);
                };

                var dbModel = DictionaryModelConverter.GetDbAgentCompany(context, company);

                dbContext.DictionaryAgentCompaniesSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.AddNew);
                transaction.Complete();

                return company.Id;
            }
        }

        public void UpdateAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentName(context, company.Id, company);

                var dbModel = DictionaryModelConverter.GetDbAgentCompany(context, company);

                dbContext.DictionaryAgentCompaniesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.Update);
                transaction.Complete();

            }

        }

        public void DeleteAgentCompanies(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.DictionaryAgentCompaniesSet.
                Where(x => x.Agent.ClientId == context.CurrentClientId).
                Where(x => list.Contains(x.Id)).Delete();
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryAgentCompanies, EnumOperationType.Delete);

                DeleteAgentIfNoAny(context, list);

                transaction.Complete();
            }
        }


        public IEnumerable<FrontAgentCompany> GetAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(context, dbContext, filter);

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
        public IEnumerable<FrontMainAgentCompany> GetMainAgentCompanies(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(context, dbContext, (filter as FilterDictionaryAgentCompany));

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

        public List<int> GetAgentCompanyIDs(IContext context, IBaseFilter filter, UISorting sortin)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(context, dbContext, (filter as FilterDictionaryAgentCompany));

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<AutocompleteItem> GetAgentCompanyList(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(context, dbContext, filter);

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

        public bool ExistsAgentCompanies(IContext context, FilterDictionaryAgentCompany filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentCompaniesQuery(context, dbContext, filter).Any();
                transaction.Complete();
                return res;
            }
        }

        public IQueryable<DictionaryAgentCompanies> GetAgentCompaniesQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentCompany filter)
        {
            var qry = dbContext.DictionaryAgentCompaniesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    //var filterContains = PredicateBuilder.False<DictionaryAgentCompanies>();
                    //filterContains = filter.IDs.Aggregate(filterContains,
                    //    (current, value) => current.Or(e => e.Id == value).Expand());

                    //qry = qry.Where(filterContains);
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAgentCompanies>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentCompanies>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Agent.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentCompanies>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentCompanies>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TaxCode).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TaxCode.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
                if (!string.IsNullOrEmpty(filter.OKPOCode))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentCompanies>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.OKPOCode).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.OKPOCode.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
                if (!string.IsNullOrEmpty(filter.VATCode))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentCompanies>();
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

        public int AddAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                if (ExistsAgents(context, new FilterDictionaryAgent() { IDs = new List<int> { bank.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской
                    UpdateAgentName(context, bank.Id, bank);
                }
                else
                {
                    bank.Id = AddAgent(context, bank);
                };


                var dbModel = DictionaryModelConverter.GetDbAgentBank(context, bank);

                dbContext.DictionaryAgentBanksSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                transaction.Complete();

                return bank.Id;
            }
        }

        public void UpdateAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                UpdateAgentName(context, bank.Id, bank);

                var dbModel = DictionaryModelConverter.GetDbAgentBank(context, bank);

                dbContext.DictionaryAgentBanksSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteAgentBank(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.DictionaryAgentBanksSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryAgentBanksSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.Delete);

                DeleteAgentIfNoAny(context, new List<int>() { id });

                transaction.Complete();
            }
        }


        public FrontAgentBank GetAgentBank(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentBanksQuery(context, dbContext, new FilterDictionaryAgentBank { IDs = new List<int> { id } });

                var res = qry.Select(x => new FrontAgentBank
                {
                    Id = x.Id,
                    MFOCode = x.MFOCode,
                    Swift = x.Swift,
                    Name = x.Agent.Name,
                    FullName = x.FullName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontMainAgentBank> GetMainAgentBanks(IContext context, IBaseFilter filter, UIPaging paging, UISorting sortin)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentBanksQuery(context, dbContext, (filter as FilterDictionaryAgentBank));

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

        public List<int> GetAgentBankIDs(IContext context, IBaseFilter filter, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentBanksQuery(context, dbContext, (filter as FilterDictionaryAgentBank));

                qry = qry.OrderBy(x => x.Agent.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentBanksQuery(context, dbContext, filter);

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

        public bool ExistsAgentBanks(IContext context, FilterDictionaryAgentBank filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = GetAgentBanksQuery(context, dbContext, filter).Any();
                transaction.Complete();
                return res;
            }
        }

        public IQueryable<DictionaryAgentBanks> GetAgentBanksQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentBank filter)
        {
            var qry = dbContext.DictionaryAgentBanksSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            qry = qry.OrderBy(x => x.Agent.Name);

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    // var filterContains = PredicateBuilder.False<DictionaryAgentBanks>();
                    // filterContains = filter.IDs.Aggregate(filterContains,
                    //     (current, value) => current.Or(e => e.Id == value).Expand());

                    // qry = qry.Where(filterContains);
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAgentBanks>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentBanks>();
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentBanks>();
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
        public FrontDictionaryAgentAccount GetAgentAccount(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res =
                    dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentAccount
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

        public void UpdateAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAccount(context, account);

                dbContext.DictionaryAgentAccountsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteAgentAccount(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryAgentAccountsSet.Remove(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public int AddAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAccount(context, account);

                dbContext.DictionaryAgentAccountsSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.AddNew);
                transaction.Complete();
                return account.Id;
            }
        }

        public IEnumerable<InternalDictionaryAgentAccount> GetInternalAgentAccounts(IContext context, FilterDictionaryAgentAccount filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentAccountsQuery(context, dbContext, filter);

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

        public IEnumerable<FrontDictionaryAgentAccount> GetAgentAccounts(IContext context, FilterDictionaryAgentAccount filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentAccountsQuery(context, dbContext, filter);

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

        public IQueryable<DictionaryAgentAccounts> GetAgentAccountsQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentAccount filter)
        {
            var qry = dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    // var filterContains = PredicateBuilder.False<DictionaryAgentAccounts>();
                    // filterContains = filter.IDs.Aggregate(filterContains,
                    //     (current, value) => current.Or(e => e.Id == value).Expand());

                    // qry = qry.Where(filterContains);
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAgentAccounts>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentAccounts>();
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentAccounts>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.AccountNumber))
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentAccounts>();
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

        public bool ExistsAgentAccounts(IContext context, FilterDictionaryAgentAccount filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentAccountsQuery(context, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+] ContactTypes ...
        public int AddContactType(IContext context, InternalDictionaryContactType model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContactType(context, model);

                dbContext.DictionaryContactTypesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContactType, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }
        public void UpdateContactType(IContext context, InternalDictionaryContactType model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContactType(context, model);

                dbContext.DictionaryContactTypesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                // Все поля кроме SpecCode
                entity.Property(x => x.InputMask).IsModified = true;
                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.IsActive).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContactType, EnumOperationType.Update);
                transaction.Complete();
            }
        }
        public void DeleteContactType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryContactTypesSet.Remove(ddt);
                dbContext.SaveChanges();

                //CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContactType, EnumOperationType.Delete);
                transaction.Complete();
            }
        }
        public bool ExistsContactTypeSpecCode(IContext context, int contactTypeId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId & x.Id == contactTypeId & x.SpecCode != null).Any();
                transaction.Complete();
                return res;
            }
        }


        public IEnumerable<FrontDictionaryContactType> GetContactTypes(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactTypeQuery(context, dbContext, filter);

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

        public FrontDictionaryContactType GetInternalDictionaryContactType(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactTypeQuery(context, dbContext, filter);

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
        public IEnumerable<FrontShortListContactType> GetShortListContactTypes(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactTypeQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

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
        public string GetContactTypeSpecCode(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryContactTypesSet.
                    Where(x => x.ClientId == context.CurrentClientId).
                    Where(x => x.Id == id).
                    AsQueryable();

                var res = qry.Select(x => x.SpecCode).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryContactTypes> GetContactTypeQuery(IContext context, DmsContext dbContext, FilterDictionaryContactType filter)
        {
            var qry = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryContactTypes>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryContactTypes>();
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
                    var filterContains = PredicateBuilder.False<DictionaryContactTypes>();
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
                    var filterContains = PredicateBuilder.False<DictionaryContactTypes>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!String.IsNullOrEmpty(filter.CodeExact))
                {
                    qry = qry.Where(x => x.Code == filter.CodeExact);
                }

            }

            return qry;
        }
        #endregion

        #region [+]Contacts ...

        public int AddContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContact(context, contact);

                dbContext.DictionaryAgentContactsSet.Add(dbModel);
                dbContext.SaveChanges();
                contact.Id = dbModel.Id;

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContacts, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContact(context, contact);

                dbContext.DictionaryAgentContactsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContacts, EnumOperationType.Update);
                transaction.Complete();
            }
        }
        public void DeleteContacts(IContext context, FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactsQuery(context, dbContext, filter);

                CommonQueries.AddFullTextCashInfo(dbContext, qry.Select(x => x.Id).ToList(), EnumObjects.DictionaryContacts, EnumOperationType.Delete);

                qry.Delete();

                transaction.Complete();
            }
        }


        public IEnumerable<FrontDictionaryAgentContact> GetContacts(IContext context, FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactsQuery(context, dbContext, filter);

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

        public IEnumerable<InternalDictionaryContact> GetInternalContacts(IContext context, FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetContactsQuery(context, dbContext, filter);

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

        public IQueryable<DictionaryAgentContacts> GetContactsQuery(IContext context, DmsContext dbContext, FilterDictionaryContact filter)
        {
            var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();



            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentContacts>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentContacts>();
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsAgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAgentContacts>();
                    filterContains = filter.NotContainsAgentIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.AgentId != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.ContactTypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentContacts>();
                    filterContains = filter.ContactTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ContactTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (!string.IsNullOrEmpty(filter.Contact))
                {
                    string searchExpression = filter.Contact.Replace('-', ' ').Replace('(', ' ').Replace(')', ' ');

                    var filterContains = PredicateBuilder.False<DictionaryAgentContacts>();
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
                    var filterContains = PredicateBuilder.True<DictionaryAgentContacts>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public IEnumerable<int> GetAgentsIDByContacts(IContext context, IEnumerable<int> contacts)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                if (contacts.Any())
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentContacts>();
                    filterContains = contacts.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                var res = qry.Select(x => x.AgentId).ToList();

                transaction.Complete();
                return res;
            }
        }

        public int GetContactsTypeId(IContext context, EnumContactTypes type)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryContactTypesSet.
                    Where(x => x.ClientId == context.CurrentClientId).
                    Where(x => x.SpecCode == type.ToString()).
                    AsQueryable();

                var res = qry.Select(x => x.Id).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] Departments ...
        public int AddDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dd = DictionaryModelConverter.GetDbDepartments(context, department);
                dbContext.DictionaryDepartmentsSet.Add(dd);
                dbContext.SaveChanges();

                department.Id = dd.Id;
                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.AddNew);
                transaction.Complete();
                return dd.Id;
            }
        }

        public void UpdateDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dd = DictionaryModelConverter.GetDbDepartments(context, department);
                dbContext.DictionaryDepartmentsSet.Attach(dd);
                dbContext.Entry(dd).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void UpdateDepartmentCode(IContext context, string codePreffix, FilterDictionaryDepartment filter)
        {
            if (string.IsNullOrEmpty(codePreffix)) return;

            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                qry.Update(x => new DictionaryDepartments { FullPath = codePreffix + "/" + x.Code });

                transaction.Complete();
            }
        }

        public void DeleteDepartments(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var childDepartments = GetDepartmentIDs(context, new FilterDictionaryDepartment() { ParentIDs = list });

                if (childDepartments.Count > 0) DeleteDepartments(context, childDepartments);

                var positions = GetPositionIDs(context, new FilterDictionaryPosition() { DepartmentIDs = list });

                if (positions.Count() > 0) DeletePositions(context, positions.ToList());

                // AdminEmployeeDepartments
                dbContext.AdminEmployeeDepartmentsSet.Where(x => list.Contains(x.DepartmentId)).Delete();

                // DictionaryRegistrationJournals
                dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId && list.Contains(x.DepartmentId)).Delete();

                dbContext.DictionaryDepartmentsSet
                    .Where(x => x.Company.ClientId == context.CurrentClientId)
                    .Where(x => list.Contains(x.Id))
                    .Delete();

                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryDepartments, EnumOperationType.Delete);

                transaction.Complete();

            }
        }

        public IEnumerable<InternalDictionaryDepartment> GetInternalDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                var res = qry.Select(x => new InternalDictionaryDepartment
                {
                    Id = x.Id,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Code = x.FullPath,
                    Index = x.Code,
                    Name = x.Name,
                    FullName = x.FullName,
                    CompanyId = x.CompanyId,
                    ChiefPositionId = x.ChiefPositionId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public InternalDictionaryDepartment GetDepartment(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new InternalDictionaryDepartment
                {
                    Id = x.Id,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Code = x.FullPath,
                    Index = x.Code,
                    Name = x.Name,
                    FullName = x.FullName,
                    CompanyId = x.CompanyId,
                    ChiefPositionId = x.ChiefPositionId
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryDepartment> GetDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Code).ThenBy(x => x.Name);

                var res = qry.Select(x => new FrontDictionaryDepartment
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Code = x.FullPath,
                    Index = x.Code,
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

        public List<int> GetDepartmentIDs(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public string GetDepartmentPrefix(IContext context, int parentId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                string res = "";

                int? id = parentId;

                while (id != null)
                {

                    var qry = GetDepartmentsQuery(context, dbContext, new FilterDictionaryDepartment() { IDs = new List<int> { id ?? 0 } });
                    var item = qry.Select(x => new FrontDictionaryDepartment() { Id = x.Id, ParentId = x.ParentId, Code = x.FullPath }).FirstOrDefault();

                    if (item == null) break;

                    res = item.Code + "/" + res;
                    id = item.ParentId;
                }

                transaction.Complete();

                return res;

            }
        }

        public IEnumerable<FrontDictionaryDepartmentTreeItem> GetDepartmentsForStaffList(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Code).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDictionaryDepartmentTreeItem
                {
                    Id = x.Id,
                    Code = x.FullPath,
                    Name = x.Name,
                    SearchText = x.Name,
                    CompanyId = x.CompanyId,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsList = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.Positions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryDepartmentTreeItem> GetDepartmentsForRegistrationJournals(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);



                qry = qry.OrderBy(x => x.Code).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDictionaryDepartmentTreeItem
                {
                    Id = x.Id,
                    Code = x.FullPath,
                    Name = x.Name,
                    SearchText = x.Name,
                    CompanyId = x.CompanyId,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsList = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.RegistrationJournals.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Code).ThenBy(x => x.Name);

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.FullPath + " " + x.Name,
                    Details = new List<string>
                    {
                        x.ParentDepartment.FullPath +" " + x.ParentDepartment.Name,
                        x.Company.Agent.Name,
                        x.ChiefPosition.Name ?? string.Empty
                    },
                }).ToList();

                transaction.Complete();
                return res;
            }

        }

        public IEnumerable<TreeItem> GetDepartmentsTree(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Code).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new TreeItem
                {
                    Id = x.Id,
                    Name = x.FullPath + " " + x.Name,
                    SearchText = x.FullPath + " " + x.Name,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetDepartmentsForDIPSubordinations(IContext context, int sourcePositionId, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Code).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPSubordinationsDepartment
                {
                    Id = x.Id,
                    Code = x.FullPath,
                    Name = x.Name,
                    SearchText = x.FullPath + " " + x.Name,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsList = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.Positions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    SourcePositionId = sourcePositionId,
                    DepartmentId = x.Id,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }



        public IEnumerable<TreeItem> GetDepartmentsForDIPJournalAccess(IContext context, int journalId, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Code).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPJournalAccessDepartment
                {
                    Id = x.Id,
                    Code = x.FullPath,
                    Name = x.Name,
                    SearchText = x.FullPath + " " + x.Name,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsList = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.Positions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    JournalId = journalId,
                    DepartmentId = x.Id,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetDepartmentsForDIPRJournalPositions(IContext context, int positionId, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Code).ThenBy(x => x.Name);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPRegistrationJournalPositionsDepartment
                {
                    Id = x.Id,
                    Code = x.FullPath,
                    Name = x.Name,
                    SearchText = x.Name,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsList = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.RegistrationJournals.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),
                    PositionId = positionId,
                    DepartmentId = x.Id,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsDictionaryDepartment(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                var res = qry.Select(x => new FrontDictionaryDepartment { Id = x.Id }).Any();
                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryDepartments> GetDepartmentsQuery(IContext context, DmsContext dbContext, FilterDictionaryDepartment filter)
        {
            var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDepartments>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryDepartments>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Отбор по родительским элементам
                if (filter.ParentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDepartments>();
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
                    var filterContains = PredicateBuilder.False<DictionaryDepartments>();
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
                    var filterContains = PredicateBuilder.False<DictionaryDepartments>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по Code
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.False<DictionaryDepartments>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по CompanyId
                if (filter.CompanyIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDepartments>();
                    filterContains = filter.CompanyIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.CompanyId == value).Expand());

                    qry = qry.Where(filterContains);
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

        public IEnumerable<FrontDictionaryDocumentDirection> GetDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentDirectionQuery(context, dbContext, filter);

                var res = qry.Select(x => new FrontDictionaryDocumentDirection
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryDocumentDirections> GetDocumentDirectionQuery(IContext context, DmsContext dbContext, FilterDictionaryDocumentDirection filter)
        {
            var qry = dbContext.DictionaryDocumentDirectionsSet.AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDocumentDirections>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryDocumentDirections>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по Name
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.False<DictionaryDocumentDirections>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

                // Условие по Code
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.False<DictionaryDocumentDirections>();
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

        #region [+] DocumentSubjects ...

        public int AddDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dds = DictionaryModelConverter.GetDbDocumentSubject(context, docSubject);
                dbContext.DictionaryDocumentSubjectsSet.Add(dds);
                dbContext.SaveChanges();
                docSubject.Id = dds.Id;

                CommonQueries.AddFullTextCashInfo(dbContext, dds.Id, EnumObjects.DictionaryDocumentSubjects, EnumOperationType.AddNew);
                transaction.Complete();
                return dds.Id;
            }
        }

        public void UpdateDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {

                // Альтернативный вариант обновления записи.
                // В этом варианте на 1 обращение больше: сначала выполняется select и если вернулась строка обновляю значения, затем update
                //var d = dbContext.DictionaryDocumentSubjectsSet.FirstOrDefault(x => x.Id == docSubject.Id);
                //if (d != null)
                //{
                //    d.IsActive = docSubject.IsActive;
                //}
                //dbContext.SaveChanges();

                // В этом варианте обновления с использованием Attach предварительная проверка на существование записи НЕ выполняется, сразу Update.
                var dds = DictionaryModelConverter.GetDbDocumentSubject(context, docSubject);

                // если нужно обновить ВСЕ поля можно без перечисления сделать вот так
                //dbContext.DictionaryDocumentSubjectsSet.Attach(dds);
                //var entity = dbContext.Entry(dds);
                //entity.State = System.Data.Entity.EntityState.Modified;

                //или еще сократить
                dbContext.DictionaryDocumentSubjectsSet.Attach(dds);
                dbContext.Entry(dds).State = System.Data.Entity.EntityState.Modified;

                // При частичном обновлении полей можно так:
                //dbContext.DictionaryDocumentSubjectsSet.Attach(dds);

                //var entity = dbContext.Entry(dds);

                //entity.Property(x => x.ParentId).IsModified = true;
                //entity.Property(x => x.LastChangeDate).IsModified = true;
                //entity.Property(x => x.LastChangeUserId).IsModified = true;
                //entity.Property(x => x.IsActive).IsModified = true;
                //entity.Property(x => x.Name).IsModified = true;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dds.Id, EnumObjects.DictionaryDocumentSubjects, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteDocumentSubject(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryDocumentSubjectsSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentSubjects, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDictionaryDocumentSubject> GetInternalDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentSubjectsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new InternalDictionaryDocumentSubject
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    IsActive = x.IsActive,
                    Name = x.Name,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).ToList();

                transaction.Complete();
                return res;

            }
        }

        public IEnumerable<FrontDictionaryDocumentSubject> GetDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {

            // Устно договорились, что функция для возврата коллекции элементов возвращает всю простыню элеменов без учета родителя и потомков.
            // Если нужно каждому элементу добавить родителя и потомков это делается на уровень выше в Logic. Функция GetDocumentSubects ВСЕГДА возвращает плоскую коллекцию
            // более толго для построения иерархии на фронте плоской коллекции достаточно.

            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                //IQueryable<DictionaryDocumentSubjects> qry=

                var qry = GetDocumentSubjectsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontDictionaryDocumentSubject
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    IsActive = x.IsActive,
                    Name = x.Name
                    //ParentDocumentSubjectName = x.ParentDocumentSubject.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsDictionaryDocumentSubject(IContext context, FilterDictionaryDocumentSubject filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentSubjectsQuery(context, dbContext, filter);

                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryDocumentSubjects> GetDocumentSubjectsQuery(IContext context, DmsContext dbContext, FilterDictionaryDocumentSubject filter)
        {
            var qry = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDocumentSubjects>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryDocumentSubjects>();
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
                    var filterContains = PredicateBuilder.False<DictionaryDocumentSubjects>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

                // Условие по ParentId
                if (filter.ParentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDocumentSubjects>();
                    filterContains = filter.ParentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ParentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        #endregion

        #region [+] DocumentTypes ...
        public int AddDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbDocumentType(context, docType);
                dbContext.DictionaryDocumentTypesSet.Add(dbModel);
                dbContext.SaveChanges();

                docType.Id = dbModel.Id;
                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbDocumentType(context, docType);
                dbContext.DictionaryDocumentTypesSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteDocumentType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryDocumentTypesSet.Remove(ddt);
                dbContext.SaveChanges();

                //CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDictionaryDocumentType> GetInternalDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentTypesQuery(context, dbContext, filter);

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

        public IEnumerable<FrontDictionaryDocumentType> GetMainDocumentTypes(IContext context, IBaseFilter filter, UIPaging paging, UISorting sortin)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentTypesQuery(context, dbContext, filter as FilterDictionaryDocumentType);

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

        public List<int> GetDocumentTypeIDs(IContext context, IBaseFilter filter, UISorting sortin)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentTypesQuery(context, dbContext, filter as FilterDictionaryDocumentType);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetShortListDocumentTypes(IContext context, FilterDictionaryDocumentType filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetDocumentTypesQuery(context, dbContext, filter);

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

        private static IQueryable<DictionaryDocumentTypes> GetDocumentTypesQuery(IContext context, DmsContext dbContext, FilterDictionaryDocumentType filter)
        {
            var qry = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDocumentTypes>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryDocumentTypes>();
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
                    var filterContains = PredicateBuilder.False<DictionaryDocumentTypes>();
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
        public IEnumerable<FrontDictionaryEventType> GetEventTypes(IContext context, FilterDictionaryEventType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryEventTypesSet.AsQueryable();
                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DictionaryEventTypes>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.True<DictionaryEventTypes>();
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.False<DictionaryEventTypes>();
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.ImportanceEventTypeIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DictionaryEventTypes>();
                        filterContains = filter.ImportanceEventTypeIDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.ImportanceEventTypeId == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.DocumentIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DBModel.Document.DocumentEvents>();
                        filterContains = filter.DocumentIDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.DocumentId == value).Expand());

                        qry = qry.Where(x =>
                                dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                    .Where(filterContains).Select(y => y.EventTypeId).Contains(x.Id)
                                    );
                    }
                }
                var res = qry.Select(x => new FrontDictionaryEventType
                {
                    EventType = (EnumEventTypes)x.Id,
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ImportanceEventTypeId = x.ImportanceEventTypeId,
                    ImportanceEventTypeName = x.ImportanceEventType.Name
                }).ToList();
                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] ImportanceEventTypes ...
        public IEnumerable<FrontDictionaryImportanceEventType> GetImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryImportanceEventTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DictionaryImportanceEventTypes>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.True<DictionaryImportanceEventTypes>();
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.False<DictionaryImportanceEventTypes>();
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (filter.DocumentIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DBModel.Document.DocumentEvents>();
                        filterContains = filter.DocumentIDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.DocumentId == value).Expand());

                        qry = qry.Where(x =>
                                dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                    .Where(filterContains).Select(y => y.EventType.ImportanceEventTypeId).Contains(x.Id)
                                    );
                    }
                }

                var res = qry.Select(x => new FrontDictionaryImportanceEventType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] LinkTypes ...
        public IEnumerable<FrontDictionaryLinkType> GetLinkTypes(IContext context, FilterDictionaryLinkType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryLinkTypesSet.AsQueryable();

                if (filter != null)
                {

                    // Список первичных ключей
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DictionaryLinkTypes>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.True<DictionaryLinkTypes>();
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
                        var filterContains = PredicateBuilder.False<DictionaryLinkTypes>();
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                        qry = qry.Where(filterContains);
                    }
                }

                var res = qry.Select(x => new FrontDictionaryLinkType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsImportant = x.IsImportant,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion

        #region [+] Positions ...
        public int AddPosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dd = DictionaryModelConverter.GetDbPosition(context, position);
                dbContext.DictionaryPositionsSet.Add(dd);
                dbContext.SaveChanges();
                UpdatePositionExecutor(context, new List<int> { dd.Id });
                position.Id = dd.Id;

                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryPositions, EnumOperationType.AddNew);
                transaction.Complete();
                return dd.Id;
            }
        }

        public void UpdatePosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbPosition(context, position);
                dbContext.DictionaryPositionsSet.Attach(dbModel);
                //pss нельзя модифицировать поля, которые проставляет вертушка
                //dbContext.Entry(dd).State = System.Data.Entity.EntityState.Modified;
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.ParentId).IsModified = true;
                entity.Property(x => x.IsActive).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.FullName).IsModified = true;
                entity.Property(x => x.DepartmentId).IsModified = true;
                entity.Property(x => x.Order).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryPositions, EnumOperationType.Update);

                UpdatePositionExecutor(context, new List<int> { position.Id });

                transaction.Complete();
            }
        }

        public void UpdatePositionOrder(IContext context, int positionId, int order)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbPosition(context, new InternalDictionaryPosition() { Id = positionId, Order = order });
                dbContext.DictionaryPositionsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.Order).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdatePositionExecutor(IContext context, List<int> positionId = null)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Where(x => x.Id >= 0);
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


        public void DeletePositions(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                //TODO переписать на вызов родных делитов с AddFullTextCashInfo!!!

                // Удаляю настройку ролей для должности
                #region [+] AdminPositionRoles ...
                var filterPositionRoles = PredicateBuilder.False<AdminPositionRoles>();
                filterPositionRoles = list.Aggregate(filterPositionRoles,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                dbContext.AdminPositionRolesSet.Where(filterPositionRoles).Delete();
                #endregion

                // Удаляю настройку рассылки
                #region [+] AdminSubordinations ...
                var filterSubordination = PredicateBuilder.False<AdminSubordinations>();
                filterSubordination = list.Aggregate(filterSubordination,
                    (current, value) => current.Or(e => e.SourcePositionId == value || e.TargetPositionId == value).Expand());

                dbContext.AdminSubordinationsSet.Where(filterSubordination).Delete();
                #endregion

                // Удаляю настройку сенд лист ????
                //#region [+] StandartSendListContents ...
                //var filterStandartSendListContents = PredicateBuilder.False<DictionaryStandartSendListContents>();
                //filterStandartSendListContents = list.Aggregate(filterStandartSendListContents,
                //    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                //dbContext.DictionaryStandartSendListContentsSet.Where(filterStandartSendListContents).Delete();
                //#endregion

                // Удаляю настройку журналов
                #region [+] AdminSubordinations ...
                var filterJournals = PredicateBuilder.False<AdminRegistrationJournalPositions>();
                filterJournals = list.Aggregate(filterJournals,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                dbContext.AdminRegistrationJournalPositionsSet.Where(filterJournals).Delete();
                #endregion

                // Удаляю руководителей подразделений
                #region [+] Departments ...
                var filterDepartments = PredicateBuilder.False<DictionaryDepartments>();
                filterDepartments = list.Aggregate(filterDepartments,
                    (current, value) => current.Or(e => e.ChiefPositionId == value).Expand());

                dbContext.DictionaryDepartmentsSet.Where(filterDepartments).Update(x => new DictionaryDepartments() { ChiefPositionId = null });
                #endregion

                // Удаляю настройку ролей для исполнителей
                #region [+] AdminPositionRoles ...
                var filterUserRoles = PredicateBuilder.False<AdminUserRoles>();
                filterUserRoles = list.Aggregate(filterUserRoles,
                    (current, value) => current.Or(e => e.PositionExecutor.PositionId == value).Expand());

                dbContext.AdminUserRolesSet.Where(filterUserRoles).Delete();
                #endregion

                // Удаляю исполнителей
                #region [+] PositionExecutors ...
                var filterPositionExecutors = PredicateBuilder.False<DictionaryPositionExecutors>();
                filterPositionExecutors = list.Aggregate(filterPositionExecutors,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                dbContext.DictionaryPositionExecutorsSet.Where(filterPositionExecutors).Delete();
                #endregion

                // удаляю саму должность
                dbContext.DictionaryPositionsSet
                    .Where(x => x.Department.Company.ClientId == context.CurrentClientId)
                    .Where(x => list.Contains(x.Id))
                    .Delete();

                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryPositions, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public InternalDictionaryPositionExecutorForDocument GetExecutorAgentIdByPositionId(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                InternalDictionaryPositionExecutorForDocument res = null;
                var qry = dbContext.DictionaryPositionsSet.AsQueryable();
                if (!context.IsAdmin)
                {
                    qry = qry.Where(x => x.Department.Company.ClientId == context.CurrentClientId);
                }
                res = qry.Where(x => x.Id == id)
                    .Select(x => new InternalDictionaryPositionExecutorForDocument
                    {
                        Id = id,
                        ExecutorAgentId = x.ExecutorAgentId,
                        ExecutorTypeId = x.PositionExecutorTypeId,
                        MainExecutorAgentId = x.MainExecutorAgentId
                    }).FirstOrDefault();
                transaction.Complete();
                if (res.MainExecutorAgentId == context.CurrentAgentId)
                {
                    res.ExecutorAgentId = res.MainExecutorAgentId;
                    res.ExecutorTypeId = null;
                }
                return res;
            }
        }

        public FrontDictionaryPosition GetPosition(IContext context, int id)
        {
            //UpdatePositionExecutor(context);//, new List<int> { id });
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var now = DateTime.UtcNow;

                var res = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Where(x => x.Id == id)
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
                        DepartmentCode = x.Department.Code,
                        DepartmentCodePath = x.Department.FullPath,
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
                                PositionExecutorTypeName = y.PositionExecutorType.Name,
                                PositionExecutorTypeId = (EnumPositionExecutionTypes)y.PositionExecutorType.Id,
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

        public IEnumerable<FrontDictionaryPosition> GetPositions(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

                var filterMaxSubordinationTypeContains = PredicateBuilder.False<DBModel.Admin.AdminSubordinations>();
                if (filter.SubordinatedPositions?.Count() > 0)
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
                    DepartmentCode = x.Department.Code,
                    DepartmentCodePath = x.Department.FullPath,
                    DepartmentName = x.Department.Name,
                    CompanyName = x.Department.Company.Agent.Name,
                    ExecutorAgentId = x.ExecutorAgentId,
                    ExecutorAgentName = x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? " (" + x.ExecutorType.Suffix + ")" : null),
                    MaxSubordinationTypeId = x.TargetPositionSubordinations.AsQueryable()
                                                        .Where(filterMaxSubordinationTypeContains)
                                                        .Max(y => y.SubordinationTypeId)
                });

                if (filter.SubordinatedTypeId.HasValue)
                {
                    qry2 = qry2.Where(x => x.MaxSubordinationTypeId >= filter.SubordinatedTypeId);
                }

                var res = qry2.ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDictionaryPosition> GetInternalPositions(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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

        public IEnumerable<int> GetPositionIDs(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);
                qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);
                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetPositionList(IContext context, FilterDictionaryPosition filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<ListItem>();

                var res = qry.Select(x => new FrontShortListPosition
                {
                    Id = x.Id,
                    Name = x.Name,
                    CompanyName = x.Department.Company.Agent.Name,
                    DepartmentName = x.Department.Name,
                    DepartmentCodePath = x.Department.FullPath,
                    ExecutorName = x.ExecutorAgent.Name,
                    ExecutorTypeSuffix = x.ExecutorType.Suffix
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetPositionsTree(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);
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

        public IEnumerable<AutocompleteItem> GetShortListPositions(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Details = new List<string>
                    {
                        x.ExecutorAgent.Name + (x.ExecutorType.Suffix != null ? x.ExecutorType.Suffix : null),
                        x.Department.FullPath + " " + x.Department.Name
                    },
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDIPSubordinationsPosition> GetPositionsForStaffList(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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
                    IsList = !(x.PositionExecutors.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any()),// || x.ChildPositions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                    Order = x.Order
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<FrontDIPRegistrationJournalPositions> GetPositionsForDIPRegistrationJournals(IContext context, int registrationJournalId, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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
        public IEnumerable<FrontDIPSubordinationsPosition> GetPositionsForDIPSubordinations(IContext context, int sourcePositionId, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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
                    IsList = true,
                    ExecutorName = x.ExecutorAgent.Name,
                    ExecutorTypeSuffix = x.ExecutorType.Suffix,
                    IsInforming = (x.TargetPositionSubordinations
                        .Where(y => y.TargetPositionId == x.Id)
                        .Where(y => y.SourcePositionId == sourcePositionId)
                        .Where(y => y.SubordinationTypeId == (int)EnumSubordinationTypes.Informing)
                        .Any() ? 1 : 0),
                    IsExecution = (x.TargetPositionSubordinations
                        .Where(y => y.TargetPositionId == x.Id)
                        .Where(y => y.SourcePositionId == sourcePositionId)
                        .Where(y => y.SubordinationTypeId == (int)EnumSubordinationTypes.Execution)
                        .Any() ? 1 : 0),
                    SourcePositionId = sourcePositionId,
                    TargetPositionId = x.Id
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDIPJournalAccessPosition> GetPositionsForDIPJournalAccess(IContext context, int journalId, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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
                    IsList = true,
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

        public IEnumerable<SortPositoin> GetPositionsForSort(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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

        public class SortPositoin : IComparable
        {
            public int Id { get; set; }

            public int OldOrder { get; set; }

            public int NewOrder { get; set; }

            public int CompareTo(object obj)
            {
                return NewOrder - (obj as SortPositoin).NewOrder;
            }
        }

        public IQueryable<DictionaryPositions> GetPositionsQuery(IContext context, DmsContext dbContext, FilterDictionaryPosition filter)
        {
            var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositions>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryPositions>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // по вышестоящим отделам
                if (filter.ParentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositions>();
                    filterContains = filter.ParentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.ParentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // по отделам
                if (filter.DepartmentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositions>();
                    filterContains = filter.DepartmentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DepartmentId == value).Expand());

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
                    var filterContains = PredicateBuilder.False<DictionaryPositions>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Условие по полному имени
                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositions>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.FullName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.FullName.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.DocumentEvents>();
                    filterContains = filter.DocumentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(filterContains).Select(y => y.SourcePositionId).Contains(x.Id)
                                ||
                                dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(filterContains).Select(y => y.TargetPositionId).Contains(x.Id)
                                );
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
                    var filterContains = PredicateBuilder.False<DictionaryPositions>();
                    filterContains = filter.Orders.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Order == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        //public IEnumerable<InternalDictionaryPositionWithActions> GetPositionsWithActions(IContext context, FilterDictionaryPosition filter)
        //{
        //    using (var dbContext = new DmsContext(context))
        //    {
        //        // pss эта сборка Where-условий повторяется 3 раза (GetPositionsWithActions, ExistsPosition, GetPosition). У меня НЕ получается вынести Where в отдельную функцию.
        //        var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

        //        // Список первичных ключей
        //        if (filter.IDs?.Count > 0)
        //        {
        //            //TODO Contains
        //            qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
        //        }

        //        // Исключение списка первичных ключей
        //        if (filter.NotContainsIDs?.Count > 0)
        //        {
        //            //TODO Contains
        //            qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.pos.Id));
        //        }

        //        // Условие по IsActive
        //        if (filter.IsActive != null)
        //        {
        //            qry = qry.Where(x => filter.IsActive == x.pos.IsActive);
        //        }

        //        // Поиск по наименованию
        //        if (!string.IsNullOrEmpty(filter.Name))
        //        {
        //            foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
        //            {
        //                qry = qry.Where(x => x.pos.Name.Contains(temp));
        //            }
        //        }

        //        // Условие по полному имени
        //        if (!string.IsNullOrEmpty(filter.FullName))
        //        {
        //            foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.FullName))
        //            {
        //                qry = qry.Where(x => x.pos.FullName.Contains(temp));
        //            }
        //        }

        //        if (filter.DocumentIDs?.Count > 0)
        //        {
        //            var filterContains = PredicateBuilder.False<DocumentEvents>();
        //            filterContains = filter.DocumentIDs.Aggregate(filterContains,
        //                (current, value) => current.Or(e => e.DocumentId == value).Expand());

        //            qry = qry.Where(x =>
        //                    dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
        //                        .Where(filterContains).Select(y => y.SourcePositionId).Contains(x.pos.Id)
        //                        ||
        //                        dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
        //                        .Where(filterContains).Select(y => y.TargetPositionId).Contains(x.pos.Id)
        //                        );
        //        }

        //        if (filter.SubordinatedPositions?.Count > 0)
        //        {
        //            var filterContains = PredicateBuilder.False<DBModel.Admin.AdminSubordinations>();
        //            filterContains = filter.SubordinatedPositions.Aggregate(filterContains,
        //                (current, value) => current.Or(e => e.SourcePositionId == value).Expand());

        //            qry = qry.GroupJoin(
        //                                dbContext.AdminSubordinationsSet.Where(filterContains),
        //                                x => x.pos.Id,
        //                                y => y.TargetPositionId,
        //                                (x, y) => new { pos = x.pos, subordMax = y.Max(z => z.SubordinationTypeId) }
        //                                )
        //                     .Where(x => x.subordMax > 0);
        //        }

        //        return qry.Select(x => new InternalDictionaryPositionWithActions
        //        {
        //            Id = x.pos.Id,
        //            Name = x.pos.Name,
        //            DepartmentId = x.pos.DepartmentId,
        //            ExecutorAgentId = x.pos.ExecutorAgentId,
        //            DepartmentName = x.pos.Department.Name,
        //            ExecutorAgentName = x.pos.ExecutorAgent.Name,
        //        }).ToList();
        //    }
        //}

        #endregion

        #region [+] PositionExecutors ...
        public int AddExecutor(IContext context, InternalDictionaryPositionExecutor executor)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                DictionaryPositionExecutors dc = DictionaryModelConverter.GetDbExecutor(context, executor);
                dbContext.DictionaryPositionExecutorsSet.Add(dc);
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCashInfo(dbContext, dc.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.AddNew);
                executor.Id = dc.Id;
                UpdatePositionExecutor(context, new List<int> { dc.PositionId });
                transaction.Complete();
                return dc.Id;
            }
        }

        public void UpdateExecutor(IContext context, InternalDictionaryPositionExecutor executor)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                DictionaryPositionExecutors drj = DictionaryModelConverter.GetDbExecutor(context, executor);
                dbContext.DictionaryPositionExecutorsSet.Attach(drj);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.Update);
                UpdatePositionExecutor(context, new List<int> { executor.PositionId });
                transaction.Complete();
            }
        }

        public void DeleteExecutors(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                dbContext.DictionaryPositionExecutorsSet
                    .Where(x => x.Position.Department.Company.ClientId == context.CurrentClientId)
                    .Where(x => list.Contains(x.Id))
                    .Delete();
                UpdatePositionExecutor(context, list);
                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryPositionExecutors, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public IEnumerable<FrontDictionaryPositionExecutor> GetPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                // DMS-367 qry = qry.OrderBy(x => x.Position.Order).ThenBy(x => x.PositionExecutorType.Id).ThenBy(x => x.Agent.Name);
                qry = qry.OrderByDescending(x => x.StartDate).ThenBy(x => x.PositionExecutorType.Id);

                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);

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
                    DepartmentName = x.Position.Department.Name,
                    AccessLevelName = x.AccessLevel.Name,
                    PositionExecutorTypeName = x.PositionExecutorType.Name,
                    PositionExecutorTypeSuffix = x.PositionExecutorType.Suffix,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDictionaryPositionExecutor> GetInternalPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Position.Order).ThenBy(x => x.PositionExecutorType.Id).ThenBy(x => x.Agent.Name);

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

        public List<int> GetPositionExecutorsIDs(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);
                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<AutocompleteItem> GetShortListPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Agent.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<AutocompleteItem>();

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.PositionId,
                    Name = x.Agent.Name + " " + (x.PositionExecutorType.Suffix != null ? " (" + x.PositionExecutorType.Suffix + ")" : null),
                    Details = new List<string> { x.Position.Name ?? string.Empty, x.Position.Department.FullPath + " " + x.Position.Department.Name },
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        public IEnumerable<TreeItem> GetPositionExecutorsForTree(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Position.Order).ThenBy(x => x.PositionExecutorType.Id).ThenBy(x => x.Agent.Name);

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
                    IsList = true,
                    Description = x.PositionExecutorType.Code
                }).ToList();

                transaction.Complete();
                return res;
            }
        }



        public IEnumerable<FrontDictionaryPositionExecutorExtended> GetPositionExecutorsDIPUserRoles(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Position.Order).ThenBy(x => x.PositionExecutorType.Id).ThenBy(x => x.Agent.Name);

                DateTime? maxDateTime = DateTime.UtcNow.AddYears(50);
                string objId = ((int)EnumObjects.DictionaryPositionExecutors).ToString();
                string parObjId = string.Empty;

                var res = qry.Select(x => new FrontDictionaryPositionExecutorExtended
                {
                    AssignmentId = x.Id,
                    IsActive = x.IsActive,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate > maxDateTime ? (DateTime?)null : x.EndDate,
                    PositionName = x.Position.Name,
                    PositionExecutorTypeName = x.PositionExecutorType.Name,
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

        public IQueryable<DictionaryPositionExecutors> GetPositionExecutorsQuery(IContext context, DmsContext dbContext, FilterDictionaryPositionExecutor filter)
        {
            var qry = dbContext.DictionaryPositionExecutorsSet.Where(x => x.Position.Department.Company.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryPositionExecutors>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.PositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                    filterContains = filter.PositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка сотрудников
                if (filter.NotContainsAgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                    filterContains = filter.NotContainsAgentIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.AgentId != value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (filter.PositionExecutorTypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                    filterContains = filter.PositionExecutorTypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionExecutorTypeId == (int)value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Тоько активные/неактивные
                if (filter.IsActive != null)
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


                if (filter.AccessLevelIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                    filterContains = filter.AccessLevelIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AccessLevelId == (int)value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsPositionExecutor(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();
                return res;
            }
        }

        #endregion

        #region [+] PositionExecutorTypes ...
        public IEnumerable<InternalDictionaryPositionExecutorType> GetInternalDictionaryPositionExecutorType(IContext context, FilterDictionaryPositionExecutorType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorTypesQuery(context, dbContext, filter);

                var res = qry.Select(x => new InternalDictionaryPositionExecutorType
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryPositionExecutorType> GetPositionExecutorTypes(IContext context, FilterDictionaryPositionExecutorType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetPositionExecutorTypesQuery(context, dbContext, filter);

                var res = qry.Select(x => new FrontDictionaryPositionExecutorType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryPositionExecutorTypes> GetPositionExecutorTypesQuery(IContext context, DmsContext dbContext, FilterDictionaryPositionExecutorType filter)
        {
            var qry = dbContext.DictionaryPositionExecutorTypesSet.AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutorTypes>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryPositionExecutorTypes>();

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
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutorTypes>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositionExecutorTypes>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.Period?.HasValue == true && filter.PositionId != null)
                {

                    // достаю всех исполнителей переданной должности в указанный срок
                    var executors = GetPositionExecutors(context, new FilterDictionaryPositionExecutor()
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
        public int AddRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal regJournal)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                DictionaryRegistrationJournals drj = DictionaryModelConverter.GetDbRegistrationJournal(context, regJournal);
                dbContext.DictionaryRegistrationJournalsSet.Add(drj);
                dbContext.SaveChanges();
                regJournal.Id = drj.Id;
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.AddNew);
                transaction.Complete();
                return drj.Id;
            }
        }

        public void UpdateRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal regJournal)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                DictionaryRegistrationJournals drj = DictionaryModelConverter.GetDbRegistrationJournal(context, regJournal);
                dbContext.DictionaryRegistrationJournalsSet.Attach(drj);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteRegistrationJournal(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var drj = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryRegistrationJournalsSet.Remove(drj);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDictionaryRegistrationJournal> GetInternalRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

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
                    IsIncoming = x.DirectionCodes.Contains(EnumDocumentDirections.Incoming.ToString()),
                    IsOutcoming = x.DirectionCodes.Contains(EnumDocumentDirections.Outcoming.ToString()),
                    IsInternal = x.DirectionCodes.Contains(EnumDocumentDirections.Internal.ToString()),
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                transaction.Complete();
                return res;
            }
        }



        public IEnumerable<FrontDictionaryRegistrationJournal> GetRegistrationJournals(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter as FilterDictionaryRegistrationJournal);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontDictionaryRegistrationJournal>();

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
                    IsIncoming = x.DirectionCodes.Contains(EnumDocumentDirections.Incoming.ToString()),
                    IsOutcoming = x.DirectionCodes.Contains(EnumDocumentDirections.Outcoming.ToString()),
                    IsInternal = x.DirectionCodes.Contains(EnumDocumentDirections.Internal.ToString()),
                    DepartmentName = x.Department.Name
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetRegistrationJournalIDs(IContext context, IBaseFilter filter, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter as FilterDictionaryRegistrationJournal);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        // Использовался для дерева журналов, как конечне элементы
        public IEnumerable<TreeItem> GetRegistrationJournalsForRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter);

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
                    IsList = true,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<AutocompleteItem> GetShortListRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new AutocompleteItem
                {
                    Id = x.Id,
                    Name = x.Index + " " + x.Name,
                    Details = new List<string> { x.Department.Code + " " + x.Department.Name }
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetRegistrationJournalsForDIPRJournalPositions(IContext context, int positionId, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter);

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
                    IsList = true,
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
        public bool ExistsDictionaryRegistrationJournal(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter);
                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        public IQueryable<DictionaryRegistrationJournals> GetRegistrationJournalsQuery(IContext context, DmsContext dbContext, FilterDictionaryRegistrationJournal filter)
        {
            var qry = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryRegistrationJournals>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryRegistrationJournals>();
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
                    var filterContains = PredicateBuilder.False<DictionaryRegistrationJournals>();
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
                    var filterContains = PredicateBuilder.False<DictionaryRegistrationJournals>();
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
                    var filterContains = PredicateBuilder.False<DictionaryRegistrationJournals>();
                    filterContains = filter.DepartmentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DepartmentId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.CompanyIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryRegistrationJournals>();
                    filterContains = filter.CompanyIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Department.CompanyId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // журналы отдела в котором работает должность
                if (filter.DepartmentByPositionIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryPositions>();
                    filterContains = filter.DepartmentByPositionIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(x => x.Department.Positions.AsQueryable().Any(filterContains));
                }

                if (filter.DocumentDirection.HasValue)
                {
                    qry = qry.Where(x => x.DirectionCodes.Contains(((int)filter.DocumentDirection).ToString()));
                }

                //// Условие по IsIncoming
                //if (filter.IsIncoming != null)
                //{
                //    qry = qry.Where(x => x.DirectionCodes.Contains(((int)EnumDocumentDirections.Incoming).ToString()));
                //}

                //// Условие по IsOutcoming
                //if (filter.IsOutcoming != null)
                //{
                //    qry = qry.Where(x => x.DirectionCodes.Contains(((int)EnumDocumentDirections.Outcoming).ToString()));
                //}

                //// Условие по IsInternal
                //if (filter.IsInternal != null)
                //{
                //    qry = qry.Where(x => x.DirectionCodes.Contains(((int)EnumDocumentDirections.Internal).ToString()));
                //}

                if (filter.PositionIdsAccessForRegistration?.Count > 0)
                {
                    var filterPositionsIdList = PredicateBuilder.False<AdminRegistrationJournalPositions>();
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
        public IEnumerable<FrontDictionaryResultType> GetResultTypes(IContext context, FilterDictionaryResultType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryResultTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DictionaryResultTypes>();
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
                        var filterContains = PredicateBuilder.True<DictionaryResultTypes>();
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
                        var filterContains = PredicateBuilder.False<DictionaryResultTypes>();
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

        public IEnumerable<FrontDictionarySendType> GetSendTypes(IContext context, FilterDictionarySendType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionarySendTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DictionarySendTypes>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.True<DictionarySendTypes>();
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.False<DictionarySendTypes>();
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                        qry = qry.Where(filterContains);
                    }
                }
                qry = qry.OrderBy(x => x.Code);
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
        public IEnumerable<ListItem> GetStageTypes(IContext context, FilterDictionaryStageType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryStageTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DictionaryStageTypes>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.True<DictionaryStageTypes>();
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.False<DictionaryStageTypes>();
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

        public IQueryable<DictionaryStandartSendListContents> GetStandartSendListContentsQuery(IContext context, DmsContext dbContext, FilterDictionaryStandartSendListContent filter)
        {
            var qry = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendListContents>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Список первичных ключей
                if (filter.StandartSendListId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendListContents>();
                    filterContains = filter.StandartSendListId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.StandartSendListId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryStandartSendListContents>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.SendTypeId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendListContents>();
                    filterContains = filter.SendTypeId.Aggregate(filterContains,
                        (current, value) => current.Or(e => (EnumSendTypes)e.SendTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TargetPositionId.HasValue)
                {
                    qry = qry.Where(x => x.TargetPositionId == filter.TargetPositionId);
                }

                if (filter.TargetPositionId.HasValue)
                {
                    qry = qry.Where(x => x.TargetAgentId == filter.TargetAgentId);
                }

                if (!string.IsNullOrEmpty(filter.Task))
                {
                    qry = CommonFilterUtilites.GetWhereExpressions(filter.Task)
                        .Aggregate(qry, (current, task) => current.Where(x => x.Task.Contains(task)));
                }


                if (!string.IsNullOrEmpty(filter.SendTypeName))
                {
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendListContents>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.SendTypeName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SendType.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TargetPositionName))
                {
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendListContents>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TargetPositionName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetPosition.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.TargetAgentName))
                {
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendListContents>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TargetAgentName).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.TargetAgent.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            return qry;
        }

        public IEnumerable<FrontDictionaryStandartSendListContent> GetStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListContentsQuery(context, dbContext, filter);

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
                    TargetExecutorTypeSuffix = x.TargetPosition.ExecutorType.Suffix,
                    AccessLevelName = x.AccessLevel.Name,
                    SendTypeIsExternal = x.SendTypeId == 45
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public int AddStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendListContent(context, content);
                dbContext.DictionaryStandartSendListContentsSet.Add(dbModel);
                dbContext.SaveChanges();
                content.Id = dbModel.Id;

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendListContent(context, content);
                dbContext.DictionaryStandartSendListContentsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteStandartSendListContent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var ddt = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryStandartSendListContentsSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.Delete);
                transaction.Complete();
            }
        }


        #endregion

        #region [+] StandartSendLists ...
        public IQueryable<DictionaryStandartSendLists> GetStandartSendListQuery(IContext context, DmsContext dbContext, FilterDictionaryStandartSendList filter)
        {
            var qry = dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendLists>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryStandartSendLists>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendLists>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {

                    qry = qry.Where(x => x.Name == filter.NameExact);

                }

                if (filter.PositionID != null)
                {
                    qry = qry.Where(x => filter.PositionID == x.PositionId);
                }
            }

            return qry;
        }

        public IEnumerable<FrontMainDictionaryStandartSendList> GetMainStandartSendLists(IContext context, IBaseFilter filter, UIPaging paging, UISorting sortin)
        {
            var sendLists = GetStandartSendLists(context, filter as FilterDictionaryStandartSendList, paging);

            var res = sendLists.GroupBy(x => new { x.PositionId, x.PositionName, x.PositionExecutorName, x.PositionExecutorTypeSuffix })
                 .OrderBy(x => x.Key.PositionName)
                 .Select(x => new FrontMainDictionaryStandartSendList()
                 {
                     Id = x.Key.PositionId ?? -1,
                     Name = x.Key.PositionName,
                     ExecutorName = x.Key.PositionExecutorName,
                     ExecutorTypeSuffix = x.Key.PositionExecutorTypeSuffix,
                     SendLists = x.OrderBy(y => y.Name).ToList()
                 });

            return res;

        }

        public List<int> GetStandartSendListIDs(IContext context, IBaseFilter filter, UISorting sortin)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListQuery(context, dbContext, filter as FilterDictionaryStandartSendList);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }

        }

        public IEnumerable<FrontDictionaryStandartSendList> GetStandartSendLists(IContext context, FilterDictionaryStandartSendList filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetStandartSendListQuery(context, dbContext, filter);

                qry = qry.OrderBy(x => x.Name);

                if (Paging.Set(ref qry, paging) == EnumPagingResult.IsOnlyCounter) return new List<FrontDictionaryStandartSendList>();

                var res = qry.Select(x => new FrontDictionaryStandartSendList
                {
                    Id = x.Id,
                    Name = x.Name,
                    PositionId = x.PositionId,
                    PositionName = x.Position.Name,
                    PositionExecutorName = x.Position.ExecutorAgent.Name,
                    PositionExecutorTypeSuffix = x.Position.ExecutorType.Suffix
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public int AddStandartSendList(IContext context, InternalDictionaryStandartSendList model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendList(context, model);

                dbContext.DictionaryStandartSendListsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateStandartSendList(IContext context, InternalDictionaryStandartSendList model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendList(context, model);

                dbContext.DictionaryStandartSendListsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteStandartSendList(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var contents = dbContext.DictionaryStandartSendListContentsSet.Where(y => y.StandartSendListId == id);
                dbContext.DictionaryStandartSendListContentsSet.RemoveRange(contents);

                var ddt = dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryStandartSendListsSet.Remove(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.Delete);
                transaction.Complete();
            }
        }


        #endregion

        #region [+] SubordinationTypes ...

        public IEnumerable<ListItem> GetSubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionarySubordinationTypesSet.AsQueryable();

                if (filter != null)
                {
                    // Список первичных ключей
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<DictionarySubordinationTypes>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Исключение списка первичных ключей
                    if (filter.NotContainsIDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.True<DictionarySubordinationTypes>();
                        filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                            (current, value) => current.And(e => e.Id != value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    // Поиск по наименованию
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        var filterContains = PredicateBuilder.False<DictionarySubordinationTypes>();
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

        private IQueryable<DictionaryTags> GetTagsQuery(IContext context, DmsContext dbContext, FilterDictionaryTag filter)
        {
            var qry = dbContext.DictionaryTagsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            if (filter != null)
            {
                if (!context.IsAdmin)
                {
                    var filterContains = PredicateBuilder.False<DictionaryTags>();
                    filterContains = context.CurrentPositionsIdList.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.PositionId == value || !e.PositionId.HasValue).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryTags>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryTags>();
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
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, dbContext, filter);

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
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, dbContext, filter);

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
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, dbContext, filter);

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
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, dbContext, filter as FilterDictionaryTag);

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
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetTagsQuery(ctx, dbContext, filter as FilterDictionaryTag);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }
        public int DocsWithTagCount(IContext context, int tagId)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var res = dbContext.DocumentTagsSet.Count(y => y.TagId == tagId);
                transaction.Complete();
                return res;
            }
        }

        public int AddTag(IContext context, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbTag(context, model);

                dbContext.DictionaryTagsSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryTag, EnumOperationType.AddNew);

                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }
        public void UpdateTag(IContext ctx, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = dbContext.DictionaryTagsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                            .Where(x => x.Id == model.Id).AsQueryable();

                //                if (!ctx.IsAdmin)
                //                {
                //                    var filterContains = PredicateBuilder.False<DictionaryTags>();
                //                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
                //                        (current, value) => current.Or(e => e.PositionId == value).Expand());
                //
                //                    qry = qry.Where(filterContains);
                //                }

                var savTag = qry.FirstOrDefault();

                if (savTag?.Id > 0)
                {
                    savTag.Name = model.Name;
                    savTag.Color = model.Color;
                    savTag.IsActive = model.IsActive;
                    savTag.LastChangeUserId = ctx.CurrentAgentId;
                    savTag.LastChangeDate = DateTime.UtcNow;
                    dbContext.SaveChanges();
                }
                else
                {
                    //TODO Это нарушение. дб уровень не содержит логики, он молча выполняет вставки и обновления. все проверки должны быть на уровне логики.
                    throw new DictionaryTagNotFoundOrUserHasNoAccess();
                }

                CommonQueries.AddFullTextCashInfo(dbContext, model.Id, EnumObjects.DictionaryTag, EnumOperationType.Update);

                transaction.Complete();
            }
        }

        public void DeleteTag(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.DictionaryTagsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.DictionaryTagsSet.Remove(item);
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCashInfo(dbContext, id, EnumObjects.DictionaryTag, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        #endregion

        // pss перенести в AdminCore
        #region [+] AdminAccessLevels ...
        public IEnumerable<FrontAdminAccessLevel> GetAdminAccessLevels(IContext ctx, FilterAdminAccessLevel filter)
        {
            using (var dbContext = new DmsContext(ctx)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAccessLevelsQuery(ctx, dbContext, filter);

                var res = qry.Select(x => new FrontAdminAccessLevel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                }).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IQueryable<AdminAccessLevels> GetAccessLevelsQuery(IContext context, DmsContext dbContext, FilterAdminAccessLevel filter)
        {
            var qry = dbContext.AdminAccessLevelsSet.AsQueryable();

            // Список первичных ключей
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<AdminAccessLevels>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<AdminAccessLevels>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Поиск но Code
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    var filterContains = PredicateBuilder.False<AdminAccessLevels>();
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
                    var filterContains = PredicateBuilder.False<AdminAccessLevels>();
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

        #region [+] CustomDictionaryTypes ...
        public int AddCustomDictionaryType(IContext context, InternalCustomDictionaryType model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionaryType(context, model);
                dbContext.CustomDictionaryTypesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateCustomDictionaryType(IContext context, InternalCustomDictionaryType model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionaryType(context, model);
                dbContext.CustomDictionaryTypesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteCustomDictionaryType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.CustomDictionariesSet.Where(x => x.DictionaryTypeId == item.Id).Delete();//.RemoveRange(item.CustomDictionaries);
                dbContext.CustomDictionaryTypesSet.Remove(item);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalCustomDictionaryType> GetInternalCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryTypesQuery(context, dbContext, filter);

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

        //public FrontCustomDictionaryType GetCustomDictionaryTypeWithCustomDictionaries(IContext context, int id)
        //{
        //    using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
        //    {
        //        var qry = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

        //        qry = qry.Where(x => x.Id == id);

        //        var res = qry.Select(x => new FrontCustomDictionaryType
        //        {
        //            Id = x.Id,
        //            Code = x.Code,
        //            Name = x.Name,
        //            Description = x.Description
        //        }).FirstOrDefault();

        //        res.CustomDictionaries = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).Where(x => x.DictionaryTypeId == res.Id)
        //            .Select(x => new FrontCustomDictionary
        //            {
        //                Id = x.Id,
        //                Code = x.Code,
        //                Name = x.Name,
        //                Description = x.Description,
        //                DictionaryTypeId = res.Id
        //            }).ToList();

        //        transaction.Complete();
        //        return res;
        //    }
        //}

        public IEnumerable<FrontCustomDictionaryType> GetCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryTypesQuery(context, dbContext, filter);

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

        public IQueryable<CustomDictionaryTypes> GetCustomDictionaryTypesQuery(IContext context, DmsContext dbContext, FilterCustomDictionaryType filter)
        {
            var qry = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            // Список первичных ключей
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<CustomDictionaryTypes>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.True<CustomDictionaryTypes>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.And(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }


            // Поиск но Code
            if (!string.IsNullOrEmpty(filter.Code))
            {
                var filterContains = PredicateBuilder.False<CustomDictionaryTypes>();
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
                var filterContains = PredicateBuilder.False<CustomDictionaryTypes>();
                filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                qry = qry.Where(filterContains);
            }

            if (!string.IsNullOrEmpty(filter.NameExact))
            {
                qry = qry.Where(x => filter.NameExact.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
            }

            return qry;
        }
        #endregion

        #region [+] CustomDictionaries ...
        public int AddCustomDictionary(IContext context, InternalCustomDictionary model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionary(context, model);
                dbContext.CustomDictionariesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.CustomDictionaries, EnumOperationType.AddNew);
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateCustomDictionary(IContext context, InternalCustomDictionary model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionary(context, model);
                dbContext.CustomDictionariesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.CustomDictionaries, EnumOperationType.Update);
                transaction.Complete();
            }
        }

        public void DeleteCustomDictionary(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var item = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.CustomDictionariesSet.Remove(item);
                dbContext.SaveChanges();
                CommonQueries.AddFullTextCashInfo(dbContext, id, EnumObjects.CustomDictionaries, EnumOperationType.Delete);
                transaction.Complete();
            }
        }

        public IEnumerable<InternalCustomDictionary> GetInternalCustomDictionarys(IContext context, FilterCustomDictionary filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryQuery(context, dbContext, filter);

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

        public IEnumerable<FrontCustomDictionary> GetMainCustomDictionaries(IContext context, IBaseFilter filter, UIPaging paging, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryQuery(context, dbContext, filter as FilterCustomDictionary);

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

        public List<int> GetCustomDictionarieIDs(IContext context, IBaseFilter filter, UISorting sorting)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetCustomDictionaryQuery(context, dbContext, filter as FilterCustomDictionary);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IQueryable<CustomDictionaries> GetCustomDictionaryQuery(IContext context, DmsContext dbContext, FilterCustomDictionary filter)
        {
            var qry = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).AsQueryable();

            // Список первичных ключей
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<CustomDictionaries>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.TypeIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<CustomDictionaries>();
                    filterContains = filter.TypeIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DictionaryTypeId == value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<CustomDictionaries>();
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
                    var filterContains = PredicateBuilder.False<CustomDictionaries>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.False<CustomDictionaries>();
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
        public int AddAgentFavourite(IContext context, InternalAgentFavourite model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentFavorite(context, model);
                dbContext.DictionaryAgentFavoritesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;

                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void AddAgentFavourites(IContext context, IEnumerable<InternalAgentFavourite> list)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentFavorites(context, list);
                dbContext.DictionaryAgentFavoritesSet.AddRange(dbModel);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdateAgentFavourite(IContext context, InternalAgentFavourite model)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentFavorite(context, model);
                dbContext.DictionaryAgentFavoritesSet.Attach(dbModel);
                dbContext.Entry(dbModel).State = EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteAgentFavourite(IContext context, FilterAgentFavourite filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentFavouriteQuery(context, dbContext, filter);
                qry.Delete();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalAgentFavourite> GetInternalAgentFavourite(IContext context, FilterAgentFavourite filter)
        {
            using (var dbContext = new DmsContext(context)) using (var transaction = Transactions.GetTransaction())
            {
                var qry = GetAgentFavouriteQuery(context, dbContext, filter);

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
        private IQueryable<DictionaryAgentFavorites> GetAgentFavouriteQuery(IContext context, DmsContext dbContext, FilterAgentFavourite filter)
        {
            var qry = dbContext.DictionaryAgentFavoritesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

            // Список первичных ключей
            if (filter != null)
            {
                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentFavorites>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryAgentFavorites>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentFavorites>();
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

                    qry = qry.Where(filterContains);
                }
                if (filter.ObjectIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentFavorites>();
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

        public IEnumerable<int> GetFavouriteList(IContext context, string module, string feature)
        {
            var list = GetInternalAgentFavourite(context, new FilterAgentFavourite
            {
                AgentIDs = new List<int> { context.CurrentAgentId },
                ModuleExact = module,
                FeatureExact = feature
            });

            return list.Select(x => x.ObjectId).ToList();
        }

    }
}
