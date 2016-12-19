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



    }
}
