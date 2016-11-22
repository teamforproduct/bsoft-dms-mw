using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.SystemCore.FrontModel;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Группы настроек
    /// </summary>
    public class FrontDictionarySettingType
    {

        /// <summary>
        /// Название группы настроек
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Список настроек
        /// </summary>
        public IEnumerable<FrontSystemSetting> Setting { get; set; }
    }
}
