using System.Collections.Generic;
using System.Linq;
using BL.Database.DatabaseContext;
using BL.Model.DictionaryCore;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.Enums;
using System;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DBModel.Dictionary;
using BL.Model.AdminCore;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;

namespace BL.Database.Dictionaries
{
    public class DictionariesDbProcess : CoreDb.CoreDb, IDictionariesDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public DictionariesDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

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

        public BaseDictionaryAgent GetDictionaryAgent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                return dbContext.DictionaryAgentsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryAgent
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsIndividual = x.IsIndividual,
                    IsEmployee = x.IsEmployee,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    /*AgentPersonsAgents = x.AgentPersonsAgents.Select(y => new BaseDictionaryAgentPerson
                    {
                        Id = y.Id,
                        AgentId = y.AgentId,
                        Name = y.Name,
                        PersonAgentId = y.PersonAgentId,
                        LastChangeUserId = y.LastChangeUserId,
                        LastChangeDate = y.LastChangeDate,
                        AgentName = y.Agent.Name,
                        PersonAgentName = y.PersonAgent.Name
                    }),
                    AgentPersonsPersonAgents = x.AgentPersonsPersonAgents.Select(y => new BaseDictionaryAgentPerson
                    {
                        Id = y.Id,
                        AgentId = y.AgentId,
                        Name = y.Name,
                        PersonAgentId = y.PersonAgentId,
                        LastChangeUserId = y.LastChangeUserId,
                        LastChangeDate = y.LastChangeDate,
                        AgentName = y.Agent.Name,
                        PersonAgentName = y.PersonAgent.Name
                    })*/
                }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter)
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
        /*        if (filter.IsIndividual.HasValue)
                {
                    qry = qry.Where(x => x.IsIndividual == filter.IsIndividual);
                }
                if (filter.IsEmployee.HasValue)
                {
                    qry = qry.Where(x => x.IsEmployee == filter.IsEmployee);
                }
*/
                return qry.Select(x => new BaseDictionaryAgent
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsIndividual = x.IsIndividual,
                    IsEmployee = x.IsEmployee,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).ToList();
            }
        }
        #endregion DictionaryAgents

        #region DictionaryAgentPerson
        public FrontDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                return
                    dbContext.DictionaryAgentPersonsSet.Where(x => x.Id == id).Select(x => new FrontDictionaryAgentPerson
                    {
                        Id = x.Id,
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
                        Description=x.Description
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<FrontDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var qry = dbContext.DictionaryAgentPersonsSet.AsQueryable();
                
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
                    qry = qry.Where(x => x.Passport.Contains(filter.Passport));
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
                    Description = x.Description
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

                if (IsAgentOneRole(context, person.Id, EnumDictionaryAgentTypes.isIndividual))
                {
                    
                }

                dbContext.SaveChanges();
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

                }
                else { }

                dbContext.SaveChanges();
                person.Id = ddt.Id;
                return ddt.Id;
            }
        }

        #endregion DictionaryAgentPerson

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
                    AddressType= new DictionaryAddressTypes { Id = addr.AddressTypeID },
                    PostCode=addr.PostCode,
                    Address=addr.Address,
                    Description=addr.Description,
                    LastChangeDate = addr.LastChangeDate,
                    LastChangeUserId = addr.LastChangeUserId,
                    IsActive = addr.IsActive
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

        public IEnumerable<FrontDictionaryAgentAddress> GetDictionaryAgentAddresses(IContext context, FilterDictionaryAgentAddress filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryAgentAddressesSet.AsQueryable();

                if (filter.AddressTypeId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AddressTypeId.Contains(x.AdressTypeId));
                }

                if (filter.AgentId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AgentId.Contains(x.AgentId));
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

                if (filter.IsActive != null)
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

        #region DictionaryContactTypes
        public FrontDictionaryContactType GetInternalDictionaryContactType(IContext context, FilterDictionaryContactType filter)
        {
            return new FrontDictionaryContactType();
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

                if (filter.IsActive != null)
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

                var ddt = dbContext.DictionaryAgentAddressesSet.FirstOrDefault(x => x.Id == contact.Id);
                if (ddt != null)
                {
                    dbContext.DictionaryAgentAddressesSet.Remove(ddt);
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
        public IEnumerable<FrontDictionaryContact> GetDictionaryContacts(IContext context, FilterDictionaryContact filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryAgentContactsSet.AsQueryable();

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

        #region DictionaryDocumentSubjects
        public BaseDictionaryDocumentSubject GetDictionaryDocumentSubject(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                return dbContext.DictionaryDocumentSubjectsSet.Where(x => x.Id == id)
                        .Select(x => new BaseDictionaryDocumentSubject
                        {
                            Id = x.Id,
                            ParentId = x.ParentId,
                            Name = x.Name,
                            LastChangeUserId = x.LastChangeUserId,
                            LastChangeDate = x.LastChangeDate,
                            ParentDocumentSubjectName = x.ParentDocumentSubject.Name,
                            ChildDocumentSubjects =
                                x.ChildDocumentSubjects.Select(y => new BaseDictionaryDocumentSubject
                                {
                                    Id = y.Id,
                                    ParentId = y.ParentId,
                                    Name = y.Name,
                                    LastChangeUserId = y.LastChangeUserId,
                                    LastChangeDate = y.LastChangeDate,
                                    ParentDocumentSubjectName = y.ParentDocumentSubject.Name,
                                })
                        }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryDocumentSubjectsSet.AsQueryable();

                if (filter.DocumentSubjectId?.Count > 0)
                {
                    qry = qry.Where(x => filter.DocumentSubjectId.Contains(x.Id));
                }

                return qry.Select(x => new BaseDictionaryDocumentSubject
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    ParentDocumentSubjectName = x.ParentDocumentSubject.Name,
                }).ToList();
            }
        }
        #endregion DictionaryDocumentSubjects

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
        #endregion DictionaryDocumentSubjects

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

        #region DictionaryPositions
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

        #region DictionaryRegistrationJournals
        public BaseDictionaryRegistrationJournal GetDictionaryRegistrationJournal(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                return dbContext.DictionaryRegistrationJournalsSet.Where(x => x.Id == id)
                    .Select(x => new BaseDictionaryRegistrationJournal
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DepartmentId = x.DepartmentId,
                        Index = x.Index,
                        NumerationPrefixFormula = x.NumerationPrefixFormula,
                        PrefixFormula = x.PrefixFormula,
                        SuffixFormula = x.SuffixFormula,
                        DirectionCodes = x.DirectionCodes,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        DepartmentName = x.Department.Name
                    }).FirstOrDefault();
            }
        }

        public IEnumerable<BaseDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.DictionaryRegistrationJournalsSet.AsQueryable();

                if (filter.RegistrationJournalId?.Count > 0)
                {
                    qry = qry.Where(x => filter.RegistrationJournalId.Contains(x.Id));
                }

                return qry.Select(x => new BaseDictionaryRegistrationJournal
                {
                    Id = x.Id,
                    Name = x.Name,
                    DepartmentId = x.DepartmentId,
                    Index = x.Index,
                    PrefixFormula = x.PrefixFormula,
                    SuffixFormula = x.SuffixFormula,
                    DirectionCodes = x.DirectionCodes,
                    DepartmentName = x.Department.Name
                }).ToList();
            }
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
