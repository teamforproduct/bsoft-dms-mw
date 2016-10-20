using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// "Соответствие ролей и пользователя", добавления/редактирования записи.
    /// </summary>
    // В модели перечислены поля, значения которых можно изменить из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyAdminUserRole
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        [Required]
        public int RoleId { get; set; }

        /// <summary>
        /// Назначение, от должности которого унаследованы роли
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Назначение, от должности которого унаследованы роли
        /// </summary>
        [Required]
        public int PositionExecutorId { get; set; }

        /// <summary>
        /// Дата назначения роли (совпадает с датой назначения на должность)
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата снятия роли (совпадает с датой снятия с должности)
        /// </summary>
        // [Required]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Тип объекта. Например: Конкретному сотруднику на конр должности разрешены действия (роль) над конкретным объектом.
        /// </summary>
        public EnumObjects ObjectId { get; set; }

        /// <summary>
        /// Id сущности. Например отдел...
        /// </summary>
        public int EntityId { get; set; }

    }
}