using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Штатное расписание"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyDictionaryPosition
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
        /// Вышестоящая должность
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Полное наименование должности
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Подразделение, в которое включена должность
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public int? ExecutorAgentId { get; set; }


        public int? MainExecutorAgentId { get; set; }

        // !!! После добавления полей внеси изменения в BL.Logic.Common.CommonDictionaryUtilities.PositionModifyToInternal

    }
}
