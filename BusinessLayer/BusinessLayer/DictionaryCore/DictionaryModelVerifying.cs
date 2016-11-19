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

            //if (dictDb.ExistsContactTypeSpecCode(context, Model.Id)) throw new ();

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


        public static void VerifyCostomDictionary(IContext context, IDictionariesDbProcess dictDb, ModifyCustomDictionary Model)
        {
            Model.Code?.Trim();

            var cd = dictDb.GetInternalCustomDictionarys(context, new FilterCustomDictionary
            {
                //pss Порверить почему IDs
                IDs = new List<int> { Model.DictionaryTypeId },
                CodeExact = Model.Code
            }).FirstOrDefault();
            if (cd != null && cd.Id != Model.Id)
            {
                throw new DictionaryCostomDictionaryNotUnique();
            }
        }

        public static void VerifyCostomDictionaryType(IContext context, IDictionariesDbProcess dictDb, ModifyCustomDictionaryType Model)
        {
            Model.Code?.Trim();

            var cdt = dictDb.GetInternalCustomDictionaryTypes(context, new FilterCustomDictionaryType
            {
                CodeExact = Model.Code
            }).FirstOrDefault();
            if (cdt != null && cdt.Id != Model.Id)
            {
                throw new DictionaryCostomDictionaryTypeNotUnique(Model.Code);
            }
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

            var spr = dictDb.GetInternalDictionaryDocumentTypes(context, new FilterDictionaryDocumentType
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> { Model.Id }
            }).FirstOrDefault();
            if (spr != null)
            {
                throw new DictionaryDocumentTypeNameNotUnique(Model.Name);
            }
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