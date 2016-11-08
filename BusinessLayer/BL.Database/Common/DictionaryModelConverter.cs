﻿using System.Collections.Generic;
using System.Linq;
using BL.Model.DictionaryCore.InternalModel;
using BL.Database.DBModel.Dictionary;
using BL.Model.Enums;
using BL.CrossCutting.Interfaces;
using System;

namespace BL.Database.Common
{
    public static class DictionaryModelConverter
    {

        public static DictionaryDepartments GetDbDepartments(IContext context, InternalDictionaryDepartment item)
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
                Code = item.Index,
                FullPath = item.Code,
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

        public static CustomDictionaries GetDbCustomDictionary(IContext context, InternalCustomDictionary item)
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
        public static DictionaryStandartSendLists GetDbStandartSendList(IContext context, InternalDictionaryStandartSendList item)
        {
            return item == null ? null : new DictionaryStandartSendLists
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                Name = item.Name,
                PositionId = item.PositionId,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId
            };
        }
        public static DictionaryStandartSendListContents GetDbStandartSendListContent(IContext context, InternalDictionaryStandartSendListContent item)
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

        public static DictionaryPositions GetDbPosition(IContext context, InternalDictionaryPosition item)
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
                //ExecutorAgentId = item.ExecutorAgentId,
                //MainExecutorAgentId = item.MainExecutorAgentId,
                Order = item.Order
            };
        }


        public static DictionaryCompanies GetDbAgentClientCompany(IContext context, InternalDictionaryAgentClientCompany item)
        {
            return item == null ? null : new DictionaryCompanies
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                FullName = item.FullName,
                Description = item.Description,
                IsActive = item.IsActive,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId
            };
        }

        public static DictionaryTags GetDbTag(IContext context, InternalDictionaryTag item)
        {
            return item == null ? null : new DictionaryTags
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                Name = item.Name,
                PositionId = null,
                IsActive = item.IsActive,
                Color = item.Color,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId
            };
        }

        public static DictionaryPositionExecutors GetDbExecutor(IContext context, InternalDictionaryPositionExecutor item)
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
                EndDate = item.EndDate ?? DateTime.MaxValue,
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
                Description = string.Empty, // item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = true// item.IsActive
            };

        }

        public static DictionaryAgentPersons GetDbAgentPerson(IContext context, InternalDictionaryAgentPerson item)
        {
            return item == null ? null : new DictionaryAgentPersons
            {
                ClientId = context.CurrentClientId,

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

        public static DictionaryAgentEmployees GetDbAgentEmployee(IContext context, InternalDictionaryAgentEmployee item)
        {
            return item == null ? null : new DictionaryAgentEmployees
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                PersonnelNumber = item.PersonnelNumber,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive
            };

        }

        public static DictionaryAgentUsers GetDbAgentUser(IContext context, InternalDictionaryAgentUser item)
        {
            return item == null ? null : new DictionaryAgentUsers
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                UserId = item.UserId,
                LanguageId = item.LanguageId,
                IsActive = item.IsActive,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
            };

        }

        public static DictionaryAgents GetDbAgentImage(IContext context, InternalDictionaryAgentImage item)
        {
            return item == null ? null : new DictionaryAgents
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                
                Image = item.Image,

                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
            };

        }



        public static DictionaryAgentAddresses GetDbAgentAddress(IContext context, InternalDictionaryAgentAddress item)
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
                Code = item.Code,
                Name = item.Name,
                IsActive = item.IsActive
            };
        }

        public static DictionaryAgentCompanies GetDbAgentCompany(IContext context, InternalDictionaryAgentCompany item)
        {
            return item == null ? null : new DictionaryAgentCompanies
            {
                ClientId = context.CurrentClientId,

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

        public static DictionaryAgentBanks GetDbAgentBank(IContext context, InternalDictionaryAgentBank item)
        {
            return item == null ? null : new DictionaryAgentBanks
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                FullName = item.FullName,
                MFOCode = item.MFOCode,
                Swift = item.Swift,
                Description = item.Description,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                IsActive = item.IsActive
            };
        }

        public static DictionaryAgentAccounts GetDbAgentAccount(IContext context, InternalDictionaryAgentAccount item)
        {
            return item == null ? null : new DictionaryAgentAccounts
            {
                Id = item.Id,
                AccountNumber = item.AccountNumber,
                AgentId = item.AgentId,
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
                InputMask = item.InputMask,
                Name = item.Name,
                Code = item.Code,
                SpecCode = item.SpecCode,
                IsActive = item.IsActive,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId
            };
        }

        public static DictionaryAgentContacts GetDbContact(IContext context, InternalDictionaryContact item)
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
                IsActive = item.IsActive,
                IsConfirmed = item.IsConfirmed
            };
        }




    }
}