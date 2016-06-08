using BL.CrossCutting.Interfaces;
using BL.Database.Common;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Dictionary;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore;
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


namespace BL.Database.Dictionaries
{
    public class DictionariesDbProcess : CoreDb.CoreDb, IDictionariesDbProcess
    {
        public DictionariesDbProcess()
        {
        }

        // Агенты
        #region DictionaryAgents


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

        public void UpdateAgentRole(IContext context, int id, EnumDictionaryAgentTypes role)
        {
            using (var dbContext = new DmsContext(context))
            {
                var agent = GetAgent(context, id);
                var ddt = new DictionaryAgents
                {
                    ClientId = context.CurrentClientId,
                    Name = agent.Name,
                    ResidentTypeId = agent.ResidentTypeId,
                    IsBank = (role == EnumDictionaryAgentTypes.isBank ? !agent.IsBank : agent.IsBank),
                    IsCompany = (role == EnumDictionaryAgentTypes.isCompany ? !agent.IsCompany : agent.IsCompany),
                    IsEmployee = (role == EnumDictionaryAgentTypes.isEmployee ? !agent.IsEmployee : agent.IsEmployee),
                    IsIndividual = (role == EnumDictionaryAgentTypes.isIndividual ? !agent.IsIndividual : agent.IsIndividual),
                    Description = agent.Description,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                    IsActive = agent.IsActive
                };
                dbContext.DictionaryAgentsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }


        public FrontDictionaryAgent GetAgent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId)
                    .Where(x => x.Id == id).Select(x => new FrontDictionaryAgent
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsIndividual = x.IsIndividual,
                        IsEmployee = x.IsEmployee,
                        IsCompany = x.IsCompany,
                        IsBank = x.IsBank,
                        IsActive = x.IsActive,
                        ResidentTypeId = x.ResidentTypeId,
                        Description = x.Description,
                        Contacts=x.AgentContacts.Select(y=> new FrontDictionaryContact
                        {
                          Id=y.Id,
                          AgentId=y.AgentId,
                          ContactType  = new FrontDictionaryContactType
                          {
                              Id=y.ContactType.Id,
                              Code=y.ContactType.Code,
                              Name = y.ContactType.Name,
                              IsActive=y.ContactType.IsActive
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

        public IEnumerable<FrontDictionaryAgent> GetAgents(IContext context, FilterDictionaryAgent filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.Name)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
                qry = qry.Where(x => (
                  (
                    (filter.IsBank.HasValue && x.IsBank == filter.IsBank) ||
                    (filter.IsIndividual.HasValue && x.IsIndividual == filter.IsIndividual) ||
                    (filter.IsCompany.HasValue && x.IsBank == filter.IsCompany) ||
                    (filter.IsEmployee.HasValue && x.IsBank == filter.IsEmployee)
                   ) || (!filter.IsBank.HasValue && !filter.IsIndividual.HasValue && !filter.IsCompany.HasValue && !filter.IsEmployee.HasValue)
                 ));

                return qry.Select(x => new FrontDictionaryAgent
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsIndividual = x.IsIndividual,
                    IsEmployee = x.IsEmployee,
                    IsCompany = x.IsCompany,
                    IsBank = x.IsBank,
                    IsActive = x.IsActive,
                    ResidentTypeId = x.ResidentTypeId,
                    Description = x.Description,
                    Contacts = x.AgentContacts.Select(y => new FrontDictionaryContact
                    {
                        Id = y.Id,
                        AgentId = y.AgentId,
                        Value = y.Contact,
                        IsActive=y.IsActive,
                        Description = y.Description,
                        ContactType = new FrontDictionaryContactType
                        {
                            Id = y.ContactType.Id,
                            Name = y.ContactType.Name,
                            Code=y.ContactType.Code,
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
                var ddt = DictionaryModelConverter.GetDbAgent(context, agent);

                dbContext.DictionaryAgentsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, agent.Id, EnumObjects.DictionaryAgents, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void UpdateAgentName(IContext context, int Id, string newName)
        {
            using (var dbContext = new DmsContext(context))
            {
                var agent = GetAgent(context, Id);
                var ddt = new DictionaryAgents
                {
                    ClientId = context.CurrentClientId,
                    Name = newName,
                    ResidentTypeId = agent.ResidentTypeId,
                    IsBank = agent.IsBank,
                    IsCompany = agent.IsCompany,
                    IsEmployee = agent.IsEmployee,
                    IsIndividual = agent.IsIndividual,
                    Description = agent.Description,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = context.CurrentAgentId,
                    IsActive = agent.IsActive
                };
                dbContext.DictionaryAgentsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, Id, EnumObjects.DictionaryAgents, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }

        }


        public void DeleteAgent(IContext context, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(context))
            {


                var ddt = dbContext.DictionaryAgentsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == agent.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentAddressesSet.RemoveRange(
                                   dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.AgentId == agent.Id)
                                   );
                    dbContext.DictionaryAgentContactsSet.RemoveRange(
                                       dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.AgentId == agent.Id)
                                       );
                    dbContext.DictionaryAgentAccountsSet.RemoveRange(
                                                           dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.AgentId == agent.Id)
                                                           );
                    dbContext.DictionaryAgentsSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, agent.Id, EnumObjects.DictionaryAgents, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }


        public int AddAgent(IContext context, InternalDictionaryAgent newAgent)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbAgent(context, newAgent);
                    
                dbContext.DictionaryAgentsSet.Add(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgents, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                newAgent.Id = ddt.Id;
                return ddt.Id;
            }
        }

        #endregion DictionaryAgents

        // Физлица
        #region DictionaryAgentPerson
        public FrontDictionaryAgentPerson GetAgentPerson(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentPersonsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentPerson
                    {
                        Id = x.Id,
                        IsIndividual = true,
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

                var qry = dbContext.DictionaryAgentPersonsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Agent.IsIndividual);

                // Пагинация
                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.LastName)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

                return qry.Select(x => new FrontDictionaryAgentPerson
                {
                    Id = x.Id,
                    IsIndividual = true,
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
                }).ToList();
            }
        }

        public void UpdateAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbAgentPerson(person);

                dbContext.DictionaryAgentPersonsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.Update);

                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();


            }
        }


