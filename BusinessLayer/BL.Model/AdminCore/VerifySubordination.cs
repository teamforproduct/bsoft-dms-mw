using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore
{
    /// <summary>
    /// Модель для проверки субординации
    /// </summary>
    public class VerifySubordination
    {
        /// <summary>
        /// Массив ИД доложностей, от которых исходит действие
        /// </summary>
        public List<int> SourcePositions { get; set; }
        /// <summary>
        /// ИД должностей, которым адресовано действие
        /// </summary>
        public int TargetPosition { get; set; }
        /// <summary>
        /// Тип субординации действия
        /// </summary>
        public EnumSubordinationTypes SubordinationType { get; set; }

    }
}
