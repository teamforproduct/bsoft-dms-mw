using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BL.Logic.Common
{
    /// <summary>
    /// Класс содержит общие приемы работы со справочниками
    /// </summary>
    public static class CommonDictionaryUtilities
    {
        public static void DocumentSubjectModifyToInternal(IContext context, ModifyDictionaryDocumentSubject modifyModel, InternalDictionaryDocumentSubject internalModel)
        {
            internalModel.Id = modifyModel.Id;
            internalModel.IsActive = modifyModel.IsActive;
            internalModel.Name = modifyModel.Name;
            internalModel.ParentId = modifyModel.ParentId;

            CommonDocumentUtilities.SetLastChange(context, internalModel);
        }

        public static void RegistrationJournalModifyToInternal(IContext context, ModifyDictionaryRegistrationJournal modifyModel, InternalDictionaryRegistrationJournal internalModel)
        {
            // pss Перегонка значений DictionaryRegistrationJournals
            internalModel.Id = modifyModel.Id;
            internalModel.IsActive = modifyModel.IsActive;
            internalModel.Name = modifyModel.Name;
            internalModel.DepartmentId = modifyModel.DepartmentId;
            internalModel.Index = modifyModel.Index;
            internalModel.IsIncoming = modifyModel.IsIncoming;
            internalModel.IsOutcoming = modifyModel.IsOutcoming;
            internalModel.IsInternal = modifyModel.IsInternal;
            internalModel.PrefixFormula = modifyModel.PrefixFormula;
            internalModel.NumerationPrefixFormula = modifyModel.NumerationPrefixFormula;
            internalModel.SuffixFormula = modifyModel.SuffixFormula;

            CommonDocumentUtilities.SetLastChange(context, internalModel);
        }








    }
}