        public void DeleteAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentPersonsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == person.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentPersonsSet.Remove(ddt);
                    dbContext.SaveChanges();

                    var agent = GetAgent(context, person.Id);

                    if ((!agent.IsCompany && !agent.IsEmployee && !agent.IsBank))
                    {
                        DeleteAgent(context, new InternalDictionaryAgent { Id = person.Id });
                    }
                    else
                    {
                        UpdateAgentRole(context, person.Id, EnumDictionaryAgentTypes.isIndividual);
                    }

                    CommonQueries.AddFullTextCashInfo(dbContext, person.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.Delete);
                }

            }
        }

        public int AddAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            {

                var agent = GetAgent(context, person.Id);
                if (agent == null)
                {
                    var newAgent = new InternalDictionaryAgent
                    {
                        Name = person.LastName.Trim() + " " + person.FirstName.Trim() + " " + person.MiddleName.Trim(),
                        IsActive = person.IsActive,
                        Description = person.Description,
                        IsBank = false,
                        IsCompany = false,
                        IsEmployee = true,
                        IsIndividual = true,
                        LastChangeDate = person.LastChangeDate,
                        LastChangeUserId = person.LastChangeUserId,
                    };
                    person.Id = AddAgent(context, newAgent);
                }
                else
                {
                    agent.IsIndividual = true;
                    UpdateAgent(context, new InternalDictionaryAgent
                    {
                        Id = agent.Id,
                        Name = agent.Name,
                        IsActive = agent.IsActive,
                        Description = agent.Description,
                        IsBank = agent.IsBank,
                        IsCompany = agent.IsCompany,
                        IsEmployee = agent.IsEmployee,
                        IsIndividual = agent.IsIndividual,
                        ResidentTypeId = agent.ResidentTypeId ?? 0,
                        LastChangeDate = person.LastChangeDate,
                        LastChangeUserId = person.LastChangeUserId
                    });
                };

                var ddt = DictionaryModelConverter.GetDbAgentPerson(person);
                    
            
                dbContext.DictionaryAgentPersonsSet.Add(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentPersons, EnumOperationType.AddNew);

                dbContext.SaveChanges();

                return person.Id;
            }
        }

        #endregion DictionaryAgentPerson

        #region DictionaryAgentEmployee

        public FrontDictionaryAgentEmployee GetAgentEmployee(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryAgentEmployeesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentEmployee
                {
                    Id = x.Id,
                    PersonnelNumber = x.PersonnelNumber,
                    IsActive = x.IsActive,
                    IsEmployee = true,
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


        public void UpdateAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentEmployees
                {
                    Id = employee.Id,
                    PersonnelNumber = employee.PersonnelNumber,
                    Description = employee.Description,
                    LastChangeDate = employee.LastChangeDate,
                    LastChangeUserId = employee.LastChangeUserId,
                    IsActive = employee.IsActive
                };

                dbContext.DictionaryAgentEmployeesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.Update);

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

                var ddt = dbContext.DictionaryAgentEmployeesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == employee.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentEmployeesSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
                else
                {
                    UpdateAgentRole(context, employee.Id, EnumDictionaryAgentTypes.isEmployee);
                }
            }
        }

        public int AddAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            {

                var agent = GetAgentPerson(context, employee.Id);
                if (agent == null)
                {
                    
                    var newAgent = new InternalDictionaryAgentPerson
                    {
                        LastName = employee.LastName,
                        FirstName = employee.FirstName,
                        MiddleName = employee.MiddleName,
                        IsActive = employee.IsActive,
                        Description = employee.Description,
                        TaxCode = employee.TaxCode,
                        BirthDate = employee.BirthDate,
                        IsMale = employee.IsMale,
                        PassportSerial = employee.PassportSerial,
                        PassportNumber = employee.PassportNumber,
                        PassportDate = employee.PassportDate,
                        PassportText = employee.PassportText,
                        LastChangeDate = employee.LastChangeDate,
                        LastChangeUserId = employee.LastChangeUserId,
                    };
                    employee.Id = AddAgentPerson(context, newAgent);
                }
                else
                {

                    UpdateAgent(context, new InternalDictionaryAgent
                    {
                        Id = agent.Id,
                        Name = agent.Name,
                        IsActive = agent.IsActive,
                        Description = agent.Description,
                        IsBank = agent.IsBank,
                        IsCompany = agent.IsCompany,
                        IsEmployee = true,
                        IsIndividual = agent.IsIndividual,
                        ResidentTypeId = agent.ResidentTypeId ?? 0,
                        LastChangeDate = employee.LastChangeDate,
                        LastChangeUserId = employee.LastChangeUserId
                    });
                };

                var ddt = new DictionaryAgentEmployees
                {
                    Id = employee.Id,
                    AgentPersonId = employee.Id,
                    PersonnelNumber = employee.PersonnelNumber,
                    Description = employee.Description,
                    LastChangeDate = employee.LastChangeDate,
                    LastChangeUserId = employee.LastChangeUserId,
                    IsActive = employee.IsActive
                };
                dbContext.DictionaryAgentEmployeesSet.Add(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentEmployees, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                return employee.Id;
            }
        }

        public IEnumerable<FrontDictionaryAgentEmployee> GetAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentEmployeesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Agent.IsEmployee);

                // Пагинация
                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.Agent.AgentPerson.LastName)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
                if (filter.BirthPeriod.IsActive)
                {
                    qry = qry.Where(x => x.Agent.AgentPerson.BirthDate >= filter.BirthPeriod.DateBeg);
                    qry = qry.Where(x => x.Agent.AgentPerson.BirthDate <= filter.BirthPeriod.DateEnd);
                }

                return qry.Select(x => new FrontDictionaryAgentEmployee
                {
                    Id = x.Id,
                    PersonnelNumber = x.PersonnelNumber,
                    IsActive = x.IsActive,
                    IsEmployee = true,
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

        #endregion DictionaryAgentEmployee

        #region DictionaryAgentAddress
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
                    AddressType = new FrontDictionaryAddressType { Id = x.AdressTypeId, Name = x.AddressType.Name,IsActive = x.AddressType.IsActive},
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
                var ddt = DictionaryModelConverter.GetDbAgentAddress(addr);

                dbContext.DictionaryAgentAddressesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.Update);

                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == addr.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentAddressesSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }

        public int AddAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbAgentAddress(addr);

                dbContext.DictionaryAgentAddressesSet.Add(ddt);
                dbContext.SaveChanges();

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentAddresses, EnumOperationType.AddNew);
                addr.Id = ddt.Id;
                return ddt.Id;
            }
        }

        public IEnumerable<FrontDictionaryAgentAddress> GetAgentAddresses(IContext context, int agentId, FilterDictionaryAgentAddress filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();


                qry = qry.Where(x => x.AgentId == filter.AgentId);

                qry = qry.OrderBy(x => x.Address);

                if (filter.AddressTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AddressTypeId.Contains(x.AdressTypeId));
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
                    
                        qry = qry.Where(x => x.PostCode==filter.PostCodeExact);
                    
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

                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                return qry.Select(x => new FrontDictionaryAgentAddress
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    AddressType = new FrontDictionaryAddressType { Id = x.AddressType.Id, Name = x.AddressType.Name },
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
                    qry = qry.Where(x => addresses.Contains(x.Id));
                }

                return qry.Select(x => x.AgentId).ToList();
            }
        }
        #endregion

        // Типы адресов 
        #region DictionaryAddressTypes
        public void UpdateAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbAddressType(context, addrType);
                   
                dbContext.DictionaryAddressTypesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAddressType, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }


        public void DeleteAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == addrType.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAddressTypesSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAddressType, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }

        public int AddAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbAddressType(context, addrType);

                dbContext.DictionaryAddressTypesSet.Add(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAddressType, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                addrType.Id = ddt.Id;
                return ddt.Id;
            }
        }

        public InternalDictionaryAddressType GetInternalDictionaryAddressType(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAddressTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }

                }

                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
                }

                return qry.Select(x => new InternalDictionaryAddressType
                {
                    Id = x.Id,
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

                qry = qry.OrderBy(x => x.Name);

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

                return qry.Select(x => new FrontDictionaryAddressType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                }).ToList();
            }
        }
        #endregion

        // Компании
        #region DictionaryAgentCompanies
        public FrontDictionaryAgentCompany GetAgentCompany(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentCompaniesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentCompany
                    {
                        Id = x.Id,
                        IsCompany = x.Agent.IsCompany,
                        IsIndividual = x.Agent.IsIndividual,
                        IsBank = x.Agent.IsBank,
                        IsEmployee = x.Agent.IsEmployee,
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
                var qry = dbContext.DictionaryAgentCompaniesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                // Пагинация
                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.FullName)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

                return qry.Select(x => new FrontDictionaryAgentCompany
                {
                    Id = x.Id,
                    IsCompany = x.Agent.IsCompany,
                    IsIndividual = x.Agent.IsIndividual,
                    IsBank = x.Agent.IsBank,
                    IsEmployee = x.Agent.IsEmployee,
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

                }).ToList();
            }
        }

        public void UpdateAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbAgentCompany(company);

                dbContext.DictionaryAgentCompaniesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                UpdateAgentName(context, company.Id, company.ShortName);

            }

        }


        public void DeleteAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentCompaniesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == company.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentCompaniesSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.Delete);
                    dbContext.SaveChanges();


                    var agent = GetAgent(context, company.Id);

                    if (!agent.IsBank && !agent.IsEmployee && !agent.IsIndividual)
                    {
                        DeleteAgent(context, new InternalDictionaryAgent { Id = company.Id });
                    }
                    else
                    {
                        UpdateAgentRole(context, company.Id, EnumDictionaryAgentTypes.isCompany);
                    }
                }
            }
        }

        public int AddAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {

                var agent = GetAgent(context, company.Id);
                if (agent == null)
                {
                    var newAgent = new InternalDictionaryAgent
                    {
                        Name = company.ShortName,
                        IsActive = company.IsActive,
                        Description = company.Description,
                        IsBank = false,
                        IsCompany = true,
                        IsEmployee = false,
                        IsIndividual = false,
                        LastChangeDate = company.LastChangeDate,
                        LastChangeUserId = company.LastChangeUserId,
                    };
                    company.Id = AddAgent(context, newAgent);
                }
                else
                {
                    agent.IsCompany = true;
                    UpdateAgent(context, new InternalDictionaryAgent
                    {
                        Id = agent.Id,
                        Name = company.ShortName,
                        IsActive = agent.IsActive,
                        Description = agent.Description,
                        IsBank = agent.IsBank,
                        IsCompany = agent.IsCompany,
                        IsEmployee = agent.IsEmployee,
                        IsIndividual = agent.IsIndividual,
                        ResidentTypeId = agent.ResidentTypeId ?? 0,
                        LastChangeDate = company.LastChangeDate,
                        LastChangeUserId = company.LastChangeUserId
                    });
                };

                var ddt = DictionaryModelConverter.GetDbAgentCompany(company);

                dbContext.DictionaryAgentCompaniesSet.Add(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentCompanies, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                return company.Id;
            }
        }
        #endregion DictionaryAgentCompanies

        #region DictionaryAgentBanks
        public FrontDictionaryAgentBank GetAgentBank(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentBanksSet.Where(x => x.Agent.ClientId == context.CurrentClientId).Where(x => x.Id == id).Select(x => new FrontDictionaryAgentBank
                    {
                        Id = x.Id,
                        IsBank = true,
                        MFOCode = x.MFOCode,
                        Swift = x.Swift,
                        IsCompany = x.Agent.IsCompany,
                        IsEmployee = x.Agent.IsEmployee,
                        IsIndividual = x.Agent.IsIndividual,
                        Name = x.Agent.Name,
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

        public void UpdateAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbAgentBank(bank);

                dbContext.DictionaryAgentBanksSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                UpdateAgentName(context, bank.Id, bank.Name);

            }
        }
        public void DeleteAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentBanksSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == bank.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentBanksSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.Delete);
                    dbContext.SaveChanges();


                    var agent = GetAgent(context, bank.Id);

                    if (!agent.IsCompany && !agent.IsEmployee && !agent.IsIndividual)
                    {
                        DeleteAgent(context, new InternalDictionaryAgent { Id = bank.Id });
                    }
                    else
                    {
                        UpdateAgentRole(context, bank.Id, EnumDictionaryAgentTypes.isBank);
                    }

                }
            }
        }

        public int AddAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            {

                var agent = GetAgent(context, bank.Id);
                if (agent == null)
                {
                    var newAgent = new InternalDictionaryAgent
                    {
                        Name = bank.Name,
                        IsActive = bank.IsActive,
                        Description = bank.Description,
                        IsBank = true,
                        IsCompany = false,
                        IsEmployee = false,
                        IsIndividual = false,
                        LastChangeDate = bank.LastChangeDate,
                        LastChangeUserId = bank.LastChangeUserId,
                    };
                    bank.Id = AddAgent(context, newAgent);
                }
                else
                {
                    UpdateAgentRole(context, bank.Id, EnumDictionaryAgentTypes.isBank);
                };

                var ddt = DictionaryModelConverter.GetDbAgentBank(bank);

                dbContext.DictionaryAgentBanksSet.Add(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentBanks, EnumOperationType.AddNew);
                dbContext.SaveChanges();

                return bank.Id;
            }
        }


        public IEnumerable<FrontDictionaryAgentBank> GetAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {

                var qry = dbContext.DictionaryAgentBanksSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.Agent.IsBank);

                // Пагинация
                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.Agent.Name)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

                return qry.Select(x => new FrontDictionaryAgentBank
                {
                    Id = x.Id,
                    IsBank = true,
                    MFOCode = x.MFOCode,
                    Swift = x.Swift,
                    IsCompany = x.Agent.IsCompany,
                    IsEmployee = x.Agent.IsEmployee,
                    IsIndividual = x.Agent.IsIndividual,
                    Name = x.Agent.Name,
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

        #region DictionaryAgentAccounts
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
                var ddt = DictionaryModelConverter.GetDbAgentAccount(account);

                dbContext.DictionaryAgentAccountsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.Update);
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

                var ddt = dbContext.DictionaryAgentAccountsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == account.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentAccountsSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }

        public int AddAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbAgentAccount(account);

                dbContext.DictionaryAgentAccountsSet.Add(ddt);

                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryAgentAccounts, EnumOperationType.AddNew);
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

        // Типы контактов
        #region DictionaryContactTypes
        public FrontDictionaryContactType GetInternalDictionaryContactType(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
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
                    
                    qry = qry.Where(x => x.Code==filter.Code);
                    
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
                    InputMask=x.InputMask,
                    Code=x.Code,
                    IsActive = x.IsActive
                }).FirstOrDefault();
            }
        }
        public void UpdateContactType(IContext context, InternalDictionaryContactType contactType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryContactTypes
                {
                    ClientId = context.CurrentClientId,
                    Id = contactType.Id,
                    InputMask = contactType.InputMask,
                    Code=contactType.Code,
                    LastChangeDate = contactType.LastChangeDate,
                    LastChangeUserId = contactType.LastChangeUserId,
                    Name = contactType.Name,
                    IsActive = contactType.IsActive
                };
                dbContext.DictionaryContactTypesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContactType, EnumOperationType.Update);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        public void DeleteContactType(IContext context, InternalDictionaryContactType contactType)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryContactTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == contactType.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryContactTypesSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContactType, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }
        public int AddContactType(IContext context, InternalDictionaryContactType contactType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryContactTypes
                {
                    ClientId = context.CurrentClientId,
                    Name = contactType.Name,
                    IsActive = contactType.IsActive,
                    InputMask=contactType.InputMask,
                    Code=contactType.Code,
                    LastChangeDate = contactType.LastChangeDate,
                    LastChangeUserId = contactType.LastChangeUserId
                };
                dbContext.DictionaryContactTypesSet.Add(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContactType, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                contactType.Id = ddt.Id;
                return ddt.Id;
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
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
                    InputMask=x.InputMask,
                    Code=x.Code,
                    IsActive = x.IsActive
                }).ToList();
            }
        }
        #endregion

        // Контакты
        #region DictionaryContacts

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
                    ContactType = new FrontDictionaryContactType { Id = x.ContactTypeId, Name = x.ContactType.Name,Code=x.ContactType.Code,IsActive=x.ContactType.IsActive },
                    Value = x.Contact,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).FirstOrDefault();
            }
        }

        public void UpdateContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbContact(contact);

                dbContext.DictionaryAgentContactsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContacts, EnumOperationType.Update);
                dbContext.SaveChanges();
            }
        }
        public void DeleteContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == contact.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentContactsSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContacts, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }
        public int AddContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbContact(contact);
                    
                dbContext.DictionaryAgentContactsSet.Add(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryContacts, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                contact.Id = ddt.Id;
                return ddt.Id;
            }
        }
        public IEnumerable<FrontDictionaryContact> GetContacts(IContext context, int agentId, FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentContactsSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                qry = qry.Where(x => x.AgentId == agentId);

                qry.OrderBy(x => x.ContactType);

                if (filter.ContactTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.ContactTypeId.Contains(x.ContactTypeId));
                }
                if (filter.AgentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AgentId.Contains(x.AgentId));
                }
                if (!string.IsNullOrEmpty(filter.Contact))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Contact))
                    {
                        qry = qry.Where(x => x.Contact.Contains(temp));
                    }
                }
                if (!String.IsNullOrEmpty(filter.ContactExact))
                {
                    qry = qry.Where(x => x.Contact==filter.ContactExact);
                }
                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
        #endregion
        public IEnumerable<int> GetAgentsIDByContacts(IContext context, IEnumerable<int> contacts)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentAddressesSet.Where(x => x.Agent.ClientId == context.CurrentClientId).AsQueryable();

                if (contacts.Any())
                {
                    qry = qry.Where(x => contacts.Contains(x.Id));
                }

                return qry.Select(x => x.AgentId).ToList();
            }
        }
        // Структура предприятия
        #region DictionaryDepartments
        public int AddDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = DictionaryModelConverter.GetDbDepartments(department);
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
                var dd = DictionaryModelConverter.GetDbDepartments(department);
                dbContext.DictionaryDepartmentsSet.Attach(dd);

                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.Update);
                dbContext.Entry(dd).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == department.Id);
                if (dd != null)
                {
                    dbContext.DictionaryDepartmentsSet.Remove(dd);
                    CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryDepartments, EnumOperationType.Delete);
                    dbContext.SaveChanges();
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
                var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).AsQueryable();

                qry = DepartmentGetWhere(ref qry, filter);

                qry = qry.OrderBy(x => x.ParentDepartment.Name).ThenBy(x => x.Name);

                return qry.Select(x => new FrontDictionaryDepartment
                {
                    Id = x.Id,
                    IsActive = x.IsActive,
                    ParentId = x.ParentId,
                    Code = x.Code,
                    Name = x.Name,
                    FullName = x.FullName,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Name,
                    ChiefPositionId = x.ChiefPositionId,
                    ChiefPositionName = x.ChiefPosition.Name,
                    ParentDepartmentName = x.ParentDepartment.Name
                }).ToList();

                //ChildDepartments = x.ChildDepartments.Select(y => new InternalDictionaryDepartment
                //{
                //    Id = x.Id,
                //    LastChangeDate = x.LastChangeDate,
                //    LastChangeUserId = x.LastChangeUserId,
                //    IsActive = x.IsActive,
                //    ParentId = x.ParentId,
                //    Code = x.Code,
                //    Name = x.Name,
                //    FullName = x.FullName,
                //    CompanyId = x.CompanyId,
                //    CompanyName = x.Company.Name,
                //    ChiefPositionId = x.ChiefPositionId,
                //    ChiefPositionName = x.ChiefPosition.Name,
                //    ParentDepartmentName = x.ParentDepartment.Name
                //})
            }
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsDictionaryDepartment(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDepartmentsSet.Where(x => x.Company.ClientId == context.CurrentClientId).AsQueryable();

                qry = DepartmentGetWhere(ref qry, filter);

                var res = qry.Select(x => new FrontDictionaryDepartment
                {
                    Id = x.Id
                }).FirstOrDefault();

                return res != null;
            }
        }

        private static IQueryable<DictionaryDepartments> DepartmentGetWhere(ref IQueryable<DictionaryDepartments> qry, FilterDictionaryDepartment filter)
        {
            // Список первичных ключей
            if (filter.IDs?.Count > 0)
            {
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
            }

            // Отбор по родительским элементам
            if (filter.ParentIDs?.Count > 0)
            {
                qry = qry.Where(x => filter.ParentIDs.Contains(x.ParentId ?? 0));
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
            if (filter.CompanyId != null)
            {
                qry = qry.Where(x => filter.CompanyId == x.CompanyId);
            }

            // Условие по ChiefPositionId
            if (filter.ChiefPositionId != null)
            {
                qry = qry.Where(x => filter.ChiefPositionId == x.ChiefPositionId);
            }

            return qry;
        }
        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
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
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
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

        // Тематики документов
        #region DictionaryDocumentSubjects

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
                if (ddt != null)
                {
                    dbContext.DictionaryDocumentSubjectsSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentSubjects, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
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
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
                qry = qry.Where(x => filter.ParentIDs.Contains(x.ParentId ?? -1));
            }

            return qry;
        }

        #endregion DictionaryDocumentSubjects

        // Типы документов
        #region DictionaryDocumentTypes
        public int AddDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbDocumentType(context, docType);
                dbContext.DictionaryDocumentTypesSet.Add(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                docType.Id = ddt.Id;
                return ddt.Id;
            }
        }

        public void UpdateDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbDocumentType(context, docType);
                dbContext.DictionaryDocumentTypesSet.Attach(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.Update);
                dbContext.Entry(ddt).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryDocumentTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == docType.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryDocumentTypesSet.Remove(ddt);
                    CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryDocumentType, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
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
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
            return qry;
        }
        #endregion DictionaryDocumentTypes

        #region DictionaryEventTypes
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
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
                    qry = qry.Where(x => filter.ImportanceEventTypeIDs.Contains(x.ImportanceEventTypeId));
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.EventTypeId).Contains(x.Id)
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

        #region DictionaryImportanceEventTypes
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
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.EventType.ImportanceEventTypeId).Contains(x.Id)
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

        #region DictionaryLinkTypes
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
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

        // Должности
        #region DictionaryPositions
        public int AddPosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = DictionaryModelConverter.GetDbDictionaryPosition(position);
                dbContext.DictionaryPositionsSet.Add(dd);
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
                var dd = DictionaryModelConverter.GetDbDictionaryPosition(position);
                dbContext.DictionaryPositionsSet.Attach(dd);
                dbContext.Entry(dd).State = System.Data.Entity.EntityState.Modified;
                CommonQueries.AddFullTextCashInfo(dbContext, dd.Id, EnumObjects.DictionaryPositions, EnumOperationType.Update);
                dbContext.SaveChanges();
            }
        }

        public void DeletePosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dp = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == position.Id);
                if (dp != null)
                {
                    dbContext.DictionaryPositionsSet.Remove(dp);
                    CommonQueries.AddFullTextCashInfo(dbContext, dp.Id, EnumObjects.DictionaryPositions, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }

        public int? GetExecutorAgentIdByPositionId(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Where(x => x.Id == id)
                    .Select(x => x.ExecutorAgentId).FirstOrDefault();
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
                        ParentId = x.ParentId,
                        Name = x.Name,
                        DepartmentId = x.DepartmentId,
                        ExecutorAgentId = x.ExecutorAgentId,
                        ParentPositionName = x.ParentPosition.Name,
                        DepartmentName = x.Department.Name,
                        ExecutorAgentName = x.ExecutorAgent.Name,
                        //pss !!!!!!! GetPosition - ChildPositions
                        //ChildPositions = x.ChildPositions.Select(y => new FrontDictionaryPosition
                        //{
                        //    Id = y.Id,
                        //    IsActive = y.IsActive,
                        //    ParentId = y.ParentId,
                        //    Name = y.Name,
                        //    FullName = x.pos.FullName,
                        //    DepartmentId = y.DepartmentId,
                        //    ExecutorAgentId = y.ExecutorAgentId,
                        //    ParentPositionName = y.ParentPosition.Name,
                        //    DepartmentName = y.Department.Name,
                        //    ExecutorAgentName = y.ExecutorAgent.Name
                        //}),
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
                            CompanyName = y.Company.Name,
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
                // pss эта сборка Where-условий повторяется 3 раза (GetPositionsWithActions, ExistsPosition, GetPosition). У меня НЕ получается вынести Where в отдельную функцию.
                var qry = dbContext.DictionaryPositionsSet.Where(x => x.Department.Company.ClientId == context.CurrentClientId).Select(x => new { pos = x, subordMax = 0 }).AsQueryable();
   
                // Список первичных ключей
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
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
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.TargetPositionId).Contains(x.pos.Id)
                                );
                }

                

                if (filter.SubordinatedPositions?.Count > 0)
                {
                    qry = qry.GroupJoin(
                                        dbContext.AdminSubordinationsSet.Where(y => filter.SubordinatedPositions.Contains(y.SourcePositionId)),
                                        x => x.pos.Id,
                                        y => y.TargetPositionId,
                                        (x, y) => new { pos = x.pos, subordMax = y.Max(z => z.SubordinationTypeId) }
                                        )
                             .Where(x => x.subordMax > 0);
                }

                return qry.Select(x => new FrontDictionaryPosition
                {
                    Id = x.pos.Id,
                    IsActive = x.pos.IsActive,
                    ParentId = x.pos.ParentId,
                    Name = x.pos.Name,
                    FullName = x.pos.FullName,
                    DepartmentId = x.pos.DepartmentId,
                    ExecutorAgentId = x.pos.ExecutorAgentId,
                    ParentPositionName = x.pos.ParentPosition.Name,
                    DepartmentName = x.pos.Department.Name,
                    ExecutorAgentName = x.pos.ExecutorAgent.Name,
                    MaxSubordinationTypeId = (x.subordMax > 0 ? (int?)x.subordMax : null)
                }).ToList();
            }
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
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
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
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.TargetPositionId).Contains(x.pos.Id)
                                );
                }



                if (filter.SubordinatedPositions?.Count > 0)
                {
                    qry = qry.GroupJoin(
                                        dbContext.AdminSubordinationsSet.Where(y => filter.SubordinatedPositions.Contains(y.SourcePositionId)),
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
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
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
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet.Where(y => y.Document.TemplateDocument.ClientId == context.CurrentClientId)
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.TargetPositionId).Contains(x.pos.Id)
                                );
                }
                
                if (filter.SubordinatedPositions?.Count > 0)
                {
                    qry = qry.GroupJoin(
                                        dbContext.AdminSubordinationsSet.Where(y => filter.SubordinatedPositions.Contains(y.SourcePositionId)),
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

        // Исполнители
        #region DictionaryPositionExecutors
        public int AddExecutor(IContext context, InternalDictionaryPositionExecutor executor)
        {
            using (var dbContext = new DmsContext(context))
            {
                DictionaryPositionExecutors dc = DictionaryModelConverter.GetDbExecutor(executor);
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
                DictionaryPositionExecutors drj = DictionaryModelConverter.GetDbExecutor(executor);
                dbContext.DictionaryPositionExecutorsSet.Attach(drj);
                CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.Update);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteExecutor(IContext context, InternalDictionaryPositionExecutor docSubject)
        {
            using (var dbContext = new DmsContext(context))
            {
                var drj = dbContext.DictionaryPositionExecutorsSet.FirstOrDefault(x => x.Id == docSubject.Id);
                if (drj != null)
                {
                    dbContext.DictionaryPositionExecutorsSet.Remove(drj);
                    CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryPositionExecutors, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDictionaryPositionExecutor GetInternalDictionaryPositionExecutor(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryPositionExecutorsSet.AsQueryable();

                qry = ExecutorGetWhere(ref qry, filter);

                return qry.Select(x => new InternalDictionaryPositionExecutor
                {
                    // pss Перегонка значений DictionaryPositionExecutors
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
                var qry = dbContext.DictionaryPositionExecutorsSet.AsQueryable();

                qry = ExecutorGetWhere(ref qry, filter);

                return qry.Select(x => new FrontDictionaryPositionExecutor
                {
                    // pss Перегонка значений DictionaryPositionExecutors
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

        // Для использования в коммандах метод CanExecute
        public bool ExistsExecutor(IContext context, FilterDictionaryPositionExecutor filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryPositionExecutorsSet.AsQueryable();

                qry = ExecutorGetWhere(ref qry, filter);

                var res = qry.Select(x => new FrontDictionaryPositionExecutor
                {
                    Id = x.Id
                }).FirstOrDefault();

                return res != null;
            }
        }

        private static IQueryable<DictionaryPositionExecutors> ExecutorGetWhere(ref IQueryable<DictionaryPositionExecutors> qry, FilterDictionaryPositionExecutor filter)
        {
            // Список первичных ключей
            if (filter.IDs?.Count > 0)
            {
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
            }

            // Тоько активные/неактивные
            if (filter.IsActive != null)
            {
                qry = qry.Where(x => filter.IsActive == x.IsActive);
            }

            return qry;
        }

        #endregion DictionaryPositionExecutors

        // Типы исполнителей
        #region DictionaryPositionExecutorTypes
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
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

        // Журналы регистрации
        #region DictionaryRegistrationJournals
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
                if (drj != null)
                {
                    dbContext.DictionaryRegistrationJournalsSet.Remove(drj);
                    CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryRegistrationJournals, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
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
                    // pss Перегонка значений DictionaryRegistrationJournals
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
                    // pss Перегонка значений DictionaryRegistrationJournals
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
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

            // Условие по Index
            if (!string.IsNullOrEmpty(filter.Index))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Index))
                {
                    qry = qry.Where(x => x.Index.Contains(temp));
                }
            }

            // Условие по DepartmentIDs
            if (filter.DepartmentIDs?.Count > 0)
            {
                qry = qry.Where(x => filter.DepartmentIDs.Contains(x.DepartmentId));
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

        // Компании
        #region DictionaryCompanies
        public int AddCompany(IContext context, InternalDictionaryCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {
                DictionaryCompanies dc = DictionaryModelConverter.GetDbCompany(context, company);
                dbContext.DictionaryCompaniesSet.Add(dc);
                CommonQueries.AddFullTextCashInfo(dbContext, dc.Id, EnumObjects.DictionaryCompanies, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                company.Id = dc.Id;
                return dc.Id;
            }
        }

        public void UpdateCompany(IContext context, InternalDictionaryCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {
                DictionaryCompanies drj = DictionaryModelConverter.GetDbCompany(context, company);
                dbContext.DictionaryCompaniesSet.Attach(drj);
                CommonQueries.AddFullTextCashInfo(dbContext,drj.Id, EnumObjects.DictionaryCompanies, EnumOperationType.Update);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteCompany(IContext context, InternalDictionaryCompany docSubject)
        {
            using (var dbContext = new DmsContext(context))
            {
                var drj = dbContext.DictionaryCompaniesSet.FirstOrDefault(x => x.Id == docSubject.Id);
                if (drj != null)
                {
                    dbContext.DictionaryCompaniesSet.Remove(drj);
                    CommonQueries.AddFullTextCashInfo(dbContext, drj.Id, EnumObjects.DictionaryCompanies, EnumOperationType.Delete);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDictionaryCompany GetInternalDictionaryCompany(IContext context, FilterDictionaryCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryCompaniesSet.AsQueryable();

                qry = CompanyGetWhere(ref qry, filter);

                return qry.Select(x => new InternalDictionaryCompany
                {
                    // pss Перегонка значений DictionaryCompany
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryCompany> GetCompanies(IContext context, FilterDictionaryCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                
                var qry = dbContext.DictionaryCompaniesSet.Where(x => x.ClientId == context.CurrentClientId).AsQueryable();

                qry = CompanyGetWhere(ref qry, filter);

                qry = qry.OrderBy(x => x.Name);

                return qry.Select(x => new FrontDictionaryCompany
                {
                    // pss Перегонка значений DictionaryCompany
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Name = x.Name
                }).ToList();
            }
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsCompany(IContext context, FilterDictionaryCompany filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryCompaniesSet.AsQueryable();

                qry = CompanyGetWhere(ref qry, filter);

                var res = qry.Select(x => new FrontDictionaryCompany
                {
                    Id = x.Id
                }).FirstOrDefault();

                return res != null;
            }
        }

        private static IQueryable<DictionaryCompanies> CompanyGetWhere(ref IQueryable<DictionaryCompanies> qry, FilterDictionaryCompany filter)
        {
            // Список первичных ключей
            if (filter.IDs?.Count > 0)
            {
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Исключение списка первичных ключей
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

            return qry;
        }

        #endregion DictionaryCompanies


        #region DictionaryResultTypes
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
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                else
                {
                    qry = qry.Where(x => x.Id >= 0);
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

        #region DictionarySendTypes
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
                        SubordinationTypeName = x.SubordinationType.Name
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
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
                    SubordinationTypeName = x.SubordinationType.Name
                }).ToList();
            }
        }
        #endregion DictionarySendTypes

        #region DictionaryStandartSendListContents
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
                        AccessLevelName = x.AccessLevel.Name
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryStandartSendListContent> GetStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryStandartSendListContentsSet.Where(x => x.StandartSendList.ClientId == context.CurrentClientId).AsQueryable();

                // Список первичных ключей
                if (filter.StandartSendListId.Count > 0)
                {
                    qry = qry.Where(x => filter.StandartSendListId.Contains(x.StandartSendListId));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
                }

                if (filter.SendTypeId.Count > 0)
                {
                    qry = qry.Where(x => filter.SendTypeId.Contains((EnumSendTypes)x.SendTypeId));
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
                    AccessLevelName = x.AccessLevel.Name
                }).ToList();
            }
        }

        public void UpdateStandartSendListContent(IContext context,
            InternalDictionaryStandartSendListContent content)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = DictionaryModelConverter.GetDbStandartSendListContent(content);
                dbContext.DictionaryStandartSendListContentsSet.Attach(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.Update);
                var entity = dbContext.Entry(ddt);
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
                if (ddt == null) return;
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
                var ddt = DictionaryModelConverter.GetDbStandartSendListContent(content);
                dbContext.DictionaryStandartSendListContentsSet.Add(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendListContent, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                content.Id = ddt.Id;
                return ddt.Id;
            }
        }
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
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
                            StandartSendListContents =
                                x.StandartSendListContents.Select(y => new FrontDictionaryStandartSendListContent()
                                {
                                    Id = y.Id,
                                    StandartSendListId = y.StandartSendListId,
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
                                    AccessLevelName = y.AccessLevel.Name
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
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
                }

                // Поиск по наименованию
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExpressions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
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
                    PositionName = x.Position.Name,
                }).ToList();
            }
        }

        public void UpdateStandartSendList(IContext context, InternalDictionaryStandartSendList list)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryStandartSendLists()
                {
                    ClientId = context.CurrentClientId,
                    Id = list.Id,
                    Name = list.Name,
                    PositionId = list.PositionId,
                    LastChangeDate = list.LastChangeDate,
                    LastChangeUserId = list.LastChangeUserId
                };

                dbContext.DictionaryStandartSendListsSet.Attach(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.Update);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteStandartSendList(IContext context, InternalDictionaryStandartSendList list)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryStandartSendListsSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == list.Id);
                if (ddt == null) return;
                dbContext.DictionaryStandartSendListsSet.Remove(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.Delete);
                dbContext.SaveChanges();
            }
        }

        public int AddStandartSendList(IContext context, InternalDictionaryStandartSendList list)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryStandartSendLists()
                {
                    ClientId = context.CurrentClientId,
                    Id = list.Id,
                    Name = list.Name,
                    PositionId = list.PositionId,
                    LastChangeDate = list.LastChangeDate,
                    LastChangeUserId = list.LastChangeUserId
                };
                dbContext.DictionaryStandartSendListsSet.Add(ddt);
                CommonQueries.AddFullTextCashInfo(dbContext, ddt.Id, EnumObjects.DictionaryStandartSendLists, EnumOperationType.AddNew);
                dbContext.SaveChanges();
                list.Id = ddt.Id;
                return ddt.Id;
            }
        }

        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
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
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

        #region DictionaryTags

        public InternalDictionaryTag GetInternalDictionaryTags(IContext ctx, FilterDictionaryTag filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.DictionaryTagsSet.Where(x => x.ClientId == ctx.CurrentClientId).AsQueryable();

                qry = qry.Where(x => ctx.IsAdmin || !x.PositionId.HasValue || ctx.CurrentPositionsIdList.Contains(x.PositionId ?? 0));

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
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

                qry = qry.Where(x => ctx.IsAdmin || !x.PositionId.HasValue || ctx.CurrentPositionsIdList.Contains(x.PositionId ?? 0));

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                return qry.Select(x => new FrontDictionaryTag
                {
                    Id = x.Id,
                    Name = x.Name,
                    PositionId = x.PositionId,
                    IsSystem = !x.PositionId.HasValue,
                    Color = x.Color,
                    PositionName = x.Position.Name
                }).ToList();
            }
        }

        public int AddTag(IContext context, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var savTag = new DictionaryTags
                {
                    ClientId = context.CurrentClientId,
                    Name = model.Name,
                    PositionId = context.CurrentPositionId,
                    Color = model.Color,
                    LastChangeUserId = context.CurrentAgentId,
                    LastChangeDate = DateTime.Now
                };

                dbContext.DictionaryTagsSet.Add(savTag);
                dbContext.SaveChanges();
                model.Id = savTag.Id;
                return savTag.Id;
            }
        }
        public void UpdateTag(IContext ctx, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var savTag = dbContext.DictionaryTagsSet.Where(x => x.ClientId == ctx.CurrentClientId)
                    .FirstOrDefault(x => x.Id == model.Id && (ctx.IsAdmin || ctx.CurrentPositionsIdList.Contains(x.PositionId ?? 0)));

                if (savTag?.Id > 0)
                {
                    savTag.Name = model.Name;
                    savTag.Color = model.Color;
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
                if (item != null)
                {
                    dbContext.DictionaryTagsSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion DictionaryTags

        #region AdminAccessLevels
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
                    qry = qry.Where(x => filter.AccessLevelId.Contains(x.Id));
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

        #region CustomDictionaryTypes
        public void UpdateCustomDictionaryType(IContext context, InternalCustomDictionaryType model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = DictionaryModelConverter.GetDbCustomDictionaryType(context, model);
                dbContext.CustomDictionaryTypesSet.Attach(item);
                var entity = dbContext.Entry(item);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public int AddCustomDictionaryType(IContext context, InternalCustomDictionaryType model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = DictionaryModelConverter.GetDbCustomDictionaryType(context, model);
                dbContext.CustomDictionaryTypesSet.Add(item);
                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void DeleteCustomDictionaryType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = dbContext.CustomDictionaryTypesSet.Where(x => x.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    dbContext.CustomDictionariesSet.RemoveRange(item.CustomDictionaries);
                    dbContext.CustomDictionaryTypesSet.Remove(item);
                    dbContext.SaveChanges();
                }
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
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
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
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
                    }
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

        #region CustomDictionaries
        public void UpdateCustomDictionary(IContext context, InternalCustomDictionary model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = DictionaryModelConverter.GetDbCustomDictionary(model);
                dbContext.CustomDictionariesSet.Attach(item);
                var entity = dbContext.Entry(item);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Description).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public int AddCustomDictionary(IContext context, InternalCustomDictionary model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = DictionaryModelConverter.GetDbCustomDictionary(model);
                dbContext.CustomDictionariesSet.Add(item);
                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void DeleteCustomDictionary(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = dbContext.CustomDictionariesSet.Where(x => x.CustomDictionaryType.ClientId == context.CurrentClientId).FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    dbContext.CustomDictionariesSet.Remove(item);
                    dbContext.SaveChanges();
                }
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
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
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
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
                    }
                }

                // Исключение списка первичных ключей
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
