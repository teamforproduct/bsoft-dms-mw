﻿using System;
using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore;
using BL.Database.Dictionaries.Interfaces;

namespace BL.Database.Dictionaries
{
    public class DictionariesDbProcess : CoreDb.CoreDb, IDictionariesDbProcess
    {
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

        public DictionaryDocumentEventTypes GetDocumentEventType(IContext context, DocumentEventTypes eventType)
        {
            var dbContext = GetUserDmsContext(context);
            return
                dbContext.DictionaryEventTypesSet.Where(x => x.Code == eventType.ToString())
                    .Select(x => new DictionaryDocumentEventTypes()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        ImpotanceEventTypeId = x.ImpotanceEventTypeId,
                        Name = x.Name,
                        EventType = (DocumentEventTypes) Enum.Parse(typeof (DocumentEventTypes), x.Code)
                    })
                    .FirstOrDefault();
        }

    }
}
