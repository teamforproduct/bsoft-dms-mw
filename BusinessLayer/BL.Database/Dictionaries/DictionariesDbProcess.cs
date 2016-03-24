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
        //        var agent = GetDictionaryAgent(context, id);
        //        if (agent.IsBank) { list.Add(EnumDictionaryAgentTypes.isBank); }
        //        if (agent.IsEmployee) { list.Add(EnumDictionaryAgentTypes.isEmployee); }
        //        if (agent.IsIndividual) { list.Add(EnumDictionaryAgentTypes.isIndividual); }
        //        if (agent.IsCompany) { list.Add(EnumDictionaryAgentTypes.isCompany); }

        //        return list;
        //    }
        //}

        public void UpdateDictionaryAgentRole(IContext context, int id, EnumDictionaryAgentTypes role)
        {
            using (var dbContext = new DmsContext(context))
            {
                var agent = GetDictionaryAgent(context, id);
                var ddt = new DictionaryAgents
                {
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


        public FrontDictionaryAgent GetDictionaryAgent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return dbContext.DictionaryAgentsSet.Where(x => x.Id == id).Select(x => new FrontDictionaryAgent
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

        public IEnumerable<FrontDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentsSet.AsQueryable();

                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.Name)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }
                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
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
                        ContactType = new FrontDictionaryContactType
                        {
                            Id = y.ContactType.Id,
                            Name = y.ContactType.Name,
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
                        }
                    })

                }).ToList();
            }
        }

        public void UpdateDictionaryAgent(IContext context, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgents
                {
                    Id = agent.Id,
                    Name = agent.Name,
                    ResidentTypeId = agent.ResidentTypeId,
                    IsBank = agent.IsBank,
                    IsCompany = agent.IsCompany,
                    IsEmployee = agent.IsEmployee,
                    IsIndividual = agent.IsIndividual,
                    Description = agent.Description,
                    LastChangeDate = agent.LastChangeDate,
                    LastChangeUserId = agent.LastChangeUserId,
                    IsActive = agent.IsActive
                };

                dbContext.DictionaryAgentsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void UpdateDictionaryAgentName(IContext context, int Id, string newName)
        {
            using (var dbContext = new DmsContext(context))
            {
                var agent = GetDictionaryAgent(context, Id);
                var ddt = new DictionaryAgents
                {
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
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }

        }


        public void DeleteDictionaryAgent(IContext context, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(context))
            {


                var ddt = dbContext.DictionaryAgentsSet.FirstOrDefault(x => x.Id == agent.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentAddressesSet.RemoveRange(
                                   dbContext.DictionaryAgentAddressesSet.Where(x => x.AgentId == agent.Id)
                                   );
                    dbContext.DictionaryAgentContactsSet.RemoveRange(
                                       dbContext.DictionaryAgentContactsSet.Where(x => x.AgentId == agent.Id)
                                       );
                    dbContext.DictionaryAgentAccountsSet.RemoveRange(
                                                           dbContext.DictionaryAgentAccountsSet.Where(x => x.AgentId == agent.Id)
                                                           );
                    dbContext.DictionaryAgentsSet.Remove(ddt);

                    dbContext.SaveChanges();
                }
            }
        }


        public int AddDictionaryAgent(IContext context, InternalDictionaryAgent newAgent)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgents
                {
                    Name = newAgent.Name,
                    ResidentTypeId = newAgent.ResidentTypeId,
                    IsBank = newAgent.IsBank,
                    IsCompany = newAgent.IsCompany,
                    IsEmployee = newAgent.IsEmployee,
                    IsIndividual = newAgent.IsIndividual,
                    Description = newAgent.Description,
                    LastChangeDate = newAgent.LastChangeDate,
                    LastChangeUserId = newAgent.LastChangeUserId,
                    IsActive = newAgent.IsActive
                };
                dbContext.DictionaryAgentsSet.Add(ddt);
                dbContext.SaveChanges();
                newAgent.Id = ddt.Id;
                return ddt.Id;
            }
        }

        #endregion DictionaryAgents

        // Физлица
        #region DictionaryAgentPerson
        public FrontDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentPersonsSet.Where(x => x.Id == id).Select(x => new FrontDictionaryAgentPerson
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

        public IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {

                var qry = dbContext.DictionaryAgentPersonsSet.AsQueryable();

                qry = qry.Where(x => x.Agent.IsIndividual);

                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.LastName)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                    {
                        qry = qry.Where(x => x.FullName.Contains(temp));
                    }
                }
                if (!string.IsNullOrEmpty(filter.Passport))
                {
                    qry = qry.Where(x => (x.PassportSerial + "-" + x.PassportNumber + " " + x.PassportDate.ToString() + " " + x.PassportText).Contains(filter.Passport));
                }
                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.TaxCode))
                    {
                        qry = qry.Where(x => x.TaxCode.Contains(temp));
                    }
                }
                if (filter.BirthDate != null)
                {
                    qry = qry.Where(x => x.BirthDate == filter.BirthDate);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
                }
                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
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

        public void UpdateDictionaryAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentPersons
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    MiddleName = person.MiddleName,
                    FullName = person.LastName + " " + person.FirstName + " " + person.MiddleName,
                    TaxCode = person.TaxCode,
                    IsMale = person.IsMale,
                    PassportSerial = person.PassportSerial,
                    PassportNumber = person.PassportNumber,
                    PassportText = person.PassportText,
                    PassportDate = person.PassportDate,
                    BirthDate = person.BirthDate,
                    Description = person.Description,
                    LastChangeDate = person.LastChangeDate,
                    LastChangeUserId = person.LastChangeUserId,
                    IsActive = person.IsActive
                };

                dbContext.DictionaryAgentPersonsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();


            }
        }


        public void DeleteDictionaryAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentPersonsSet.FirstOrDefault(x => x.Id == person.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentPersonsSet.Remove(ddt);
                    dbContext.SaveChanges();

                    var agent = GetDictionaryAgent(context, person.Id);

                    if ((!agent.IsCompany && !agent.IsEmployee && !agent.IsBank))
                    {
                        DeleteDictionaryAgent(context, new InternalDictionaryAgent { Id = person.Id });
                    }
                    else
                    {
                        UpdateDictionaryAgentRole(context, person.Id, EnumDictionaryAgentTypes.isIndividual);
                    }
                }

            }
        }

        public int AddDictionaryAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(context))
            {

                var agent = GetDictionaryAgent(context, person.Id);
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
                    person.Id = AddDictionaryAgent(context, newAgent);
                }
                else {
                    agent.IsIndividual = true;
                    UpdateDictionaryAgent(context, new InternalDictionaryAgent
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

                var ddt = new DictionaryAgentPersons
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    MiddleName = person.MiddleName,
                    FullName = person.LastName + " " + person.FirstName + " " + person.MiddleName,
                    TaxCode = person.TaxCode,
                    IsMale = person.IsMale,
                    PassportSerial = person.PassportSerial,
                    PassportNumber = person.PassportNumber,
                    PassportText = person.PassportText,
                    PassportDate = person.PassportDate,
                    BirthDate = person.BirthDate,
                    Description = person.Description,
                    LastChangeDate = person.LastChangeDate,
                    LastChangeUserId = person.LastChangeUserId,
                    IsActive = person.IsActive
                };
                dbContext.DictionaryAgentPersonsSet.Add(ddt);
                dbContext.SaveChanges();

                return person.Id;
            }
        }

        #endregion DictionaryAgentPerson

        #region DictionaryAgentEmployee

        public FrontDictionaryAgentEmployee GetDictionaryAgentEmployee(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryAgentEmployeesSet.Where(x => x.Id == id).Select(x => new FrontDictionaryAgentEmployee
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


        public void UpdateDictionaryAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
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
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                UpdateDictionaryAgentPerson(context, new InternalDictionaryAgentPerson
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
        public void DeleteDictionaryAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentEmployeesSet.FirstOrDefault(x => x.Id == employee.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentEmployeesSet.Remove(ddt);
                    dbContext.SaveChanges();
                }
                else
                {
                    UpdateDictionaryAgentRole(context, employee.Id, EnumDictionaryAgentTypes.isEmployee);
                }
            }
        }

        public int AddDictionaryAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee)
        {
            using (var dbContext = new DmsContext(context))
            {

                var agent = GetDictionaryAgentPerson(context, employee.Id);
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
                    employee.Id = AddDictionaryAgentPerson(context, newAgent);
                }
                else {

                    UpdateDictionaryAgent(context, new InternalDictionaryAgent
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
                dbContext.SaveChanges();

                return employee.Id;
            }
        }

        public IEnumerable<FrontDictionaryAgentEmployee> GetDictionaryAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentEmployeesSet.AsQueryable();

                qry = qry.Where(x => x.Agent.IsEmployee);

                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.Agent.AgentPerson.LastName)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                if (!string.IsNullOrEmpty(filter.PersonnelNumber))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.PersonnelNumber))
                    {
                        qry = qry.Where(x => x.PersonnelNumber.Contains(temp));
                    }
                }
                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                    {
                        qry = qry.Where(x => x.Agent.AgentPerson.FullName.Contains(temp));
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
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.TaxCode))
                    {
                        qry = qry.Where(x => x.Agent.AgentPerson.TaxCode.Contains(temp));
                    }
                }
                if (filter.BirthDate != null)
                {
                    qry = qry.Where(x => x.Agent.AgentPerson.BirthDate == filter.BirthDate);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
        public FrontDictionaryAgentAddress GetDictionaryAgentAddress(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentAddressesSet.AsQueryable();

                return qry.Select(x => new FrontDictionaryAgentAddress
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    AddressType = new FrontDictionaryAddressType { Id = x.AdressTypeId, Name = x.AddressType.Name },
                    PostCode = x.PostCode,
                    Address = x.Address,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).FirstOrDefault();
            }
        }

        public void UpdateDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentAddresses
                {
                    Id = addr.Id,
                    AgentId = addr.AgentId,
                    AdressTypeId = addr.AddressTypeID,
                    PostCode = addr.PostCode,
                    Address = addr.Address,
                    Description = addr.Description,
                    LastChangeDate = addr.LastChangeDate,
                    LastChangeUserId = addr.LastChangeUserId,
                    IsActive = addr.IsActive,
                };

                dbContext.DictionaryAgentAddressesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentAddressesSet.FirstOrDefault(x => x.Id == addr.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentAddressesSet.Remove(ddt);
                    dbContext.SaveChanges();
                }
            }
        }

        public int AddDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentAddresses
                {
                    AgentId = addr.AgentId,
                    AdressTypeId = addr.AddressTypeID,
                    PostCode = addr.PostCode,
                    Address = addr.Address,
                    Description = addr.Description,
                    LastChangeDate = addr.LastChangeDate,
                    LastChangeUserId = addr.LastChangeUserId,
                    IsActive = addr.IsActive
                };
                dbContext.DictionaryAgentAddressesSet.Add(ddt);
                dbContext.SaveChanges();
                addr.Id = ddt.Id;
                return ddt.Id;
            }
        }

        public IEnumerable<FrontDictionaryAgentAddress> GetDictionaryAgentAddresses(IContext context, int agentId, FilterDictionaryAgentAddress filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentAddressesSet.AsQueryable();


                qry = qry.Where(x => x.AgentId == filter.AgentId);

                qry = qry.OrderBy(x => x.Address);

                if (filter.AddressTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AddressTypeId.Contains(x.AdressTypeId));
                }

                if (!String.IsNullOrEmpty(filter.PostCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.PostCode))
                    {
                        qry = qry.Where(x => x.PostCode.Contains(temp));
                    }
                }

                if (!String.IsNullOrEmpty(filter.Address))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Address))
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
        #endregion

        // Типы адресов 
        #region DictionaryAddressTypes
        public void UpdateDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAddressTypes
                {
                    Id = addrType.Id,
                    LastChangeDate = addrType.LastChangeDate,
                    LastChangeUserId = addrType.LastChangeUserId,
                    Name = addrType.Name,
                    IsActive = addrType.IsActive
                };
                dbContext.DictionaryAddressTypesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);

                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }


        public void DeleteDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAddressTypesSet.FirstOrDefault(x => x.Id == addrType.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAddressTypesSet.Remove(ddt);
                    dbContext.SaveChanges();
                }
            }
        }

        public int AddDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAddressTypes
                {
                    Name = addrType.Name,
                    IsActive = addrType.IsActive,
                    LastChangeDate = addrType.LastChangeDate,
                    LastChangeUserId = addrType.LastChangeUserId
                };
                dbContext.DictionaryAddressTypesSet.Add(ddt);
                dbContext.SaveChanges();
                addrType.Id = ddt.Id;
                return ddt.Id;
            }
        }

        public InternalDictionaryAddressType GetInternalDictionaryAddressType(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDocumentTypesSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
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

        public IEnumerable<FrontDictionaryAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAddressTypesSet.AsQueryable();

                qry = qry.OrderBy(x => x.Name);

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
        public FrontDictionaryAgentCompany GetDictionaryAgentCompany(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentCompaniesSet.Where(x => x.Id == id).Select(x => new FrontDictionaryAgentCompany
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

        public IEnumerable<FrontDictionaryAgentCompany> GetDictionaryAgentCompanies(IContext context, FilterDictionaryAgentCompany filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentCompaniesSet.AsQueryable();

                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.FullName)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                    {
                        qry = qry.Where(x => x.FullName.Contains(temp));
                    }
                }
                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.TaxCode))
                    {
                        qry = qry.Where(x => x.TaxCode.Contains(temp));
                    }
                }
                if (!string.IsNullOrEmpty(filter.OKPOCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.OKPOCode))
                    {
                        qry = qry.Where(x => x.OKPOCode.Contains(temp));
                    }
                }
                if (!string.IsNullOrEmpty(filter.VATCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.VATCode))
                    {
                        qry = qry.Where(x => x.VATCode.Contains(temp));
                    }
                }
                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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

        public void UpdateDictionaryAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentCompanies
                {
                    Id = company.Id,
                    FullName = company.FullName,
                    OKPOCode = company.OKPOCode,
                    VATCode = company.VATCode,
                    TaxCode = company.TaxCode,
                    Description = company.Description,
                    LastChangeDate = company.LastChangeDate,
                    LastChangeUserId = company.LastChangeUserId,
                    IsActive = company.IsActive
                };

                dbContext.DictionaryAgentCompaniesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                UpdateDictionaryAgentName(context, company.Id, company.ShortName);

            }

        }


        public void DeleteDictionaryAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentCompaniesSet.FirstOrDefault(x => x.Id == company.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentCompaniesSet.Remove(ddt);
                    dbContext.SaveChanges();


                    var agent = GetDictionaryAgent(context, company.Id);

                    if (!agent.IsBank && !agent.IsEmployee && !agent.IsIndividual)
                    {
                        DeleteDictionaryAgent(context, new InternalDictionaryAgent { Id = company.Id });
                    }
                    else
                    {
                        UpdateDictionaryAgentRole(context, company.Id, EnumDictionaryAgentTypes.isCompany);
                    }
                }
            }
        }

        public int AddDictionaryAgentCompany(IContext context, InternalDictionaryAgentCompany company)
        {
            using (var dbContext = new DmsContext(context))
            {

                var agent = GetDictionaryAgent(context, company.Id);
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
                    company.Id = AddDictionaryAgent(context, newAgent);
                }
                else {
                    agent.IsCompany = true;
                    UpdateDictionaryAgent(context, new InternalDictionaryAgent
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

                var ddt = new DictionaryAgentCompanies
                {
                    Id = company.Id,
                    FullName = company.FullName,
                    OKPOCode = company.OKPOCode,
                    VATCode = company.VATCode,
                    TaxCode = company.TaxCode,
                    Description = company.Description,
                    LastChangeDate = company.LastChangeDate,
                    LastChangeUserId = company.LastChangeUserId,
                    IsActive = company.IsActive
                };
                dbContext.DictionaryAgentCompaniesSet.Add(ddt);
                dbContext.SaveChanges();

                return company.Id;
            }
        }
        #endregion DictionaryAgentCompanies

        #region DictionaryAgentBanks
        public FrontDictionaryAgentBank GetDictionaryAgentBank(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentBanksSet.Where(x => x.Id == id).Select(x => new FrontDictionaryAgentBank
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

        public void UpdateDictionaryAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentBanks
                {
                    Id = bank.Id,
                    MFOCode = bank.MFOCode,
                    Swift = bank.Swift,
                    Description = bank.Description,
                    LastChangeDate = bank.LastChangeDate,
                    LastChangeUserId = bank.LastChangeUserId,
                    IsActive = bank.IsActive
                };

                dbContext.DictionaryAgentBanksSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                UpdateDictionaryAgentName(context, bank.Id, bank.Name);

            }
        }
        public void DeleteDictionaryAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentBanksSet.FirstOrDefault(x => x.Id == bank.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentBanksSet.Remove(ddt);
                    dbContext.SaveChanges();


                    var agent = GetDictionaryAgent(context, bank.Id);

                    if (!agent.IsCompany && !agent.IsEmployee && !agent.IsIndividual)
                    {
                        DeleteDictionaryAgent(context, new InternalDictionaryAgent { Id = bank.Id });
                    }
                    else
                    {
                        UpdateDictionaryAgentRole(context, bank.Id, EnumDictionaryAgentTypes.isBank);
                    }

                }
            }
        }

        public int AddDictionaryAgentBank(IContext context, InternalDictionaryAgentBank bank)
        {
            using (var dbContext = new DmsContext(context))
            {

                var agent = GetDictionaryAgent(context, bank.Id);
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
                    bank.Id = AddDictionaryAgent(context, newAgent);
                }
                else {
                    UpdateDictionaryAgentRole(context, bank.Id, EnumDictionaryAgentTypes.isBank);
                };

                var ddt = new DictionaryAgentBanks
                {
                    Id = bank.Id,
                    MFOCode = bank.MFOCode,
                    Swift = bank.Swift,
                    Description = bank.Description,
                    LastChangeDate = bank.LastChangeDate,
                    LastChangeUserId = bank.LastChangeUserId,
                    IsActive = bank.IsActive
                };
                dbContext.DictionaryAgentBanksSet.Add(ddt);
                dbContext.SaveChanges();

                return bank.Id;
            }
        }


        public IEnumerable<FrontDictionaryAgentBank> GetDictionaryAgentBanks(IContext context, FilterDictionaryAgentBank filter, UIPaging paging)
        {
            using (var dbContext = new DmsContext(context))
            {

                var qry = dbContext.DictionaryAgentBanksSet.AsQueryable();

                qry = qry.Where(x => x.Agent.IsBank);

                if (paging != null)
                {
                    paging.TotalItemsCount = qry.Count();

                    qry = qry.OrderBy(x => x.Agent.Name)
                        .Skip(paging.PageSize * (paging.CurrentPage - 1)).Take(paging.PageSize);
                }

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                    {
                        qry = qry.Where(x => x.Agent.Name.Contains(temp));
                    }
                }
                if (!string.IsNullOrEmpty(filter.MFOCode))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.MFOCode))
                    {
                        qry = qry.Where(x => x.MFOCode.Contains(temp));
                    }
                }
                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }
                if (filter.NotContainsIDs?.Count > 0)
                {
                    qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
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
        public FrontDictionaryAgentAccount GetDictionaryAgentAccount(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return
                    dbContext.DictionaryAgentAccountsSet.Where(x => x.Id == id).Select(x => new FrontDictionaryAgentAccount
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
                var accounts = GetDictionaryAgentAccounts(context, AgentId, new FilterDictionaryAgentAccount());
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
                        UpdateDictionaryAgentAccount(context, newAcc);
                    }
                }
            }
        }

        public void UpdateDictionaryAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentAccounts
                {
                    Id = account.Id,
                    AccountNumber = account.AccountNumber,
                    AgentId = account.AgentId,
                    AgentBankId = account.AgentBankId,
                    IsMain = account.IsMain,
                    Name = account.Name,
                    Description = account.Description,
                    LastChangeDate = account.LastChangeDate,
                    LastChangeUserId = account.LastChangeUserId,
                    IsActive = account.IsActive
                };

                dbContext.DictionaryAgentAccountsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                if (account.IsMain)
                {
                    SetMainAgentAccount(context, account.AgentId, account.Id);
                }

            }
        }

        public void DeleteDictionaryAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentAccountsSet.FirstOrDefault(x => x.Id == account.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentAccountsSet.Remove(ddt);
                    dbContext.SaveChanges();
                }
            }
        }

        public int AddDictionaryAgentAccount(IContext context, InternalDictionaryAgentAccount account)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentAccounts
                {
                    Id = account.Id,
                    AccountNumber = account.AccountNumber,
                    AgentId = account.AgentId,
                    AgentBankId = account.AgentBankId,
                    IsMain = account.IsMain,
                    Name = account.Name,
                    Description = account.Description,
                    LastChangeDate = account.LastChangeDate,
                    LastChangeUserId = account.LastChangeUserId,
                    IsActive = account.IsActive
                };
                dbContext.DictionaryAgentAccountsSet.Add(ddt);
                dbContext.SaveChanges();

                if (account.IsMain)
                {
                    SetMainAgentAccount(context, account.AgentId, account.Id);
                }

                return account.Id;
            }
        }

        public IEnumerable<FrontDictionaryAgentAccount> GetDictionaryAgentAccounts(IContext context, int AgentId, FilterDictionaryAgentAccount filter)
        {
            using (var dbContext = new DmsContext(context))
            {

                var qry = dbContext.DictionaryAgentAccountsSet.AsQueryable();

                qry = qry.Where(x => x.AgentId == AgentId);


                if (!string.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                if (!string.IsNullOrEmpty(filter.AccountNumber))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.AccountNumber))
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
                var qry = dbContext.DictionaryContactTypesSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                return qry.Select(x => new FrontDictionaryContactType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                }).FirstOrDefault();
            }
        }
        public void UpdateDictionaryContactType(IContext context, InternalDictionaryContactType contactType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryContactTypes
                {
                    Id = contactType.Id,
                    LastChangeDate = contactType.LastChangeDate,
                    LastChangeUserId = contactType.LastChangeUserId,
                    Name = contactType.Name,
                    IsActive = contactType.IsActive
                };
                dbContext.DictionaryContactTypesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);

                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
        public void DeleteDictionaryContactType(IContext context, InternalDictionaryContactType contactType)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryContactTypesSet.FirstOrDefault(x => x.Id == contactType.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryContactTypesSet.Remove(ddt);
                    dbContext.SaveChanges();
                }
            }
        }
        public int AddDictionaryContactType(IContext context, InternalDictionaryContactType contactType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryContactTypes
                {
                    Name = contactType.Name,
                    IsActive = contactType.IsActive,
                    LastChangeDate = contactType.LastChangeDate,
                    LastChangeUserId = contactType.LastChangeUserId
                };
                dbContext.DictionaryContactTypesSet.Add(ddt);
                dbContext.SaveChanges();
                contactType.Id = ddt.Id;
                return ddt.Id;
            }
        }
        public IEnumerable<FrontDictionaryContactType> GetDictionaryContactTypes(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryContactTypesSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                    {
                        qry = qry.Where(x => x.Name.Contains(temp));
                    }
                }

                if (filter.IsActive.HasValue)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                return qry.Select(x => new FrontDictionaryContactType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                }).ToList();
            }
        }
        #endregion

        // Контакты
        #region DictionaryContacts

        public FrontDictionaryContact GetDictionaryContact(IContext context,
          FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentContactsSet.AsQueryable();

                return qry.Select(x => new FrontDictionaryContact
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    ContactType = new FrontDictionaryContactType { Id = x.ContactTypeId, Name = x.ContactType.Name },
                    Value = x.Contact,
                    Description = x.Description,
                    IsActive = x.IsActive,
                }).FirstOrDefault();
            }
        }

        public void UpdateDictionaryContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentContacts
                {
                    Id = contact.Id,
                    AgentId = contact.AgentId,
                    ContactTypeId = contact.ContactTypeId,
                    Contact = contact.Value,
                    Description = contact.Description,
                    LastChangeDate = contact.LastChangeDate,
                    LastChangeUserId = contact.LastChangeUserId,
                    IsActive = contact.IsActive
                };

                dbContext.DictionaryAgentContactsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }
        public void DeleteDictionaryContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryAgentContactsSet.FirstOrDefault(x => x.Id == contact.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentContactsSet.Remove(ddt);

                    dbContext.SaveChanges();
                }
            }
        }
        public int AddDictionaryContact(IContext context, InternalDictionaryContact contact)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryAgentContacts
                {
                    AgentId = contact.AgentId,
                    ContactTypeId = contact.ContactTypeId,
                    Contact = contact.Value,
                    Description = contact.Description,
                    LastChangeDate = contact.LastChangeDate,
                    LastChangeUserId = contact.LastChangeUserId,
                    IsActive = contact.IsActive
                };
                dbContext.DictionaryAgentContactsSet.Add(ddt);
                dbContext.SaveChanges();
                contact.Id = ddt.Id;
                return ddt.Id;
            }
        }
        public IEnumerable<FrontDictionaryContact> GetDictionaryContacts(IContext context, int agentId, FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryAgentContactsSet.AsQueryable();

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
                if (!String.IsNullOrEmpty(filter.Contact))
                {
                    foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Contact))
                    {
                        qry = qry.Where(x => x.Contact.Contains(temp));
                    }
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
                    ContactType = new FrontDictionaryContactType { Id = x.ContactType.Id, Name = x.ContactType.Name },
                    Value = x.Contact,
                    Description = x.Description,
                    IsActive = x.IsActive
                }).ToList();
            }
        }
        #endregion

        // Структура предприятия
        #region DictionaryDepartments
        public int AddDictionaryDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = new DictionaryDepartments
                {
                    ParentId = department.ParentId,
                    LastChangeDate = department.LastChangeDate,
                    LastChangeUserId = department.LastChangeUserId,
                    IsActive = department.IsActive,
                    Name = department.Name,
                    FullName = department.FullName,
                    CompanyId = department.CompanyId,
                    Code = department.Code,
                    ChiefPositionId = department.ChiefPositionId
                };
                dbContext.DictionaryDepartmentsSet.Add(dd);
                dbContext.SaveChanges();
                department.Id = dd.Id;
                return dd.Id;
            }
        }

        public void UpdateDictionaryDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = new DictionaryDepartments
                {
                    Id = department.Id,
                    ParentId = department.ParentId,
                    LastChangeDate = department.LastChangeDate,
                    LastChangeUserId = department.LastChangeUserId,
                    IsActive = department.IsActive,
                    Name = department.Name,
                    FullName = department.FullName,
                    CompanyId = department.CompanyId,
                    Code = department.Code,
                    ChiefPositionId = department.ChiefPositionId
                };

                dbContext.DictionaryDepartmentsSet.Attach(dd);
                dbContext.Entry(dd).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeleteDictionaryDepartment(IContext context, InternalDictionaryDepartment department)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = dbContext.DictionaryDepartmentsSet.FirstOrDefault(x => x.Id == department.Id);
                if (dd != null)
                {
                    dbContext.DictionaryDepartmentsSet.Remove(dd);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDictionaryDepartment GetDictionaryDepartment(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDepartmentsSet.AsQueryable();

                qry = DepartmentGetWhere(ref qry, filter);

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

        public IEnumerable<FrontDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDepartmentsSet.AsQueryable();

                qry = DepartmentGetWhere(ref qry, filter);

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
                var qry = dbContext.DictionaryDepartmentsSet.AsQueryable();

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
            // Условие по ID
            if (filter.IDs?.Count > 0)
            {
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Условие по NotContainsId
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
            }

            // Условие по ParentIDs
            if (filter.ParentIDs?.Count > 0)
            {
                qry = qry.Where(x => filter.ParentIDs.Contains(x.Id));
            }

            // Условие по IsActive
            if (filter.IsActive != null)
            {
                qry = qry.Where(x => filter.IsActive == x.IsActive);
            }

            // Условие по Name
            if (!String.IsNullOrEmpty(filter.Name))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }

            }

            // Условие по FullName
            if (!String.IsNullOrEmpty(filter.FullName))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.FullName))
                {
                    qry = qry.Where(x => x.FullName.Contains(temp));
                }
            }

            // Условие по Code
            if (!String.IsNullOrEmpty(filter.Code))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Code))
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
        public BaseDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return
                    dbContext.DictionaryDocumentDirectionsSet.Where(x => x.Id == id)
                        .Select(x => new BaseDictionaryDocumentDirection
                        {
                            Id = x.Id,
                            Code = x.Code,
                            Name = x.Name,
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate
                        }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDocumentDirectionsSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                return qry.Select(x => new BaseDictionaryDocumentDirection
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

        public int AddDictionaryDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryDocumentSubjects
                {
                    ParentId = docSubject.ParentId,
                    LastChangeDate = docSubject.LastChangeDate,
                    LastChangeUserId = docSubject.LastChangeUserId,
                    IsActive = docSubject.IsActive,
                    Name = docSubject.Name
                };
                dbContext.DictionaryDocumentSubjectsSet.Add(ddt);
                dbContext.SaveChanges();
                docSubject.Id = ddt.Id;
                return ddt.Id;
            }
        }

        public void UpdateDictionaryDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
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
                var dds = new DictionaryDocumentSubjects
                {
                    Id = docSubject.Id,
                    ParentId = docSubject.ParentId,
                    LastChangeDate = docSubject.LastChangeDate,
                    LastChangeUserId = docSubject.LastChangeUserId,
                    IsActive = docSubject.IsActive,
                    Name = docSubject.Name
                };

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
            }
        }

        public void DeleteDictionaryDocumentSubject(IContext context, InternalDictionaryDocumentSubject docSubject)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = dbContext.DictionaryDocumentSubjectsSet.FirstOrDefault(x => x.Id == docSubject.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryDocumentSubjectsSet.Remove(ddt);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDictionaryDocumentSubject GetInternalDictionaryDocumentSubject(IContext context, FilterDictionaryDocumentSubject filter)
        {
            // Устно договорились, что функция для возврата одного элемента возвращает параметры только конкретного элемена без учета родителя и потомков.

            using (var dbContext = new DmsContext(context))
            {

                var qry = dbContext.DictionaryDocumentSubjectsSet.AsQueryable();

                qry = DocumentSubjectGetWhere(ref qry, filter);

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

        public IEnumerable<FrontDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {

            // Устно договорились, что функция для возврата коллекции элементов возвращает всю простыню элеменов без учета родителя и потомков.
            // Если нужно каждому элементу добавить родителя и потомков это делается на уровень выше в Logic. Функция GetDictionaryDocumentSubects ВСЕГДА возвращает плоскую коллекцию
            // более толго для построения иерархии на фронте плоской коллекции достаточно.

            using (var dbContext = new DmsContext(context))
            {
                //IQueryable<DictionaryDocumentSubjects> qry=

                var qry = dbContext.DictionaryDocumentSubjectsSet.AsQueryable();

                qry = DocumentSubjectGetWhere(ref qry, filter);

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
                var qry = dbContext.DictionaryDocumentSubjectsSet.AsQueryable();

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
            // Условие по ID
            if (filter.IDs?.Count > 0)
            {
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Условие по NotContainsId
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
            }

            // Условие по Name
            if (!String.IsNullOrEmpty(filter.Name))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }
            }

            // Условие по IsActive
            if (filter.IsActive != null)
            {
                qry = qry.Where(x => filter.IsActive == x.IsActive);
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
        public int AddDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryDocumentTypes
                {
                    Name = docType.Name,
                    IsActive = docType.IsActive,
                    LastChangeDate = docType.LastChangeDate,
                    LastChangeUserId = docType.LastChangeUserId
                };
                dbContext.DictionaryDocumentTypesSet.Add(ddt);
                dbContext.SaveChanges();
                docType.Id = ddt.Id;
                return ddt.Id;
            }
        }

        public void UpdateDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            {
                var ddt = new DictionaryDocumentTypes
                {
                    Id = docType.Id,
                    LastChangeDate = docType.LastChangeDate,
                    LastChangeUserId = docType.LastChangeUserId,
                    Name = docType.Name,
                    IsActive = docType.IsActive
                };
                dbContext.DictionaryDocumentTypesSet.Attach(ddt);

                dbContext.Entry(ddt).State = System.Data.Entity.EntityState.Modified;
                
                dbContext.SaveChanges();
            }
        }


        public void DeleteDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(context))
            {

                var ddt = dbContext.DictionaryDocumentTypesSet.FirstOrDefault(x => x.Id == docType.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryDocumentTypesSet.Remove(ddt);
                    dbContext.SaveChanges();
                }
            }
        }

        

        public InternalDictionaryDocumentType GetInternalDictionaryDocumentType(IContext context, FilterDictionaryDocumentType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDocumentTypesSet.AsQueryable();

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

        public IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryDocumentTypesSet.AsQueryable();

                qry = DocumentTypeGetWhere(ref qry, filter);

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
            
            // Условие по ID
            if (filter.IDs?.Count > 0)
            {
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Условие по NotContainsId
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
            }

            // Условие по Name
            if (!String.IsNullOrEmpty(filter.Name))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }
            }

            // Условие по IsActive
            if (filter.IsActive != null)
            {
                qry = qry.Where(x => filter.IsActive == x.IsActive);
            }
            
            return qry;
        }
        #endregion DictionaryDocumentTypes

        #region DictionaryEventTypes
        public BaseDictionaryEventType GetDictionaryEventType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryEventTypesSet.Where(x => x.Id == id).Select(x => new BaseDictionaryEventType
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

        public IEnumerable<BaseDictionaryEventType> GetDictionaryEventTypes(IContext context, FilterDictionaryEventType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryEventTypesSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                if (filter.ImportanceEventTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.ImportanceEventTypeId.Contains(x.ImportanceEventTypeId));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.EventTypeId).Contains(x.Id)
                                );
                }

                return qry.Select(x => new BaseDictionaryEventType
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
        public BaseDictionaryImportanceEventType GetDictionaryImportanceEventType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return dbContext.DictionaryImportanceEventTypesSet.Where(x => x.Id == id)
                    .Select(x => new BaseDictionaryImportanceEventType
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryImportanceEventType> GetDictionaryImportanceEventTypes(IContext context, FilterDictionaryImportanceEventType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryImportanceEventTypesSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.EventType.ImportanceEventTypeId).Contains(x.Id)
                                );
                }

                return qry.Select(x => new BaseDictionaryImportanceEventType
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
        public BaseDictionaryLinkType GetDictionaryLinkType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryLinkTypesSet.Where(x => x.Id == id)
                    .Select(x => new BaseDictionaryLinkType
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsImportant = x.IsImportant,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryLinkType> GetDictionaryLinkTypes(IContext context, FilterDictionaryLinkType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryLinkTypesSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                return qry.Select(x => new BaseDictionaryLinkType
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
                var dd = new DictionaryPositions
                {
                    ParentId = position.ParentId,
                    LastChangeDate = position.LastChangeDate,
                    LastChangeUserId = position.LastChangeUserId,
                    IsActive = position.IsActive,
                    Name = position.Name,
                    FullName = position.FullName,
                    DepartmentId = position.DepartmentId,
                    ExecutorAgentId = position.ExecutorAgentId,
                    MainExecutorAgentId = position.MainExecutorAgentId
                };
                dbContext.DictionaryPositionsSet.Add(dd);
                dbContext.SaveChanges();
                position.Id = dd.Id;
                return dd.Id;
            }
        }

        public void UpdatePosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dd = new DictionaryPositions
                {
                    Id = position.Id,
                    ParentId = position.ParentId,
                    LastChangeDate = position.LastChangeDate,
                    LastChangeUserId = position.LastChangeUserId,
                    IsActive = position.IsActive,
                    Name = position.Name,
                    FullName = position.FullName,
                    DepartmentId = position.DepartmentId,
                    ExecutorAgentId = position.ExecutorAgentId,
                    MainExecutorAgentId = position.MainExecutorAgentId
                };

                dbContext.DictionaryPositionsSet.Attach(dd);
                dbContext.Entry(dd).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();
            }
        }

        public void DeletePosition(IContext context, InternalDictionaryPosition position)
        {
            using (var dbContext = new DmsContext(context))
            {
                var dp = dbContext.DictionaryPositionsSet.FirstOrDefault(x => x.Id == position.Id);
                if (dp != null)
                {
                    dbContext.DictionaryPositionsSet.Remove(dp);
                    dbContext.SaveChanges();
                }
            }
        }

        public int? GetExecutorAgentIdByPositionId(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryPositionsSet.Where(x => x.Id == id)
                    .Select(x => x.ExecutorAgentId).FirstOrDefault();
            }
        }


        public FrontDictionaryPosition GetDictionaryPosition(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryPositionsSet.Where(x => x.Id == id)
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
                        ChildPositions = x.ChildPositions.Select(y => new FrontDictionaryPosition
                        {
                            Id = y.Id,
                            ParentId = y.ParentId,
                            Name = y.Name,
                            DepartmentId = y.DepartmentId,
                            ExecutorAgentId = y.ExecutorAgentId,
                            ParentPositionName = y.ParentPosition.Name,
                            DepartmentName = y.Department.Name,
                            ExecutorAgentName = y.ExecutorAgent.Name
                        }),
                        ChiefDepartments = x.ChiefDepartments.Select(y => new FrontDictionaryDepartment
                        {
                            Id = y.Id,
                            ParentId = y.ParentId,
                            CompanyId = y.CompanyId,
                            Name = y.Name,
                            ChiefPositionId = y.ChiefPositionId,
                            ParentDepartmentName = y.ParentDepartment.Name,
                            CompanyName = y.Company.Name,
                            ChiefPositionName = y.ChiefPosition.Name
                        }),
                        StandartSendLists = x.StandartSendLists.Select(y => new BaseDictionaryStandartSendList
                        {
                            Id = y.Id,
                            Name = y.Name,
                            PositionId = y.PositionId,
                            PositionName = y.Position.Name
                        })

                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryPositionsSet.Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet
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
                    ParentId = x.pos.ParentId,
                    Name = x.pos.Name,
                    DepartmentId = x.pos.DepartmentId,
                    ExecutorAgentId = x.pos.ExecutorAgentId,
                    ParentPositionName = x.pos.ParentPosition.Name,
                    DepartmentName = x.pos.Department.Name,
                    ExecutorAgentName = x.pos.ExecutorAgent.Name,
                    MaxSubordinationTypeId = (x.subordMax > 0 ? (int?)x.subordMax : null)
                }).ToList();
            }
        }

        public IEnumerable<InternalDictionaryPositionWithActions> GetDictionaryPositionsWithActions(IContext context,
            FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryPositionsSet.Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet
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

        // Для использования в коммандах метод CanExecute
        public bool ExistsPosition(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryPositionsSet.Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.pos.Id));
                }

                if (filter.DocumentIDs?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentIDs.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet
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

        //private static IQueryable<DictionaryPositions> PositionGetWhere(ref IQueryable<DictionaryPositions> qry, FilterDictionaryPosition filter)
        //{
        //    // Условие по ID
        //    if (filter.IDs?.Count > 0)
        //    {
        //        qry = qry.Where(x => filter.IDs.Contains(x.Id));
        //    }

        //    // Условие по NotContainsId
        //    if (filter.NotContainsIDs?.Count > 0)
        //    {
        //        qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
        //    }

        //    // Условие по IsActive
        //    if (filter.IsActive != null)
        //    {
        //        qry = qry.Where(x => filter.IsActive == x.IsActive);
        //    }

        //    // Условие по Name
        //    if (!String.IsNullOrEmpty(filter.Name))
        //    {
        //        qry = qry.Where(x => x.Name.Contains(filter.Name));
        //    }

        //    return qry;
        //}

        #endregion DictionaryPositions

        // Журналы регистрации
        #region DictionaryRegistrationJournals
        public int AddDictionaryRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal regJournal)
        {
            using (var dbContext = new DmsContext(context))
            {

                string s = (regJournal.IsIncoming ? "1" : "") + (regJournal.IsOutcoming ? "2" : "") + (regJournal.IsInternal ? "3" : "");

                var drj = new DictionaryRegistrationJournals
                {
                    // pss Перегонка значений DictionaryRegistrationJournals
                    LastChangeDate = regJournal.LastChangeDate,
                    LastChangeUserId = regJournal.LastChangeUserId,
                    IsActive = regJournal.IsActive,
                    Name = regJournal.Name,
                    DepartmentId = regJournal.DepartmentId,
                    Index = regJournal.Index,
                    DirectionCodes = s,
                    PrefixFormula = regJournal.PrefixFormula,
                    NumerationPrefixFormula = regJournal.NumerationPrefixFormula,
                    SuffixFormula = regJournal.SuffixFormula,
                };
                dbContext.DictionaryRegistrationJournalsSet.Add(drj);
                dbContext.SaveChanges();
                regJournal.Id = drj.Id;
                return drj.Id;
            }
        }

        public void UpdateDictionaryRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal regJournal)
        {
            using (var dbContext = new DmsContext(context))
            {
                string s = (regJournal.IsIncoming ? "1" : "") + (regJournal.IsOutcoming ? "2" : "") + (regJournal.IsInternal ? "3" : "");

                var drj = new DictionaryRegistrationJournals
                {
                    // pss Перегонка значений DictionaryRegistrationJournals
                    Id = regJournal.Id,
                    LastChangeDate = regJournal.LastChangeDate,
                    LastChangeUserId = regJournal.LastChangeUserId,
                    IsActive = regJournal.IsActive,
                    Name = regJournal.Name,
                    DepartmentId = regJournal.DepartmentId,
                    Index = regJournal.Index,
                    DirectionCodes = s,
                    PrefixFormula = regJournal.PrefixFormula,
                    NumerationPrefixFormula = regJournal.NumerationPrefixFormula,
                    SuffixFormula = regJournal.SuffixFormula,
                };

                dbContext.DictionaryRegistrationJournalsSet.Attach(drj);
                dbContext.Entry(drj).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public void DeleteDictionaryRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal docSubject)
        {
            using (var dbContext = new DmsContext(context))
            {
                var drj = dbContext.DictionaryRegistrationJournalsSet.FirstOrDefault(x => x.Id == docSubject.Id);
                if (drj != null)
                {
                    dbContext.DictionaryRegistrationJournalsSet.Remove(drj);
                    dbContext.SaveChanges();
                }
            }
        }

        public InternalDictionaryRegistrationJournal GetInternalDictionaryRegistrationJournal(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryRegistrationJournalsSet.AsQueryable();

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
                    IsIncoming = x.DirectionCodes.Contains("1"),
                    IsOutcoming = x.DirectionCodes.Contains("2"),
                    IsInternal = x.DirectionCodes.Contains("3"),
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryRegistrationJournalsSet.AsQueryable();

                qry = RegistrationJournalGetWhere(ref qry, filter);

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
                    IsIncoming = x.DirectionCodes.Contains("1"),
                    IsOutcoming = x.DirectionCodes.Contains("2"),
                    IsInternal = x.DirectionCodes.Contains("3"),
                    DepartmentName = x.Department.Name
                }).ToList();
            }
        }

        // Для использования в коммандах метод CanExecute
        public bool ExistsDictionaryRegistrationJournal(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryRegistrationJournalsSet.AsQueryable();

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
            // Условие по ID
            if (filter.IDs?.Count > 0)
            {
                qry = qry.Where(x => filter.IDs.Contains(x.Id));
            }

            // Условие по NotContainsId
            if (filter.NotContainsIDs?.Count > 0)
            {
                qry = qry.Where(x => !filter.NotContainsIDs.Contains(x.Id));
            }

            // Условие по IsActive
            if (filter.IsActive != null)
            {
                qry = qry.Where(x => filter.IsActive == x.IsActive);
            }

            // Условие по Name
            if (!String.IsNullOrEmpty(filter.Name))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(temp));
                }
            }

            // Условие по Index
            if (!String.IsNullOrEmpty(filter.Index))
            {
                foreach (string temp in CommonFilterUtilites.GetWhereExptessions(filter.Index))
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
                qry = qry.Where(x => x.DirectionCodes.Contains("1"));
            }

            // Условие по IsOutcoming
            if (filter.IsOutcoming != null)
            {
                qry = qry.Where(x => x.DirectionCodes.Contains("2"));
            }

            // Условие по IsInternal
            if (filter.IsInternal != null)
            {
                qry = qry.Where(x => x.DirectionCodes.Contains("3"));
            }

            return qry;
        }

        #endregion DictionaryRegistrationJournals

        #region DictionaryResultTypes
        public BaseDictionaryResultType GetDictionaryResultType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {

                return dbContext.DictionaryResultTypesSet.Where(x => x.Id == id)
                    .Select(x => new BaseDictionaryResultType
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsExecute = x.IsExecute,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryResultType> GetDictionaryResultTypes(IContext context, FilterDictionaryResultType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryResultTypesSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                return qry.Select(x => new BaseDictionaryResultType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsExecute = x.IsExecute
                }).ToList();
            }
        }
        #endregion DictionaryResultTypes

        #region DictionarySendTypes
        public BaseDictionarySendType GetDictionarySendType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionarySendTypesSet.Where(x => x.Id == id)
                    .Select(x => new BaseDictionarySendType
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

        public IEnumerable<BaseDictionarySendType> GetDictionarySendTypes(IContext context, FilterDictionarySendType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionarySendTypesSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                return qry.Select(x => new BaseDictionarySendType
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
        public BaseDictionaryStandartSendListContent GetDictionaryStandartSendListContent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryStandartSendListContentsSet.Where(x => x.Id == id)
                    .Select(x => new BaseDictionaryStandartSendListContent
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
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        SendTypeName = x.SendType.Name,
                        TargetPositionName = x.TargetPosition.Name,
                        TargetAgentName = x.TargetPosition.ExecutorAgent.Name ?? x.TargetAgent.Name,
                        AccessLevelName = x.AccessLevel.Name
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryStandartSendListContent> GetDictionaryStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryStandartSendListContentsSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                return qry.Select(x => new BaseDictionaryStandartSendListContent
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
        #endregion DictionaryStandartSendListContents

        #region DictionaryStandartSendLists
        public BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionaryStandartSendListsSet.Where(x => x.Id == id)
                        .Select(x => new BaseDictionaryStandartSendList
                        {
                            Id = x.Id,
                            Name = x.Name,
                            PositionId = x.PositionId,
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate,
                            PositionName = x.Position.Name,
                            StandartSendListContents =
                                x.StandartSendListContents.Select(y => new BaseDictionaryStandartSendListContent
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

        public IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionaryStandartSendListsSet.AsQueryable();
                if (filter.IDs != null && filter.IDs.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }
                if (filter.PositionIDs != null && filter.PositionIDs.Count > 0)
                {
                    qry = qry.Where(x => filter.PositionIDs.Contains(x.PositionId));
                }
                return qry.Select(x => new BaseDictionaryStandartSendList
                {
                    Id = x.Id,
                    Name = x.Name,
                    PositionId = x.PositionId,
                    PositionName = x.Position.Name
                }).ToList();
            }
        }
        #endregion DictionaryStandartSendList

        #region DictionarySubordinationTypes
        public BaseDictionarySubordinationType GetDictionarySubordinationType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(context))
            {
                return dbContext.DictionarySubordinationTypesSet.Where(x => x.Id == id)
                    .Select(x => new BaseDictionarySubordinationType
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionarySubordinationType> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
        {
            using (var dbContext = new DmsContext(context))
            {
                var qry = dbContext.DictionarySubordinationTypesSet.AsQueryable();

                if (filter.IDs?.Count > 0)
                {
                    qry = qry.Where(x => filter.IDs.Contains(x.Id));
                }

                return qry.Select(x => new BaseDictionarySubordinationType
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
                var qry = dbContext.DictionaryTagsSet.AsQueryable();

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

        public IEnumerable<FrontDictionaryTag> GetDictionaryTags(IContext ctx, FilterDictionaryTag filter)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var qry = dbContext.DictionaryTagsSet.AsQueryable();

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

        public int AddDictionaryTag(IContext context, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var savTag = new DictionaryTags
                {
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
        public void UpdateDictionaryTag(IContext ctx, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(ctx))
            {
                var savTag = dbContext.DictionaryTagsSet
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

        public void DeleteDictionaryTag(IContext context, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(context))
            {

                var item = dbContext.DictionaryTagsSet.FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.DictionaryTagsSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }

        #endregion DictionaryTags

        #region Admins
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
        #endregion

        #region CustomDictionaryTypes
        public void UpdateCustomDictionaryType(IContext context, InternalCustomDictionaryType model)
        {
            using (var dbContext = new DmsContext(context))
            {
                var item = new CustomDictionaryTypes
                {
                    Id = model.Id,
                    Code = model.Code,
                    Description = model.Description,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
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
                var item = new CustomDictionaryTypes
                {
                    Code = model.Code,
                    Description = model.Description,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId
                };
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
                var item = dbContext.CustomDictionaryTypesSet.FirstOrDefault(x => x.Id == id);
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
                var qry = dbContext.CustomDictionaryTypesSet.AsQueryable();

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
                var qry = dbContext.CustomDictionaryTypesSet.AsQueryable();

                qry = qry.Where(x => x.Id == id);

                var item = qry.Select(x => new FrontCustomDictionaryType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Description = x.Description
                }).FirstOrDefault();

                item.CustomDictionaries = dbContext.CustomDictionariesSet.Where(x => x.DictionaryTypeId == item.Id)
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
                var qry = dbContext.CustomDictionaryTypesSet.AsQueryable();

                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        qry = qry.Where(x => filter.IDs.Contains(x.Id));
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
                var item = new CustomDictionaries
                {
                    Id = model.Id,
                    Code = model.Code,
                    Description = model.Description,
                    DictionaryTypeId = model.DictionaryTypeId,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId,
                };
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
                var item = new CustomDictionaries
                {
                    Code = model.Code,
                    Description = model.Description,
                    DictionaryTypeId = model.DictionaryTypeId,
                    LastChangeDate = model.LastChangeDate,
                    LastChangeUserId = model.LastChangeUserId
                };
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
                var item = dbContext.CustomDictionariesSet.FirstOrDefault(x => x.Id == id);
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
                var qry = dbContext.CustomDictionariesSet.AsQueryable();

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
                var qry = dbContext.CustomDictionariesSet.AsQueryable();

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
                var qry = dbContext.CustomDictionariesSet.AsQueryable();

                if (filter != null)
                {
                    if (filter.IDs?.Count > 0)
                    {
                        qry = qry.Where(x => filter.IDs.Contains(x.DictionaryTypeId));
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
