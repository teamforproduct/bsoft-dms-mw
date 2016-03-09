using System;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.PropertyCore.Commands
{
    public class AddPropertyCommand : BasePropertCommand
    {
        private readonly ISystemDbProcess _systDb;

        public AddPropertyCommand(ISystemDbProcess systDb)
        {
            _systDb = systDb;
        }

        private ModifyProperty Model
        {
            get
            {
                if (!(_param is ModifyProperty))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyProperty)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            //TODO: Проверка
            return true;
        }

        public override object Execute()
        {
            try
            {
                var item = new InternalProperty
                {
                    Code = Model.Code,
                    Description = Model.Description,
                    Label = Model.Label,
                    Hint = Model.Hint,
                    ValueTypeId = Model.ValueTypeId,
                    OutFormat = Model.OutFormat,
                    InputFormat = Model.InputFormat,
                    SelectAPI = Model.SelectAPI,
                    SelectFilter = Model.SelectFilter,
                    SelectFieldCode = Model.SelectFieldCode,
                    SelectDescriptionFieldCode = Model.SelectDescriptionFieldCode,
                };
                CommonDocumentUtilities.SetLastChange(_context, item);
                return _systDb.AddProperty(_context, item);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        public override EnumPropertyAction CommandType => EnumPropertyAction.AddProperty;
    }
}