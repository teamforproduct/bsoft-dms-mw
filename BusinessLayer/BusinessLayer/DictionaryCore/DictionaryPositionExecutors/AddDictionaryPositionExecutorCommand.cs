using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore.DocumentType
{
    public class AddDictionaryPositionExecutorCommand : BaseDictionaryCommand
    {

        private ModifyDictionaryPositionExecutor Model
        {
            get
            {
                if (!(_param is ModifyDictionaryPositionExecutor))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryPositionExecutor)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType, false);

            //var fd = new FilterDictionaryPositionExecutor {NotContainsIDs = new List<int> { Model.Id } };
            
            //if (_dictDb.ExistsExecutor(_context, fd))
            //{
            //    throw new DictionaryRecordNotUnique();
            //}

            return true;
        }

        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.PositionExecutorModifyToInternal(_context, Model);

                return _dictDb.AddExecutor(_context, dp);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}