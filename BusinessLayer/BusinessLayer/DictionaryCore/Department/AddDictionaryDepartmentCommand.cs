using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryDepartmentCommand : BaseDictionaryCommand
    {

        private ModifyDictionaryDepartment Model
        {
            get
            {
                if (!(_param is ModifyDictionaryDepartment))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryDepartment)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyDepartment(_context, _dictDb, Model);

            return true;
        }

        public override object Execute()
        {
            try
            {
                var dds = CommonDictionaryUtilities.DepartmentModifyToInternal(_context, Model);

                return _dictDb.AddDepartment(_context, dds);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}