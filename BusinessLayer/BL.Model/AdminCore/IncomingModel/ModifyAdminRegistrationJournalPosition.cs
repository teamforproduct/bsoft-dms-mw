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
    /// Настройка доступа к документам в журналах регистрации
    /// </summary>
    // В модели перечислены поля, значения которых можно изменить из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyAdminRegistrationJournalPosition
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Id Должности
        /// </summary>
        [Required]
        public int PositionId { get; set; }

        /// <summary>
        /// Id журнала регистрации
        /// </summary>
        [Required]
        public int RegistrationJournalId { get; set; }

        /// <summary>
        /// Тип доступа к журналу
        /// </summary>
        [Required]
        public EnumRegistrationJournalAccessTypes RegJournalAccessTypeId { get; set; }

        /// <summary>
        /// Установить галочку
        /// </summary>
        [Required]
        public bool IsChecked { get; set; }

    }
}