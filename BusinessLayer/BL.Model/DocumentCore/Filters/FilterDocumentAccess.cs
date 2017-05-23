using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр доступов документа
    /// </summary>
    public class FilterDocumentAccess
    {
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> DocumentId { get; set; }

        /// <summary>
        /// Массив ИД уровней доступа по документу
        /// </summary>
        public List<int> AccessLevelId { get; set; }

        /// <summary>
        /// Массив ИД должностей
        /// </summary>
        public List<int> AccessPositionId { get; set; }

        /// <summary>
        /// Признак в работе
        /// </summary>
        public bool? IsInWork { get; set; } // should be true by default

        /// <summary>
        /// Признак фаварита
        /// </summary>
        public bool? IsFavourite { get; set; } // should be true by default
                                               /// <summary>
                                               /// Признак того, что доступ исполнителя по документу. true - выполнять поиск иначе ничего не делаеться
                                               /// </summary>
        public bool? IsExecutorPosition { get; set; }

    }
}
