﻿using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore.DocumentType
{
    public class AddDictionaryPositionCommand : BaseDictionaryCommand
    {

        private ModifyDictionaryPosition Model
        {
            get
            {
                if (!(_param is ModifyDictionaryPosition))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryPosition)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType, false);

            var fdd = new FilterDictionaryPosition { Name = Model.Name, NotContainsIDs = new List<int> { Model.Id } };

            if (Model.ParentId != null)
            {
                fdd.ParentIDs = new List<int> { Model.ParentId.Value };
            }
            // Находим запись с таким-же именем в этой-же папке
            if (_dictDb.ExistsPosition(_context, fdd))
            {
                throw new DictionaryRecordNotUnique();
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                var dp = CommonDictionaryUtilities.PositionModifyToInternal(_context, Model);

                return _dictDb.AddPosition(_context, dp);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}