using BL.CrossCutting.Interfaces;
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
using BL.Database.DBModel.Document;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.SqlServer;
using BL.Model.Common;
using System.Transactions;
using BL.Model.AdminCore.Clients;
using BL.Model.Tree;

namespace BL.Database.Dictionaries
{
    public class DictionariesDbProcess : CoreDb.CoreDb, IDictionariesDbProcess
    {
        public DictionariesDbProcess()
        {
        }

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
        //            LastChangeDate = DateTime.Now,
        //            LastChangeUserId = context.CurrentAgentId,
        //            IsActive = agent.IsActive
        //        };
        //        dbContext.DictionaryAgentsSet.Attach(dbModel);
        //        var entity = dbContext.Entry(dbModel);
        //        entity.State = System.Data.Entity.EntityState.Modified;
        //        dbContext.SaveChanges();
        //    }
        //}



        public FrontDictionaryAgent GetAgent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == id).Select(x => new FrontDictionaryAgent
                    {
                        Id = x.Id,
                        Name = x.Name,
                        //IsIndividual = x.IsIndividual,
                        //IsEmployee = x.IsEmployee,
                        //IsCompany = x.IsCompany,
                        //IsBank = x.IsBank,
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
            }
        }

        public bool ExistsAgent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                var res = dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == id).Select(x => new FrontDictionaryAgent { Id = x.Id }).FirstOrDefault();

                return res != null;

            }
        }

        public IEnumerable<FrontDictionaryAgent> GetAgents(IContext context, FilterDictionaryAgent filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();



                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    //var filterContains = PredicateBuilder.False<DictionaryAgents>();
                    //filterContains = filter.IDs.Aggregate(filterContains,
                    //    (current, value) => current.Or(e => e.Id == value).Expand());

                    //qry = qry.Where(filterContains);
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgents>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                // Сравнение по ИЛИ или не задано ни одно условие
                //qry = qry.Where(x => (
                //  (
                //    (filter.IsBank.HasValue && x.IsBank == filter.IsBank) ||
                //    (filter.IsIndividual.HasValue && x.IsIndividual == filter.IsIndividual) ||
                //    (filter.IsCompany.HasValue && x.IsBank == filter.IsCompany) ||
                //    (filter.IsEmployee.HasValue && x.IsBank == filter.IsEmployee)
                //   ) || (!filter.IsBank.HasValue && !filter.IsIndividual.HasValue && !filter.IsCompany.HasValue && !filter.IsEmployee.HasValue)
                // ));

                qry = qry.OrderBy(x => x.Name);
                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontDictionaryAgent>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;
                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }

                return qry.Select(x => new FrontDictionaryAgent
                {
                    Id = x.Id,
                    Name = x.Name,
                    //IsIndividual = x.IsIndividual,
                    //IsEmployee = x.IsEmployee,
                    //IsCompany = x.IsCompany,
                    //IsBank = x.IsBank,
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
            }
        }

        public void UpdateAgent(IContext context, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(context, agent);

                dbContext.DictionaryAgentsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, agent.Id, EnumObjects.DictionaryAgents, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void UpdateAgentName(IContext context, int id, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(context))
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
            }

        }


        public void DeleteAgent(IContext context, int agentId)
        {
            using (var dbContext = new DmsContext(context))
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
            }
        }

        /// <summary>
        /// Удаляет агента если нет выносок
        /// </summary>
        /// <param name="context"></param>
        /// <param name="agent"></param>
        public void DeleteAgentIfNoAny(IContext context, List<int> list)
        {
            //pss реализовать проверку на существование выносок перед удалением агента
            foreach (int agentId in list)
            {
                if (ExistsAgentClientCompanies(context, new FilterDictionaryAgentClientCompany() { IDs = new List<int>() { agentId } })) return;

                if (ExistsAgentEmployees(context, new FilterDictionaryAgentEmployee() { IDs = new List<int>() { agentId } })) return;
                
                //if (ExistsAgentUsers(context, new FilterDictionaryAgentClientCompany() { IDs = new List<int>() { agentId } })) return;

                //if (ExistsAgentPersons(context, new FilterDictionaryAgentClientCompany() { IDs = new List<int>() { agentId } })) return;

                //if (ExistsAgentBanks(context, new FilterDictionaryAgentClientCompany() { IDs = new List<int>() { agentId } })) return;

                //if (ExistsAgentCompanies(context, new FilterDictionaryAgentClientCompany() { IDs = new List<int>() { agentId } })) return;

                DeleteAgent(context, agentId);
            }
        }

        public int AddAgent(IContext context, InternalDictionaryAgent newAgent)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgent(context, newAgent);

                dbContext.DictionaryAgentsSet.Add(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgents, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                newAgent.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        #endregion DictionaryAgents

        #region [+] DictionaryAgentPerson ...
        public int AddAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            {
                if (ExistsAgent(context, person.Id))
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

                return person.Id;
            }
        }

        public void UpdateAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgentPerson(context, person);

                dbContext.DictionaryAgentPersonsSet.Attach(dbModel);
                dbContext.SaveChanges();
                var entity = dbContext.Entry(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.Update);

                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();


            }
        }


        public void DeleteAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = dbContext.DictionaryAgentPersonsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == person.Id);
                dbContext.DictionaryAgentPersonsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, person.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.Delete);
                dbContext.SaveChanges();

                DeleteAgentIfNoAny(context, new List<int>(){ person.Id });
            }
        }


        public FrontDictionaryAgentPerson GetAgentPerson(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentPersonsSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentPerson
                    {
                        Id = x.Id,
                        //IsIndividual = true,
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
            }
        }

        public IEnumerable<FrontDictionaryAgentPerson> GetAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {

                var qry = dbContext.DictionaryAgentPersonsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                //qry = qry.Where(x => x.Agent.IsIndividual);

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
                if (filter.AgentCompanyId?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
                    filterContains = filter.AgentCompanyId.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentCompanyId == value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentPersons>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.FullName.Contains(temp));
                    }
                }

                // Поиск по паспортным данным
                if (!string.IsNullOrEmpty(filter.Passport))
                {
                    qry = qry.Where(x => (x.PassportSerial + "-" + x.PassportNumber + " " + x.PassportDate.ToString() + " " + x.PassportText).Contains(filter.Passport));
                }

                // Поиск по ИНН
                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.TaxCode))
                    {
                        qry = qry.Where(x => x.TaxCode.Contains(temp));
                    }
                }

                // Поиск по дате рождения
                if (filter.BirthPeriod?.IsActive ?? false)
                {
                    qry = qry.Where(x => x.BirthDate >= filter.BirthPeriod.DateBeg);
                    qry = qry.Where(x => x.BirthDate <= filter.BirthPeriod.DateEnd);
                }

                if (!string.IsNullOrEmpty(filter.FirstNameExact))
                {
                    qry = qry.Where(x => x.FirstName == filter.FirstNameExact);
                }

                if (!string.IsNullOrEmpty(filter.LastNameExact))
                {
                    qry = qry.Where(x => x.LastName == filter.LastNameExact);
                }

                if (!string.IsNullOrEmpty(filter.PassportSerial))
                {
                    qry = qry.Where(x => x.PassportSerial == filter.PassportSerial);
                }

                if (filter.PassportNumber != null)
                {
                    qry = qry.Where(x => x.PassportNumber == filter.PassportNumber);
                }
                qry = qry.OrderBy(x => x.LastName);
                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontDictionaryAgentPerson>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }

                return qry.Select(x => new FrontDictionaryAgentPerson
                {
                    Id = x.Id,
                    //IsIndividual = true,
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
                    //IsBank = x.Agent.IsBank,
                    //IsCompany = x.Agent.IsCompany,
                    //IsEmployee = x.Agent.IsEmployee,
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
                }).ToList();
            }
        }
        #endregion DictionaryAgentPerson

        #region [+] DictionaryAgentEmployee ...
        public int AddAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            {

                //pss это нужно выполнять под одной транзакцией
                employee.Id = AddAgentPerson(context, new InternalDictionaryAgentPerson(employee));
                // решили, что сотрудник и пользователь всегда создаются парой, пользователь может быть деактивирован
                AddAgentUser(context, new InternalDictionaryAgentUser(employee));

                var dbModel = DictionaryModelConverter.GetDbAgentEmployee(context, employee);
                dbContext.DictionaryAgentEmployeesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                return employee.Id;
            }
        }

        public void UpdateAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgentEmployee(context, employee);

                dbContext.DictionaryAgentEmployeesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.Update);

                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                UpdateAgentPerson(context, new InternalDictionaryAgentPerson
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    MiddleName = employee.MiddleName,
                    TaxCode = employee.TaxCode,
                    IsMale = employee.IsMale,
                    PassportSerial = employee.PassportSerial,
                    PassportNumber = employee.PassportNumber,
                    PassportText = employee.PassportText,
                    PassportDate = employee.PassportDate,
                    BirthDate = employee.BirthDate,
                    Description = employee.Description,
                    LastChangeDate = employee.LastChangeDate,
                    LastChangeUserId = employee.LastChangeUserId,
                    IsActive = employee.IsActive
                });


            }
        }

        public void DeleteAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == employee.Id);
                dbContext.DictionaryAgentEmployeesSet.Remove(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public FrontDictionaryAgentEmployee GetAgentEmployee(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryAgentEmployeesSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentEmployee
                {
                    Id = x.Id,
                    PersonnelNumber = x.PersonnelNumber,
                    IsActive = x.IsActive,
                    Description = x.Description,
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

            }
        }

        public FrontDictionaryAgentEmployee GetAgentEmployeePersonnelNumber(IContext context)
        {
            using (var dbContext = new DmsContext(context))
            {
                var tmp = dbContext.DictionaryAgentEmployeesSet.AsEnumerable();

                if (!tmp.Any())
                {
                    return new FrontDictionaryAgentEmployee { PersonnelNumber = "1" };
                }

                return new FrontDictionaryAgentEmployee
                {
                    PersonnelNumber = (tmp.Max(y => Convert.ToInt32(y.PersonnelNumber)) + 1).ToString()
                };
            }
        }

        public IEnumerable<FrontDictionaryAgentEmployee> GetAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetAgentEmployeesQuery(context, dbContext, filter, paging);

                return qry.Select(x => new FrontDictionaryAgentEmployee
                {
                    Id = x.Id,
                    PersonnelNumber = x.PersonnelNumber,
                    IsActive = x.IsActive,
                    Description = x.Description,
                    FirstName = x.Agent.AgentPerson.FirstName,
                    LastName = x.Agent.AgentPerson.LastName,
                    MiddleName = x.Agent.AgentPerson.MiddleName,
                    TaxCode = x.Agent.AgentPerson.TaxCode,
                    //IsMale = x.Agent.AgentPerson.IsMale,
                    PassportSerial = x.Agent.AgentPerson.PassportSerial,
                    PassportNumber = x.Agent.AgentPerson.PassportNumber,
                    PassportText = x.Agent.AgentPerson.PassportText,
                    PassportDate = x.Agent.AgentPerson.PassportDate,
                    BirthDate = x.Agent.AgentPerson.BirthDate//,
                    //Contacts = x.Agent.AgentContacts.Select(y => new FrontDictionaryContact
                    //{
                    //    Id = y.Id,
                    //    AgentId = y.AgentId,
                    //    ContactType = new FrontDictionaryContactType
                    //    {
                    //        Id = y.ContactType.Id,
                    //        Name = y.ContactType.Name,
                    //        Code = y.ContactType.Code,
                    //        InputMask = y.ContactType.InputMask,
                    //        IsActive = y.ContactType.IsActive
                    //    },
                    //    Value = y.Contact,
                    //    IsActive = y.IsActive,
                    //    Description = y.Description
                    //}),
                    //Addresses = x.Agent.AgentAddresses.Select(z => new FrontDictionaryAgentAddress
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
            }
        }

        private IQueryable<DictionaryAgentEmployees> GetWhereAgentEmployees(ref IQueryable<DictionaryAgentEmployees> qry, FilterDictionaryAgentEmployee filter, UIPaging paging)
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

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<DictionaryAgentEmployees>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

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
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                {
                    qry = qry.Where(x => x.Agent.AgentPerson.FullName.Contains(temp));
                }
            }

            if (!string.IsNullOrEmpty(filter.PersonnelNumber))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.PersonnelNumber))
                {
                    qry = qry.Where(x => x.PersonnelNumber.Contains(temp));
                }
            }

            if (!string.IsNullOrEmpty(filter.Passport))
            {
                qry = qry.Where(x => (x.Agent.AgentPerson.PassportSerial + "-" + x.Agent.AgentPerson.PassportNumber + " " +
                                      x.Agent.AgentPerson.PassportDate.ToString() + " " +
                                      x.Agent.AgentPerson.PassportText).Contains(filter.Passport));
            }

            if (!string.IsNullOrEmpty(filter.TaxCode))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.TaxCode))
                {
                    qry = qry.Where(x => x.Agent.AgentPerson.TaxCode.Contains(temp));
                }
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

            if (!string.IsNullOrEmpty(filter.FirstNameExact))
            {
                qry = qry.Where(x => x.Agent.AgentPerson.FirstName == filter.FirstNameExact);
            }

            if (!string.IsNullOrEmpty(filter.LastNameExact))
            {
                qry = qry.Where(x => x.Agent.AgentPerson.LastName == filter.LastNameExact);
            }

            if (!string.IsNullOrEmpty(filter.PassportSerial))
            {
                qry = qry.Where(x => x.Agent.AgentPerson.PassportSerial == filter.PassportSerial);
            }

            if (filter.PassportNumber != null)
            {
                qry = qry.Where(x => x.Agent.AgentPerson.PassportNumber == filter.PassportNumber);
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
            {
                return GetAgentEmployeesQuery(context, dbContext, filter, null).Any();

                //return qry.Select(x => new FrontDictionaryAgentClientCompany { Id = x.Id }).Any();
            }
        }

        #endregion DictionaryAgentEmployee

        #region [+] DictionaryAgentUser ...
        public int AddAgentUser(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context))
            {

                var dbModel = DictionaryModelConverter.GetDbAgentUser(context, User);
                dbContext.DictionaryAgentUsersSet.Add(dbModel);
                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentUsers, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                return User.Id;
            }
        }

        public void UpdateAgentUser(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgentUser(context, User);

                dbContext.DictionaryAgentUsersSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentUsers, EnumOperationType.Update);

                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteAgentUser(IContext context, InternalDictionaryAgentUser User)
        {
            using (var dbContext = new DmsContext(context))
            {   //.Where(x => x.ClientId == context.CurrentClientId)
                var dbModel = dbContext.DictionaryAgentUsersSet.FirstOrDefault(x => x.Id == User.Id);
                dbContext.DictionaryAgentUsersSet.Remove(dbModel);
                //CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentUsers, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }


        public IEnumerable<InternalDictionaryAgentUser> GetAgentUser(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                // Where(x => x.ClientId == context.CurrentClientId).
                return dbContext.DictionaryAgentUsersSet.Where(x => x.Id == id).Select(x => new InternalDictionaryAgentUser
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId
                }).ToList();
            }
        }

        #endregion

        #region [+] DictionaryAgentAddress ...
        public FrontDictionaryAgentAddress GetAgentAddress(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Id == id);

                return qry.Select(x => new FrontDictionaryAgentAddress
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    AddressType = new FrontDictionaryAddressType { Id = x.AdressTypeId, Name = x.AddressType.Name, IsActive = x.AddressType.IsActive },
                    PostCode = x.PostCode,
                    Address = x.Address,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).FirstOrDefault();
            }
        }

        public void UpdateAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAddress(context, addr);

                dbContext.DictionaryAgentAddressesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.Update);

                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == addr.Id);
                dbContext.DictionaryAgentAddressesSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public int AddAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAddress(context, addr);

                dbContext.DictionaryAgentAddressesSet.Add(dbModel);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.AddNew);
                addr.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public IEnumerable<FrontDictionaryAgentAddress> GetAgentAddresses(IContext context, int agentId, FilterDictionaryAgentAddress filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.PostCode))
                    {
                        qry = qry.Where(x => x.PostCode.Contains(temp));
                    }
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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Address))
                    {
                        qry = qry.Where(x => x.Address.Contains(temp));
                    }
                }



                return qry.Select(x => new FrontDictionaryAgentAddress
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    AddressType = new FrontDictionaryAddressType { Id = x.AddressType.Id, Code = x.AddressType.Code, Name = x.AddressType.Name },
                    Address = x.Address,
                    PostCode = x.PostCode,
                    Description = x.Description,
                    IsActive = x.IsActive
                }).ToList();
            }
        }

        public IEnumerable<int> GetAgentsIDByAddress(IContext context, IEnumerable<int> addresses)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                if (addresses.Any())
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentAddresses>();
                    filterContains = addresses.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                return qry.Select(x => x.AgentId).ToList();
            }
        }
        #endregion

        #region [+] DictionaryAddressTypes ...

        public int AddAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAddressType(context, addrType);

                dbContext.DictionaryAddressTypesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAddressType, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                addrType.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public void UpdateAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAddressType(context, addrType);

                dbContext.DictionaryAddressTypesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAddressType, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }


        public void DeleteAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == addrType.Id);
                dbContext.DictionaryAddressTypesSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAddressType, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }


        public InternalDictionaryAddressType GetInternalDictionaryAddressType(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereAddressType(ref qry, filter);

                return qry.Select(x => new InternalDictionaryAddressType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryAddressType> GetAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = GetWhereAddressType(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new FrontDictionaryAddressType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsActive = x.IsActive
                }).ToList();
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
                var filterContains = PredicateBuilder.False<DictionaryAddressTypes>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

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
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }
            }

            return qry;
        }
        #endregion


        #region [+] DictionaryAgentClientCompanies ...
        public int AddAgentClientCompany(IContext context, InternalDictionaryAgentClientCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {

                    if (ExistsAgent(context, company.Id))
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

                    transaction.Complete();

                    company.Id = dc.Id;
                }



                return company.Id;
            }
        }



        public void UpdateAgentClientCompany(IContext context, InternalDictionaryAgentClientCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
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
        }

        public void DeleteAgentClientCompany(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            {

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
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
        }

        public InternalDictionaryAgentClientCompany GetInternalAgentClientCompany(IContext context, FilterDictionaryAgentClientCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetAgentClientCompaniesQuery(context, dbContext, filter);

                return qry.Select(x => new InternalDictionaryAgentClientCompany
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Agent.Name,
                    FullName = x.FullName,
                    Description = x.Description,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryAgentClientCompany> GetAgentClientCompanies(IContext context, FilterDictionaryAgentClientCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetAgentClientCompaniesQuery(context, dbContext, filter);

                return qry.Select(x => new FrontDictionaryAgentClientCompany
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Agent.Name,
                    FullName = x.FullName,
                    Description = x.Description
                }).ToList();
            }
        }


        public IEnumerable<TreeItem> GetAgentClientCompaniesForTree(IContext context, FilterDictionaryAgentClientCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            {

                var qry = GetAgentClientCompaniesQuery(context, dbContext, filter);

                var objId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                return qry.Select(x => new TreeItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    ObjectId = (int)EnumObjects.DictionaryAgentClientCompanies,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = string.Empty,
                    IsActive = x.IsActive,
                    IsList = !(x.Departments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();
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
            {
                return GetAgentClientCompaniesQuery(context, dbContext, filter).Any();

                //return qry.Select(x => new FrontDictionaryAgentClientCompany { Id = x.Id }).Any();
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
                var filterContains = PredicateBuilder.False<DictionaryCompanies>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

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
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                {
                    qry = qry.Where(x => x.FullName.Contains(temp));
                }
            }

            return qry;
        }

        #endregion DictionaryCompanies


        #region [+] DictionaryAgentCompanies ...
        public int AddAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {

                if (ExistsAgent(context, company.Id))
                {
                    //pss Здесь перетирается имя сформированное предыдущей выноской 
                    UpdateAgentName(context, company.Id, new InternalDictionaryAgent(company));
                }
                else
                {
                    company.Id = AddAgent(context, new InternalDictionaryAgent(company));
                    //AddAgentByName(context, company.Name);
                };

                var dbModel = DictionaryModelConverter.GetDbAgentCompany(context, company);

                dbContext.DictionaryAgentCompaniesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                return company.Id;
            }
        }

        public void UpdateAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgentCompany(context, company);

                dbContext.DictionaryAgentCompaniesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                UpdateAgentName(context, company.Id, new InternalDictionaryAgent(company));

            }

        }

        public void DeleteAgentCompanies(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            {
                dbContext.DictionaryAgentCompaniesSet.RemoveRange(dbContext.DictionaryAgentCompaniesSet.
                    Where(x => x.Agent.ClientId == context.CurrentClientId).
                    Where(x=> list.Contains(x.Id)));
                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryAgentCompanies, EnumOperationType.Delete);
                dbContext.SaveChanges();

                DeleteAgentIfNoAny(context, list);
            }
        }

        public FrontDictionaryAgentCompany GetAgentCompany(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentCompaniesSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentCompany
                    {
                        Id = x.Id,
                        //IsCompany = x.Agent.IsCompany,
                        //IsIndividual = x.Agent.IsIndividual,
                        //IsBank = x.Agent.IsBank,
                        //IsEmployee = x.Agent.IsEmployee,
                        ResidentTypeId = x.Agent.ResidentTypeId,
                        FullName = x.FullName,
                        ShortName = x.Agent.Name,
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
            }
        }

        public IEnumerable<FrontDictionaryAgentCompany> GetAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentCompaniesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

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
                    var filterContains = PredicateBuilder.False<DictionaryAgentCompanies>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.FullName.Contains(temp));
                    }
                }

                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.TaxCode))
                    {
                        qry = qry.Where(x => x.TaxCode.Contains(temp));
                    }
                }
                if (!string.IsNullOrEmpty(filter.OKPOCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.OKPOCode))
                    {
                        qry = qry.Where(x => x.OKPOCode.Contains(temp));
                    }
                }
                if (!string.IsNullOrEmpty(filter.VATCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.VATCode))
                    {
                        qry = qry.Where(x => x.VATCode.Contains(temp));
                    }
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
                        return new List<FrontDictionaryAgentCompany>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }

                return qry.Select(x => new FrontDictionaryAgentCompany
                {
                    Id = x.Id,
                    //IsCompany = x.Agent.IsCompany,
                    //IsIndividual = x.Agent.IsIndividual,
                    //IsBank = x.Agent.IsBank,
                    //IsEmployee = x.Agent.IsEmployee,
                    ResidentTypeId = x.Agent.ResidentTypeId,
                    FullName = x.FullName,
                    ShortName = x.Agent.Name,
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
                        MiddleName = t.MiddleName,
                        IsActive = t.IsActive,
                        IsMale = t.IsMale
                    })

                }).ToList();
            }
        }
        #endregion DictionaryAgentCompanies

        #region [+] DictionaryAgentBanks ...

        public int AddAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            {
                if (ExistsAgent(context, bank.Id))
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

                return bank.Id;
            }
        }

        public void UpdateAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgentBank(context, bank);

                dbContext.DictionaryAgentBanksSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                UpdateAgentName(context, bank.Id, new InternalDictionaryAgent(bank));

            }
        }
        public void DeleteAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = dbContext.DictionaryAgentBanksSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == bank.Id);
                dbContext.DictionaryAgentBanksSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.Delete);
                dbContext.SaveChanges();

                DeleteAgentIfNoAny(context, new List<int>() { bank.Id });
            }
        }



        public FrontDictionaryAgentBank GetAgentBank(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentBanksSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentBank
                    {
                        Id = x.Id,
                        //IsBank = true,
                        MFOCode = x.MFOCode,
                        Swift = x.Swift,
                        //IsCompany = x.Agent.IsCompany,
                        //IsEmployee = x.Agent.IsEmployee,
                        //IsIndividual = x.Agent.IsIndividual,
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
            }
        }

        public IEnumerable<FrontDictionaryAgentBank> GetAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {

                var qry = dbContext.DictionaryAgentBanksSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                //qry = qry.Where(x => x.Agent.IsBank);
                qry = qry.OrderBy(x => x.FullName); //Agent.Name

                // Пагинация
                if (paging != null)
                {
                    if (paging.IsOnlyCounter ?? true)
                    {
                        paging.TotalItemsCount = qry.Count();
                    }

                    if (paging.IsOnlyCounter ?? false)
                    {
                        return new List<FrontDictionaryAgentBank>();
                    }

                    if (!paging.IsAll)
                    {
                        var skip = paging.PageSize * (paging.CurrentPage - 1);
                        var take = paging.PageSize;

                        qry = qry.Skip(() => skip).Take(() => take);
                    }
                }

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
                    var filterContains = PredicateBuilder.False<DictionaryAgentBanks>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Agent.Name.Contains(temp));
                    }
                }

                if (!string.IsNullOrEmpty(filter.MFOCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.MFOCode))
                    {
                        qry = qry.Where(x => x.MFOCode.Contains(temp));
                    }
                }

                if (!string.IsNullOrEmpty(filter.MFOCodeExact))
                {

                    qry = qry.Where(x => x.MFOCode == filter.MFOCodeExact);

                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {

                    qry = qry.Where(x => x.Agent.Name == filter.NameExact);

                }
                qry = qry.OrderBy(x => x.Agent.Name);
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

                return qry.Select(x => new FrontDictionaryAgentBank
                {
                    Id = x.Id,
                    //IsBank = true,
                    MFOCode = x.MFOCode,
                    Swift = x.Swift,
                    //IsCompany = x.Agent.IsCompany,
                    //IsEmployee = x.Agent.IsEmployee,
                    //IsIndividual = x.Agent.IsIndividual,
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
                }).ToList();
            }
        }
        #endregion DictionaryAgentBanks

        #region [+] DictionaryAgentAccounts ...
        public FrontDictionaryAgentAccount GetAgentAccount(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentAccount
                    {
                        Id = x.Id,
                        AccountNumber = x.AccountNumber,
                        Name = x.Name,
                        IsMain = x.IsMain,
                        AgentBankId = x.AgentBankId,
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
            }
        }

        public void SetMainAgentAccount(IContext context, int AgentId, int AccountId)
        {
            using (var dbContext = new DmsContext(context))
            {
                var accounts = GetAgentAccounts(context, AgentId, new FilterDictionaryAgentAccount());
                foreach (FrontDictionaryAgentAccount account in accounts)
                {
                    if (account.Id != AccountId)
                    {
                        var newAcc = new InternalDictionaryAgentAccount
                        {
                            Id = account.Id,
                            Name = account.Name,
                            AgentId = account.AgentId,
                            AgentBankId = account.AgentBankId,
                            AccountNumber = account.AccountNumber,
                            Description = account.Description,
                            IsActive = account.IsActive,
                            LastChangeDate = DateTime.Now,
                            LastChangeUserId = context.CurrentAgentId,
                            IsMain = false
                        };
                        UpdateAgentAccount(context, newAcc);
                    }
                }
            }
        }

        public void UpdateAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAccount(context, account);

                dbContext.DictionaryAgentAccountsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                if (account.IsMain)
                {
                    SetMainAgentAccount(context, account.AgentId, account.Id);
                }

            }
        }

        public void DeleteAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context))
            {

                var dbModel = dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == account.Id);
                dbContext.DictionaryAgentAccountsSet.Remove(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.Delete);
                dbContext.SaveChanges();

            }
        }

        public int AddAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbAgentAccount(context, account);

                dbContext.DictionaryAgentAccountsSet.Add(dbModel);

                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                if (account.IsMain)
                {
                    SetMainAgentAccount(context, account.AgentId, account.Id);
                }

                return account.Id;
            }
        }

        public IEnumerable<FrontDictionaryAgentAccount> GetAgentAccounts(IContext context, int AgentId, FilterDictionaryAgentAccount filter)
        {
            using (var dbContext = new DmsContext(context))
            {

                var qry = dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.AgentId == AgentId);


                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                if (!string.IsNullOrEmpty(filter.AccountNumber))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.AccountNumber))
                    {
                        qry = qry.Where(x => x.AccountNumber.Contains(temp));
                    }
                }

                if (filter.AgentBankId.HasValue)
                {
                    qry = qry.Where(x => x.AgentBankId == filter.AgentBankId);
                }

                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }


                return qry.Select(x => new FrontDictionaryAgentAccount
                {
                    Id = x.Id,
                    AccountNumber = x.AccountNumber,
                    Name = x.Name,
                    IsMain = x.IsMain,
                    AgentBankId = x.AgentBankId,
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
            }
        }
        #endregion DictionaryAgentAccounts

        #region [+] DictionaryContactTypes ...
        public FrontDictionaryContactType GetInternalDictionaryContactType(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<DictionaryContactTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                if (!String.IsNullOrEmpty(filter.Code))
                {

                    qry = qry.Where(x => x.Code == filter.Code);

                }

                if (!String.IsNullOrEmpty(filter.NameExact))
                {

                    qry = qry.Where(x => x.Name == filter.NameExact);

                }

                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                return qry.Select(x => new FrontDictionaryContactType
                {
                    Id = x.Id,
                    Name = x.Name,
                    InputMask = x.InputMask,
                    Code = x.Code,
                    IsActive = x.IsActive
                }).FirstOrDefault();
            }
        }
        public void UpdateContactType(IContext context, InternalDictionaryContactType model)
        {
            using (var dbContext = new DmsContext(context))
            {

                var dbModel = DictionaryModelConverter.GetDbContactType(context, model);

                dbContext.DictionaryContactTypesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContactType, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        public void DeleteContactType(IContext context, InternalDictionaryContactType contactType)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == contactType.Id);
                dbContext.DictionaryContactTypesSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContactType, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }
        public int AddContactType(IContext context, InternalDictionaryContactType model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbContactType(context, model);

                dbContext.DictionaryContactTypesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContactType, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }
        public IEnumerable<FrontDictionaryContactType> GetContactTypes(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<DictionaryContactTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }
                if (!String.IsNullOrEmpty(filter.Code))
                {

                    qry = qry.Where(x => x.Code == filter.Code);

                }
                return qry.Select(x => new FrontDictionaryContactType
                {
                    Id = x.Id,
                    Name = x.Name,
                    InputMask = x.InputMask,
                    Code = x.Code,
                    IsActive = x.IsActive
                }).ToList();
            }
        }
        #endregion

        #region [+] DictionaryAgentContacts ...

        public int AddContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbContact(context, contact);

                dbContext.DictionaryAgentContactsSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContacts, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                contact.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public void UpdateContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbContact(context, contact);

                dbContext.DictionaryAgentContactsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryContacts, EnumOperationType.Update);
                dbContext.SaveChanges();
            }
        }
        public void DeleteContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == contact.Id);
                dbContext.DictionaryAgentContactsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContacts, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public FrontDictionaryContact GetContact(IContext context, int id)

        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Id == id);

                return qry.Select(x => new FrontDictionaryContact
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    ContactType = new FrontDictionaryContactType { Id = x.ContactTypeId, Name = x.ContactType.Name, Code = x.ContactType.Code, IsActive = x.ContactType.IsActive },
                    Value = x.Contact,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    IsConfirmed = x.IsConfirmed
                }).FirstOrDefault();
            }
        }

        //pss Зачем отдельным параметром передавать agentId если в filter есть такой параметр
        public IEnumerable<FrontDictionaryContact> GetContacts(IContext context, int agentId, FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.OrderBy(x => x.ContactType.Id).ThenBy(x => x.Contact);


                qry = qry.Where(x => x.AgentId == agentId);

                if (filter.AgentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentContacts>();
                    filterContains = filter.AgentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.AgentId == value).Expand());

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
                    //pss проверить, что по номерам с учетом "---" поиск работает

                    string searchExpression = filter.Contact.Replace('-', ' ').Replace('(', ' ').Replace(')', ' ');

                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(searchExpression))
                    {
                        qry = qry.Where(x => x.Contact.Contains(temp));
                    }
                }

                if (!String.IsNullOrEmpty(filter.ContactExact))
                {
                    //pss Здесь ContactExact нужно делать более умным в сравнении телефонов
                    qry = qry.Where(x => x.Contact == filter.ContactExact);
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
                    var filterContains = PredicateBuilder.False<DictionaryAgentContacts>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                return qry.Select(x => new FrontDictionaryContact
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    ContactType = new FrontDictionaryContactType { Id = x.ContactTypeId, Name = x.ContactType.Name, Code = x.ContactType.Code, IsActive = x.ContactType.IsActive },
                    Value = x.Contact,
                    Description = x.Description,
                    IsActive = x.IsActive
                }).ToList();
            }
        }



        public IEnumerable<int> GetAgentsIDByContacts(IContext context, IEnumerable<int> contacts)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                if (contacts.Any())
                {
                    var filterContains = PredicateBuilder.False<DictionaryAgentContacts>();
                    filterContains = contacts.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                return qry.Select(x => x.AgentId).ToList();
            }
        }
        #endregion

        #region [+] DictionaryDepartments ...
        public int AddDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = DictionaryModelConverter.GetDbDepartments(context, department);
                dbContext.DictionaryDepartmentsSet.Add(dd);
                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                department.Id = dd.Id;
                return dd.Id;
            }
        }

        public void UpdateDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = DictionaryModelConverter.GetDbDepartments(context, department);
                dbContext.DictionaryDepartmentsSet.Attach(dd);

                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.Update);
                dbContext.Entry(dd).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteDepartments(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            {
                var childDepartments = GetDepartmentIDs(context, new FilterDictionaryDepartment() { ParentIDs = list });

                if (childDepartments.Count > 0) DeleteDepartments(context, childDepartments);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    var positions = GetPositionsIDs(context, new FilterDictionaryPosition() { DepartmentIDs = list });

                    if (positions.Count> 0) DeletePositions(context, positions);

                    dbContext.DictionaryDepartmentsSet.RemoveRange(dbContext.DictionaryDepartmentsSet.
                        Where(x => x.Company.ClientId == context.CurrentClientId).
                        Where(x => list.Contains(x.Id)));
                    CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryDepartments, EnumOperationType.Delete);
                    dbContext.SaveChanges();

                    transaction.Complete();
                }
            }
        }

        public InternalDictionaryDepartment GetDepartment(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).AsQueryable();

                qry = DepartmentGetWhere(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new InternalDictionaryDepartment
                {
                    Id = x.Id,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Code = x.Code,
                    Name = x.Name,
                    FullName = x.FullName,
                    CompanyId = x.CompanyId,
                    ChiefPositionId = x.ChiefPositionId
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryDepartment> GetDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                return qry.Select(x => new FrontDictionaryDepartment
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Code = x.Code,
                    Name = x.Name,
                    FullName = x.FullName,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Agent.Name,
                    ChiefPositionId = x.ChiefPositionId,
                    ChiefPositionName = x.ChiefPosition.Name,
                    ParentDepartmentName = x.ParentDepartment.Name
                }).ToList();
            }
        }

        public List<int> GetDepartmentIDs(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                return qry.Select(x => x.Id).ToList();
            }
        }

        public string GetDepartmentPrefix(IContext context, int parentId)
        {
            using (var dbContext = new DmsContext(context))
            {
                string res = "";

                int? id = parentId;

                while (id != null)
                {

                    var qry = GetDepartmentsQuery(context, dbContext, new FilterDictionaryDepartment() { IDs = new List<int> { id ?? 0 } });
                    var item = qry.Select(x => new FrontDictionaryDepartment() { Id = x.Id, ParentId = x.ParentId, Code = x.Code }).FirstOrDefault();

                    if (item == null) break;

                    res = item.Code + "/" + res;
                    id = item.ParentId;
                }

                return res;

            }
        }
        public IEnumerable<FrontDictionaryDepartmentTreeItem> GetDepartmentsForTree(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetDepartmentsQuery(context, dbContext, filter);

                var objId = ((int)EnumObjects.DictionaryDepartments).ToString();
                var companyObjId = ((int)EnumObjects.DictionaryAgentClientCompanies).ToString();

                return qry.Select(x => new FrontDictionaryDepartmentTreeItem
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ObjectId = (int)EnumObjects.DictionaryDepartments,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = (x.ParentId == null) ? string.Concat(x.CompanyId, "_", companyObjId) : string.Concat(x.ParentId, "_", objId),
                    IsActive = x.IsActive,
                    IsList = !(x.ChildDepartments.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any() || x.Positions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();
            }
        }

        public IQueryable<DictionaryDepartments> GetDepartmentsQuery(IContext context, DmsContext dbContext, FilterDictionaryDepartment filter)
        {
            var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).AsQueryable();

            qry = DepartmentGetWhere(ref qry, filter);

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

        private static IQueryable<DictionaryDepartments> DepartmentGetWhere(ref IQueryable<DictionaryDepartments> qry, FilterDictionaryDepartment filter)
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
                var filterContains = PredicateBuilder.False<DictionaryDepartments>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

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
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }

            }

            // Условие по FullName
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.FullName))
                {
                    qry = qry.Where(x => x.FullName.Contains(temp));
                }
            }

            // Условие по Code
            if (!string.IsNullOrEmpty(filter.Code))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Code))
                {
                    qry = qry.Where(x => x.Code.Contains(temp));
                }
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
        public FrontDictionaryDocumentDirection GetDocumentDirection(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return
                    dbContext.DictionaryDocumentDirectionsSet.Where(x => x.Id == id)
                        .Select(x => new FrontDictionaryDocumentDirection
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate
                        }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryDocumentDirection> GetDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDocumentDirectionsSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DictionaryDocumentDirections>();
                    filterContains = filter.IDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id == value).Expand());

                    qry = qry.Where(filterContains);
                }

                return qry.Select(x => new FrontDictionaryDocumentDirection
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();
            }
        }
        #endregion DictionaryDepartments

        #region [+] DictionaryDocumentSubjects ...

        public int AddDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dds = DictionaryModelConverter.GetDbDocumentSubject(context, docSubject);
                dbContext.DictionaryDocumentSubjectsSet.Add(dds);
                CommonQueries.AddFullTextCashInfo(dbContext, dds.Id, EnumObjects.DictionaryDocumentSubjects, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                docSubject.Id = dds.Id;
                return dds.Id;
            }
        }

        public void UpdateDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context))
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
            }
        }

        public void DeleteDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == docSubject.Id);
                dbContext.DictionaryDocumentSubjectsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentSubjects, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public InternalDictionaryDocumentSubject GetInternalDictionaryDocumentSubject(IContext context, FilterDictionaryDocumentSubject filter)
        {
            // Устно договорились, что функция для возврата одного элемента возвращает параметры только конкретного элемена без учета родителя и потомков.

            using (var dbContext = new DmsContext(context))
            {

                var qry = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = DocumentSubjectGetWhere(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new InternalDictionaryDocumentSubject
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    IsActive = x.IsActive,
                    Name = x.Name,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).FirstOrDefault();

            }
        }

        public IEnumerable<FrontDictionaryDocumentSubject> GetDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {

            // Устно договорились, что функция для возврата коллекции элементов возвращает всю простыню элеменов без учета родителя и потомков.
            // Если нужно каждому элементу добавить родителя и потомков это делается на уровень выше в Logic. Функция GetDocumentSubects ВСЕГДА возвращает плоскую коллекцию
            // более толго для построения иерархии на фронте плоской коллекции достаточно.

            using (var dbContext = new DmsContext(context))
            {
                //IQueryable<DictionaryDocumentSubjects> qry=

                var qry = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = DocumentSubjectGetWhere(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new FrontDictionaryDocumentSubject
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    IsActive = x.IsActive,
                    Name = x.Name
                    //ParentDocumentSubjectName = x.ParentDocumentSubject.Name,
                }).ToList();
            }
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsDictionaryDocumentSubject(IContext context, FilterDictionaryDocumentSubject filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDocumentSubjectsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = DocumentSubjectGetWhere(ref qry, filter);

                var res = qry.Select(x => new FrontDictionaryDocumentSubject
                {
                    Id = x.Id
                }).FirstOrDefault();

                return res != null;
            }
        }

        private static IQueryable<DictionaryDocumentSubjects> DocumentSubjectGetWhere(ref IQueryable<DictionaryDocumentSubjects> qry, FilterDictionaryDocumentSubject filter)
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
                var filterContains = PredicateBuilder.False<DictionaryDocumentSubjects>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

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
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }
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
            {
                var dbModel = DictionaryModelConverter.GetDbDocumentType(context, docType);
                dbContext.DictionaryDocumentTypesSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                docType.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public void UpdateDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbDocumentType(context, docType);
                dbContext.DictionaryDocumentTypesSet.Attach(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.Update);
                dbContext.Entry(dbModel).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == docType.Id);
                dbContext.DictionaryDocumentTypesSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public InternalDictionaryDocumentType GetInternalDictionaryDocumentType(IContext context, FilterDictionaryDocumentType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = DocumentTypeGetWhere(ref qry, filter);

                return qry.Select(x => new InternalDictionaryDocumentType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryDocumentType> GetDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = DocumentTypeGetWhere(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new FrontDictionaryDocumentType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                }).ToList();
            }
        }

        private static IQueryable<DictionaryDocumentTypes> DocumentTypeGetWhere(ref IQueryable<DictionaryDocumentTypes> qry, FilterDictionaryDocumentType filter)
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
                var filterContains = PredicateBuilder.False<DictionaryDocumentTypes>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

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
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }
            }

            if (!string.IsNullOrEmpty(filter.NameExact))
            {
                qry = qry.Where(x => x.Name == filter.NameExact);
            }

            return qry;
        }
        #endregion DictionaryDocumentTypes

        #region [+] DictionaryEventTypes ...
        public FrontDictionaryEventType GetEventType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryEventTypesSet.Where(x => x.Id == id).Select(x => new FrontDictionaryEventType
                {
                    EventType = (EnumEventTypes)x.Id,
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ImportanceEventTypeId = x.ImportanceEventTypeId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    ImportanceEventTypeName = x.ImportanceEventType.Name
                }).FirstOrDefault();
            }
        }

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
                    var filterContains = PredicateBuilder.False<DictionaryEventTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
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
        public FrontDictionaryImportanceEventType GetImportanceEventType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return dbContext.DictionaryImportanceEventTypesSet.Where(x => x.Id == id)
                    .Select(x => new FrontDictionaryImportanceEventType
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryImportanceEventType> GetImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<DictionaryImportanceEventTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
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

                return qry.Select(x => new FrontDictionaryImportanceEventType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();
            }
        }
        #endregion DictionaryImportanceEventTypes

        #region [+] DictionaryLinkTypes ...
        public FrontDictionaryLinkType GetLinkType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryLinkTypesSet.Where(x => x.Id == id)
                    .Select(x => new FrontDictionaryLinkType
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsImportant = x.IsImportant,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryLinkType> GetLinkTypes(IContext context, FilterDictionaryLinkType filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<DictionaryLinkTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                return qry.Select(x => new FrontDictionaryLinkType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsImportant = x.IsImportant,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();
            }
        }
        #endregion DictionaryLinkTypes

        #region [+] DictionaryPositions ...
        public int AddPosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = DictionaryModelConverter.GetDbPosition(context, position);
                dbContext.DictionaryPositionsSet.Add(dd);
                //pss нельзя модифицировать поля, которые проставляет вертушка

                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryPositions, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                position.Id = dd.Id;
                return dd.Id;
            }
        }

        public void UpdatePosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = DictionaryModelConverter.GetDbPosition(context, position);
                dbContext.DictionaryPositionsSet.Attach(dd);
                dbContext.Entry(dd).State = System.Data.Entity.EntityState.Modified;
                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryPositions, EnumOperationType.Update);
                dbContext.SaveChanges();
            }
        }

        public void UpdatePositionOrder(IContext context, int positionId, int order)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbPosition(context, new InternalDictionaryPosition() { Id = positionId, Order = order });
                dbContext.DictionaryPositionsSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);
                entity.Property(x => x.Order).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public void DeletePositions(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {

                    var positionExecutors = GetPositionExecutorsIDs(context, new FilterDictionaryPositionExecutor() { PositionIDs = list });

                    if (positionExecutors.Count > 0) DeleteExecutors(context, positionExecutors);

                    dbContext.DictionaryPositionsSet.RemoveRange(dbContext.DictionaryPositionsSet.
                        Where(x => x.Department.Company.ClientId == context.CurrentClientId).
                        Where(x => list.Contains(x.Id)));
                    CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryPositions, EnumOperationType.Delete);
                    dbContext.SaveChanges();

                    transaction.Complete();
                }
            }
        }

        

        

        public int? GetExecutorAgentIdByPositionId(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryPositionsSet.AsQueryable();
                if (!context.IsAdmin)
                {
                    qry = qry.Where(x => x.Department.Company.ClientId == context.CurrentClientId);
                }
                return qry.Where(x => x.Id == id).Select(x => x.ExecutorAgentId).FirstOrDefault();
            }
        }

        public FrontDictionaryPosition GetPosition(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Where(x => x.Id == id)
                    .Select(x => new FrontDictionaryPosition
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        ParentId = x.ParentId,
                        Name = x.Name,
                        FullName = x.FullName,
                        DepartmentId = x.DepartmentId,
                        ExecutorAgentId = x.ExecutorAgentId,
                        ExecutorAgentName = x.ExecutorAgent.Name,
                        MainExecutorAgentId = x.MainExecutorAgentId,
                        MainExecutorAgentName = x.MainExecutorAgent.Name,
                        ParentPositionName = x.ParentPosition.Name,
                        DepartmentName = x.Department.Name,
                        Order = x.Order,
                        PositionExecutors = x.PositionExecutors.
                            Where(y=> DateTime.Now > y.StartDate).
                            Where(y => DateTime.Now < y.EndDate).
                            Where(y => y.IsActive == true).
                            OrderBy(y=> y.PositionExecutorTypeId).ThenBy(y => y.Agent.Name).
                            Select(y => new FrontDictionaryPositionExecutor
                            {
                                Id = y.Id,
                                IsActive = y.IsActive,
                                AgentName = y.Agent.Name,
                                PositionExecutorTypeName = y.PositionExecutorType.Name
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
            }
        }

        public IEnumerable<FrontDictionaryPosition> GetPositions(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    DepartmentId = x.DepartmentId,
                    ExecutorAgentId = x.ExecutorAgentId,
                    ParentPositionName = x.ParentPosition.Name,
                    DepartmentName = x.Department.Name,
                    ExecutorAgentName = x.ExecutorAgent.Name,
                    MaxSubordinationTypeId = x.TargetPositionSubordinations.AsQueryable()
                                                        .Where(filterMaxSubordinationTypeContains)
                                                        .Max(y => y.SubordinationTypeId)
                });

                if (filter.SubordinatedTypeId.HasValue)
                {
                    qry2 = qry2.Where(x => x.MaxSubordinationTypeId >= filter.SubordinatedTypeId);
                }

                return qry2.ToList();
            }
        }

        public List<int> GetPositionsIDs(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetPositionsQuery(context, dbContext, filter);
                return qry.Select(x => x.Id).ToList();
            }
        }

        public IEnumerable<TreeItem> GetPositionsForTree(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetPositionsQuery(context, dbContext, filter);

                string objId = ((int)EnumObjects.DictionaryPositions).ToString();
                string parObjId = ((int)EnumObjects.DictionaryDepartments).ToString();

                return qry.Select(x => new TreeItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    ObjectId = (int)EnumObjects.DictionaryPositions,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = x.DepartmentId.ToString() + "_" + parObjId,
                    IsActive = x.IsActive,
                    IsList = !(x.PositionExecutors.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())// || x.ChildPositions.Where(y => y.IsActive == (filter.IsActive ?? x.IsActive)).Any())
                }).ToList();
            }
        }

        public IEnumerable<SortPositoin> GetPositionsForSort(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetPositionsQuery(context, dbContext, filter);


                return qry.Select(x => new SortPositoin
                {
                    Id = x.Id,
                    OldOrder = x.Order,
                    NewOrder = x.Order
                }).ToList();
            }
        }

        public class SortPositoin: IComparable
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
                var filterContains = PredicateBuilder.False<DictionaryPositions>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

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
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }
            }

            // Условие по полному имени
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.FullName))
                {
                    qry = qry.Where(x => x.FullName.Contains(temp));
                }
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

            return qry;
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsPosition(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                // pss эта сборка Where-условий повторяется 3 раза (GetPositionsWithActions, ExistsPosition, GetPosition). У меня НЕ получается вынести Where в отдельную функцию.
                var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Select(x => new { pos = x, subordMax = 0 }).AsQueryable();


                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    //TODO Contains
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    //TODO Contains
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.pos.Id));
                }

                // Условие по IsActive
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.pos.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.pos.Name.Contains(temp));
                    }
                }

                // Условие по полному имени
                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.FullName))
                    {
                        qry = qry.Where(x => x.pos.FullName.Contains(temp));
                    }
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Document.DocumentEvents>();
                    filterContains = filter.DocumentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(filterContains).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(filterContains).Select(y => y.TargetPositionId).Contains(x.pos.Id)
                                );
                }



                if (filter.SubordinatedPositions?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Admin.AdminSubordinations>();
                    filterContains = filter.SubordinatedPositions.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value).Expand());

                    qry = qry.GroupJoin(
                                        dbContext.AdminSubordinationsSet.Where(filterContains),
                                        x => x.pos.Id,
                                        y => y.TargetPositionId,
                                        (x, y) => new { pos = x.pos, subordMax = y.Max(z => z.SubordinationTypeId) }
                                        )
                             .Where(x => x.subordMax > 0);
                }

                var res = qry.Select(x => new FrontDictionaryPosition
                {
                    Id = x.pos.Id
                }).FirstOrDefault();

                return res != null;
            }
        }

        public IEnumerable<InternalDictionaryPositionWithActions> GetPositionsWithActions(IContext context,
            FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                // pss эта сборка Where-условий повторяется 3 раза (GetPositionsWithActions, ExistsPosition, GetPosition). У меня НЕ получается вынести Where в отдельную функцию.
                var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    //TODO Contains
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    //TODO Contains
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.pos.Id));
                }

                // Условие по IsActive
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.pos.IsActive);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.pos.Name.Contains(temp));
                    }
                }

                // Условие по полному имени
                if (!string.IsNullOrEmpty(filter.FullName))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.FullName))
                    {
                        qry = qry.Where(x => x.pos.FullName.Contains(temp));
                    }
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DocumentEvents>();
                    filterContains = filter.DocumentIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.DocumentId == value).Expand());

                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(filterContains).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(filterContains).Select(y => y.TargetPositionId).Contains(x.pos.Id)
                                );
                }

                if (filter.SubordinatedPositions?.Count > 0)
                {
                    var filterContains = PredicateBuilder.False<DBModel.Admin.AdminSubordinations>();
                    filterContains = filter.SubordinatedPositions.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.SourcePositionId == value).Expand());

                    qry = qry.GroupJoin(
                                        dbContext.AdminSubordinationsSet.Where(filterContains),
                                        x => x.pos.Id,
                                        y => y.TargetPositionId,
                                        (x, y) => new { pos = x.pos, subordMax = y.Max(z => z.SubordinationTypeId) }
                                        )
                             .Where(x => x.subordMax > 0);
                }

                return qry.Select(x => new InternalDictionaryPositionWithActions
                {
                    Id = x.pos.Id,
                    Name = x.pos.Name,
                    DepartmentId = x.pos.DepartmentId,
                    ExecutorAgentId = x.pos.ExecutorAgentId,
                    DepartmentName = x.pos.Department.Name,
                    ExecutorAgentName = x.pos.ExecutorAgent.Name,
                }).ToList();
            }
        }

        #endregion DictionaryPositions

        #region [+] DictionaryPositionExecutors ...
        public int AddExecutor(IContext context, InternalDictionaryPositionExecutor executor)
        {
            using (var dbContext = new DmsContext(context))
            {
                DictionaryPositionExecutors dc = DictionaryModelConverter.GetDbExecutor(context, executor);
                dbContext.DictionaryPositionExecutorsSet.Add(dc);
                CommonQueries.AddFullTextCashInfo(dbContext, dc.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                executor.Id = dc.Id;
                return dc.Id;
            }
        }

        public void UpdateExecutor(IContext context, InternalDictionaryPositionExecutor executor)
        {
            using (var dbContext = new DmsContext(context))
            {
                DictionaryPositionExecutors drj = DictionaryModelConverter.GetDbExecutor(context, executor);
                dbContext.DictionaryPositionExecutorsSet.Attach(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.Update);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteExecutors(IContext context, List<int> list)
        {
            using (var dbContext = new DmsContext(context))
            {
                dbContext.DictionaryPositionExecutorsSet.RemoveRange(dbContext.DictionaryPositionExecutorsSet.
                    Where(x => x.Position.Department.Company.ClientId == context.CurrentClientId).
                    Where(x => list.Contains(x.Id)));
                CommonQueries.AddFullTextCashInfo(dbContext, list, EnumObjects.DictionaryPositionExecutors, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public InternalDictionaryPositionExecutor GetInternalDictionaryPositionExecutor(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                return qry.Select(x => new InternalDictionaryPositionExecutor
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    AgentId = x.AgentId,
                    PositionId = x.PositionId,
                    PositionExecutorTypeId = x.PositionExecutorTypeId,
                    AccessLevelId = x.AccessLevelId,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryPositionExecutor> GetPositionExecutors(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                return qry.Select(x => new FrontDictionaryPositionExecutor
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    AgentId = x.AgentId,
                    PositionId = x.PositionId,
                    PositionExecutorTypeId = x.PositionExecutorTypeId,
                    AccessLevelId = x.AccessLevelId,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    AgentName = x.Agent.Name,
                    PositionName = x.Position.Name,
                    PositionFullName = x.Position.FullName,
                    AccessLevelName = x.AccessLevel.Name,
                    PositionExecutorTypeName = x.PositionExecutorType.Name
                }).ToList();
            }
        }

        public List<int> GetPositionExecutorsIDs(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                return qry.Select(x => x.Id).ToList();
            }
        }

        public IEnumerable<TreeItem> GetPositionExecutorsForTree(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                string objId = ((int)EnumObjects.DictionaryPositionExecutors).ToString();
                string parObjId = ((int)EnumObjects.DictionaryPositions).ToString();

                return qry.Select(x => new TreeItem
                {
                    Id = x.Id,
                    Name = x.Agent.Name,
                    ObjectId = (int)EnumObjects.DictionaryPositionExecutors,
                    TreeId = string.Concat(x.Id.ToString(), "_", objId),
                    TreeParentId = x.PositionId.ToString() + "_" + parObjId,
                    IsActive = x.IsActive,
                    IsList = true,
                    Description = x.PositionExecutorType.Code
                }).ToList();
            }
        }

        public IQueryable<DictionaryPositionExecutors> GetPositionExecutorsQuery(IContext context, DmsContext dbContext, FilterDictionaryPositionExecutor filter)
        {
            var qry = dbContext.DictionaryPositionExecutorsSet.Where(x => x.Position.Department.Company.ClientId == context.CurrentClientId).AsQueryable();

            qry = qry.OrderBy(x => x.Position.Order).ThenBy(x=>x.PositionExecutorType.Id).ThenBy(x => x.Agent.Name);

            qry = ExecutorGetWhere(ref qry, filter);

            return qry;
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsPositionExecutor(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = GetPositionExecutorsQuery(context, dbContext, filter);

                return qry.Select(x => new FrontDictionaryPositionExecutor { Id = x.Id }).Any();
            }
        }

        private static IQueryable<DictionaryPositionExecutors> ExecutorGetWhere(ref IQueryable<DictionaryPositionExecutors> qry, FilterDictionaryPositionExecutor filter)
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
                var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.PositionIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                filterContains = filter.PositionIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.PositionExecutorTypeIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                filterContains = filter.PositionExecutorTypeIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionExecutorTypeId == value).Expand());

                qry = qry.Where(filterContains);
            }

            // Тоько активные/неактивные
            if (filter.IsActive != null)
            {
                qry = qry.Where(x => filter.IsActive == x.IsActive);
            }

            if (filter.Period?.IsActive == true)
            {
                qry = qry.Where(x =>
                                (x.StartDate > filter.Period.DateBeg && x.EndDate < filter.Period.DateEnd) ||
                                (x.StartDate < filter.Period.DateBeg && x.EndDate > filter.Period.DateBeg) ||
                                (x.StartDate < filter.Period.DateEnd && x.EndDate > filter.Period.DateEnd)
                                );
            }

            // Список 
            if (filter.PositionExecutorTypeIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                filterContains = filter.PositionExecutorTypeIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.PositionExecutorTypeId == value).Expand());

                qry = qry.Where(filterContains);
            }

            if (filter.AccessLevelIDs?.Count > 0)
            {
                var filterContains = PredicateBuilder.False<DictionaryPositionExecutors>();
                filterContains = filter.AccessLevelIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.AccessLevelId == value).Expand());

                qry = qry.Where(filterContains);
            }

            return qry;
        }

        #endregion DictionaryPositionExecutors

        #region [+] DictionaryPositionExecutorTypes ...
        public InternalDictionaryPositionExecutorType GetInternalDictionaryPositionExecutorType(IContext context, FilterDictionaryPositionExecutorType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryPositionExecutorTypesSet.AsQueryable();

                qry = ExecutorTypeGetWhere(ref qry, filter);

                return qry.Select(x => new InternalDictionaryPositionExecutorType
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryPositionExecutorType> GetPositionExecutorTypes(IContext context, FilterDictionaryPositionExecutorType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryPositionExecutorTypesSet.AsQueryable();

                qry = ExecutorTypeGetWhere(ref qry, filter);

                return qry.Select(x => new FrontDictionaryPositionExecutorType
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Code = x.Code,
                    Name = x.Name
                }).ToList();
            }
        }

        private static IQueryable<DictionaryPositionExecutorTypes> ExecutorTypeGetWhere(ref IQueryable<DictionaryPositionExecutorTypes> qry, FilterDictionaryPositionExecutorType filter)
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
                var filterContains = PredicateBuilder.False<DictionaryPositionExecutorTypes>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

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
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }
            }

            // Поиск по наименованию
            if (!string.IsNullOrEmpty(filter.Code))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Code))
                {
                    qry = qry.Where(x => x.Code.Contains(temp));
                }
            }

            return qry;
        }

        #endregion DictionaryPositionExecutorTypes

        #region [+] DictionaryRegistrationJournals ...
        public int AddRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal regJournal)
        {
            using (var dbContext = new DmsContext(context))
            {
                DictionaryRegistrationJournals drj = DictionaryModelConverter.GetDbRegistrationJournal(context, regJournal);
                dbContext.DictionaryRegistrationJournalsSet.Add(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                regJournal.Id = drj.Id;
                return drj.Id;
            }
        }

        public void UpdateRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal regJournal)
        {
            using (var dbContext = new DmsContext(context))
            {
                DictionaryRegistrationJournals drj = DictionaryModelConverter.GetDbRegistrationJournal(context, regJournal);
                dbContext.DictionaryRegistrationJournalsSet.Attach(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Update);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal docSubject)
        {
            using (var dbContext = new DmsContext(context))
            {
                var drj = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == docSubject.Id);
                dbContext.DictionaryRegistrationJournalsSet.Remove(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public InternalDictionaryRegistrationJournal GetInternalDictionaryRegistrationJournal(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = RegistrationJournalGetWhere(ref qry, filter);

                return qry.Select(x => new InternalDictionaryRegistrationJournal
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
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryRegistrationJournal> GetRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = RegistrationJournalGetWhere(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new FrontDictionaryRegistrationJournal
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Name,
                    DepartmentId = x.DepartmentId,
                    Index = x.Index,
                    PrefixFormula = x.PrefixFormula,
                    SuffixFormula = x.SuffixFormula,
                    IsIncoming = x.DirectionCodes.Contains(EnumDocumentDirections.Incoming.ToString()),
                    IsOutcoming = x.DirectionCodes.Contains(EnumDocumentDirections.Outcoming.ToString()),
                    IsInternal = x.DirectionCodes.Contains(EnumDocumentDirections.Internal.ToString()),
                    DepartmentName = x.Department.Name
                }).ToList();
            }
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsDictionaryRegistrationJournal(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryRegistrationJournalsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = RegistrationJournalGetWhere(ref qry, filter);

                var res = qry.Select(x => new FrontDictionaryRegistrationJournal
                {
                    Id = x.Id
                }).FirstOrDefault();

                return res != null;
            }
        }

        private static IQueryable<DictionaryRegistrationJournals> RegistrationJournalGetWhere(ref IQueryable<DictionaryRegistrationJournals> qry, FilterDictionaryRegistrationJournal filter)
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
                var filterContains = PredicateBuilder.False<DictionaryRegistrationJournals>();
                filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                    (current, value) => current.Or(e => e.Id != value).Expand());

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
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }
            }

            if (!string.IsNullOrEmpty(filter.NameExact))
            {
                qry = qry.Where(x => x.Name == filter.NameExact);
            }

            // Условие по Index
            if (!string.IsNullOrEmpty(filter.Index))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Index))
                {
                    qry = qry.Where(x => x.Index.Contains(temp));
                }
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

            return qry;
        }

        #endregion DictionaryRegistrationJournals

        #region [+] DictionaryResultTypes ...
        public FrontDictionaryResultType GetResultType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return dbContext.DictionaryResultTypesSet.Where(x => x.Id == id)
                    .Select(x => new FrontDictionaryResultType
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsExecute = x.IsExecute,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                    }).FirstOrDefault();
            }
        }

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
                    var filterContains = PredicateBuilder.False<DictionaryResultTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                return qry.Select(x => new FrontDictionaryResultType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsExecute = x.IsExecute
                }).ToList();
            }
        }
        #endregion DictionaryResultTypes

        #region [+] DictionarySendTypes ...
        public FrontDictionarySendType GetSendType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionarySendTypesSet.Where(x => x.Id == id)
                    .Select(x => new FrontDictionarySendType
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        IsImportant = x.IsImportant,
                        SubordinationType = (EnumSubordinationTypes)x.SubordinationTypeId,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        SubordinationTypeName = x.SubordinationType.Name,
                        IsExternal = x.Id == 45
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionarySendType> GetSendTypes(IContext context, FilterDictionarySendType filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<DictionarySendTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                return qry.Select(x => new FrontDictionarySendType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsImportant = x.IsImportant,
                    SubordinationType = (EnumSubordinationTypes)x.SubordinationTypeId,
                    SubordinationTypeName = x.SubordinationType.Name,
                    IsExternal = x.Id == 45
                }).ToList();
            }
        }
        #endregion DictionarySendTypes

        #region [+] DictionaryStandartSendListContents ...
        public FrontDictionaryStandartSendListContent GetStandartSendListContent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).Where(x => x.Id == id)
                    .Select(x => new FrontDictionaryStandartSendListContent
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
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryStandartSendListContent> GetStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendListContents>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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

                    foreach (string str in CommonFilterUtilites.GetWhereExpressions(filter.SendTypeName))
                    {
                        qry = qry.Where(x => x.SendType.Name.Contains(str));
                    }
                }

                if (!string.IsNullOrEmpty(filter.TargetPositionName))
                {
                    foreach (string str in CommonFilterUtilites.GetWhereExpressions(filter.TargetPositionName))
                    {
                        qry = qry.Where(x => x.TargetPosition.Name.Contains(str));
                    }
                }

                if (!string.IsNullOrEmpty(filter.TargetAgentName))
                {
                    foreach (string str in CommonFilterUtilites.GetWhereExpressions(filter.TargetAgentName))
                    {
                        qry = qry.Where(x => x.TargetAgent.Name.Contains(str));
                    }
                }


                return qry.Select(x => new FrontDictionaryStandartSendListContent
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
            }
        }

        public void UpdateStandartSendListContent(IContext context,
            InternalDictionaryStandartSendListContent content)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendListContent(context, content);
                dbContext.DictionaryStandartSendListContentsSet.Attach(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.Update);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteStandartSendListContent(IContext context,
            InternalDictionaryStandartSendListContent content)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == content.Id);
                dbContext.DictionaryStandartSendListContentsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public int AddStandartSendListContent(IContext context,
            InternalDictionaryStandartSendListContent content)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendListContent(context, content);
                dbContext.DictionaryStandartSendListContentsSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                content.Id = dbModel.Id;
                return dbModel.Id;
            }
        }
        #endregion DictionaryStandartSendListContents

        #region [+] DictionaryStandartSendLists ...
        public FrontDictionaryStandartSendList GetStandartSendList(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == context.CurrentClientId).Where(x => x.Id == id)
                        .Select(x => new FrontDictionaryStandartSendList
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
                        }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryStandartSendList> GetStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<DictionaryStandartSendLists>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                if (!string.IsNullOrEmpty(filter.NameExact))
                {

                    qry = qry.Where(x => x.Name == filter.NameExact);

                }

                if (filter.PositionID != null)
                {
                    qry = qry.Where(x => filter.PositionID == x.PositionId);
                }
                return qry.Select(x => new FrontDictionaryStandartSendList
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
            }
        }

        public int AddStandartSendList(IContext context, InternalDictionaryStandartSendList model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendList(context, model);

                dbContext.DictionaryStandartSendListsSet.Add(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public void UpdateStandartSendList(IContext context, InternalDictionaryStandartSendList model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbStandartSendList(context, model);

                dbContext.DictionaryStandartSendListsSet.Attach(dbModel);
                CommonQueries.AddFullTextCashInfo(dbContext, dbModel.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.Update);
                var entity = dbContext.Entry(dbModel);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteStandartSendList(IContext context, InternalDictionaryStandartSendList list)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == list.Id);
                dbContext.DictionaryStandartSendListsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }


        #endregion DictionaryStandartSendList

        #region [+] DictionarySubordinationTypes ...
        public FrontDictionarySubordinationType GetSubordinationType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionarySubordinationTypesSet.Where(x => x.Id == id)
                    .Select(x => new FrontDictionarySubordinationType
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionarySubordinationType> GetSubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<DictionarySubordinationTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                return qry.Select(x => new FrontDictionarySubordinationType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                }).ToList();
            }
        }

        #endregion DictionarySubordinationTypes

        #region [+] DictionaryTags ...

        public InternalDictionaryTag GetInternalDictionaryTags(IContext ctx, FilterDictionaryTag filter)
        {
            using (var dbContext = new DmsContext(ctx))
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


                return qry.Select(x => new InternalDictionaryTag
                {
                    Id = x.Id,
                    Name = x.Name,
                    PositionId = x.PositionId,
                    Color = x.Color,
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryTag> GetTags(IContext ctx, FilterDictionaryTag filter)
        {
            using (var dbContext = new DmsContext(ctx))
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
                    var filterContains = PredicateBuilder.False<DictionaryTags>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }


                if (!string.IsNullOrEmpty(filter.NameExact))
                {
                    qry = qry.Where(x => x.Name == filter.NameExact);
                }

                return qry.Select(x => new FrontDictionaryTag
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
            }
        }


        public int DocsWithTagCount(IContext context, int tagId)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DocumentTagsSet.Count(y => y.TagId == tagId);
            }
        }

        public int AddTag(IContext context, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbTag(context, model);

                dbContext.DictionaryTagsSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }
        public void UpdateTag(IContext ctx, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(ctx))
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


                //PSS Это нарушение. дб уровень не содержит логики, он молча выполняет вставки и обновления. все проверки должны быть на уровне логики.
                var savTag = qry.FirstOrDefault();

                if (savTag?.Id > 0)
                {
                    savTag.Name = model.Name;
                    savTag.Color = model.Color;
                    savTag.IsActive = model.IsActive;
                    savTag.LastChangeUserId = ctx.CurrentAgentId;
                    savTag.LastChangeDate = DateTime.Now;
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new DictionaryTagNotFoundOrUserHasNoAccess();
                }
            }
        }

        public void DeleteTag(IContext context, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(context))
            {

                var item = dbContext.DictionaryTagsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == model.Id);
                dbContext.DictionaryTagsSet.Remove(item);
                dbContext.SaveChanges();
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
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionaryType(context, model);
                dbContext.CustomDictionaryTypesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public void UpdateCustomDictionaryType(IContext context, InternalCustomDictionaryType model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionaryType(context, model);
                dbContext.CustomDictionaryTypesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public void DeleteCustomDictionaryType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.CustomDictionariesSet.RemoveRange(item.CustomDictionaries);
                dbContext.CustomDictionaryTypesSet.Remove(item);
                dbContext.SaveChanges();
            }
        }

        public InternalCustomDictionaryType GetInternalCustomDictionaryType(IContext context, FilterCustomDictionaryType filter)
        {
            using (var dbContext = new DmsContext(context))
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
                        qry = qry.Where(x => filter.Code.Equals(x.Code, StringComparison.OrdinalIgnoreCase));
                    }
                }

                var items = qry.Select(x => new InternalCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description
                }).FirstOrDefault();

                return items;
            }
        }

        public FrontCustomDictionaryType GetCustomDictionaryType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Id == id);

                var item = qry.Select(x => new FrontCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description
                }).FirstOrDefault();

                item.CustomDictionaries = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).Where(x => x.DictionaryTypeId == item.Id)
                    .Select(x => new FrontCustomDictionary
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Description = x.Description,
                        DictionaryTypeId = item.Id
                    }).ToList();

                return item;
            }
        }

        public IEnumerable<FrontCustomDictionaryType> GetCustomDictionaryTypes(IContext context, FilterCustomDictionaryType filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<CustomDictionaryTypes>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

                    qry = qry.Where(filterContains);
                }


                // Поиск но Code
                if (!string.IsNullOrEmpty(filter.Code))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Code))
                    {
                        qry = qry.Where(x => x.Code.Contains(temp));
                    }
                }

                var items = qry.Select(x => new FrontCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description
                }).ToList();

                return items;
            }
        }
        #endregion CustomDictionaryTypes

        #region [+] CustomDictionaries ...
        public int AddCustomDictionary(IContext context, InternalCustomDictionary model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionary(context, model);
                dbContext.CustomDictionariesSet.Add(dbModel);
                dbContext.SaveChanges();
                model.Id = dbModel.Id;
                return dbModel.Id;
            }
        }

        public void UpdateCustomDictionary(IContext context, InternalCustomDictionary model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dbModel = DictionaryModelConverter.GetDbCustomDictionary(context, model);
                dbContext.CustomDictionariesSet.Attach(dbModel);
                var entity = dbContext.Entry(dbModel);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public void DeleteCustomDictionary(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                dbContext.CustomDictionariesSet.Remove(item);
                dbContext.SaveChanges();
            }
        }

        public InternalCustomDictionary GetInternalCustomDictionary(IContext context, FilterCustomDictionary filter)
        {
            using (var dbContext = new DmsContext(context))
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
                        qry = qry.Where(x => filter.Code.Equals(x.Code, StringComparison.OrdinalIgnoreCase));
                    }
                }

                var items = qry.Select(x => new InternalCustomDictionary
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    DictionaryTypeId = x.DictionaryTypeId
                }).FirstOrDefault();

                return items;
            }
        }

        public FrontCustomDictionary GetCustomDictionary(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Id == id);

                var item = qry.Select(x => new FrontCustomDictionary
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    DictionaryTypeId = x.DictionaryTypeId
                }).FirstOrDefault();

                return item;
            }
        }

        public IEnumerable<FrontCustomDictionary> GetCustomDictionaries(IContext context, FilterCustomDictionary filter)
        {
            using (var dbContext = new DmsContext(context))
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
                    var filterContains = PredicateBuilder.False<CustomDictionaries>();
                    filterContains = filter.NotContainsIDs.Aggregate(filterContains,
                        (current, value) => current.Or(e => e.Id != value).Expand());

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
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Code))
                    {
                        qry = qry.Where(x => x.Code.Contains(temp));
                    }
                }

                var items = qry.Select(x => new FrontCustomDictionary
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description,
                    DictionaryTypeId = x.DictionaryTypeId
                }).ToList();

                return items;
            }
        }



        #endregion CustomDictionaries

    }
}
