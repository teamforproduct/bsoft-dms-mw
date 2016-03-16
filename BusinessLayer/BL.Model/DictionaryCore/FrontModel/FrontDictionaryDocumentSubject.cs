using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// ОСНОВНОЙ. Справочник тематик документов. 
    /// </summary>
    // 
    public class FrontDictionaryDocumentSubject: ModifyDictionaryDocumentSubject
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Имя родителя
        /// </summary>
        public string ParentDocumentSubjectName { get; set; }


    }
}