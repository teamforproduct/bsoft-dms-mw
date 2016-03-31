using System.Collections.Generic;
using System.Linq;
using BL.Model.DictionaryCore.InternalModel;
using BL.Database.DBModel.Dictionary;
using BL.Model.Enums;

namespace BL.Database.Common
{
    public static class DictionaryModelConverter
    {

        public static DictionaryDepartments GetDbDepartments(InternalDictionaryDepartment item)
        {
            return item == null ? null : new DictionaryDepartments
            {
                Id = item.Id,
                ParentId = item.ParentId,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive,
                Name = item.Name,
                FullName = item.FullName,
                CompanyId = item.CompanyId,
                Code = item.Code,
                ChiefPositionId = item.ChiefPositionId
            };
        }

        public static DictionaryDocumentSubjects GetDbDocumentSubject(InternalDictionaryDocumentSubject item)
        {
            return item == null ? null : new DictionaryDocumentSubjects
            {
                Id = item.Id,
                ParentId = item.ParentId,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive,
                Name = item.Name
            };
        }

        public static DictionaryRegistrationJournals GetDbRegistrationJournal(InternalDictionaryRegistrationJournal item)
        {
            return item == null ? null : new DictionaryRegistrationJournals
            {
                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive,
                Name = item.Name,
                DepartmentId = item.DepartmentId,
                Index = item.Index,
                DirectionCodes =
                    (item.IsIncoming ? EnumDocumentDirections.Incoming.ToString() : "") +
                    (item.IsOutcoming ? EnumDocumentDirections.Outcoming.ToString() : "") +
                    (item.IsInternal ? EnumDocumentDirections.Internal.ToString() : ""),
                PrefixFormula = item.PrefixFormula,
                NumerationPrefixFormula = item.NumerationPrefixFormula,
                SuffixFormula = item.SuffixFormula,
            };
        }

        public static DictionaryDocumentTypes GetDbDocumentType(InternalDictionaryDocumentType item)
        {
            return item == null ? null : new DictionaryDocumentTypes
            {
                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                Name = item.Name,
                IsActive = item.IsActive
            };
        }

        public static CustomDictionaries GetDbCustomDictionary(InternalCustomDictionary item)
        {
            return item == null ? null : new CustomDictionaries
            {
                Id = item.Id,
                Code = item.Code,
                Description = item.Description,
                DictionaryTypeId = item.DictionaryTypeId,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
            };
        }

        public static CustomDictionaryTypes GetDbCustomDictionaryType(InternalCustomDictionaryType item)
        {
            return item == null ? null : new CustomDictionaryTypes
            {
                Id = item.Id,
                Code = item.Code,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
            };
        }

        public static DictionaryStandartSendListContents GetDbStandartSendListContent(InternalDictionaryStandartSendListContent item)
        {
            return item == null ? null : new DictionaryStandartSendListContents
            {
                Id = item.Id,
                AccessLevelId = (int)item.AccessLevel,
                TargetPositionId = item.TargetPositionId,
                TargetAgentId = item.TargetAgentId,
                DueDate = item.DueDate,
                DueDay = item.DueDay,
                SendTypeId = (int)item.SendType,
                Stage = item.Stage,
                StandartSendListId = item.StandartSendListId,
                Task = item.Task,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId
            };
        }

        public static DictionaryPositions GetDbDictionaryPosition(InternalDictionaryPosition item)
        {
            return item == null ? null : new DictionaryPositions
            {
                Id = item.Id,
                ParentId = item.ParentId,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive,
                Name = item.Name,
                FullName = item.FullName,
                DepartmentId = item.DepartmentId,
                ExecutorAgentId = item.ExecutorAgentId,
                MainExecutorAgentId = item.MainExecutorAgentId
            };
        }


        public static DictionaryCompanies GetDbCompany(InternalDictionaryCompany item)
        {
            return item == null ? null : new DictionaryCompanies
            {
                Id = item.Id,
                Name = item.Name,
                IsActive = item.IsActive,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId
            };
        }

        public static DictionaryPositionExecutors GetDbExecutor(InternalDictionaryPositionExecutor item)
        {
            return item == null ? null : new DictionaryPositionExecutors
            {
                Id = item.Id,
                Description = item.Description,
                IsActive = item.IsActive,
                AgentId = item.AgentId,
                PositionId = item.PositionId,
                PositionExecutorTypeId = item.PositionExecutorTypeId,
                AccessLevelId = item.AccessLevelId,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId
            };

        }
    }
}