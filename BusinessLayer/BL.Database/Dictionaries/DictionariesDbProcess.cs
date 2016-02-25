﻿using System.Collections.Generic;
using System.Linq;
using BL.Database.DatabaseContext;
using BL.Model.DictionaryCore;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.Enums;
using System;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.DBModel.Dictionary;
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
        public BaseDictionaryAgent GetDictionaryAgent(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                return dbContext.DictionaryAgentsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryAgent
                {
                    Id = x.Id,
                    Name = x.Name,
                    TaxCode = x.TaxCode,
                    IsIndividual = x.IsIndividual,
                    IsEmployee = x.IsEmployee,
                    LastChangeUserId = x.LastChangeUserId,
                    LastChangeDate = x.LastChangeDate,
                    AgentPersonsAgents = x.AgentPersonsAgents.Select(y => new BaseDictionaryAgentPerson
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
                    })
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
        }
        #endregion DictionaryAgents

        #region DictionaryAgentPersons
        public BaseDictionaryAgentPerson GetDictionaryAgentPerson(IContext context, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                return
                    dbContext.DictionaryAgentPersonsSet.Where(x => x.Id == id).Select(x => new BaseDictionaryAgentPerson
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
        }

        public IEnumerable<BaseDictionaryAgentPerson> GetDictionaryAgentPersons(IContext context, FilterDictionaryAgentPerson filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var qry = dbContext.DictionaryAgentPersonsSet.AsQueryable();

                if (filter.AgentPersonId?.Count > 0)
                {
                    qry = qry.Where(x => filter.AgentPersonId.Contains(x.Id));
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
        }
        #endregion DictionaryAgentPersons

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
                    Name = docType.Name
                };
                dbContext.DictionaryDocumentTypesSet.Attach(ddt);
                var entity = dbContext.Entry(ddt);
                entity.State = System.Data.Entity.EntityState.Modified; // you can set there if all field should be updated

                // otherwise you should set IsModified for each field which should be updated
                //entity.Property(x => x.Name).IsModified = true;
                //entity.Property(x => x.LastChangeDate).IsModified = true;
                //entity.Property(x => x.LastChangeUserId).IsModified = true;
                dbContext.SaveChanges();
            }
        }

        public int AddDictionaryDocumentType(IContext context, InternalDictionaryDocumentType docType)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var ddt = new DictionaryDocumentTypes
                {
                    Name = docType.Name,
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

                if (!String.IsNullOrEmpty (filter.Name))
                {
                    qry = qry.Where(x => filter.Name == x.Name);
                }

                return qry.Select(x => new InternalDictionaryDocumentType
                {
                    Id = x.Id,
                    Name = x.Name,
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

                return qry.Select(x => new FrontDictionaryDocumentType
                {
                    Id = x.Id,
                    Name = x.Name,
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
                var qry = dbContext.DictionaryPositionsSet.Select(x=> new { pos = x, subordMax = 0}).AsQueryable();

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
                                        dbContext.AdminSubordinationsSet.Where(y=> filter.SubordinatedPositions.Contains(y.SourcePositionId)),
                                        x => x.pos.Id,
                                        y => y.TargetPositionId,
                                        (x, y) => new { pos = x.pos, subordMax = y.Max(z=>z.SubordinationTypeId) }
                                        )
                             .Where(x=>x.subordMax>0);
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
                        Description = x.Description,
                        DueDate = x.DueDate,
                        DueDay = x.DueDay,
                        AccessLevelId = x.AccessLevelId,
                        LastChangeUserId = x.LastChangeUserId,
                        LastChangeDate = x.LastChangeDate,
                        SendTypeName = x.SendType.Name,
                        TargetPositionName = x.TargetPosition.Name,
                        TargetPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name,
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
                    Description = x.Description,
                    DueDate = x.DueDate,
                    DueDay = x.DueDay,
                    AccessLevelId = x.AccessLevelId,
                    SendTypeName = x.SendType.Name,
                    TargetPositionName = x.TargetPosition.Name,
                    TargetPositionExecutorAgentName = x.TargetPosition.ExecutorAgent.Name,
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
                                    Description = y.Description,
                                    DueDate = y.DueDate,
                                    DueDay = y.DueDay,
                                    AccessLevelId = y.AccessLevelId,
                                    SendTypeName = y.SendType.Name,
                                    TargetPositionName = y.TargetPosition.Name,
                                    TargetPositionExecutorAgentName = y.TargetPosition.ExecutorAgent.Name,
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

                if (filter.TagId?.Count>0)
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
    }
}
