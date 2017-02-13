using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using BL.Model.Enums;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// "Настройка правил рассылки между должностями (для исполнения, для сведения)", добавления/редактирования записи.
    /// </summary>
    // В модели перечислены поля, значения которых можно изменить из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class SetSubordinationByCompany
    {

        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        [Required]
        public int SourcePositionId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        [Required]
        public int CompanyId { get; set; }

        /// <summary>
        /// Тип рассылки (для исполнения, для сведения)
        /// </summary>
        [Required]
        public EnumSubordinationTypes SubordinationTypeId { get; set; }

        /// <summary>
        /// Установить галочку
        /// </summary>
        [Required]
        public bool IsChecked { get; set; }

    }
}