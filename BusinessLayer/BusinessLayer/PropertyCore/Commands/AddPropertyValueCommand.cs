using System;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.PropertyCore.Commands
{
    public class AddPropertyValueCommand : BasePropertCommand
    {
        private readonly ISystemDbProcess _systDb;

        public AddPropertyValueCommand(ISystemDbProcess systDb)
        {
            _systDb = systDb;
        }

        private ModifyPropertyValue Model
        {
            get
            {
                if (!(_param is ModifyPropertyValue))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyPropertyValue)_param;
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
                var item = new InternalPropertyValue
                {
                    PropertyLinkId = Model.PropertyLinkId,
                    RecordId = Model.RecordId,
                    ValueString = Model.ValueString,
                    ValueDate = Model.ValueDate,
                    ValueNumeric = Model.ValueNumeric,
                };
                CommonDocumentUtilities.SetLastChange(_context, item);
                return _systDb.AddPropertyValue(_context, item);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        public override EnumPropertyAction CommandType => EnumPropertyAction.AddPropertyValue;
    }
}