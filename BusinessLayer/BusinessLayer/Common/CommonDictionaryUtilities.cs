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
        public static InternalDictionaryDocumentSubject DocumentSubjectModifyToInternal(IContext context, ModifyDictionaryDocumentSubject modifyModel)
        {
            InternalDictionaryDocumentSubject internalModel = new InternalDictionaryDocumentSubject(modifyModel);

            CommonDocumentUtilities.SetLastChange(context, internalModel);

            return internalModel;
        }

        public static InternalDictionaryRegistrationJournal RegistrationJournalModifyToInternal(IContext context, ModifyDictionaryRegistrationJournal modifyModel)
        {
            InternalDictionaryRegistrationJournal internalModel = new InternalDictionaryRegistrationJournal(modifyModel);

            CommonDocumentUtilities.SetLastChange(context, internalModel);

            return internalModel;
        }

        public static InternalDictionaryDepartment DepartmentModifyToInternal(IContext context, ModifyDictionaryDepartment modifyModel)
        {
            InternalDictionaryDepartment internalModel = new InternalDictionaryDepartment(modifyModel);

            CommonDocumentUtilities.SetLastChange(context, internalModel);

            return internalModel;
        }

        public static InternalDictionaryPosition PositionModifyToInternal(IContext context, ModifyDictionaryPosition modifyModel)
        {

            InternalDictionaryPosition internalModel = new InternalDictionaryPosition(modifyModel);

            CommonDocumentUtilities.SetLastChange(context, internalModel);

            return internalModel;

        }






    }
}
