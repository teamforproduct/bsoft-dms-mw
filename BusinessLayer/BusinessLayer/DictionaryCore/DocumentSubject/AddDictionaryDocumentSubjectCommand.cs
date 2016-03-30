using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore.DocumentType
{
    public class AddDictionaryDocumentSubjectCommand : BaseDictionaryCommand
    {
   
        private ModifyDictionaryDocumentSubject Model
        {
            get
            {
                if (!(_param is ModifyDictionaryDocumentSubject))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryDocumentSubject)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType, false);
            // Находим запись с таким-же именем в этой-же папке
            if (_dictDb.ExistsDictionaryDocumentSubject(_context, new FilterDictionaryDocumentSubject { Name = Model.Name, ParentIDs = new List<int> { Model.ParentId ?? -1 }, NotContainsIDs = new List<int> { Model.Id } }))
            {
                throw new DictionaryRecordNotUnique();
            }
         
            return true;
        }

        public override object Execute()
        {
            try
            {
                var dds = CommonDictionaryUtilities.DocumentSubjectModifyToInternal(_context, Model);

                return _dictDb.AddDictionaryDocumentSubject(_context, dds);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}