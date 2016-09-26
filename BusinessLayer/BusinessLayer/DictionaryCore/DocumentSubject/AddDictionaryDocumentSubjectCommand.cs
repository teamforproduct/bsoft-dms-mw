using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
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
            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyDocumentSubject(_context, _dictDb, Model);
         
            return true;
        }

        public override object Execute()
        {
            try
            {
                var dds = CommonDictionaryUtilities.DocumentSubjectModifyToInternal(_context, Model);

                return _dictDb.AddDocumentSubject(_context, dds);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}