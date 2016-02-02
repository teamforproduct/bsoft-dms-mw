using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.Enums;

namespace BL.Database.Dictionaries
{
    public class DictionariesDbProcess : CoreDb.CoreDb, IDictionariesDbProcess
    {
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
                })
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

        #region DictionaryDocumentDirections
        public BaseDictionaryDocumentDirection GetDictionaryDocumentDirection(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryDocumentDirectionsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryDocumentDirection
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate
            }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryDocumentDirection> GetDictionaryDocumentDirections(IContext context, FilterDictionaryDocumentDirection filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryDocumentDirectionsSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
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
        #endregion DictionaryDepartments

        #region DictionaryDocumentSubjects
        public BaseDictionaryDocumentSubject GetDictionaryDocumentSubject(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryDocumentSubjectsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryDocumentSubject
            {
                Id = x.Id,
                ParentId = x.ParentId,
                Name = x.Name,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                ParentDocumentSubjectName = x.ParentDocumentSubject.Name,
                ChildDocumentSubjects = x.ChildDocumentSubjects.Select(y => new BaseDictionaryDocumentSubject
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

        public IEnumerable<BaseDictionaryDocumentSubject> GetDictionaryDocumentSubjects(IContext context, FilterDictionaryDocumentSubject filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryDocumentSubjectsSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
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
        #endregion DictionaryDocumentSubjects

        #region DictionaryDocumentTypes
        public BaseDictionaryDocumentType GetDictionaryDocumentType(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryDocumentTypesSet.Where(x => x.Id == id).Select(x => new BaseDictionaryDocumentType
            {
                Id = x.Id,
                Name = x.Name,
                DirectionCodes = x.DirectionCodes,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
            }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryDocumentType> GetDictionaryDocumentTypes(IContext context, FilterDictionaryDocumentType filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryDocumentTypesSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseDictionaryDocumentType
            {
                Id = x.Id,
                Name = x.Name,
                DirectionCodes = x.DirectionCodes,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
            }).ToList();
        }
        #endregion DictionaryDocumentSubjects

        #region DictionaryEventTypes
        public BaseDictionaryEventType GetDictionaryEventType(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryEventTypesSet.Where(x => x.Id == id).Select(x => new BaseDictionaryEventType
            {
                EventType = (EnumEventTypes)x.Id,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ImpotanceEventTypeId = x.ImpotanceEventTypeId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                ImpotanceEventTypeName = x.ImpotanceEventType.Name
            }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryEventType> GetDictionaryEventTypes(IContext context, FilterDictionaryEventType filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryEventTypesSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseDictionaryEventType
            {
                EventType = (EnumEventTypes)x.Id,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ImpotanceEventTypeId = x.ImpotanceEventTypeId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                ImpotanceEventTypeName = x.ImpotanceEventType.Name
            }).ToList();
        }
        #endregion DictionaryEventTypes

        #region DictionaryImpotanceEventTypes
        public BaseDictionaryImpotanceEventType GetDictionaryImpotanceEventType(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryImpotanceEventTypesSet.Where(x => x.Id == id)
                .Select(x => new BaseDictionaryImpotanceEventType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryImpotanceEventType> GetDictionaryImpotanceEventTypes(IContext context, FilterDictionaryImpotanceEventType filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryEventTypesSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseDictionaryImpotanceEventType
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate
            }).ToList();
        }
        #endregion DictionaryImpotanceEventTypes

        #region DictionaryLinkTypes
        public BaseDictionaryLinkType GetDictionaryLinkType(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryLinkTypesSet.Where(x => x.Id == id)
                .Select(x => new BaseDictionaryLinkType
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsImpotant = x.IsImpotant,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate
                }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryLinkType> GetDictionaryLinkTypes(IContext context, FilterDictionaryLinkType filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryLinkTypesSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseDictionaryLinkType
            {
                Id = x.Id,
                Name = x.Name,
                IsImpotant = x.IsImpotant,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate
            }).ToList();
        }
        #endregion DictionaryLinkTypes

        #region DictionaryPositions
        public BaseDictionaryPosition GetDictionaryPosition(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

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

        public IEnumerable<BaseDictionaryPosition> GetDictionaryPositions(IContext context, FilterDictionaryPosition filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryPositionsSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseDictionaryPosition
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
                ExecutorAgentName = x.ExecutorAgent.Name
            }).ToList();
        }
        #endregion DictionaryPositions

        #region DictionaryRegistrationJournals
        public BaseDictionaryRegistrationJournal GetDictionaryRegistrationJournal(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryRegistrationJournalsSet.Where(x => x.Id == id)
                .Select(x => new BaseDictionaryRegistrationJournal
                {
                    Id = x.Id,
                    Name = x.Name,
                    DepartmentId = x.DepartmentId,
                    Index = x.Index,
                    PrefixFormula = x.PrefixFormula,
                    SuffixFormula = x.SuffixFormula,
                    DirectionCodes = x.DirectionCodes,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    DepartmentName = x.Department.Name
                }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryRegistrationJournal> GetDictionaryRegistrationJournals(IContext context, FilterDictionaryRegistrationJournal filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryRegistrationJournalsSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
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
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                DepartmentName = x.Department.Name
            }).ToList();
        }
        #endregion DictionaryRegistrationJournals

        #region DictionaryResultTypes
        public BaseDictionaryResultType GetDictionaryResultType(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

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

        public IEnumerable<BaseDictionaryResultType> GetDictionaryResultTypes(IContext context, FilterDictionaryResultType filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryResultTypesSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseDictionaryResultType
            {
                Id = x.Id,
                Name = x.Name,
                IsExecute = x.IsExecute,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
            }).ToList();
        }
        #endregion DictionaryResultTypes

        #region DictionarySendTypes
        public BaseDictionarySendType GetDictionarySendType(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionarySendTypesSet.Where(x => x.Id == id)
                .Select(x => new BaseDictionarySendType
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsImpotant = x.IsImpotant,
                    SubordinationTypeId = x.SubordinationTypeId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    SubordinationTypeName = x.SubordinationType.Name
                }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionarySendType> GetDictionarySendTypes(IContext context, FilterDictionarySendType filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionarySendTypesSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseDictionarySendType
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                IsImpotant = x.IsImpotant,
                SubordinationTypeId = x.SubordinationTypeId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                SubordinationTypeName = x.SubordinationType.Name
            }).ToList();
        }
        #endregion DictionarySendTypes

        #region DictionaryStandartSendListContents
        public BaseDictionaryStandartSendListContent GetDictionaryStandartSendListContent(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

            return dbContext.DictionaryStandartSendListContentsSet.Where(x => x.Id == id)
                .Select(x => new BaseDictionaryStandartSendListContent
                {
                    Id = x.Id,
                    StandartSendListId = x.StandartSendListId,
                    OrderNumber = x.OrderNumber,
                    SendTypeId = x.SendTypeId,
                    TargetPositionId = x.TargetPositionId,
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevelId = x.AccessLevelId,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    SendTypeName = x.SendType.Name,
                    TargetPositionName = x.TargetPosition.Name,
                    AccessLevelName = x.AccessLevel.Name
                }).FirstOrDefault();
        }

        public IEnumerable<BaseDictionaryStandartSendListContent> GetDictionaryStandartSendListContents(IContext context, FilterDictionaryStandartSendListContent filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionaryStandartSendListContentsSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseDictionaryStandartSendListContent
            {
                Id = x.Id,
                StandartSendListId = x.StandartSendListId,
                OrderNumber = x.OrderNumber,
                SendTypeId = x.SendTypeId,
                TargetPositionId = x.TargetPositionId,
                Description = x.Description,
                DueDate = x.DueDate,
                DueDay = x.DueDay,
                AccessLevelId = x.AccessLevelId,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
                SendTypeName = x.SendType.Name,
                TargetPositionName = x.TargetPosition.Name,
                AccessLevelName = x.AccessLevel.Name
            }).ToList();
        }
        #endregion DictionaryStandartSendListContents

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
                })
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

        #region DictionarySubordinationTypes
        public BaseDictionarySubordinationType GetDictionarySubordinationType(IContext context, int id)
        {
            var dbContext = GetUserDmsContext(context);

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

        public IEnumerable<BaseDictionarySubordinationType> GetDictionarySubordinationTypes(IContext context, FilterDictionarySubordinationType filter)
        {
            var dbContext = GetUserDmsContext(context);
            var qry = dbContext.DictionarySendTypesSet.AsQueryable();

            if (filter.Id?.Count > 0)
            {
                qry = qry.Where(x => filter.Id.Contains(x.Id));
            }

            return qry.Select(x => new BaseDictionarySubordinationType
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                LastChangeUserId = x.LastChangeUserId,
                LastChangeDate = x.LastChangeDate,
            }).ToList();
        }
        #endregion DictionarySubordinationTypes
    }
}
