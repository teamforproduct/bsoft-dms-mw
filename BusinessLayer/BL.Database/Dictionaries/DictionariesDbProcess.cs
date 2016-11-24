﻿using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Dictionary;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.AdminCore;
using BL.Model.SystemCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using BL.Model.FullTextSearch;
using LinqKit;
using System.Data.Entity;
using BL.Model.Common;
using System.Transactions;
using BL.Model.Tree;
using EntityFramework.Extensions;
using BL.Database.DBModel.Admin;
using BL.Model.AdminCore.FrontModel;

namespace BL.Database.Dictionaries
{
    public class DictionariesDbProcess : CoreDb.CoreDb, IDictionariesDbProcess
    {
        public DictionariesDbProcess()
        {
        }

        private TransactionScope GetTransaction() => new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted });

        #region [+] DictionaryAgents ...


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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(context, model);

                dbContext.DictionaryAgentsSet.Add(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgents, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateAgent(IContext context, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(context, agent);

                dbContext.DictionaryAgentsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, agent.Id, EnumObjects.DictionaryAgents, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdateAgentName(IContext context, int id, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(context, agent);

                dbContext.DictionaryAgentsSet.Attach(dbModel);

                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;

                CommonQueries.AddFullTextCashInfo(dbContext, id, EnumObjects.DictionaryAgents, EnumOperationType.Update);
                //entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }

        }


        public void DeleteAgent(IContext context, int agentId)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
                CommonQueries.AddFullTextCashInfo(dbContext, agentId, EnumObjects.DictionaryAgents, EnumOperationType.Delete);
                dbContext.SaveChanges();
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
                if (ExistsAgentClientCompanies(context, new FilterDictionaryAgentClientCompany() { IDs = new List<int>() { agentId } })) return;

                if (ExistsAgentEmployees(context, new FilterDictionaryAgentEmployee() { IDs = new List<int>() { agentId } })) return;
                //if (ExistsAgentUsers(context, new FilterDictionaryAgent() { IDs = new List<int>() { agentId } })) return;

                if (ExistsAgentPersons(context, new FilterDictionaryAgentPerson() { IDs = new List<int>() { agentId } })) return;

                if (ExistsAgentBanks(context, new FilterDictionaryAgentBank() { IDs = new List<int>() { agentId } })) return;

                if (ExistsAgentCompanies(context, new FilterDictionaryAgentCompany() { IDs = new List<int>() { agentId } })) return;

                DeleteAgent(context, agentId);
            }
        }

        public void SetAgentImage(IContext context, InternalDictionaryAgentImage User)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

        public FrontDictionaryAgent GetAgent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                var res = dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == id).Select(x => new FrontDictionaryAgent
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        ResidentTypeId = x.ResidentTypeId,
                        Description = x.Description,
                        Contacts = x.AgentContacts.Select(y => new FrontDictionaryContact
                        {
                            Id = y.Id,
                            AgentId = y.AgentId,
                            ContactType = new FrontDictionaryContactType
                            {
                                Id = y.ContactType.Id,
                                Code = y.ContactType.Code,
                                Name = y.ContactType.Name,
                                IsActive = y.ContactType.IsActive
                            },
                            Value = y.Contact,
                            IsActive = y.IsActive,
                            Description = y.Description
                        }),

                        Addresses = x.AgentAddresses.Select(z => new FrontDictionaryAgentAddress
                        {
                            Id = z.Id,
                            AgentId = z.AgentId,
                            AddressType = new FrontDictionaryAddressType
                            {
                                Id = z.AddressType.Id,
                                Name = z.AddressType.Name,
                                IsActive = z.AddressType.IsActive
                            },
                            PostCode = z.PostCode,
                            Address = z.Address,
                            IsActive = z.IsActive,
                            Description = z.Description
                        })

                    })
                .FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }



        public IEnumerable<FrontDictionaryAgent> GetAgents(IContext context, FilterDictionaryAgent filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentsQuery(context, dbContext, filter, paging);

                var res = qry.Select(x => new FrontDictionaryAgent
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    ResidentTypeId = x.ResidentTypeId,
                    Description = x.Description,
                    Contacts = x.AgentContacts.Select(y => new FrontDictionaryContact
                    {
                        Id = y.Id,
                        AgentId = y.AgentId,
                        Value = y.Contact,
                        IsActive = y.IsActive,
                        Description = y.Description,
                        ContactType = new FrontDictionaryContactType
                        {
                            Id = y.ContactType.Id,
                            Name = y.ContactType.Name,
                            Code = y.ContactType.Code,
                            InputMask = y.ContactType.InputMask,
                            IsActive = y.ContactType.IsActive
                        }
                    }),
                    Addresses = x.AgentAddresses.Select(z => new FrontDictionaryAgentAddress
                    {
                        Id = z.Id,
                        AgentId = z.AgentId,
                        AddressType = new FrontDictionaryAddressType
                        {
                            Id = z.AddressType.Id,
                            Name = z.AddressType.Name,
                            IsActive = z.AddressType.IsActive
                        },
                        PostCode = z.PostCode,
                        Address = z.Address,
                        IsActive = z.IsActive,
                        Description = z.Description
                    })

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

        private IQueryable<DictionaryAgents> GetWhereAgents(ref IQueryable<DictionaryAgents> qry, FilterDictionaryAgent filter, UIPaging paging)
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
                qry = qry.Where(x => filter.IsActive == x.IsActive);
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

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = qry.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    //pss Разобраться зачем это
                    //return new List<FrontDictionaryAgent>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;
                    qry = qry.Skip(() => skip).Take(() => take);
                }
            }

            return qry;
        }

        public IQueryable<DictionaryAgents> GetAgentsQuery(IContext context, DmsContext dbContext, FilterDictionaryAgent filter, UIPaging paging)
        {
            var qry = dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            qry = qry.OrderBy(x => x.Name);

            qry = GetWhereAgents(ref qry, filter, paging);

            return qry;
        }

        public bool ExistsAgents(IContext context, FilterDictionaryAgent filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = GetAgentsQuery(context, dbContext, filter, null).Any();

                transaction.Complete();
                return res;
            }
        }

        #endregion DictionaryAgents

        #region [+] DictionaryAgentPerson ...
        public int AddAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                if (ExistsAgents(context, new FilterDictionaryAgent() { IDs = new List<int> { person.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской (для персон и пользователей оно формируется по разному)
                    UpdateAgentName(context, person.Id, new InternalDictionaryAgent(person));
                }
                else
                {
                    person.Id = AddAgent(context, new InternalDictionaryAgent(person));
                };

                var dbModel = DictionaryModelConverter.GetDbAgentPerson(context, person);

                dbContext.DictionaryAgentPersonsSet.Add(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.AddNew);

                dbContext.SaveChanges();

                transaction.Complete();

                return person.Id;
            }
        }

        public void UpdateAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                UpdateAgentName(context, person.Id, new InternalDictionaryAgent(person));

                var dbModel = DictionaryModelConverter.GetDbAgentPerson(context, person);

                dbContext.DictionaryAgentPersonsSet.Attach(dbModel);
                dbContext.SaveChanges();
                var entity = dbContext.Entry(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.Update);

                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                transaction.Complete();

            }
        }


        public void DeleteAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var ddt = dbContext.DictionaryAgentPersonsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == person.Id);
                dbContext.DictionaryAgentPersonsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, person.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.Delete);
                dbContext.SaveChanges();

                DeleteAgentIfNoAny(context, new List<int>() { person.Id });

                transaction.Complete();
            }
        }


        public FrontDictionaryAgentPerson GetAgentPerson(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                var res =
                    dbContext.DictionaryAgentPersonsSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentPerson
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        MiddleName = x.MiddleName,
                        Name = x.Agent.Name,
                        FullName = x.FullName,
                        TaxCode = x.TaxCode,
                        IsMale = x.IsMale,
                        PassportSerial = x.PassportSerial,
                        PassportNumber = x.PassportNumber,
                        PassportText = x.PassportText,
                        PassportDate = x.PassportDate,
                        BirthDate = x.BirthDate,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        Contacts = x.Agent.AgentContacts.Select(y => new FrontDictionaryContact
                        {
                            Id = y.Id,
                            AgentId = y.AgentId,
                            ContactType = new FrontDictionaryContactType
                            {
                                Id = y.ContactType.Id,
                                Name = y.ContactType.Name,
                                InputMask = y.ContactType.InputMask,
                                IsActive = y.ContactType.IsActive
                            },
                            Value = y.Contact,
                            IsActive = y.IsActive,
                            Description = y.Description
                        }),
                        Addresses = x.Agent.AgentAddresses.Select(z => new FrontDictionaryAgentAddress
                        {
                            Id = z.Id,
                            AgentId = z.AgentId,
                            AddressType = new FrontDictionaryAddressType
                            {
                                Id = z.AddressType.Id,
                                Name = z.AddressType.Name,
                                IsActive = z.AddressType.IsActive
                            },
                            PostCode = z.PostCode,
                            Address = z.Address,
                            IsActive = z.IsActive,
                            Description = z.Description
                        })

                    }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryAgentPerson> GetAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                var qry = GetAgentPersonsQuery(context, dbContext, filter, paging);

                var res = qry.Select(x => new FrontDictionaryAgentPerson
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    FullName = x.FullName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    TaxCode = x.TaxCode,
                    IsMale = x.IsMale,
                    PassportSerial = x.PassportSerial,
                    PassportNumber = x.PassportNumber,
                    PassportText = x.PassportText,
                    PassportDate = x.PassportDate,
                    BirthDate = x.BirthDate,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public bool ExistsAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = GetAgentPersonsQuery(context, dbContext, filter, null).Any();

                transaction.Complete();
                return res;
            }
        }

        public IQueryable<DictionaryAgentPersons> GetAgentPersonsQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            var qry = dbContext.DictionaryAgentPersonsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            qry = qry.OrderBy(x => x.Agent.Name);

            qry = GetWhereAgentPersons(ref qry, filter, paging);

            return qry;
        }

        private IQueryable<DictionaryAgentPersons> GetWhereAgentPersons(ref IQueryable<DictionaryAgentPersons> qry, FilterDictionaryAgentPerson filter, UIPaging paging)
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
            if (filter.AgentCompanyIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
                filterContains = filter.AgentCompanyIDs.Aggregate(filterContains,
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
                    (current, value) => current.Or(e => e.FullName.Contains(value)).Expand());

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
                qry = qry.Where(x => x.FirstName == filter.FirstNameExact);
            }

            if (!string.IsNullOrEmpty(filter.LastNameExact))
            {
                qry = qry.Where(x => x.LastName == filter.LastNameExact);
            }

            // Поиск по паспортным данным
            if (!string.IsNullOrEmpty(filter.Passport))
            {
                var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
                filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Passport).Aggregate(filterContains,
                    (current, value) => current.Or(e => (e.PassportSerial + "-" + e.PassportNumber + " " + e.PassportDate.ToString() + " " + e.PassportText) == value).Expand());

                qry = qry.Where(filterContains);

            }

            // Поиск по ИНН
            if (!string.IsNullOrEmpty(filter.TaxCode))
            {
                var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
                filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TaxCode).Aggregate(filterContains,
                    (current, value) => current.Or(e => e.TaxCode.Contains(value)).Expand());

                qry = qry.Where(filterContains);
            }

            if (!string.IsNullOrEmpty(filter.TaxCodeExact))
            {
                qry = qry.Where(x => x.TaxCode == filter.TaxCodeExact);
            }

            // Поиск по дате рождения
            if (filter.BirthPeriod?.IsActive ?? false)
            {
                qry = qry.Where(x => x.BirthDate >= filter.BirthPeriod.DateBeg);
                qry = qry.Where(x => x.BirthDate <= filter.BirthPeriod.DateEnd);
            }

            // Поиск по дате рождения
            if (filter.BirthDateExact != null)
            {
                qry = qry.Where(x => x.BirthDate == filter.BirthDateExact);
            }

            if (!string.IsNullOrEmpty(filter.PassportSerialExact))
            {
                qry = qry.Where(x => x.PassportSerial == filter.PassportSerialExact);
            }

            if (filter.PassportNumberExact != null)
            {
                qry = qry.Where(x => x.PassportNumber == filter.PassportNumberExact);
            }

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = qry.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    //pss !!!! Какой здесь заложен смысл
                    //return new List<FrontDictionaryAgentPerson>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    qry = qry.Skip(() => skip).Take(() => take);
                }
            }

            return qry;
        }


        #endregion DictionaryAgentPerson

        #region [+] DictionaryAgentEmployee ...
        public int AddAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                employee.Id = AddAgentPerson(context, new InternalDictionaryAgentPerson(employee));
                // решили, что сотрудник и пользователь всегда создаются парой, пользователь может быть деактивирован
                AddAgentUser(context, new InternalDictionaryAgentUser(employee));

                var dbModel = DictionaryModelConverter.GetDbAgentEmployee(context, employee);
                dbContext.DictionaryAgentEmployeesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                transaction.Complete();

                return employee.Id;
            }
        }

        public void UpdateAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                UpdateAgentPerson(context, new InternalDictionaryAgentPerson(employee));

                var dbModel = DictionaryModelConverter.GetDbAgentEmployee(context, employee);

                dbContext.DictionaryAgentEmployeesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.Update);

                entity.State = EntityState.Modified;
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void DeleteAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                var dbModel = dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == employee.Id);
                dbContext.DictionaryAgentEmployeesSet.Remove(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.Delete);
                dbContext.SaveChanges();

                DeleteAgentUser(context, new InternalDictionaryAgentUser(employee));
                DeleteAgentPerson(context, new InternalDictionaryAgentPerson(employee));

                transaction.Complete();
            }
        }

        public FrontDictionaryAgentEmployee GetAgentEmployee(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentEmployee
                {
                    Id = x.Id,
                    PersonnelNumber = x.PersonnelNumber,
                    IsActive = x.IsActive,
                    Description = x.Description,
                    Name = x.Agent.Name,
                    FullName = x.Agent.AgentPerson.FullName,
                    FirstName = x.Agent.AgentPerson.FirstName,
                    LastName = x.Agent.AgentPerson.LastName,
                    MiddleName = x.Agent.AgentPerson.MiddleName,
                    TaxCode = x.Agent.AgentPerson.TaxCode,
                    IsMale = x.Agent.AgentPerson.IsMale,
                    PassportSerial = x.Agent.AgentPerson.PassportSerial,
                    PassportNumber = x.Agent.AgentPerson.PassportNumber,
                    PassportText = x.Agent.AgentPerson.PassportText,
                    PassportDate = x.Agent.AgentPerson.PassportDate,
                    BirthDate = x.Agent.AgentPerson.BirthDate,
                    LanguageId = x.Agent.AgentUser.LanguageId,

                    Contacts = x.Agent.AgentContacts.Select(y => new FrontDictionaryContact
                    {
                        Id = y.Id,
                        AgentId = y.AgentId,
                        ContactType = new FrontDictionaryContactType
                        {
                            Id = y.ContactType.Id,
                            Name = y.ContactType.Name,
                            Code = y.ContactType.Code,
                            InputMask = y.ContactType.InputMask,
                            IsActive = y.ContactType.IsActive
                        },
                        Value = y.Contact,
                        IsActive = y.IsActive,
                        Description = y.Description
                    }),
                    Addresses = x.Agent.AgentAddresses.Select(z => new FrontDictionaryAgentAddress
                    {
                        Id = z.Id,
                        AgentId = z.AgentId,
                        AddressType = new FrontDictionaryAddressType
                        {
                            Id = z.AddressType.Id,
                            Name = z.AddressType.Name,
                            IsActive = z.AddressType.IsActive
                        },
                        PostCode = z.PostCode,
                        Address = z.Address,
                        IsActive = z.IsActive,
                        Description = z.Description
                    })
                }).FirstOrDefault();

                transaction.Complete();
                return res;

            }
        }

        public int GetAgentEmployeePersonnelNumber(IContext context)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

        public IEnumerable<FrontDictionaryAgentEmployee> GetAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(context, dbContext, filter, paging);

                var res = qry.Select(x => new FrontDictionaryAgentEmployee
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Description = x.Description,
                    PersonnelNumber = x.PersonnelNumber,
                    Name = x.Agent.Name,
                    FullName = x.Agent.AgentPerson.FullName,
                    FirstName = x.Agent.AgentPerson.FirstName,
                    LastName = x.Agent.AgentPerson.LastName,
                    MiddleName = x.Agent.AgentPerson.MiddleName,

                    TaxCode = x.Agent.AgentPerson.TaxCode,
                    IsMale = x.Agent.AgentPerson.IsMale,
                    PassportSerial = x.Agent.AgentPerson.PassportSerial,
                    PassportNumber = x.Agent.AgentPerson.PassportNumber,
                    PassportText = x.Agent.AgentPerson.PassportText,
                    PassportDate = x.Agent.AgentPerson.PassportDate,
                    BirthDate = x.Agent.AgentPerson.BirthDate,

                    LanguageId = x.Agent.AgentUser.LanguageId,

                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetAgentEmployeeList(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentEmployeesQuery(context, dbContext, filter, paging);

                var res = qry.Select(x => new ListItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        private IQueryable<DictionaryAgentEmployees> GetWhereAgentEmployees(ref IQueryable<DictionaryAgentEmployees> qry, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {

            if (filter == null) return qry;

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
                    (current, value) => current.Or(e => e.Agent.AgentPerson.FullName.Contains(value)).Expand());

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
                qry = qry.Where(x => x.Agent.AgentPerson.FirstName == filter.FirstNameExact);
            }

            if (!string.IsNullOrEmpty(filter.LastNameExact))
            {
                qry = qry.Where(x => x.Agent.AgentPerson.LastName == filter.LastNameExact);
            }

            if (filter.PersonnelNumber != null)
            {
                qry = qry.Where(x => x.Agent.AgentEmployee.PersonnelNumber == filter.PersonnelNumber);
            }

            if (!string.IsNullOrEmpty(filter.Passport))
            {
                var filterContains = PredicateBuilder.False<DictionaryAgentEmployees>();
                filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Passport).Aggregate(filterContains,
                    (current, value) => current.Or(e => (e.Agent.AgentPerson.PassportSerial + "-" + e.Agent.AgentPerson.PassportNumber + " " +
                                      e.Agent.AgentPerson.PassportDate.ToString() + " " +
                                      e.Agent.AgentPerson.PassportText) == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (!string.IsNullOrEmpty(filter.TaxCode))
            {
                var filterContains = PredicateBuilder.False<DictionaryAgentEmployees>();
                filterContains = CommonFilterUtilites.GetWhereExpressions(filter.TaxCode).Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Agent.AgentPerson.TaxCode.Contains(value)).Expand());

                qry = qry.Where(filterContains);
            }

            if (!string.IsNullOrEmpty(filter.TaxCodeExact))
            {
                qry = qry.Where(x => x.Agent.AgentPerson.TaxCode == filter.TaxCodeExact);
            }

            // Поиск по дате рождения
            if (filter.BirthPeriod != null)
            {
                if (filter.BirthPeriod.IsActive)
                {
                    qry = qry.Where(x => x.Agent.AgentPerson.BirthDate >= filter.BirthPeriod.DateBeg);
                    qry = qry.Where(x => x.Agent.AgentPerson.BirthDate <= filter.BirthPeriod.DateEnd);
                }
            }

            // Поиск по дате рождения
            if (filter.BirthDateExact != null)
            {
                qry = qry.Where(x => x.Agent.AgentPerson.BirthDate == filter.BirthDateExact);
            }


            if (!string.IsNullOrEmpty(filter.PassportSerialExact))
            {
                qry = qry.Where(x => x.Agent.AgentPerson.PassportSerial == filter.PassportSerialExact);
            }

            if (filter.PassportNumberExact != null)
            {
                qry = qry.Where(x => x.Agent.AgentPerson.PassportNumber == filter.PassportNumberExact);
            }

            if (filter.RoleIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<AdminUserRoles>();
                filterContains = filter.RoleIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.RoleId == value).Expand());

                //qry = qry.Where(x => x.PositionExecutors.UserRoles.Any(y => filter.RoleIDs.Any(RoleId => y.RoleId == RoleId)));

                qry = qry.Where(x => x.PositionExecutors.Any(y => y.UserRoles.Any(z => filter.RoleIDs.Contains(z.RoleId))));
            }

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = qry.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    //pss !!!! Какой здесь заложен смысл
                    //return new List<FrontDictionaryAgentEmployee>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;
                    qry = qry.Skip(() => skip).Take(() => take);
                }
            }

            return qry;
        }

        public IQueryable<DictionaryAgentEmployees> GetAgentEmployeesQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            var qry = dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            qry = qry.OrderBy(x => x.Agent.AgentPerson.LastName);

            qry = GetWhereAgentEmployees(ref qry, filter, paging);

            return qry;
        }

        public bool ExistsAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = GetAgentEmployeesQuery(context, dbContext, filter, null).Any();
                transaction.Complete();
                return res;
            }
        }

        #endregion DictionaryAgentEmployee

        #region [+] DictionaryAgentUser ...
        public int AddAgentUser(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(context, User);
                dbContext.DictionaryAgentUsersSet.Add(dbModel);
                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentUsers, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                transaction.Complete();
                return User.Id;
            }
        }

        public void UpdateAgentUser(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(context, User);

                dbContext.DictionaryAgentUsersSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentUsers, EnumOperationType.Update);

                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void DeleteAgentUser(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = dbContext.DictionaryAgentUsersSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == User.Id);
                dbContext.DictionaryAgentUsersSet.Remove(dbModel);
                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentUsers, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void SetAgentUserLanguage(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

        public void SetAgentUserUserId(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
                    Contacts = x.Agent.AgentContacts.Select(y => new FrontDictionaryContact
                    {
                        Id = y.Id,
                        AgentId = y.AgentId,
                        ContactType = new FrontDictionaryContactType
                        {
                            Id = y.ContactType.Id,
                            Code = y.ContactType.Code,
                            Name = y.ContactType.Name,
                            IsActive = y.ContactType.IsActive
                        },
                        Value = y.Contact,
                        IsActive = y.IsActive,
                        Description = y.Description
                    }),
                    Addresses = x.Agent.AgentAddresses.Select(z => new FrontDictionaryAgentAddress
                    {
                        Id = z.Id,
                        AgentId = z.AgentId,
                        AddressType = new FrontDictionaryAddressType
                        {
                            Id = z.AddressType.Id,
                            Name = z.AddressType.Name,
                            IsActive = z.AddressType.IsActive
                        },
                        PostCode = z.PostCode,
                        Address = z.Address,
                        IsActive = z.IsActive,
                        Description = z.Description
                    })

                }).FirstOrDefault();
                transaction.Complete();
                return res;
            }
        }

        public InternalDictionaryAgentUser GetInternalAgentUser(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

        #region [+] DictionaryAgentAddress ...
        public FrontDictionaryAgentAddress GetAgentAddress(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Id == id);

                var res = qry.Select(x => new FrontDictionaryAgentAddress
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    AddressType = new FrontDictionaryAddressType { Id = x.AdressTypeId, Name = x.AddressType.Name, IsActive = x.AddressType.IsActive },
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAddress(context, addr);

                dbContext.DictionaryAgentAddressesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.Update);

                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var ddt = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == addr.Id);
                dbContext.DictionaryAgentAddressesSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public int AddAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

        public IEnumerable<FrontDictionaryAgentAddress> GetAgentAddresses(IContext context, int agentId, FilterDictionaryAgentAddress filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();


                qry = qry.Where(x => x.AgentId == filter.AgentId);

                qry = qry.OrderBy(x => x.AddressType.Code).ThenBy(x => x.Address);

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

                if (filter.AddressTypeId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
                    filterContains = filter.AddressTypeId.Aggregate(filterContains,
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
                    AddressType = new FrontDictionaryAddressType { Id = x.AddressType.Id, Code = x.AddressType.Code, Name = x.AddressType.Name },
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

        #region [+] DictionaryAddressTypes ...
        public int AddAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAddressType(context, addrType);

                dbContext.DictionaryAddressTypesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAddressType, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                addrType.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAddressType, EnumOperationType.Update);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        public void DeleteAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var ddt = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == addrType.Id);
                dbContext.DictionaryAddressTypesSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAddressType, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public bool ExistsAddressTypeSpecCode(IContext context, int addressTypeId)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId & x.Id == addressTypeId & x.SpecCode != null).Any();
                transaction.Complete();
                return res;
            }
        }

        public InternalDictionaryAddressType GetInternalDictionaryAddressType(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereAddressType(ref qry, filter);

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

        public IEnumerable<FrontDictionaryAddressType> GetAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereAddressType(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                var res = qry.Select(x => new FrontDictionaryAddressType
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

        private static IQueryable<DictionaryAddressTypes> GetWhereAddressType(ref IQueryable<DictionaryAddressTypes> qry, FilterDictionaryAddressType filter)
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

            return qry;
        }
        #endregion

        #region [+] DictionaryAgentClientCompanies ...
        public int AddAgentClientCompany(IContext context, InternalDictionaryAgentClientCompany company)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                if (ExistsAgents(context, new FilterDictionaryAgent() { IDs = new List<int> { company.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской 
                    UpdateAgentName(context, company.Id, new InternalDictionaryAgent(company));
                }
                else
                {
                    company.Id = AddAgent(context, new InternalDictionaryAgent(company));
                };

                DictionaryCompanies dc = DictionaryModelConverter.GetDbAgentClientCompany(context, company);
                dbContext.DictionaryAgentClientCompaniesSet.Add(dc);
                CommonQueries.AddFullTextCashInfo(dbContext, dc.Id, EnumObjects.DictionaryAgentClientCompanies, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                company.Id = dc.Id;

                transaction.Complete();

                return company.Id;
            }

        }

        public void UpdateAgentClientCompany(IContext context, InternalDictionaryAgentClientCompany company)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                UpdateAgentName(context, company.Id, new InternalDictionaryAgent(company));

                DictionaryCompanies drj = DictionaryModelConverter.GetDbAgentClientCompany(context, company);
                dbContext.DictionaryAgentClientCompaniesSet.Attach(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryAgentClientCompanies, EnumOperationType.Update);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void DeleteAgentClientCompany(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var departments = GetDepartmentIDs(context, new FilterDictionaryDepartment() { CompanyIDs = list });

                if (departments.Count > 0) DeleteDepartments(context, departments);

                dbContext.DictionaryAgentClientCompaniesSet.RemoveRange(
                    dbContext.DictionaryAgentClientCompaniesSet.
                    Where(x => x.ClientId == context.CurrentClientId).
                    Where(x => list.Contains(x.Id)));
                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryAgentClientCompanies, EnumOperationType.Delete);
                dbContext.SaveChanges();

                DeleteAgentIfNoAny(context, list);

                transaction.Complete();
            }
        }

        public InternalDictionaryAgentClientCompany GetInternalAgentClientCompany(IContext context, FilterDictionaryAgentClientCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentClientCompaniesQuery(context, dbContext, filter);

                var res = qry.Select(x => new InternalDictionaryAgentClientCompany
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

        public IEnumerable<FrontDictionaryAgentClientCompany> GetAgentClientCompanies(IContext context, FilterDictionaryAgentClientCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentClientCompaniesQuery(context, dbContext, filter);

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


        public IEnumerable<TreeItem> GetAgentClientCompaniesForStaffList(IContext context, FilterDictionaryAgentClientCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentClientCompaniesQuery(context, dbContext, filter);

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

        public IEnumerable<TreeItem> GetAgentClientCompaniesForDIPSubordinations(IContext context, int positionId, FilterDictionaryAgentClientCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentClientCompaniesQuery(context, dbContext, filter);

                var objId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPSubordinationsClientCompany
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

        public IEnumerable<TreeItem> GetAgentClientCompaniesForDIPRJournalPositions(IContext context, int positionId, FilterDictionaryAgentClientCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentClientCompaniesQuery(context, dbContext, filter);

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


        public IQueryable<DictionaryCompanies> GetAgentClientCompaniesQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentClientCompany filter)
        {
            var qry = dbContext.DictionaryAgentClientCompaniesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            qry = GetWhereAgentClientCompany(ref qry, filter);

            qry = qry.OrderBy(x => x.Agent.Name);

            return qry;
        }

        public bool ExistsAgentClientCompanies(IContext context, FilterDictionaryAgentClientCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = GetAgentClientCompaniesQuery(context, dbContext, filter).Any();
                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryCompanies> GetWhereAgentClientCompany(ref IQueryable<DictionaryCompanies> qry, FilterDictionaryAgentClientCompany filter)
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

            return qry;
        }

        #endregion DictionaryCompanies

        #region [+] DictionaryAgentCompanies ...
        public int AddAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                if (ExistsAgents(context, new FilterDictionaryAgent() { IDs = new List<int> { company.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской 
                    UpdateAgentName(context, company.Id, new InternalDictionaryAgent(company));
                }
                else
                {
                    company.Id = AddAgent(context, new InternalDictionaryAgent(company));
                };

                var dbModel = DictionaryModelConverter.GetDbAgentCompany(context, company);

                dbContext.DictionaryAgentCompaniesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                transaction.Complete();

                return company.Id;
            }
        }

        public void UpdateAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                UpdateAgentName(context, company.Id, new InternalDictionaryAgent(company));

                var dbModel = DictionaryModelConverter.GetDbAgentCompany(context, company);

                dbContext.DictionaryAgentCompaniesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                transaction.Complete();

            }

        }

        public void DeleteAgentCompanies(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                dbContext.DictionaryAgentCompaniesSet.RemoveRange(dbContext.DictionaryAgentCompaniesSet.
                Where(x => x.Agent.ClientId == context.CurrentClientId).
                Where(x => list.Contains(x.Id)));
                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryAgentCompanies, EnumOperationType.Delete);
                dbContext.SaveChanges();

                DeleteAgentIfNoAny(context, list);

                transaction.Complete();
            }
        }

        public FrontDictionaryAgentCompany GetAgentCompany(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                var res =
                    dbContext.DictionaryAgentCompaniesSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentCompany
                    {
                        Id = x.Id,
                        ResidentTypeId = x.Agent.ResidentTypeId,
                        FullName = x.FullName,
                        Name = x.Agent.Name,
                        TaxCode = x.TaxCode,
                        OKPOCode = x.OKPOCode,
                        VATCode = x.VATCode,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        Contacts = x.Agent.AgentContacts.Select(y => new FrontDictionaryContact
                        {
                            Id = y.Id,
                            AgentId = y.AgentId,
                            ContactType = new FrontDictionaryContactType
                            {
                                Id = y.ContactType.Id,
                                Name = y.ContactType.Name,
                                Code = y.ContactType.Code,
                                InputMask = y.ContactType.InputMask,
                                IsActive = y.ContactType.IsActive
                            },
                            Value = y.Contact,
                            IsActive = y.IsActive,
                            Description = y.Description
                        }),
                        Addresses = x.Agent.AgentAddresses.Select(z => new FrontDictionaryAgentAddress
                        {
                            Id = z.Id,
                            AgentId = z.AgentId,
                            AddressType = new FrontDictionaryAddressType
                            {
                                Id = z.AddressType.Id,
                                Name = z.AddressType.Name,
                                IsActive = z.AddressType.IsActive
                            },
                            PostCode = z.PostCode,
                            Address = z.Address,
                            IsActive = z.IsActive,
                            Description = z.Description
                        }),
                        ContactsPersons = x.AgentPersons.Select(t => new FrontDictionaryAgentPerson
                        {
                            Id = t.Id,
                            FirstName = t.FirstName,
                            LastName = t.LastName,
                            MiddleName = t.MiddleName
                        })

                    }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryAgentCompany> GetAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentCompaniesQuery(context, dbContext, filter, paging);

                var res = qry.Select(x => new FrontDictionaryAgentCompany
                {
                    Id = x.Id,
                    ResidentTypeId = x.Agent.ResidentTypeId,
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


        public bool ExistsAgentCompanies(IContext context, FilterDictionaryAgentCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = GetAgentCompaniesQuery(context, dbContext, filter, null).Any();
                transaction.Complete();
                return res;
            }
        }

        public IQueryable<DictionaryAgentCompanies> GetAgentCompaniesQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            var qry = dbContext.DictionaryAgentCompaniesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            qry = qry.OrderBy(x => x.Agent.Name);

            qry = GetWhereAgentCompanies(ref qry, filter, paging);

            return qry;
        }

        private IQueryable<DictionaryAgentCompanies> GetWhereAgentCompanies(ref IQueryable<DictionaryAgentCompanies> qry, FilterDictionaryAgentCompany filter, UIPaging paging)
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

            // Пагинация
            qry = qry.OrderBy(x => x.FullName);

            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = qry.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    //pss !!!! Какой здесь заложен смысл
                    //return new List<FrontDictionaryAgentCompany>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    qry = qry.Skip(() => skip).Take(() => take);
                }
            }

            return qry;
        }

        #endregion DictionaryAgentCompanies

        #region [+] DictionaryAgentBanks ...

        public int AddAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                if (ExistsAgents(context, new FilterDictionaryAgent() { IDs = new List<int> { bank.Id } }))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской
                    UpdateAgentName(context, bank.Id, new InternalDictionaryAgent(bank));
                }
                else
                {
                    bank.Id = AddAgent(context, new InternalDictionaryAgent(bank));
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                UpdateAgentName(context, bank.Id, new InternalDictionaryAgent(bank));

                var dbModel = DictionaryModelConverter.GetDbAgentBank(context, bank);

                dbContext.DictionaryAgentBanksSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void DeleteAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var ddt = dbContext.DictionaryAgentBanksSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == bank.Id);
                dbContext.DictionaryAgentBanksSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.Delete);
                dbContext.SaveChanges();

                DeleteAgentIfNoAny(context, new List<int>() { bank.Id });

                transaction.Complete();
            }
        }

        public FrontDictionaryAgentBank GetAgentBank(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                var res =
                    dbContext.DictionaryAgentBanksSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentBank
                    {
                        Id = x.Id,
                        MFOCode = x.MFOCode,
                        Swift = x.Swift,
                        Name = x.Agent.Name,
                        FullName = x.FullName,
                        ResidentTypeId = x.Agent.ResidentTypeId,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        Contacts = x.Agent.AgentContacts.Select(y => new FrontDictionaryContact
                        {
                            Id = y.Id,
                            AgentId = y.AgentId,
                            ContactType = new FrontDictionaryContactType
                            {
                                Id = y.ContactType.Id,
                                Name = y.ContactType.Name,
                                Code = y.ContactType.Code,
                                InputMask = y.ContactType.InputMask,
                                IsActive = y.ContactType.IsActive
                            },
                            Value = y.Contact,
                            IsActive = y.IsActive,
                            Description = y.Description
                        }),
                        Addresses = x.Agent.AgentAddresses.Select(z => new FrontDictionaryAgentAddress
                        {
                            Id = z.Id,
                            AgentId = z.AgentId,
                            AddressType = new FrontDictionaryAddressType
                            {
                                Id = z.AddressType.Id,
                                Name = z.AddressType.Name,
                                IsActive = z.AddressType.IsActive
                            },
                            PostCode = z.PostCode,
                            Address = z.Address,
                            IsActive = z.IsActive,
                            Description = z.Description
                        })

                    }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryAgentBank> GetAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentBanksQuery(context, dbContext, filter, paging);

                var res = qry.Select(x => new FrontDictionaryAgentBank
                {
                    Id = x.Id,
                    MFOCode = x.MFOCode,
                    Swift = x.Swift,
                    Name = x.Agent.Name,
                    FullName = x.FullName,
                    ResidentTypeId = x.Agent.ResidentTypeId,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public bool ExistsAgentBanks(IContext context, FilterDictionaryAgentBank filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = GetAgentBanksQuery(context, dbContext, filter, null).Any();
                transaction.Complete();
                return res;
            }
        }

        public IQueryable<DictionaryAgentBanks> GetAgentBanksQuery(IContext context, DmsContext dbContext, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            var qry = dbContext.DictionaryAgentBanksSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

            qry = qry.OrderBy(x => x.Agent.Name);

            qry = GetWhereAgentBanks(ref qry, filter, paging);

            return qry;
        }

        private IQueryable<DictionaryAgentBanks> GetWhereAgentBanks(ref IQueryable<DictionaryAgentBanks> qry, FilterDictionaryAgentBank filter, UIPaging paging)
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

            // Пагинация
            if (paging != null)
            {
                if (paging.IsOnlyCounter ?? true)
                {
                    paging.TotalItemsCount = qry.Count();
                }

                if (paging.IsOnlyCounter ?? false)
                {
                    //pss !!!! Какой здесь заложен смысл
                    //return new List<FrontDictionaryAgentBank>();
                }

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    qry = qry.Skip(() => skip).Take(() => take);
                }
            }

            if (paging != null)
            {
                paging.TotalItemsCount = qry.Count();

                if (!paging.IsAll)
                {
                    var skip = paging.PageSize * (paging.CurrentPage - 1);
                    var take = paging.PageSize;

                    qry = qry.Skip(() => skip).Take(() => take);
                }
            }

            return qry;
        }

        #endregion DictionaryAgentBanks

        #region [+] DictionaryAgentAccounts ...
        public FrontDictionaryAgentAccount GetAgentAccount(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
                        Bank = new FrontDictionaryAgentBank
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAccount(context, account);

                dbContext.DictionaryAgentAccountsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public void DeleteAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == account.Id);
                dbContext.DictionaryAgentAccountsSet.Remove(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public int AddAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAccount(context, account);

                dbContext.DictionaryAgentAccountsSet.Add(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                transaction.Complete();
                return account.Id;
            }
        }

        public IEnumerable<InternalDictionaryAgentAccount> GetInternalAgentAccounts(IContext context, FilterDictionaryAgentAccount filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
                    Bank = new FrontDictionaryAgentBank
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

            qry = AgentAccountsGetWhere(ref qry, filter);

            return qry;
        }

        public bool ExistsAgentAccounts(IContext context, FilterDictionaryAgentAccount filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetAgentAccountsQuery(context, dbContext, filter);

                var res = qry.Any();

                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryAgentAccounts> AgentAccountsGetWhere(ref IQueryable<DictionaryAgentAccounts> qry, FilterDictionaryAgentAccount filter)
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

            if (filter.AgentId.HasValue)
            {
                qry = qry.Where(x => x.AgentId == filter.AgentId);
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

            return qry;
        }

        #endregion DictionaryAgentAccounts

        #region [+] DictionaryContactTypes ...
        public FrontDictionaryContactType GetInternalDictionaryContactType(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryContactTypes>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.True<DictionaryContactTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.And(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    var filterContains = PredicateBuilder.False<DictionaryContactTypes>();
                    filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Name).Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Name.Contains(value)).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!String.IsNullOrEmpty(filter.Code))
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

                if (!String.IsNullOrEmpty(filter.NameExact))
                {

                    qry = qry.Where(x => x.Name == filter.NameExact);

                }

                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

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
        public void UpdateContactType(IContext context, InternalDictionaryContactType model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContactType, EnumOperationType.Update);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public void DeleteContactType(IContext context, InternalDictionaryContactType contactType)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var ddt = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == contactType.Id);
                dbContext.DictionaryContactTypesSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContactType, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public bool ExistsContactTypeSpecCode(IContext context, int contactTypeId)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId & x.Id == contactTypeId & x.SpecCode != null).Any();
                transaction.Complete();
                return res;
            }
        }

        public int AddContactType(IContext context, InternalDictionaryContactType model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContactType(context, model);

                dbContext.DictionaryContactTypesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContactType, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }
        public IEnumerable<FrontDictionaryContactType> GetContactTypes(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

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

        public string GetContactTypeSpecCode(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
        #endregion

        #region [+] DictionaryAgentContacts ...

        public int AddContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContact(context, contact);

                dbContext.DictionaryAgentContactsSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContacts, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                contact.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbContact(context, contact);

                dbContext.DictionaryAgentContactsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContacts, EnumOperationType.Update);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }
        public void DeleteContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var ddt = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == contact.Id);
                dbContext.DictionaryAgentContactsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContacts, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        public FrontDictionaryContact GetContact(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Id == id);

                var res = qry.Select(x => new FrontDictionaryContact
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    ContactType = new FrontDictionaryContactType { Id = x.ContactTypeId, Name = x.ContactType.Name, Code = x.ContactType.Code, IsActive = x.ContactType.IsActive },
                    Value = x.Contact,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    IsConfirmed = x.IsConfirmed
                }).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDictionaryContact> GetContacts(IContext context, FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetContactsQuery(context, dbContext, filter);

                var res = qry.Select(x => new FrontDictionaryContact
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    ContactType = new FrontDictionaryContactType { Id = x.ContactTypeId, Name = x.ContactType.Name, Code = x.ContactType.Code, IsActive = x.ContactType.IsActive },
                    Value = x.Contact,
                    Description = x.Description,
                    IsActive = x.IsActive
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDictionaryContact> GetInternalContacts(IContext context, FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

        private IQueryable<DictionaryAgentContacts> GetWhereContacts(ref IQueryable<DictionaryAgentContacts> qry, FilterDictionaryContact filter)
        {
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

            return qry;
        }

        public IQueryable<DictionaryAgentContacts> GetContactsQuery(IContext context, DmsContext dbContext, FilterDictionaryContact filter)
        {
            var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

            qry = qry.OrderBy(x => x.ContactType.Id).ThenBy(x => x.Contact);

            qry = GetWhereContacts(ref qry, filter);

            return qry;
        }

        public IEnumerable<int> GetAgentsIDByContacts(IContext context, IEnumerable<int> contacts)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

        #region [+] DictionaryDepartments ...
        public int AddDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dd = DictionaryModelConverter.GetDbDepartments(context, department);
                dbContext.DictionaryDepartmentsSet.Add(dd);
                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                department.Id = dd.Id;
                transaction.Complete();
                return dd.Id;
            }
        }

        public void UpdateDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dd = DictionaryModelConverter.GetDbDepartments(context, department);
                dbContext.DictionaryDepartmentsSet.Attach(dd);

                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.Update);
                dbContext.Entry(dd).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void UpdateDepartmentCode(IContext context, string codePreffix, FilterDictionaryDepartment filter)
        {
            if (string.IsNullOrEmpty(codePreffix)) return;

            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereDepartments(ref qry, filter);

                qry.Update(x => new DictionaryDepartments { FullPath = codePreffix + "/" + x.Code });

                transaction.Complete();
            }
        }

        public void DeleteDepartments(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            {
                var childDepartments = GetDepartmentIDs(context, new FilterDictionaryDepartment() { ParentIDs = list });

                if (childDepartments.Count > 0) DeleteDepartments(context, childDepartments);

                using (var transaction = GetTransaction())
                {
                    var positions = GetPositionIDs(context, new FilterDictionaryPosition() { DepartmentIDs = list });

                    if (positions.Count() > 0) DeletePositions(context, positions.ToList());

                    dbContext.DictionaryDepartmentsSet.RemoveRange(dbContext.DictionaryDepartmentsSet.
                        Where(x => x.Company.ClientId == context.CurrentClientId).
                        Where(x => list.Contains(x.Id)));
                    CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryDepartments, EnumOperationType.Delete);
                    dbContext.SaveChanges();

                    transaction.Complete();
                }
            }
        }

        public IEnumerable<InternalDictionaryDepartment> GetInternalDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereDepartments(ref qry, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereDepartments(ref qry, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                var res = qry.Select(x => x.Id).ToList();

                transaction.Complete();
                return res;
            }
        }

        public string GetDepartmentPrefix(IContext context, int parentId)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

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

        public IEnumerable<TreeItem> GetDepartmentsForDIPSubordinations(IContext context, int sourcePositionId, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                var res = qry.Select(x => new FrontDIPSubordinationsDepartment
                {
                    Id = x.Id,
                    Code = x.FullPath,
                    Name = x.Name,
                    SearchText = x.Name,
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

        public IEnumerable<TreeItem> GetDepartmentsForDIPRJournalPositions(IContext context, int positionId, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

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

        public IQueryable<DictionaryDepartments> GetDepartmentsQuery(IContext context, DmsContext dbContext, FilterDictionaryDepartment filter)
        {
            var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).AsQueryable();

            qry = GetWhereDepartments(ref qry, filter);

            qry = qry.OrderBy(x => x.Code).ThenBy(x => x.Name);

            return qry;
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsDictionaryDepartment(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                return qry.Select(x => new FrontDictionaryDepartment { Id = x.Id }).Any();
            }
        }

        private static IQueryable<DictionaryDepartments> GetWhereDepartments(ref IQueryable<DictionaryDepartments> qry, FilterDictionaryDepartment filter)
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

            return qry;
        }
        #endregion DictionaryDepartments

        #region [+] DictionaryDocumentDirections ...

        public IEnumerable<FrontDictionaryDocumentDirection> GetDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryDocumentDirectionsSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDocumentDirections>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                var res = qry.Select(x => new FrontDictionaryDocumentDirection
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion DictionaryDepartments

        #region [+] DictionaryDocumentSubjects ...

        public int AddDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dds = DictionaryModelConverter.GetDbDocumentSubject(context, docSubject);
                dbContext.DictionaryDocumentSubjectsSet.Add(dds);
                CommonQueries.AddFullTextCashInfo(dbContext, dds.Id, EnumObjects.DictionaryDocumentSubjects, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                docSubject.Id = dds.Id;
                transaction.Complete();
                return dds.Id;
            }
        }

        public void UpdateDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

                CommonQueries.AddFullTextCashInfo(dbContext, dds.Id, EnumObjects.DictionaryDocumentSubjects, EnumOperationType.Update);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var ddt = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == docSubject.Id);
                dbContext.DictionaryDocumentSubjectsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentSubjects, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDictionaryDocumentSubject> GetInternalDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                var qry = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereDocumentSubjects(ref qry, filter);

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

            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                //IQueryable<DictionaryDocumentSubjects> qry=

                var qry = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereDocumentSubjects(ref qry, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereDocumentSubjects(ref qry, filter);

                var res = qry.Any();
                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryDocumentSubjects> GetWhereDocumentSubjects(ref IQueryable<DictionaryDocumentSubjects> qry, FilterDictionaryDocumentSubject filter)
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

            return qry;
        }

        #endregion DictionaryDocumentSubjects

        #region [+] DictionaryDocumentTypes ...
        public int AddDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbDocumentType(context, docType);
                dbContext.DictionaryDocumentTypesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                docType.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbDocumentType(context, docType);
                dbContext.DictionaryDocumentTypesSet.Attach(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.Update);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var ddt = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == docType.Id);
                dbContext.DictionaryDocumentTypesSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDictionaryDocumentType> GetInternalDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereDocumentTypes(ref qry, filter);

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

        public IEnumerable<FrontDictionaryDocumentType> GetDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereDocumentTypes(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

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

        private static IQueryable<DictionaryDocumentTypes> GetWhereDocumentTypes(ref IQueryable<DictionaryDocumentTypes> qry, FilterDictionaryDocumentType filter)
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

            return qry;
        }
        #endregion DictionaryDocumentTypes

        #region [+] DictionaryEventTypes ...
        public IEnumerable<FrontDictionaryEventType> GetEventTypes(IContext context, FilterDictionaryEventType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryEventTypesSet.AsQueryable();

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

                return qry.Select(x => new FrontDictionaryEventType
                {
                    EventType = (EnumEventTypes)x.Id,
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ImportanceEventTypeId = x.ImportanceEventTypeId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    ImportanceEventTypeName = x.ImportanceEventType.Name
                }).ToList();
            }
        }
        #endregion DictionaryEventTypes

        #region [+] DictionaryImportanceEventTypes ...
        public IEnumerable<FrontDictionaryImportanceEventType> GetImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryImportanceEventTypesSet.AsQueryable();

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

                var res = qry.Select(x => new FrontDictionaryImportanceEventType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion DictionaryImportanceEventTypes

        #region [+] DictionaryLinkTypes ...
        public IEnumerable<FrontDictionaryLinkType> GetLinkTypes(IContext context, FilterDictionaryLinkType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryLinkTypesSet.AsQueryable();

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

                var res = qry.Select(x => new FrontDictionaryLinkType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsImportant = x.IsImportant,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion DictionaryLinkTypes

        #region [+] DictionaryPositions ...
        public int AddPosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dd = DictionaryModelConverter.GetDbPosition(context, position);
                dbContext.DictionaryPositionsSet.Add(dd);
                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryPositions, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                UpdatePositionExecutor(context, new List<int> { dd.Id});
                position.Id = dd.Id;
                transaction.Complete();
                return dd.Id;
            }
        }

        public void UpdatePosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryPositions, EnumOperationType.Update);
                dbContext.SaveChanges();
                UpdatePositionExecutor(context, new List<int> { position.Id });
                transaction.Complete();
            }
        }

        public void UpdatePositionOrder(IContext context, int positionId, int order)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId);
                if (positionId?.Any()??false)
                    qry = qry.Where(x=> positionId.Contains(x.Id));
                var posUpd = qry.Select(x => new
                {
                    x.Id,   oldExecutorAgentId = x.ExecutorAgentId ?? 0,
                    newExecutorAgentId = dbContext.DictionaryPositionExecutorsSet
                        .Where(y=>  y.PositionId == x.Id && DateTime.UtcNow >= y.StartDate && DateTime.UtcNow <= y.EndDate 
                                    && (y.PositionExecutorTypeId == (int)EnumPositionExecutionTypes.IO || y.PositionExecutorTypeId == (int)EnumPositionExecutionTypes.Personal))
                        .OrderBy(y=>y.PositionExecutorTypeId).Select(y=>y.AgentId).FirstOrDefault()
                }).Where(x=> x.newExecutorAgentId != x.oldExecutorAgentId)
                .ToDictionary(x=>x.Id, y => y.newExecutorAgentId != 0 ? y.newExecutorAgentId : (int?)null);
                if (posUpd.Any())
                    foreach (var pos in posUpd)
                    {
                        var id = pos.Key;
                        var agentId = pos.Value;
                        dbContext.DictionaryPositionsSet.Where(x => x.Id == id).Update(x => new DictionaryPositions { ExecutorAgentId = agentId });
                    }
                transaction.Complete();
            }
        }


        public void DeletePositions(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
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
                //#region [+] DictionaryStandartSendListContents ...
                //var filterStandartSendListContents = PredicateBuilder.False<DictionaryStandartSendListContents>();
                //filterStandartSendListContents = list.Aggregate(filterStandartSendListContents,
                //    (current, value) => current.Or(e => e.TargetPositionId == value).Expand());

                //dbContext.DictionaryStandartSendListContentsSet.Where(filterStandartSendListContents).Delete();
                //#endregion

                // Удаляю руководителей подразделений
                #region [+] DictionaryDepartments ...
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
                #region [+] DictionaryPositionExecutors ...
                var filterPositionExecutors = PredicateBuilder.False<DictionaryPositionExecutors>();
                filterPositionExecutors = list.Aggregate(filterPositionExecutors,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                dbContext.DictionaryPositionExecutorsSet.Where(filterPositionExecutors).Delete();
                #endregion

                // удаляю саму должность
                dbContext.DictionaryPositionsSet.RemoveRange(dbContext.DictionaryPositionsSet.
                    Where(x => x.Department.Company.ClientId == context.CurrentClientId).
                    Where(x => list.Contains(x.Id)));
                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryPositions, EnumOperationType.Delete);
                dbContext.SaveChanges();

                transaction.Complete();
            }
        }

        public int? GetExecutorAgentIdByPositionId(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryPositionsSet.AsQueryable();
                if (!context.IsAdmin)
                {
                    qry = qry.Where(x => x.Department.Company.ClientId == context.CurrentClientId);
                }
                var res = qry.Where(x => x.Id == id).Select(x => x.ExecutorAgentId).FirstOrDefault();

                transaction.Complete();
                return res;
            }
        }

        public FrontDictionaryPosition GetPosition(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
                        ExecutorAgentName = x.ExecutorAgent.Name,
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {

                var qry = GetPositionsQuery(context, dbContext, filter);

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
                    ExecutorAgentName = x.ExecutorAgent.Name,
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);
                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<ListItem> GetPositionList(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);
                var res = qry.Select(x => new ListItem
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontDIPSubordinationsPosition> GetPositionsForStaffList(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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

        public IEnumerable<SortPositoin> GetPositionsForSort(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

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

            qry = qry.OrderBy(x => x.DepartmentId).ThenBy(x => x.Order).ThenBy(x => x.Name);

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

        #endregion DictionaryPositions

        #region [+] DictionaryPositionExecutors ...
        public int AddExecutor(IContext context, InternalDictionaryPositionExecutor executor)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                DictionaryPositionExecutors dc = DictionaryModelConverter.GetDbExecutor(context, executor);
                dbContext.DictionaryPositionExecutorsSet.Add(dc);
                CommonQueries.AddFullTextCashInfo(dbContext, dc.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                executor.Id = dc.Id;
                UpdatePositionExecutor(context, new List<int> { dc.PositionId });
                transaction.Complete();
                return dc.Id;
            }
        }

        public void UpdateExecutor(IContext context, InternalDictionaryPositionExecutor executor)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                DictionaryPositionExecutors drj = DictionaryModelConverter.GetDbExecutor(context, executor);
                dbContext.DictionaryPositionExecutorsSet.Attach(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.Update);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                UpdatePositionExecutor(context, new List<int> { executor.PositionId });
                transaction.Complete();
            }
        }

        public void DeleteExecutors(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                dbContext.DictionaryPositionExecutorsSet.RemoveRange(dbContext.DictionaryPositionExecutorsSet.
                    Where(x => x.Position.Department.Company.ClientId == context.CurrentClientId).
                    Where(x => list.Contains(x.Id)));
                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryPositionExecutors, EnumOperationType.Delete);
                dbContext.SaveChanges();
                UpdatePositionExecutor(context, list);
                transaction.Complete();
            }
        }

        public IEnumerable<FrontDictionaryPositionExecutor> GetPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

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
                    PositionExecutorTypeName = x.PositionExecutorType.Name
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<InternalDictionaryPositionExecutor> GetInternalPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

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
                    EndDate = x.EndDate == maxDateTime ? (DateTime?)null : x.EndDate,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public List<int> GetPositionExecutorsIDs(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);
                var res = qry.Select(x => x.Id).ToList();
                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<TreeItem> GetPositionExecutorsForTree(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

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



        public IEnumerable<FrontDictionaryPositionExecutor> GetPositionExecutorsDIPUserRoles(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                string objId = ((int)EnumObjects.DictionaryPositionExecutors).ToString();
                string parObjId = string.Empty;

                var res = qry.Select(x => new FrontDictionaryPositionExecutor
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
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

            qry = qry.OrderBy(x => x.Position.Order).ThenBy(x => x.PositionExecutorType.Id).ThenBy(x => x.Agent.Name);

            qry = GetWherePositionExecutors(ref qry, filter);

            return qry;
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsPositionExecutor(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                var res = qry.Select(x => new FrontDictionaryPositionExecutor { Id = x.Id }).Any();

                transaction.Complete();
                return res;
            }
        }

        private static IQueryable<DictionaryPositionExecutors> GetWherePositionExecutors(ref IQueryable<DictionaryPositionExecutors> qry, FilterDictionaryPositionExecutor filter)
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

            return qry;
        }

        #endregion DictionaryPositionExecutors

        #region [+] DictionaryPositionExecutorTypes ...
        public IEnumerable<InternalDictionaryPositionExecutorType> GetInternalDictionaryPositionExecutorType(IContext context, FilterDictionaryPositionExecutorType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryPositionExecutorTypesSet.AsQueryable();

                qry = GetWhereExecutorTypes(ref qry, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetPositionExecutorTypesQuery(context, dbContext, filter);

                var res = qry.Select(x => new FrontDictionaryPositionExecutorType
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Code = x.Code,
                    Name = x.Name
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IQueryable<DictionaryPositionExecutorTypes> GetPositionExecutorTypesQuery(IContext context, DmsContext dbContext, FilterDictionaryPositionExecutorType filter)
        {
            var qry = dbContext.DictionaryPositionExecutorTypesSet.AsQueryable();

            if (filter.Period?.IsActive == true && filter.PositionId != null)
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

            qry = GetWhereExecutorTypes(ref qry, filter);

            return qry;
        }

        private static IQueryable<DictionaryPositionExecutorTypes> GetWhereExecutorTypes(ref IQueryable<DictionaryPositionExecutorTypes> qry, FilterDictionaryPositionExecutorType filter)
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

            return qry;
        }

        #endregion DictionaryPositionExecutorTypes

        #region [+] DictionaryRegistrationJournals ...
        public int AddRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal regJournal)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                DictionaryRegistrationJournals drj = DictionaryModelConverter.GetDbRegistrationJournal(context, regJournal);
                dbContext.DictionaryRegistrationJournalsSet.Add(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                regJournal.Id = drj.Id;
                transaction.Complete();
                return drj.Id;
            }
        }

        public void UpdateRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal regJournal)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                DictionaryRegistrationJournals drj = DictionaryModelConverter.GetDbRegistrationJournal(context, regJournal);
                dbContext.DictionaryRegistrationJournalsSet.Attach(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Update);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal docSubject)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var drj = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == docSubject.Id);
                dbContext.DictionaryRegistrationJournalsSet.Remove(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalDictionaryRegistrationJournal> GetInternalDictionaryRegistrationJournal(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter);

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



        public IEnumerable<FrontDictionaryRegistrationJournal> GetRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter);

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

        public IEnumerable<TreeItem> GetRegistrationJournalsForRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter);

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


        public IEnumerable<TreeItem> GetRegistrationJournalsForDIPRJournalPositions(IContext context, int positionId, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = GetRegistrationJournalsQuery(context, dbContext, filter);

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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

            qry = GetWhereRegistrationJournal(context, dbContext, ref qry, filter);

            qry = qry.OrderBy(x => x.Name);

            return qry;
        }

        private static IQueryable<DictionaryRegistrationJournals> GetWhereRegistrationJournal(IContext context, DmsContext dbContext, ref IQueryable<DictionaryRegistrationJournals> qry, FilterDictionaryRegistrationJournal filter)
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

            // журналы отдела в котором работает должность
            if (filter.DepartmentByPositionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<DictionaryPositions>();
                filterContains = filter.DepartmentByPositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id == value).Expand());

                qry = qry.Where(x => x.Department.Positions.AsQueryable().Any(filterContains));
            }

            // Условие по IsIncoming
            if (filter.IsIncoming != null)
            {
                qry = qry.Where(x => x.DirectionCodes.Contains(EnumDocumentDirections.Incoming.ToString()));
            }

            // Условие по IsOutcoming
            if (filter.IsOutcoming != null)
            {
                qry = qry.Where(x => x.DirectionCodes.Contains(EnumDocumentDirections.Outcoming.ToString()));
            }

            // Условие по IsInternal
            if (filter.IsInternal != null)
            {
                qry = qry.Where(x => x.DirectionCodes.Contains(EnumDocumentDirections.Internal.ToString()));
            }

            if (filter.PositionIdsAccessForRegistration?.Count > 0)
            {
                var filterPositionsIdList = PredicateBuilder.False<AdminRegistrationJournalPositions>();
                filterPositionsIdList = filter.PositionIdsAccessForRegistration.Aggregate(filterPositionsIdList, (current, value) => current.Or(e => e.PositionId == value).Expand());
                qry = qry.Where(x => dbContext.AdminRegistrationJournalPositionsSet
                                            .Where(filterPositionsIdList).Where(y => y.RegJournalAccessTypeId == (int)EnumRegistrationJournalAccessTypes.Registration)
                                            .Select(y => y.RegJournalId).Contains(x.Id));
            }

            return qry;
        }

        #endregion DictionaryRegistrationJournals

        #region [+] DictionaryResultTypes ...
        public IEnumerable<FrontDictionaryResultType> GetResultTypes(IContext context, FilterDictionaryResultType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryResultTypesSet.AsQueryable();

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

                return qry.Select(x => new FrontDictionaryResultType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsExecute = x.IsExecute,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                }).ToList();
            }
        }
        #endregion DictionaryResultTypes

        #region [+] DictionarySendTypes ...
        public IEnumerable<FrontDictionarySendType> GetSendTypes(IContext context, FilterDictionarySendType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionarySendTypesSet.AsQueryable();

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

                var res = qry.Select(x => new FrontDictionarySendType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsImportant = x.IsImportant,
                    SubordinationType = (EnumSubordinationTypes)x.SubordinationTypeId,
                    SubordinationTypeName = x.SubordinationType.Name,
                    IsExternal = x.Id == 45,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion DictionarySendTypes

        #region [+] DictionaryStandartSendListContents ...

        public IEnumerable<FrontDictionaryStandartSendListContent> GetStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).AsQueryable();

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

                var res = qry.Select(x => new FrontDictionaryStandartSendListContent
                {
                    Id = x.Id,
                    StandartSendListId = x.StandartSendListId,
                    Stage = x.Stage,
                    SendTypeId = x.SendTypeId,
                    TargetPositionId = x.TargetPositionId,
                    Task = x.Task,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevelId = x.AccessLevelId,
                    SendTypeName = x.SendType.Name,
                    TargetPositionName = x.TargetPosition.Name,
                    TargetAgentName = x.TargetPosition.ExecutorAgent.Name ?? x.TargetAgent.Name,
                    AccessLevelName = x.AccessLevel.Name,
                    SendTypeIsExternal = x.SendTypeId == 45
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public int AddStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendListContent(context, content);
                dbContext.DictionaryStandartSendListContentsSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                content.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendListContent(context, content);
                dbContext.DictionaryStandartSendListContentsSet.Attach(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.Update);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent content)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var ddt = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == content.Id);
                dbContext.DictionaryStandartSendListContentsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        #endregion DictionaryStandartSendListContents

        #region [+] DictionaryStandartSendLists ...
        public IEnumerable<FrontDictionaryStandartSendList> GetStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                // Список первичных ключей
                if (filter.IDs != null && filter.IDs.Count > 0)
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
                var res = qry.Select(x => new FrontDictionaryStandartSendList
                {
                    Id = x.Id,
                    Name = x.Name,
                    PositionId = x.PositionId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    PositionName = x.Position.Name,
                    PositionExecutorName = x.Position.ExecutorAgent.Name,
                    StandartSendListContents =
                                x.StandartSendListContents.Select(y => new FrontDictionaryStandartSendListContent()
                                {
                                    Id = y.Id,
                                    StandartSendListId = x.Id,
                                    Stage = y.Stage,
                                    SendTypeId = y.SendTypeId,
                                    TargetPositionId = y.TargetPositionId,
                                    Task = y.Task,
                                    Description = y.Description,
                                    DueDate = y.DueDate,
                                    DueDay = y.DueDay,
                                    AccessLevelId = y.AccessLevelId,
                                    SendTypeName = y.SendType.Name,
                                    TargetPositionName = y.TargetPosition.Name,
                                    TargetAgentName = y.TargetPosition.ExecutorAgent.Name ?? y.TargetAgent.Name,
                                    AccessLevelName = y.AccessLevel.Name,
                                    SendTypeIsExternal = y.SendTypeId == 45
                                })
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public int AddStandartSendList(IContext context, InternalDictionaryStandartSendList model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendList(context, model);

                dbContext.DictionaryStandartSendListsSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateStandartSendList(IContext context, InternalDictionaryStandartSendList model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendList(context, model);

                dbContext.DictionaryStandartSendListsSet.Attach(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.Update);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteStandartSendList(IContext context, InternalDictionaryStandartSendList list)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var contents = dbContext.DictionaryStandartSendListContentsSet.Where(y => y.StandartSendListId == list.Id);
                dbContext.DictionaryStandartSendListContentsSet.RemoveRange(contents);

                var ddt = dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == list.Id);
                dbContext.DictionaryStandartSendListsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.Delete);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }


        #endregion DictionaryStandartSendList

        #region [+] DictionarySubordinationTypes ...

        public IEnumerable<FrontDictionarySubordinationType> GetSubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionarySubordinationTypesSet.AsQueryable();

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

                var res = qry.Select(x => new FrontDictionarySubordinationType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        #endregion DictionarySubordinationTypes

        #region [+] DictionaryTags ...

        public IEnumerable<InternalDictionaryTag> GetInternalDictionaryTags(IContext ctx, FilterDictionaryTag filter)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryTagsSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

                if (!ctx.IsAdmin)
                {
                    var filterContains = PredicateBuilder.False<DictionaryTags>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
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

                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

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

        public IEnumerable<FrontDictionaryTag> GetTags(IContext ctx, FilterDictionaryTag filter)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.DictionaryTagsSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();
                qry = qry.OrderBy(x => x.Name);
                if (!ctx.IsAdmin)
                {
                    var filterContains = PredicateBuilder.False<DictionaryTags>();
                    filterContains = ctx.CurrentPositionsIdList.Aggregate(filterContains,
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

                var res = qry.Select(x => new FrontDictionaryTag
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
                    LastChangeUserName = dbContext.DictionaryAgentPersonsSet.FirstOrDefault(y => y.Id == x.LastChangeUserId).FullName,
                    DocCount = filter.WithDocCount ? dbContext.DocumentTagsSet.Count(z => z.TagId == x.Id) : 0
                }).ToList();

                transaction.Complete();
                return res;
            }
        }


        public int DocsWithTagCount(IContext context, int tagId)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var res = dbContext.DocumentTagsSet.Count(y => y.TagId == tagId);
                transaction.Complete();
                return res;
            }
        }

        public int AddTag(IContext context, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbTag(context, model);

                dbContext.DictionaryTagsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }
        public void UpdateTag(IContext ctx, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(ctx))
            using (var transaction = GetTransaction())
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
                transaction.Complete();
            }
        }

        public void DeleteTag(IContext context, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var item = dbContext.DictionaryTagsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == model.Id);
                dbContext.DictionaryTagsSet.Remove(item);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        #endregion DictionaryTags

        // pss перенести в AdminCore
        #region [+] AdminAccessLevels ...
        public FrontAdminAccessLevel GetAdminAccessLevel(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(ctx))
            {

                return dbContext.AdminAccessLevelsSet.Where(x => x.Id == id).Select(x => new FrontAdminAccessLevel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontAdminAccessLevel> GetAdminAccessLevels(IContext ctx, FilterAdminAccessLevel filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.AdminAccessLevelsSet.AsQueryable();

                if (filter.AccessLevelId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Admin.AdminAccessLevels>();
                    filterContains = filter.AccessLevelId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                return qry.Select(x => new FrontAdminAccessLevel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                }).ToList();
            }
        }
        #endregion AdminAccessLevels

        #region [+] CustomDictionaryTypes ...
        public int AddCustomDictionaryType(IContext context, InternalCustomDictionaryType model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionaryType(context, model);
                dbContext.CustomDictionaryTypesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteCustomDictionaryType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var item = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.CustomDictionariesSet.RemoveRange(item.CustomDictionaries);
                dbContext.CustomDictionaryTypesSet.Remove(item);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalCustomDictionaryType> GetInternalCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<CustomDictionaryTypes>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

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
                }

                var res = qry.Select(x => new InternalCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public FrontCustomDictionaryType GetCustomDictionaryTypeWithCustomDictionaries(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Id == id);

                var res = qry.Select(x => new FrontCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description
                }).FirstOrDefault();

                res.CustomDictionaries = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).Where(x => x.DictionaryTypeId == res.Id)
                    .Select(x => new FrontCustomDictionary
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Description = x.Description,
                        DictionaryTypeId = res.Id
                    }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontCustomDictionaryType> GetCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

                var res = qry.Select(x => new FrontCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description
                }).ToList();

                transaction.Complete();
                return res;
            }
        }
        #endregion CustomDictionaryTypes

        #region [+] CustomDictionaries ...
        public int AddCustomDictionary(IContext context, InternalCustomDictionary model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionary(context, model);
                dbContext.CustomDictionariesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                transaction.Complete();
                return dbModel.Id;
            }
        }

        public void UpdateCustomDictionary(IContext context, InternalCustomDictionary model)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionary(context, model);
                dbContext.CustomDictionariesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public void DeleteCustomDictionary(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var item = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.CustomDictionariesSet.Remove(item);
                dbContext.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<InternalCustomDictionary> GetInternalCustomDictionarys(IContext context, FilterCustomDictionary filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
            {
                var qry = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).AsQueryable();

                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        var filterContains = PredicateBuilder.False<CustomDictionaries>();
                        filterContains = filter.IDs.Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Id == value).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (!string.IsNullOrEmpty(filter.Code))
                    {
                        var filterContains = PredicateBuilder.False<CustomDictionaries>();
                        filterContains = CommonFilterUtilites.GetWhereExpressions(filter.Code).Aggregate(filterContains,
                            (current, value) => current.Or(e => e.Code.Contains(value)).Expand());

                        qry = qry.Where(filterContains);
                    }

                    if (!string.IsNullOrEmpty(filter.CodeExact))
                    {
                        qry = qry.Where(x => filter.Code.Equals(x.Code, StringComparison.OrdinalIgnoreCase));
                    }
                }

                var res = qry.Select(x => new InternalCustomDictionary
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    DictionaryTypeId = x.DictionaryTypeId
                }).ToList();

                transaction.Complete();
                return res;
            }
        }

        public IEnumerable<FrontCustomDictionary> GetCustomDictionaries(IContext context, FilterCustomDictionary filter)
        {
            using (var dbContext = new DmsContext(context))
            using (var transaction = GetTransaction())
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

                var res = qry.Select(x => new FrontCustomDictionary
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    DictionaryTypeId = x.DictionaryTypeId
                }).ToList();

                transaction.Complete();
                return res;
            }
        }



        #endregion CustomDictionaries

    }
}
