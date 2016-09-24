using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using BL.Model.DictionaryCore.FrontModel;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryContactCommand : BaseDictionaryCommand
    {
        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType,false,true);

            return true;
        }

        public override object Execute()
        {
            try
            {
                var item = new InternalDictionaryContact() { Id = Model };
                _dictDb.DeleteContact(_context, item);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }
}
