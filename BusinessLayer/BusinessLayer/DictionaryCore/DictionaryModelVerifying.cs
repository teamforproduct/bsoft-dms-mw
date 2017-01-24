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




    }
}