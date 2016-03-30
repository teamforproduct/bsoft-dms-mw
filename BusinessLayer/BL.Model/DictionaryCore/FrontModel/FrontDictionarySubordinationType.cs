using System;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Типы субординации
    /// </summary>
    public class FrontDictionarySubordinationType
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime LastChangeDate { get; set; }
    }
}