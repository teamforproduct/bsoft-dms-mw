using System;
using BL.Logic.Common;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore
{
    public class DeleteCustomDictionaryCommand : BaseDictionaryCommand
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
            return true;
        }

        public override object Execute()
        {
            try
            {
                _dictDb.DeleteCustomDictionary(_context, Model);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }
}