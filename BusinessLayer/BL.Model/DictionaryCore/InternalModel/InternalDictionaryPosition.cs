using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Internal элемент справочника "Штатное расписание"
    /// </summary>
    public class InternalDictionaryPosition : LastChangeInfo
    {

        public InternalDictionaryPosition()
        { }

        public InternalDictionaryPosition(ModifyDictionaryPosition model)
        {
            Id = model.Id;
            IsActive = model.IsActive;
            ParentId = model.ParentId;
            Name = model.Name;
            FullName = model.FullName;
            DepartmentId = model.DepartmentId;
            ExecutorAgentId = model.ExecutorAgentId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Признак активности
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
        /// Подразделение, в которое включена эта должность
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Исполняющий обязанности, значения проставляются вертушкой
        /// </summary>
        public int? ExecutorAgentId { get; set; }

        /// <summary>
        /// Сотрудник на должности, значения проставляются вертушкой
        /// </summary>
        public int? MainExecutorAgentId { get; set; }

        // !!! После добавления полей внеси изменения в BL.Logic.Common.CommonDictionaryUtilities.PositionModifyToInternal
    }
}