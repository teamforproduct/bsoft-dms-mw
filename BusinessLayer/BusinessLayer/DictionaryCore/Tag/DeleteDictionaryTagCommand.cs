using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryTagCommand : BaseDictionaryCommand
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
            //TODO: Проверка возможности удаления записи
            return true;
        }

        public override object Execute()
        {
            try
            {
                var item = new InternalDictionaryTag
                {
                    Id = Model
                  
                };
                _dictDb.DeleteTag(_context, item);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }

}

