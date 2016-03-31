using System;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Результаты исполнения
    /// </summary>
    public class FrontDictionaryResultType
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Считать выполненным
        /// </summary>
        public bool IsExecute { get; set; }
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