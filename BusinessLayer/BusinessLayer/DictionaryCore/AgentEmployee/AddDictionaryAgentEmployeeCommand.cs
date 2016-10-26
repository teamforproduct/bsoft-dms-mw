using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using System.Collections.Generic;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Linq;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentEmployeeCommand : BaseDictionaryAgentEmployeeCommand
    {
        public override object Execute()
        {
            try
            {
                var item = new InternalDictionaryAgentEmployee(Model);

                CommonDocumentUtilities.SetLastChange(_context, item);
                int agent =_dictDb.AddAgentEmployee(_context, item);

                if ((item.Login ?? string.Empty) != string.Empty)
                {
                    var contact = new InternalDictionaryContact()
                    {
                        AgentId = agent,
                        ContactTypeId = _dictDb.GetContactsTypeId(_context, EnumContactTypes.MainEmail),
                        Value = item.Login,
                        IsActive = true
                    };
                    CommonDocumentUtilities.SetLastChange(_context, contact);
                    _dictDb.AddContact(_context, contact);
                }

                if ((item.Phone ?? string.Empty) != string.Empty)
                {
                    var contact = new InternalDictionaryContact()
                    {
                        AgentId = agent,
                        ContactTypeId = _dictDb.GetContactsTypeId(_context, EnumContactTypes.MainPhone),
                        Value = item.Phone,
                        IsActive = true
                    };
                    CommonDocumentUtilities.SetLastChange(_context, contact);
                    _dictDb.AddContact(_context, contact);
                }

                return agent;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
