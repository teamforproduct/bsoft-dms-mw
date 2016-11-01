using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Enums
{
    public enum EnumResultTypes
    {
        CloseByChanging = -1,
        CloseByWithdrawing = -2,
        CloseByRejecting = -3,
        CloseByAffixing = -4,

        Excellent= 2841,// Отлично
        Good = 2842,// Хорошо
        Satisfactorily = 2843,// Удовлетворительно
        Bad = 2844, //Плохо
        WithoutEvaluation = 4062, //Без оценки

    }
}
