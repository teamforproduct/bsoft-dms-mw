using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DocumentCore;
using BL.Model.DictionaryCore;

namespace BL.Database.Dictionary.Interfaces
{
    public interface IDictionariesDbProcess
    {
        IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter);
        BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id);
    }
}