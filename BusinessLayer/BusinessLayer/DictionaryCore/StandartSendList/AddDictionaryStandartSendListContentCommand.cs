using System;
using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;


namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryStandartSendListContentCommand : BaseDictionaryCommand
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
            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyStandartSendListContent(_context, _dictDb, Model);

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newCont = new InternalDictionaryStandartSendListContent(Model);
                CommonDocumentUtilities.SetLastChange(_context, newCont);
                return _dictDb.AddStandartSendListContent(_context, newCont);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
