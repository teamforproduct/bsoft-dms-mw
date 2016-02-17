using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore
{
    /// <summary>
    /// Модель для проверки прав доступа
    /// </summary>
    public class VerifyAccess
    {
        /// <summary>
        /// ИД юзера
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Код объекта
        /// </summary>
        public string ObjectCode { get; set; }
        /// <summary>
        /// Код действия
        /// </summary>
        public string ActionCode { get; set; }
        /// <summary>
        /// ИД записи, если права должны раздавать в разрезе каждой записи объекта
        /// </summary>
        public int? RecordId { get; set; }
        /// <summary>
        /// ИД должности, для которой нужно проверить права
        /// </summary>
        public int? PositionId { get; set; }
        /// <summary>
        /// Массив ИД должностей, от которых работает пользователь
        /// </summary>
        public List<int> PositionsIdList { get; set; }
    }
}
