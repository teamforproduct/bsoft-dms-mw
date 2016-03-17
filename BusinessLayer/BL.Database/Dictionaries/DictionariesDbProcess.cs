using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DatabaseContext;
using BL.Database.DBModel.Dictionary;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore;
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
        private readonly IConnectionStringHelper _helper;

        public DictionariesDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        // Агенты
        #region DictionaryAgents

        public bool IsAgentOneRole(IContext context,int id,EnumDictionaryAgentTypes source)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var agent = GetDictionaryAgent(context, id);

                switch (source)
                {
                    case EnumDictionaryAgentTypes.isEmployee:
                        if (!agent.IsIndividual && !agent.IsCompany && !agent.IsBank) { return true; }
                        break;
                    case EnumDictionaryAgentTypes.isCompany:
                        if (!agent.IsIndividual && !agent.IsEmployee && !agent.IsBank) { return true; }
                        break;
                    case EnumDictionaryAgentTypes.isIndividual:
                        if (!agent.IsEmployee && !agent.IsCompany && !agent.IsBank) { return true; }
                        break;
                    case EnumDictionaryAgentTypes.isBank:
                        if (!agent.IsEmployee && !agent.IsCompany && !agent.IsIndividual) { return true; }
                        break;
                }
                return false;
            }
        }

        public IEnumerable<EnumDictionaryAgentTypes> GetAgentRoles(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                List<EnumDictionaryAgentTypes> list = new List<EnumDictionaryAgentTypes>();
                var agent = GetDictionaryAgent(context, id);
                if (agent.IsBank) { list.Add(EnumDictionaryAgentTypes.isBank); }
                if (agent.IsEmployee) { list.Add(EnumDictionaryAgentTypes.isEmployee); }
                if (agent.IsIndividual) { list.Add(EnumDictionaryAgentTypes.isIndividual); }
                if (agent.IsCompany) { list.Add(EnumDictionaryAgentTypes.isCompany); }

                return list;
            }
        }

        public FrontDictionaryAgent GetDictionaryAgent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
                        Value=y.Contact,
                        IsActive=y.IsActive,
                        Description=y.Description
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
                        PostCode=z.PostCode,
                        Address=z.Address,
                        IsActive = z.IsActive,
                        Description = z.Description
                    })

                })            
                .FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryAgentsSet.AsQueryable();

                if (filter.AgentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AgentId.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(filter.Name));
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

        public void UpdateDictionaryAgent(IContext context, InternalDictionaryAgent agent) {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryAgents
                {
                    Id = agent.Id,
                    Name = agent.Name,
                    ResidentTypeId=agent.ResidentTypeId,
                    IsBank=agent.IsBank,
                    IsCompany=agent.IsCompany,
                    IsEmployee=agent.IsEmployee,
                    IsIndividual=agent.IsIndividual,
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

        public void DeleteDictionaryAgent(IContext context, InternalDictionaryAgent agent)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var ddt = dbContext.DictionaryAgentsSet.FirstOrDefault(x => x.Id == agent.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentsSet.Remove(ddt);

                    dbContext.SaveChanges();
                }

            }
        }

        public int AddDictionaryAgent(IContext context, InternalDictionaryAgent newAgent)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                return
                    dbContext.DictionaryAgentPersonsSet.Where(x => x.Id == id).Select(x => new FrontDictionaryAgentPerson
                    {
                        Id = x.Id,
                        IsIndividual=true,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        MiddleName = x.MiddleName,
                        TaxCode=x.TaxCode,
                        IsMale=x.IsMale,
                        PassportSerial=x.PassportSerial,
                        PassportNumber=x.PassportNumber,
                        PassportText=x.PassportText,
                        PassportDate=x.PassportDate,
                        BirthDate=x.BirthDate,
                        Description=x.Description,
                        IsActive=x.IsActive,
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

        public IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var qry = dbContext.DictionaryAgentPersonsSet.AsQueryable();

                qry = qry.Where(x => x.Agent.IsIndividual);


                if (filter.AgentId?.Count > 0)
                {
                   qry = qry.Where(x => filter.AgentId.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.FullName.Contains(filter.Name));
                }
                if (!string.IsNullOrEmpty(filter.Passport))
                {
                    qry = qry.Where(x => x.PassportText.Contains(filter.Passport));
                }
                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    qry = qry.Where(x => x.TaxCode.Contains(filter.TaxCode));
                }
                if (filter.BirthDate!=null )
                {
                    qry = qry.Where(x => x.BirthDate == filter.BirthDate);
                }

                return qry.Select(x => new FrontDictionaryAgentPerson
                {
                    Id = x.Id,
                    IsIndividual=true,
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

        public void UpdateDictionaryAgentPerson(IContext context, InternalDictionaryAgentPerson person) {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryAgentPersons
                {
                    Id = person.Id,
                    FirstName=person.FirstName,
                    LastName=person.LastName,
                    MiddleName=person.MiddleName,
                    FullName=person.LastName + " " + person.FirstName + " " + person.MiddleName,
                    TaxCode=person.TaxCode,
                    IsMale=person.IsMale,
                    PassportSerial=person.PassportSerial,
                    PassportNumber=person.PassportNumber,
                    PassportText=person.PassportText,
                    PassportDate=person.PassportDate,
                    BirthDate=person.BirthDate,
                    Description = person.Description,
                    LastChangeDate = person.LastChangeDate,
                    LastChangeUserId = person.LastChangeUserId,
                    IsActive = person.IsActive
                };

                dbContext.DictionaryAgentPersonsSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                if (IsAgentOneRole(context, person.Id, EnumDictionaryAgentTypes.isIndividual))
                {
                    var agent = new InternalDictionaryAgent
                    {
                        Id = person.Id,
                        Name = person.LastName.Trim() + " " + person.FirstName.Trim() + " " + person.MiddleName.Trim(),
                        IsActive=person.IsActive,
                        Description=person.Description,
                        IsBank=false,
                        IsCompany=false,
                        IsEmployee=false,
                        IsIndividual=true
                    };
                    UpdateDictionaryAgent(context, agent);
                }
            }
        }


        public void DeleteDictionaryAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var ddt = dbContext.DictionaryAgentPersonsSet.FirstOrDefault(x => x.Id == person.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentPersonsSet.Remove(ddt);
                    

                    if (IsAgentOneRole(context,person.Id,EnumDictionaryAgentTypes.isIndividual)) {
                        DeleteDictionaryAgent(context, new InternalDictionaryAgent { Id = person.Id });
                    }

                    dbContext.SaveChanges();
                }

            }
        }

        public int AddDictionaryAgentPerson(IContext context, InternalDictionaryAgentPerson person)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryAgentPersons
                {
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

                var agent = GetDictionaryAgent(context, person.Id); 
                if (agent==null)
                {
                    var newAgent = new InternalDictionaryAgent
                    {
                        Id = person.Id,
                        Name = person.LastName.Trim() + " " + person.FirstName.Trim() + " " + person.MiddleName.Trim(),
                        IsActive = person.IsActive,
                        Description = person.Description,
                        IsBank = false,
                        IsCompany = false,
                        IsEmployee = false,
                        IsIndividual = true
                    };
                    AddDictionaryAgent(context, newAgent);
                }
                else {
                    var curAgent = GetDictionaryAgent(context, person.Id);
                    curAgent.IsIndividual = true;
                    UpdateDictionaryAgent(context, new InternalDictionaryAgent
                    {
                        Id = curAgent.Id,
                        Name = curAgent.Name,
                        IsActive = curAgent.IsActive,
                        Description = curAgent.Description,
                        IsBank = curAgent.IsBank,
                        IsCompany = curAgent.IsCompany,
                        IsEmployee = curAgent.IsEmployee,
                        IsIndividual = curAgent.IsIndividual,
                        ResidentTypeId = curAgent.ResidentTypeId ?? 0,
                        LastChangeDate = person.LastChangeDate,
                        LastChangeUserId = person.LastChangeUserId
                    });

                }

                dbContext.SaveChanges();
                person.Id = ddt.Id;
                return ddt.Id;
            }
        }

        #endregion DictionaryAgentPerson

        #region DictionaryAgentEmployee

        public FrontDictionaryAgentEmployee GetDictionaryAgentEmployee(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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


        public void UpdateDictionaryAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee) { }
        public void DeleteDictionaryAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee) { }
        public int AddDictionaryAgentEmployee(IContext context, InternalDictionaryAgentEmployee employee) { return 0; }

        public IEnumerable<FrontDictionaryAgentEmployee> GetDictionaryAgentEmployees(IContext context, FilterDictionaryAgentEmployee filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryAgentEmployeesSet.AsQueryable();

                qry = qry.Where(x => x.Agent.IsEmployee);

                if (filter.AgentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AgentId.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Agent.AgentPerson.FullName.Contains(filter.Name));
                }
                if (!string.IsNullOrEmpty(filter.Passport))
                {
                    qry = qry.Where(x => x.Agent.AgentPerson.PassportText.Contains(filter.Passport));
                }
                if (!string.IsNullOrEmpty(filter.TaxCode))
                {
                    qry = qry.Where(x => x.Agent.AgentPerson.TaxCode.Contains(filter.TaxCode));
                }
                if (filter.BirthDate != null)
                {
                    qry = qry.Where(x => x.Agent.AgentPerson.BirthDate == filter.BirthDate);
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryAgentAddressesSet.AsQueryable();

                return qry.Select(x => new FrontDictionaryAgentAddress
                {
                    Id = x.Id,
                    AgentId = x.AgentId,
                    AddressType= new FrontDictionaryAddressType { Id = x.AdressTypeId, Name = x.AddressType.Name },
                    PostCode=x.PostCode,
                    Address=x.Address,
                    Description=x.Description,
                    IsActive = x.IsActive,
                }).FirstOrDefault();
            }
        }

        public void UpdateDictionaryAgentAddress(IContext context, InternalDictionaryAgentAddress addr)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryAgentAddresses
                {
                    Id = addr.Id,
                    AgentId=addr.AgentId,
                    AdressTypeId = addr.AddressTypeID,
                    PostCode=addr.PostCode,
                    Address=addr.Address,
                    Description=addr.Description,
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryAgentAddresses
                {
                    AgentId = addr.AgentId,
                    AdressTypeId =  addr.AddressTypeID,
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

        public IEnumerable<FrontDictionaryAgentAddress> GetDictionaryAgentAddresses(IContext context, int agentId,FilterDictionaryAgentAddress filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryAgentAddressesSet.AsQueryable();

                qry = qry.Where(x => x.AgentId == filter.AgentId);

                if (filter.AddressTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AddressTypeId.Contains(x.AdressTypeId));
                }

                if (!String.IsNullOrEmpty(filter.PostCode))
                {
                    qry = qry.Where(x => x.PostCode.Contains(filter.PostCode));
                }

                if (!String.IsNullOrEmpty(filter.Address))
                {
                    qry = qry.Where(x => x.Address.Contains(filter.Address));
                }

                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                return qry.Select(x => new FrontDictionaryAgentAddress
                {
                    Id = x.Id,
                    AgentId=x.AgentId,
                    AddressType=new FrontDictionaryAddressType { Id = x.AddressType.Id, Name = x.AddressType.Name },
                    Address=x.Address,
                    PostCode=x.PostCode,
                    Description=x.Description,
                    IsActive = x.IsActive
                }).ToList();
            }
        }
        #endregion

        // Типы адресов 
        #region DictionaryAddressTypes
        public void UpdateDictionaryAddressType(IContext context, InternalDictionaryAddressType addrType)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryAddressTypes
                {
                    Name = addrType.Name,
                    IsActive=addrType.IsActive,
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryDocumentTypesSet.AsQueryable();

                if (filter.AddressTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AddressTypeId.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => filter.Name == x.Name);
                }

                if (filter.IsActive !=null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                return qry.Select(x => new InternalDictionaryAddressType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive=x.IsActive,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryAddressType> GetDictionaryAddressTypes(IContext context, FilterDictionaryAddressType filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryAddressTypesSet.AsQueryable();

                if (filter.AddressTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AddressTypeId.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(filter.Name));
                }

                if (filter.IsActive.HasValue )
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
                }

                return qry.Select(x => new FrontDictionaryAddressType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive=x.IsActive
                }).ToList();
            }
        }
        #endregion

        #region DictionaryCompanies
        public BaseDictionaryCompany GetDictionaryCompany(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                return dbContext.DictionaryCompaniesSet.Where(x => x.Id == id).Select(x => new BaseDictionaryCompany
                {
                    Id = x.Id,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryCompany> GetDictionaryCompanies(IContext context, FilterDictionaryCompany filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryCompaniesSet.AsQueryable();

                if (filter.CompanyId?.Count > 0)
                {
                    qry = qry.Where(x => filter.CompanyId.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(filter.Name));
                }

                return qry.Select(x => new BaseDictionaryCompany
                {
                    Id = x.Id,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                }).ToList();
            }
        }
        #endregion DictionaryCompanies

        // Типы контактов
        #region DictionaryContactTypes
        public FrontDictionaryContactType GetInternalDictionaryContactType(IContext context, FilterDictionaryContactType filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryContactTypesSet.AsQueryable();

                if (filter.ContactTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.ContactTypeId.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => filter.Name == x.Name);
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryContactTypesSet.AsQueryable();

                if (filter.ContactTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.ContactTypeId.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(filter.Name));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryAgentContacts
                {
                    Id = contact.Id,
                    AgentId = contact.AgentId,
                    ContactTypeId=contact.ContactTypeId,
                    Contact=contact.Value,
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
        public IEnumerable<FrontDictionaryContact> GetDictionaryContacts(IContext context, int agentId,FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryAgentContactsSet.AsQueryable();

                qry = qry.Where(x => x.AgentId==agentId);

                if (filter.ContactTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.ContactTypeId.Contains(x.ContactTypeId));
                }

                if (filter.AgentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AgentId.Contains(x.AgentId));
                }


                if (!String.IsNullOrEmpty(filter.Value))
                {
                    qry = qry.Where(x => x.Contact.Contains(filter.Value));
                }

                if (filter.IsActive != null)
                {
                    qry = qry.Where(x => x.IsActive == filter.IsActive);
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

        // Штатное расписание
        #region DictionaryDepartments
        public BaseDictionaryDepartment GetDictionaryDepartment(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                return
                    dbContext.DictionaryDepartmentsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryDepartment
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        CompanyId = x.CompanyId,
                        Name = x.Name,
                        ChiefPositionId = x.ChiefPositionId,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        ParentDepartmentName = x.ParentDepartment.Name,
                        CompanyName = x.Company.Name,
                        ChiefPositionName = x.ChiefPosition.Name,
                        ChildDepartments = x.ChildDepartments.Select(y => new BaseDictionaryDepartment
                        {
                            Id = y.Id,
                            ParentId = y.ParentId,
                            CompanyId = y.CompanyId,
                            Name = y.Name,
                            ChiefPositionId = y.ChiefPositionId,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
                            ParentDepartmentName = y.ParentDepartment.Name,
                            CompanyName = y.Company.Name,
                            ChiefPositionName = y.ChiefPosition.Name,
                        })
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryDepartmentsSet.AsQueryable();

                if (filter.DepartmentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.DepartmentId.Contains(x.Id));
                }
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => x.Name.Contains(filter.Name));
                }

                return qry.Select(x => new BaseDictionaryDepartment
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    CompanyId = x.CompanyId,
                    Name = x.Name,
                    ChiefPositionId = x.ChiefPositionId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    ParentDepartmentName = x.ParentDepartment.Name,
                    CompanyName = x.Company.Name,
                    ChiefPositionName = x.ChiefPosition.Name,
                }).ToList();
            }
        }
        #endregion DictionaryDepartments

        #region DictionaryDocumentDirections
        public BaseDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryDocumentDirectionsSet.AsQueryable();

                if (filter.DocumentDirectionId?.Count > 0)
                {
                    qry = qry.Where(x => filter.DocumentDirectionId.Contains(x.Id));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryDocumentSubjects
                {
                    ParentId =  docSubject.ParentId,
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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

            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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

            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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

        //TODO Эта  функция может находиться в FilterDictionaryDocumentSubject. Очень удобно: добавляешь параметр и сразу же добавляешь ифчик. У меня упихнуть НЕ получилось из-за пространства имен
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
                qry = qry.Where(x => x.Name.Contains(filter.Name));
            }

            // Условие по IsActive
            if (filter.IsActive != null)
            {
                qry = qry.Where(x => filter.IsActive == x.IsActive);
            }

            // Условие по ParentId
            if (filter.ParentId != null)
            {
                qry = qry.Where(x => filter.ParentId == x.ParentId);
            }

            return qry;
        }

        #endregion DictionaryDocumentSubjects

        // Типы документов
        #region DictionaryDocumentTypes
        public void UpdateDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryDocumentTypes
                {
                    Id = docType.Id,
                    LastChangeDate = docType.LastChangeDate,
                    LastChangeUserId = docType.LastChangeUserId,
                    Name = docType.Name,
                    IsActive= docType.IsActive
                };
                dbContext.DictionaryDocumentTypesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);

                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.LastChangeDate).IsModified = true;
                entity.Property(x => x.LastChangeUserId).IsModified = true;
                entity.Property(x => x.IsActive).IsModified = true;
                dbContext.SaveChanges();
            }
        }


        public void DeleteDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                
                var ddt = dbContext.DictionaryDocumentTypesSet.FirstOrDefault(x => x.Id == docType.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryDocumentTypesSet.Remove(ddt);
                    dbContext.SaveChanges();
                }
            }
        }

        public int AddDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryDocumentTypes
                {
                    Name = docType.Name,
                    IsActive=docType.IsActive,
                    LastChangeDate = docType.LastChangeDate,
                    LastChangeUserId = docType.LastChangeUserId
                };
                dbContext.DictionaryDocumentTypesSet.Add(ddt);
                dbContext.SaveChanges();
                docType.Id = ddt.Id;
                return ddt.Id;
            }
        }

        public InternalDictionaryDocumentType GetInternalDictionaryDocumentType(IContext context, FilterDictionaryDocumentType filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryDocumentTypesSet.AsQueryable();

                if (filter.DocumentTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.DocumentTypeId.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x =>  x.Name.Contains(filter.Name));
                }

                if (filter.IsActive!=null)
                {
                    qry = qry.Where(x => filter.IsActive == x.IsActive);
                }

                return qry.Select(x => new InternalDictionaryDocumentType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive=x.IsActive,
                    LastChangeDate = x.LastChangeDate,
                    LastChangeUserId = x.LastChangeUserId
                }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryDocumentTypesSet.AsQueryable();

                if (filter.DocumentTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.DocumentTypeId.Contains(x.Id));
                }

                if (!String.IsNullOrEmpty(filter.Name))
                {
                    qry = qry.Where(x => filter.Name.Contains(x.Name));
                }

                if (filter.IsActive != null)
                { 
                qry = qry.Where(x => x.IsActive==filter.IsActive);
                }

            return qry.Select(x => new FrontDictionaryDocumentType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive=x.IsActive
                }).ToList();
            }
        }
        #endregion DictionaryDocumentTypes

        #region DictionaryEventTypes
        public BaseDictionaryEventType GetDictionaryEventType(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryEventTypesSet.AsQueryable();

                if (filter.EventTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.EventTypeId.Contains(x.Id));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryImportanceEventTypesSet.AsQueryable();

                if (filter.ImportanceEventTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.ImportanceEventTypeId.Contains(x.Id));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.EventType.ImportanceEventTypeId).Contains(x.Id)
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryLinkTypesSet.AsQueryable();

                if (filter.LinkTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.LinkTypeId.Contains(x.Id));
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

        public int? GetExecutorAgentIdByPositionId(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                return dbContext.DictionaryPositionsSet.Where(x => x.Id == id)
                    .Select(x => x.ExecutorAgentId).FirstOrDefault();
            }
        }


        public BaseDictionaryPosition GetDictionaryPosition(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                return dbContext.DictionaryPositionsSet.Where(x => x.Id == id)
                    .Select(x => new BaseDictionaryPosition
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        Name = x.Name,
                        DepartmentId = x.DepartmentId,
                        ExecutorAgentId = x.ExecutorAgentId,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        ParentPositionName = x.ParentPosition.Name,
                        DepartmentName = x.Department.Name,
                        ExecutorAgentName = x.ExecutorAgent.Name,
                        ChildPositions = x.ChildPositions.Select(y => new BaseDictionaryPosition
                        {
                            Id = y.Id,
                            ParentId = y.ParentId,
                            Name = y.Name,
                            DepartmentId = y.DepartmentId,
                            ExecutorAgentId = y.ExecutorAgentId,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
                            ParentPositionName = y.ParentPosition.Name,
                            DepartmentName = y.Department.Name,
                            ExecutorAgentName = y.ExecutorAgent.Name
                        }),
                        ChiefDepartments = x.ChiefDepartments.Select(y => new BaseDictionaryDepartment
                        {
                            Id = y.Id,
                            ParentId = y.ParentId,
                            CompanyId = y.CompanyId,
                            Name = y.Name,
                            ChiefPositionId = y.ChiefPositionId,
                            LastChangeUserId = y.LastChangeUserId,
                            LastChangeDate = y.LastChangeDate,
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

        public IEnumerable<BaseDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryPositionsSet.Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

                if (filter.PositionId?.Count > 0)
                {
                    qry = qry.Where(x => filter.PositionId.Contains(x.pos.Id));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.TargetPositionId).Contains(x.pos.Id)
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

                return qry.Select(x => new BaseDictionaryPosition
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryPositionsSet.Select(x => new { pos = x, subordMax = 0 }).AsQueryable();

                if (filter.PositionId?.Count > 0)
                {
                    qry = qry.Where(x => filter.PositionId.Contains(x.pos.Id));
                }

                if (filter.DocumentId?.Count > 0)
                {
                    qry = qry.Where(x =>
                            dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.SourcePositionId).Contains(x.pos.Id)
                                ||
                                dbContext.DocumentEventsSet
                                .Where(y => filter.DocumentId.Contains(y.DocumentId)).Select(y => y.TargetPositionId).Contains(x.pos.Id)
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

        // Журналы регистрации
        #region DictionaryRegistrationJournals
        public int AddDictionaryRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal regJournal)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                 
                // Не нашел функцию IIf. Знаю, что можно компактнее
                string s=null;

                if (regJournal.IsIncoming)
                {
                    s = s + "1";
                }

                if (regJournal.IsOutcoming)
                {
                    s = s + "2";
                }

                if (regJournal.IsInternal)
                {
                    s = s + "3";
                }
                                    
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                // Не нашел функцию IIf. Знаю, что можно компактнее
                string s = null;

                if (regJournal.IsIncoming)
                {
                    s = s + "1";
                }

                if (regJournal.IsOutcoming)
                {
                    s = s + "2";
                }

                if (regJournal.IsInternal)
                {
                    s = s + "3";
                }

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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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

        //TODO Эта  функция может находиться в FilterDictionaryRegistrationJournal. Очень удобно: добавляешь параметр и сразу же добавляешь ифчик. У меня упихнуть НЕ получилось из-за пространства имен
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
                qry = qry.Where(x => x.Name.Contains(filter.Name));
            }

            // Условие по Index
            if (!String.IsNullOrEmpty(filter.Index))
            {
                qry = qry.Where(x => x.Index.Contains(filter.Index));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryResultTypesSet.AsQueryable();

                if (filter.ResultTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.ResultTypeId.Contains(x.Id));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionarySendTypesSet.AsQueryable();

                if (filter.SendTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.SendTypeId.Contains(x.Id));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryStandartSendListContentsSet.AsQueryable();

                if (filter.StandartSendListContentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.StandartSendListContentId.Contains(x.Id));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryStandartSendListsSet.AsQueryable();
                if (filter.StandartSendListId != null && filter.StandartSendListId.Count > 0)
                {
                    qry = qry.Where(x => filter.StandartSendListId.Contains(x.Id));
                }
                if (filter.PositionId != null && filter.PositionId.Count > 0)
                {
                    qry = qry.Where(x => filter.PositionId.Contains(x.PositionId));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionarySubordinationTypesSet.AsQueryable();

                if (filter.SubordinationTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.SubordinationTypeId.Contains(x.Id));
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

        public InternalDictionaryTag GetInternalDictionaryTags(IContext context, FilterDictionaryTag filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryTagsSet.AsQueryable();

                qry = qry.Where(x => !x.PositionId.HasValue || context.CurrentPositionsIdList.Contains(x.PositionId ?? 0));

                if (filter.TagId?.Count > 0)
                {
                    qry = qry.Where(x => filter.TagId.Contains(x.Id));
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

        public IEnumerable<FrontDictionaryTag> GetDictionaryTags(IContext context, FilterDictionaryTag filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryTagsSet.AsQueryable();

                qry = qry.Where(x => !x.PositionId.HasValue || context.CurrentPositionsIdList.Contains(x.PositionId ?? 0));

                if (filter.TagId?.Count > 0)
                {
                    qry = qry.Where(x => filter.TagId.Contains(x.Id));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
        public void UpdateDictionaryTag(IContext context, InternalDictionaryTag model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var savTag = dbContext.DictionaryTagsSet
                    .Where(x => x.Id == model.Id)
                    .Where(x => context.CurrentPositionsIdList.Contains(x.PositionId ?? 0))
                    .FirstOrDefault();

                if (savTag?.Id > 0)
                {
                    savTag.Name = model.Name;
                    savTag.Color = model.Color;
                    savTag.LastChangeUserId = context.CurrentAgentId;
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.CustomDictionaryTypesSet.AsQueryable();

                if (filter != null)
                {
                    if (filter.CustomDictionaryTypeId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.CustomDictionaryTypeId.Contains(x.Id));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.CustomDictionaryTypesSet.AsQueryable();

                if (filter != null)
                {
                    if (filter.CustomDictionaryTypeId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.CustomDictionaryTypeId.Contains(x.Id));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.CustomDictionariesSet.AsQueryable();

                if (filter != null)
                {
                    if (filter.CustomDictionaryTypeId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.CustomDictionaryTypeId.Contains(x.Id));
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
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
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.CustomDictionariesSet.AsQueryable();

                if (filter != null)
                {
                    if (filter.CustomDictionaryTypeId?.Count > 0)
                    {
                        qry = qry.Where(x => filter.CustomDictionaryTypeId.Contains(x.DictionaryTypeId));
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
