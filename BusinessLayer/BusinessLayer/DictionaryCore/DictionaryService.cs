
using BL.Logic.DictionaryCore.Interfaces;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore;
using BL.CrossCutting.DependencyInjection;
using BL.Database.Dictionaries.Interfaces;

namespace BL.Logic.DictionaryCore
{
    public class DictionaryService : IDictionaryService
    {
        public IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendList(IContext context, FilterDictionaryStandartSendList filter)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryStandartSendLists(context, filter);
        }

        public BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
            return dictDb.GetDictionaryStandartSendList(context, id);
        }
    }
}
