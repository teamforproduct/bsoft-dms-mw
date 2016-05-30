using System.Collections.Generic;
using System.Linq;
using BL.Model.DictionaryCore.InternalModel;
using BL.Database.DBModel.Dictionary;
using BL.Model.Enums;
using BL.CrossCutting.Interfaces;

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

        public static DictionaryDocumentSubjects GetDbDocumentSubject(IContext context, InternalDictionaryDocumentSubject item)
        {
            return item == null ? null : new DictionaryDocumentSubjects
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                ParentId = item.ParentId,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive,
                Name = item.Name
            };
        }

        public static DictionaryRegistrationJournals GetDbRegistrationJournal(IContext context, InternalDictionaryRegistrationJournal item)
        {
            return item == null ? null : new DictionaryRegistrationJournals
            {
                ClientId = context.CurrentClientId,

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

        public static DictionaryDocumentTypes GetDbDocumentType(IContext context, InternalDictionaryDocumentType item)
        {
            return item == null ? null : new DictionaryDocumentTypes
            {
                ClientId = context.CurrentClientId,

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

        public static CustomDictionaryTypes GetDbCustomDictionaryType(IContext context, InternalCustomDictionaryType item)
        {
            return item == null ? null : new CustomDictionaryTypes
            {
                ClientId = context.CurrentClientId,

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


        public static DictionaryCompanies GetDbCompany(IContext context, InternalDictionaryCompany item)
        {
            return item == null ? null : new DictionaryCompanies
            {
                ClientId = context.CurrentClientId,
                
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

        public static DictionaryAgents GetDbAgent(IContext context, InternalDictionaryAgent item)
        {
            return item == null ? null : new DictionaryAgents
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                Name = item.Name,
                ResidentTypeId = item.ResidentTypeId,
                IsBank = item.IsBank,
                IsCompany = item.IsCompany,
                IsEmployee = item.IsEmployee,
                IsIndividual = item.IsIndividual,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive
            };

        }

        public static DictionaryAgentPersons GetDbAgentPerson(InternalDictionaryAgentPerson item)
        {
            return item == null ? null : new DictionaryAgentPersons
            {

                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                MiddleName = item.MiddleName,
                FullName = item.LastName + " " + item.FirstName + " " + item.MiddleName,
                TaxCode = item.TaxCode,
                IsMale = item.IsMale,
                PassportSerial = item.PassportSerial,
                PassportNumber = item.PassportNumber,
                PassportText = item.PassportText,
                PassportDate = item.PassportDate,
                BirthDate = item.BirthDate,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive

            };

        }
        public static DictionaryAgentAddresses GetDbAgentAddress(InternalDictionaryAgentAddress item)
        {
            return item == null ? null : new DictionaryAgentAddresses
            {
                Id = item.Id,
                AgentId = item.AgentId,
                AdressTypeId = item.AddressTypeID,
                PostCode = item.PostCode,
                Address = item.Address,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive
            };
        }

        public static DictionaryAddressTypes GetDbAddressType(IContext context, InternalDictionaryAddressType item)
        {
            return item == null ? null : new DictionaryAddressTypes
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                Name = item.Name,
                IsActive = item.IsActive
            };
        }

        public static DictionaryAgentCompanies GetDbAgentCompany(InternalDictionaryAgentCompany item)
        {
            return item == null ? null : new DictionaryAgentCompanies
            {
                Id = item.Id,
                FullName = item.FullName,
                OKPOCode = item.OKPOCode,
                VATCode = item.VATCode,
                TaxCode = item.TaxCode,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive
            };
        }

        public static DictionaryAgentBanks GetDbAgentBank(InternalDictionaryAgentBank item)
        {
            return item == null ? null : new DictionaryAgentBanks
            {
                Id = item.Id,
                MFOCode = item.MFOCode,
                Swift = item.Swift,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive
            };
        }

        public static DictionaryAgentAccounts GetDbAgentAccount(InternalDictionaryAgentAccount item)
        {
            return item == null ? null : new DictionaryAgentAccounts
            {
                Id = item.Id,
                AccountNumber = item.AccountNumber,
                AgentId = item.AgentId,
                AgentBankId = item.AgentBankId,
                IsMain = item.IsMain,
                Name = item.Name,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive
            };
        }


        public static DictionaryContactTypes GetDbContactType(IContext context, InternalDictionaryContactType item)
        {
            return item == null ? null : new DictionaryContactTypes
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                Name = item.Name,
                IsActive = item.IsActive
            };
        }

        public static DictionaryAgentContacts GetDbContact(InternalDictionaryContact item)
        {
            return item == null ? null : new DictionaryAgentContacts
            {
                Id = item.Id,
                AgentId = item.AgentId,
                ContactTypeId = item.ContactTypeId,
                Contact = item.Value,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive
            };
        }




    }
}