using BL.CrossCutting.Interfaces;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Logic.DictionaryCore
{
    public static class DictionaryModelVerifying
    {
        public static void VerifyContactType(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryContactType Model)
        {
            Model.Code?.Trim();
            Model.Name?.Trim();

            var spr = dictDb.GetInternalDictionaryContactType(context, new FilterDictionaryContactType
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            });
            if (spr != null)
            { throw new DictionaryContactTypeNameNotUnique(Model.Name); }

            spr = dictDb.GetInternalDictionaryContactType(context, new FilterDictionaryContactType
            {
                CodeExact = Model.Code,
                NotContainsIDs = new List<int> { Model.Id }
            });
            if (spr != null)
            { throw new DictionaryContactTypeNameNotUnique(Model.Code); }
        }

        public static void VerifyAgentEmployee(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryAgentEmployee Model)
        {
            Model.Name?.Trim();
            Model.PersonnelNumber?.Trim();
            Model.PassportSerial?.Trim();
            Model.TaxCode?.Trim();

            // Обрезаю время для даты рождения и даты получения паспорта
            if (Model.PassportDate.HasValue) Model.PassportDate = Model.PassportDate?.Date;

            if (Model.BirthDate.HasValue) Model.BirthDate = Model.BirthDate?.Date;


            if (dictDb.ExistsAgents(context, new FilterDictionaryAgent()
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            {
                throw new DictionaryAgentNameNotUnique(Model.Name);
            }

            if (dictDb.ExistsAgentEmployees(context, new FilterDictionaryAgentEmployee()
            {
                PersonnelNumberExact = Model.PersonnelNumber,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            {
                throw new DictionaryAgentEmployeePersonnelNumberNotUnique(Model.PersonnelNumber);
            }

            // Если указаны необязательные паспортные данные, проверяю нет ли таких уже
            if (!string.IsNullOrEmpty(Model.PassportSerial + Model.PassportNumber))
            {
                if (dictDb.ExistsAgentPersons(context, new FilterDictionaryAgentPerson
                {
                    PassportSerialExact = Model.PassportSerial,
                    PassportNumberExact = Model.PassportNumber,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                {
                    throw new DictionaryAgentEmployeePassportNotUnique(Model.PassportSerial, Model.PassportNumber);
                }
            }

            // Если указан необязательный ИНН, проверяю нет ли такого уже
            if (!string.IsNullOrEmpty(Model.TaxCode))
            {
                if (dictDb.ExistsAgentPersons(context, new FilterDictionaryAgentPerson
                {
                    TaxCodeExact = Model.TaxCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                {
                    throw new DictionaryAgentEmployeeTaxCodeNotUnique(Model.TaxCode);
                }
            }
        }

        public static void VerifyCostomDictionary(IContext context, IDictionariesDbProcess dictDb, ModifyCustomDictionary Model)
        {
            Model.Code?.Trim();

            var cd = dictDb.GetInternalCustomDictionary(context, new FilterCustomDictionary
            {
                //pss Порверить почему IDs
                IDs = new List<int> { Model.DictionaryTypeId },
                CodeExact = Model.Code
            });
            if (cd != null && cd.Id != Model.Id)
            {
                throw new DictionaryCostomDictionaryNotUnique();
            }
        }

        public static void VerifyCostomDictionaryType(IContext context, IDictionariesDbProcess dictDb, ModifyCustomDictionaryType Model)
        {
            Model.Code?.Trim();

            var cdt = dictDb.GetInternalCustomDictionaryType(context, new FilterCustomDictionaryType
            {
                CodeExact = Model.Code
            });
            if (cdt != null && cdt.Id != Model.Id)
            {
                throw new DictionaryCostomDictionaryTypeNotUnique(Model.Code);
            }
        }

        public static void VerifyDepartment(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryDepartment Model)
        {
            Model.Name?.Trim();

            // отдел нельзя подчинить сасому себе и (дочерним отделам)
            if ((Model.ParentId ?? -1) == Model.Id)
            {
                throw new DictionarysdDepartmentNotBeSubordinated(Model.Name);
            }

            if (dictDb.ExistsDictionaryDepartment(context, new FilterDictionaryDepartment
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            { throw new DictionaryDepartmentNameNotUnique(Model.Name); }
        }

        public static void VerifyDocumentSubject(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryDocumentSubject Model)
        {
            Model.Name?.Trim();

            // Находим запись с таким-же именем в этой-же папке
            if (dictDb.ExistsDictionaryDocumentSubject(context, new FilterDictionaryDocumentSubject
            {
                NameExact = Model.Name,
                ParentIDs = new List<int> { Model.ParentId ?? -1 },
                NotContainsIDs = new List<int> { Model.Id }
            }))
            {
                throw new DictionaryDocumentSubjectNameNotUnique(Model.Name);
            }
        }

        public static void VerifyDocumentType(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryDocumentType Model)
        {
            Model.Name?.Trim();

            var spr = dictDb.GetInternalDictionaryDocumentType(context, new FilterDictionaryDocumentType
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            });
            if (spr != null)
            {
                throw new DictionaryDocumentTypeNameNotUnique(Model.Name);
            }
        }

        public static void VerifyPosition(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryPosition Model)
        {
            //var fdd = new FilterDictionaryPosition { Name = Model.Name, NotContainsIDs = new List<int> { Model.Id } };

            //if (Model.ParentId != null)
            //{
            //    fdd.ParentIDs = new List<int> { Model.ParentId.Value };
            //}

            //// Находим запись с таким-же именем в этой-же папке
            //if (_dictDb.ExistsPosition(_context, fdd))
            //{
            //    throw new DictionaryRecordNotUnique();
            //}
        }

        public static void VerifyRegistrationJournal(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryRegistrationJournal Model)
        {
            Model.Name?.Trim();

            // Находим запись с таким-же именем и индексом журнала в этом-же подразделении
            if (dictDb.ExistsDictionaryRegistrationJournal(context, new FilterDictionaryRegistrationJournal
            {
                NameExact = Model.Name,
                IndexExact = Model.Index,
                DepartmentIDs = new List<int> { Model.DepartmentId },
                NotContainsIDs = new List<int> { Model.Id }
            }))
            {
                throw new DictionaryRegistrationJournalNotUnique(Model.Name);
            }
        }

        public static void VerifyStandartSendList(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryStandartSendList Model)
        {
            Model.Name?.Trim();

            var contents = dictDb.GetStandartSendLists(context, new FilterDictionaryStandartSendList()
            {
                NameExact = Model.Name,
                PositionID = Model.PositionId ?? 0,
                NotContainsIDs = new List<int> { Model.Id }
            });

            if (contents.Any())
            {
                throw new DictionaryStandartSendListNotUnique(Model.Name);
            }
        }

        public static void VerifyStandartSendListContent(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryStandartSendListContent Model)
        {
            var contents = dictDb.GetStandartSendListContents(context, new FilterDictionaryStandartSendListContent()
            {
                StandartSendListId = new List<int> { Model.StandartSendListId },
                TargetAgentId = Model.TargetAgentId,
                TargetPositionId = Model.TargetPositionId,
                Task = Model.Task,
                SendTypeId = new List<EnumSendTypes> { Model.SendTypeId },
                NotContainsIDs = new List<int> { Model.Id }
            });

            //StandartSendListId = new List<int> { Model.StandartSendListId },
            //    TargetAgentId = Model.TargetAgentId,
            //    TargetPositionId = Model.TargetPositionId,
            //    SendTypeId = new List<EnumSendTypes> { Model.SendTypeId }

            if (contents.Any())
            {
                throw new DictionaryStandartSendListContentNotUnique(Model.Task);
            }
        }
        public static void VerifyTag(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryTag Model)
        {
            Model.Name?.Trim();

            var spr = dictDb.GetTags(context, new FilterDictionaryTag
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            });
            if (spr.Any())
            {
                throw new DictionaryTagNotUnique(Model.Name);
            }
        }
    }
}