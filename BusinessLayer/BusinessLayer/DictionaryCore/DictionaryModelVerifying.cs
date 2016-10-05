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

        public static void VerifyAddressType(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryAddressType Model)
        {
            Model.Code?.Trim();
            Model.Name?.Trim();

            var spr = dictDb.GetInternalDictionaryAddressType(context, new FilterDictionaryAddressType
            {
                CodeExact = Model.Code,
                NotContainsIDs = new List<int> { Model.Id }
            });
            if (spr != null)
            {
                throw new DictionaryAddressTypeCodeNotUnique(Model.Code);
            }

            spr = dictDb.GetInternalDictionaryAddressType(context, new FilterDictionaryAddressType
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            });
            if (spr != null)
            {
                throw new DictionaryAddressTypeNameNotUnique(Model.Name);
            }

        }

        public static void VerifyAgent(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryAgent Model)
        {
            Model.Name?.Trim();

            if (dictDb.ExistsAgents(context, new FilterDictionaryAgent()
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            {
                throw new DictionaryAgentNameNotUnique(Model.Name);
            }
        }

        public static void VerifyAgentAddress(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryAgentAddress Model)
        {
            Model.PostCode?.Trim();

            var spr = dictDb.GetAgentAddresses(context, Model.AgentId, new FilterDictionaryAgentAddress
            {
                AgentId = Model.AgentId,
                NotContainsIDs = new List<int> { Model.Id },
                AddressTypeId = new List<int> { Model.AddressTypeId },
            });
            if (spr.Count() != 0)
            {
                throw new DictionaryAgentAddressTypeNotUnique();
            }

            spr = dictDb.GetAgentAddresses(context, Model.AgentId, new FilterDictionaryAgentAddress
            {
                AgentId = Model.AgentId,
                NotContainsIDs = new List<int> { Model.Id },
                PostCodeExact = Model.PostCode,
                AddressExact = Model.Address,
            });
            if (spr.Count() != 0)
            {
                throw new DictionaryAgentAddressNameNotUnique(Model.PostCode, Model.Address);
            }

        }

        public static void VerifyAgentBank(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryAgentBank Model)
        {
            Model.Name?.Trim();
            Model.MFOCode?.Trim();

            if (dictDb.ExistsAgents(context, new FilterDictionaryAgent()
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            {
                throw new DictionaryAgentNameNotUnique(Model.Name);
            }

            if (!string.IsNullOrEmpty(Model.MFOCode))
            {
                if (dictDb.ExistsAgentBanks(context, new FilterDictionaryAgentBank
                {
                    MFOCodeExact = Model.MFOCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                {
                    throw new DictionaryAgentBankMFOCodeNotUnique(Model.Name, Model.MFOCode);
                }
            }
        }

        public static void VerifyAgentClientCompany(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryAgentClientCompany Model)
        {
            Model.Name?.Trim();

            if (dictDb.ExistsAgents(context, new FilterDictionaryAgent()
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            {
                throw new DictionaryAgentNameNotUnique(Model.Name);
            }
        }

        public static void VerifyAgentCompany(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryAgentCompany Model)
        {
            Model.Name?.Trim();
            Model.TaxCode?.Trim();
            Model.OKPOCode?.Trim();
            Model.VATCode?.Trim();

            if (dictDb.ExistsAgents(context, new FilterDictionaryAgent()
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            { throw new DictionaryAgentNameNotUnique(Model.Name); }

            if (!string.IsNullOrEmpty(Model.TaxCode))
            {
                if (dictDb.ExistsAgentCompanies(context, new FilterDictionaryAgentCompany()
                {
                    TaxCodeExact = Model.TaxCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentCompanyTaxCodeNotUnique(Model.Name, Model.TaxCode); }
            }

            if (!string.IsNullOrEmpty(Model.OKPOCode))
            {
                if (dictDb.ExistsAgentCompanies(context, new FilterDictionaryAgentCompany()
                {
                    OKPOCodeExact = Model.OKPOCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentCompanyOKPOCodeNotUnique(Model.Name, Model.OKPOCode); }
            }

            if (!string.IsNullOrEmpty(Model.VATCode))
            {
                if (dictDb.ExistsAgentCompanies(context, new FilterDictionaryAgentCompany()
                {
                    VATCodeExact = Model.VATCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentCompanyVATCodeNotUnique(Model.Name, Model.VATCode); }
            }
        }

        public static void VerifyAgentContact(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryContact Model)
        {
            Model.Value?.Trim();

            // У одного агента не должно быть два контакта одинакового типа
            var spr = dictDb.GetContacts(context, Model.AgentId,
                   new FilterDictionaryContact
                   {
                       NotContainsIDs = new List<int> { Model.Id },
                       ContactTypeIDs = new List<int> { Model.ContactTypeId },
                       AgentIDs = new List<int> { Model.AgentId }
                   });

            if (spr.Count() != 0) throw new DictionaryAgentContactTypeNotUnique(Model.AgentId.ToString(), Model.Value);

            // У одного агента не должно быть два контакта с одинаковыми значениями
            spr = dictDb.GetContacts(context, Model.AgentId,
                   new FilterDictionaryContact
                   {
                       NotContainsIDs = new List<int> { Model.Id },
                       ContactExact = Model.Value,
                       AgentIDs = new List<int> { Model.AgentId }
                   });

            if (spr.Count() != 0) throw new DictionaryAgentContactNotUnique(Model.Value);
        }

        public static void VerifyAgentEmployee(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryAgentEmployee Model)
        {
            Model.Name?.Trim();
            Model.PersonnelNumber?.Trim();
            Model.PassportSerial?.Trim();
            Model.TaxCode?.Trim();

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

        public static void VerifyAgentPerson(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryAgentPerson Model)
        {
            Model.Name?.Trim();
            Model.PassportSerial?.Trim();
            Model.TaxCode?.Trim();

            if (dictDb.ExistsAgents(context, new FilterDictionaryAgent
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }))
            {
                throw new DictionaryAgentNameNotUnique(Model.Name);
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
                { throw new DictionaryAgentPersonPassportNotUnique(Model.PassportSerial, Model.PassportNumber); }
            }

            // Если указан необязательный ИНН, проверяю нет ли такого уже
            if (!string.IsNullOrEmpty(Model.TaxCode))
            {
                if (dictDb.ExistsAgentPersons(context, new FilterDictionaryAgentPerson
                {
                    TaxCodeExact = Model.TaxCode,
                    NotContainsIDs = new List<int> { Model.Id }
                }))
                { throw new DictionaryAgentPersonTaxCodeNotUnique(Model.TaxCode); }
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

        public static void VerifyPositionExecutor(IContext context, IDictionariesDbProcess dictDb, ModifyDictionaryPositionExecutor Model)
        {

            FrontDictionaryPositionExecutor executor = null;

            executor = dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor
            {
                NotContainsIDs = new List<int> { Model.Id },
                PositionIDs = new List<int> { Model.PositionId },
                Period = new Period(Model.StartDate, Model.EndDate),
                AgentIDs = new List<int> { Model.AgentId },
            }).FirstOrDefault();

            if (executor != null)
            { throw new DictionaryPositionExecutorNotUnique(executor.PositionName, executor.AgentName, executor.StartDate, executor.EndDate); }


            switch (Model.PositionExecutorTypeId)
            {
                case EnumPositionExecutionTypes.Personal:
                    // Personal может быть только один на должности за период
                    executor = dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor
                    {
                        NotContainsIDs = new List<int> { Model.Id },
                        PositionIDs = new List<int> { Model.PositionId },
                        Period = new Period(Model.StartDate, Model.EndDate),
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { (EnumPositionExecutionTypes)Model.PositionExecutorTypeId },
                    }).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorPersonalNotUnique(executor.PositionName, executor.AgentName, executor.StartDate, executor.EndDate); }
                    break;
                case EnumPositionExecutionTypes.IO:
                    // IO может быть только один на должности за период
                    executor = dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor
                    {
                        NotContainsIDs = new List<int> { Model.Id },
                        PositionIDs = new List<int> { Model.PositionId },
                        Period = new Period(Model.StartDate, Model.EndDate),
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { (EnumPositionExecutionTypes)Model.PositionExecutorTypeId },
                    }).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorIONotUnique(executor.PositionName, executor.AgentName, executor.StartDate, executor.EndDate); }
                    break;
                case EnumPositionExecutionTypes.Referent:
                    // Референтов может быть несколько может быть н на должности за период
                    executor = dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor
                    {
                        NotContainsIDs = new List<int> { Model.Id },
                        PositionIDs = new List<int> { Model.PositionId },
                        Period = new Period(Model.StartDate, Model.EndDate),
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { (EnumPositionExecutionTypes)Model.PositionExecutorTypeId },
                        AgentIDs = new List<int> { Model.AgentId },
                    }).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorReferentNotUnique(executor.PositionName, executor.AgentName, executor.StartDate, executor.EndDate); }

                    break;
                default:
                    executor = dictDb.GetPositionExecutors(context, new FilterDictionaryPositionExecutor
                    {
                        NotContainsIDs = new List<int> { Model.Id },
                        PositionIDs = new List<int> { Model.PositionId },
                        Period = new Period(Model.StartDate, Model.EndDate),
                        PositionExecutorTypeIDs = new List<EnumPositionExecutionTypes> { (EnumPositionExecutionTypes)Model.PositionExecutorTypeId },
                        AgentIDs = new List<int> { Model.AgentId },
                    }).FirstOrDefault();

                    if (executor != null)
                    { throw new DictionaryPositionExecutorReferentNotUnique(executor.PositionName, executor.AgentName, executor.StartDate, executor.EndDate); }

                    break;
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