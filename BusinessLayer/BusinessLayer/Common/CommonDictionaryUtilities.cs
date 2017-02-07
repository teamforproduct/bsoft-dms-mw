using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.CrossCutting.Interfaces;


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
