﻿using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryPosition
    /// </summary>
    public class FilterDictionaryPosition : BaseFilterNameIsActive
    {
 
        /// <summary>
        /// Сужение по полному наименованию подразделений
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// По отделам
        /// </summary>
        public List<int> DepartmentIDs { get; set; }

        /// <summary>
        /// По компаниям
        /// </summary>
        public List<int> CompanyIDs { get; set; }

        /// <summary>
        /// Массив ИД документов для поиска корреспондентов в событиях
        /// </summary>
        public List<int> DocumentIDs { get; set; }
        /// <summary>
        /// Массив ИД должностей для проверки субординации
        /// </summary>
        public List<int> SubordinatedPositions { get; set; }

        /// <summary>
        /// Тип субординации
        /// </summary>
        public int? SubordinatedTypeId { get; set; }

        /// <summary>
        /// Должности, которым назначены указанные роли
        /// </summary>
        public List<int> RoleIDs { get; set; }

        /// <summary>
        ///  Ниже по списку
        /// </summary>
        public int? OrderMore { get; set; }

        /// <summary>
        /// Выше по списку
        /// </summary>
        public int? OrderLess { get; set; }

        /// <summary>
        /// Номер в списке
        /// </summary>
        public List<int> Orders { get; set; }

        /// <summary>
        /// Сужение по списку вышестоящих элементов
        /// </summary>
        public List<int> ParentIDs { get; set; }

        /// <summary>
        /// Не показывать вакантные должности
        /// </summary>
        public bool? IsHideVacated { get; set; }

        /// <summary>
        /// Сужение по наименованию, отделу, исполнителю
        /// </summary>
        [IgnoreDataMember]
        public string NameDepartmentExecutor { get; set; }
    }
}
