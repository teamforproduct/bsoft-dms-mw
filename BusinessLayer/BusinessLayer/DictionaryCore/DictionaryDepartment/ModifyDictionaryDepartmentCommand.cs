using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore.DocumentType
{
    public class ModifyDictionaryDepartmentCommand : BaseDictionaryCommand
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

            _admin.VerifyAccess(_context, CommandType, false);

            var fdd = new FilterDictionaryDepartment { Name = Model.Name, NotContainsIDs = new List<int> { Model.Id } };

            if (Model.ParentId != null)
            {
                fdd.ParentIDs = new List<int> { Model.ParentId.Value };
            }

            // Находим запись с таким-же именем в этой-же папке
            if (_dictDb.ExistsDictionaryDepartment(_context, fdd))
            {
                throw new DictionaryRecordNotUnique();
            }

            return true;
        }


        public override object Execute()
        {
            try
            {
                var dds = CommonDictionaryUtilities.DepartmentModifyToInternal(_context, Model);

                _dictDb.UpdateDictionaryDepartment(_context, dds);
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