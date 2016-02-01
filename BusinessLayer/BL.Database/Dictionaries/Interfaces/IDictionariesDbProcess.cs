﻿using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.DictionaryCore;

namespace BL.Database.Dictionaries.Interfaces
{
    public interface IDictionariesDbProcess
    {
        IEnumerable<BaseDictionaryStandartSendList> GetDictionaryStandartSendLists(IContext context, FilterDictionaryStandartSendList filter);
        BaseDictionaryStandartSendList GetDictionaryStandartSendList(IContext context, int id);
        DictionaryDocumentEventTypes GetDocumentEventType(IContext context, DocumentEventTypes eventType);
    }
}