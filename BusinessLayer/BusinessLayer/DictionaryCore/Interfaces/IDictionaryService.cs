using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Logic.DictionaryCore.Interfaces
{
    public interface IDictionaryService
    {
        IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendList(IContext context, FilterDictionaryStandartSendList filter);
        BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id);
    }
}
