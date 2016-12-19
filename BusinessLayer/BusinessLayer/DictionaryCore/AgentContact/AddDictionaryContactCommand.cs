﻿using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryContactCommand : BaseDictionaryContactCommand
    {
        private AddAgentContact Model { get { return GetModel<AddAgentContact>(); } }

        public override object Execute()
        {
            try
            {
                var newContact = new InternalDictionaryContact(Model);       
                         
                CommonDocumentUtilities.SetLastChange(_context, newContact);

                return _dictDb.AddContact(_context, newContact);
            }
     
            catch (Exception ex)
            {
                
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
