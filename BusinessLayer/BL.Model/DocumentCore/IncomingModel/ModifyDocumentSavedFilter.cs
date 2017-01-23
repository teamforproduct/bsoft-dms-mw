using System.Runtime.Serialization;
using BL.Model.Users;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/изменения записи сохраненного фильтра для документом
    /// </summary>
    public class ModifyDocumentSavedFilter : AddDocumentSavedFilter
    {
        /// <summary>
        /// ИД записи сохраненного фильтра
        /// </summary>      
        public int Id { get; set; }
    }
}
