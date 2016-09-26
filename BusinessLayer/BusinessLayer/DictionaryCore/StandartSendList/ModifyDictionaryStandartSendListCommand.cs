﻿using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryStandartSendListCommand :BaseDictionaryCommand
    {
        private ModifyDictionaryStandartSendList Model
        {
            get
            {
                if (!(_param is ModifyDictionaryStandartSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryStandartSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyStandartSendList(_context, _dictDb, Model);
            
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newList = new InternalDictionaryStandartSendList(Model);
                CommonDocumentUtilities.SetLastChange(_context, newList);
                _dictDb.UpdateStandartSendList(_context, newList);


            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}
