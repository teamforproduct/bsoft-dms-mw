using System.Runtime.Serialization;
using BL.Model.Users;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления записи сохраненного фильтра для документом
    /// </summary>
    public class AddDocumentSavedFilter //: CurrentPosition
    {
        /// <summary>
        /// Имя фильтра
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Иконка фильтра
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Содержание фильтра
        /// </summary>
        public object Filter { get; set; }
        /// <summary>
        /// Признак является ли фильтр общим
        /// </summary>
        public bool IsCommon { get; set; }
    }
}
