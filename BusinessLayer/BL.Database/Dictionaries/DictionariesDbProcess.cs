using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore;
using BL.Database.Dictionaries.Interfaces;

namespace BL.Database.Dictionaries
{
    public class DictionariesDbProcess : CoreDb.CoreDb, IDictionariesDbProcess
    {
        #region DictionaryStandartSendLists
        public BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryStandartSendListsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryStandartSendList
            {
                Id = x.Id,
                Name = x.Name,
                PositionId = x.PositionId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                PositionName = x.Position.Name,
                StandartSendListContents = x.StandartSendListContents.Select(y => new BaseDictionaryStandartSendListContent
                {
                    Id = y.Id,
                    StandartSendListId = y.StandartSendListId,
                    OrderNumber = y.OrderNumber,
                    SendTypeId = y.SendTypeId,
                    TargetPositionId = y.TargetPositionId,
                    Description = y.Description,
                    DueDate = y.DueDate,
                    DueDay = y.DueDay,
                    AccessLevelId = y.AccessLevelId,
                    LastChangeUserId = y.LastChangeUserId,
                    LastChangeDate = y.LastChangeDate,
                    SendTypeName = y.SendType.Name,
                    TargetPositionName = y.TargetPosition.Name,
                    AccessLevelName = y.AccessLevel.Name
                }
                ).ToList()
            }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryStandartSendListsSet.AsQueryable();
            if (filter.Id != null && filter.Id.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
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
        #endregion DictionaryStandartSendList

        #region DictionaryAgents
        public BaseDictionaryAgent GetDictionaryAgent(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryAgentsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryAgent
            {
                Id = x.Id,
                Name = x.Name,
                TaxCode = x.TaxCode,
                IsIndividual = x.IsIndividual,
                IsEmployee = x.IsEmployee,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                AgentPersons = x.AgentPersons.Select(y => new BaseDictionaryAgentPerson
                {
                    Id = y.Id,
                    AgentId = y.AgentId,
                    Name = y.Name,
                    PersonAgentId = y.PersonAgentId,
                    LastChangeUserId = y.LastChangeUserId,
                    LastChangeDate = y.LastChangeDate,
                    AgentName = y.Agent.Name,
                    PersonAgentName = y.PersonAgent.Name
                }).ToList()
            }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryAgent> GetDictionaryAgents(IContext context, FilterDictionaryAgent filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryAgentsSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }
            if (!string.IsNullOrEmpty(filter.Name))
            {
                qry = qry.Where(x => x.Name.Contains(filter.Name));
            }
            if (!string.IsNullOrEmpty(filter.TaxCode))
            {
                qry = qry.Where(x => x.TaxCode.Contains(filter.TaxCode));
            }
            if (filter.IsIndividual.HasValue)
            {
                qry = qry.Where(x => x.IsIndividual == filter.IsIndividual);
            }
            if (filter.IsEmployee.HasValue)
            {
                qry = qry.Where(x => x.IsEmployee == filter.IsEmployee);
            }

            return qry.Select(x => new BaseDictionaryAgent
            {
                Id = x.Id,
                Name = x.Name,
                TaxCode = x.TaxCode,
                IsIndividual = x.IsIndividual,
                IsEmployee = x.IsEmployee,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate
            }).ToList();
        }
        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        public BaseDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryAgentPersonsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryAgentPerson
            {
                Id = x.Id,
                AgentId = x.AgentId,
                Name = x.Name,
                PersonAgentId = x.PersonAgentId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                AgentName = x.Agent.Name,
                PersonAgentName = x.PersonAgent.Name
            }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryAgentPersonsSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }
            if (filter.AgentId?.Count > 0)
            {
                qry = qry.Where(x => filter.AgentId.Contains(x.AgentId));
            }
            if (!string.IsNullOrEmpty(filter.Name))
            {
                qry = qry.Where(x => x.Name.Contains(filter.Name));
            }
            if (!string.IsNullOrEmpty(filter.AgentName))
            {
                qry = qry.Where(x => x.Agent.Name.Contains(filter.AgentName));
            }
            if (!string.IsNullOrEmpty(filter.PersonAgentName))
            {
                qry = qry.Where(x => x.PersonAgent.Name.Contains(filter.PersonAgentName));
            }

            return qry.Select(x => new BaseDictionaryAgentPerson
            {
                Id = x.Id,
                AgentId = x.AgentId,
                Name = x.Name,
                PersonAgentId = x.PersonAgentId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                AgentName = x.Agent.Name,
                PersonAgentName = x.PersonAgent.Name
            }).ToList();
        }
        #endregion DictionaryAgentPersons

        #region DictionaryCompanies
        public BaseDictionaryCompany GetDictionaryCompany(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryCompaniesSet.Where(x => x.Id == id).Select(x => new BaseDictionaryCompany
            {
                Id = x.Id,
                Name = x.Name,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
            }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryCompany> GetDictionaryCompanies(IContext context, FilterDictionaryCompany filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryCompaniesSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
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
        #endregion DictionaryCompanies

        #region DictionaryDepartments
        public BaseDictionaryDepartment GetDictionaryDepartment(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryDepartmentsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryDepartment
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
                ChildDepartments = x.ChildDepartments.Select(y=>new BaseDictionaryDepartment {
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
                }).ToList()
            }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryDepartment> GetDictionaryDepartments(IContext context, FilterDictionaryDepartment filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryDepartmentsSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
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
        #endregion DictionaryDepartments

    }
}
