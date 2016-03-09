using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore.DocumentType
{
    public class DeleteDictionaryDocumentTypeCommand : BaseDictionaryCommand
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
            //      Удаление возможно только если отсутствуют документы этого типа. + есть грант на удаление
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newDocType = new InternalDictionaryDocumentType
                {
                    Id = Model
                  
                };
                _dictDb.DeleteDictionaryDocumentType(_context, newDocType);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }

}

