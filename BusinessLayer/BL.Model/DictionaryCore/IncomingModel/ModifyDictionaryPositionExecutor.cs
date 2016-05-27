using System;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Исполнители"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyDictionaryPositionExecutor
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        
        /// <summary>
        /// Признак активности.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Агент
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Тип исполнителя
        /// </summary>
        public int PositionExecutorTypeId { get; set; }

        /// <summary>
        /// Уровень доступа: лично, референт, ио
        /// </summary>
        public int AccessLevelId { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата начала исполнения должности
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания исполнения должности
        /// </summary>
        public DateTime EndDate { get; set; }

    }
}
