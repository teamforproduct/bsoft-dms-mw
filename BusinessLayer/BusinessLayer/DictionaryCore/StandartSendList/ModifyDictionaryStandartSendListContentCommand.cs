using System;
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
    public class ModifyDictionaryStandartSendListContentCommand :BaseDictionaryCommand
    {
        private ModifyDictionaryStandartSendListContent Model
        {
            get
            {
                if (!(_param is ModifyDictionaryStandartSendListContent))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryStandartSendListContent)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            DictionaryModelVerifying.VerifyStandartSendListContent(_context, _dictDb, Model);
            
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newCont = new InternalDictionaryStandartSendListContent(Model);
                CommonDocumentUtilities.SetLastChange(_context, newCont);
                _dictDb.UpdateStandartSendListContent(_context, newCont);


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
