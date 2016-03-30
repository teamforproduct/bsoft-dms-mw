using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Internal элемент справочника "Структура предприятия"
    /// </summary>
    public class InternalDictionaryDepartment : LastChangeInfo
    {

        public InternalDictionaryDepartment()
        { }

        public InternalDictionaryDepartment(ModifyDictionaryDepartment model)
        {
            Id = model.Id;
            IsActive = model.IsActive;
            ParentId = model.ParentId;
            Name = model.Name;
            FullName = model.FullName;
            Code = model.Code;
            CompanyId = model.CompanyId;
            ChiefPositionId = model.ChiefPositionId;
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
        /// Вышестоящее подразделение
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Полное наименование подразделения
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Индекс подразделения
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Компания
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Руководитель подразделения
        /// </summary>
        public int? ChiefPositionId { get; set; }

        // !!! После добавления полей внеси изменения в BL.Logic.Common.CommonDictionaryUtilities.DepartmentModifyToInternal

    }
}