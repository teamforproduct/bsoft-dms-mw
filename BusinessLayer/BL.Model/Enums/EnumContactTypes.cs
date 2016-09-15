using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Enums
{
    /// <summary>
    /// Типы контактов 
    /// </summary>
    public enum EnumContactTypes
    {
        MainEmail,
        WorkEmail,
        PersonalEmail,

        MainPhone,
        MobilePhone,
        WorkPhone,
        HomePhone,

        WorkFax,
        HomeFax,

        SIP,
        Skype,
        Viber,
        ICQ,
        Jabber,
        Telegram,
        Pager,
        Another
    }
}
