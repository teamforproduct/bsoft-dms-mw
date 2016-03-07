using System;
using BL.Logic.Common;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.PropertyCore.Commands
{
    public class ModifyPropertyValueCommand : BasePropertCommand
    {
        private readonly ISystemDbProcess _systDb;

        public ModifyPropertyValueCommand(ISystemDbProcess systDb)
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
                    Id = Model.Id,
                    PropertyLinkId = Model.PropertyLinkId,
                    RecordId = Model.RecordId,
                    ValueString = Model.ValueString,
                    ValueDate = Model.ValueDate,
                    ValueNumeric = Model.ValueNumeric,
                };
                CommonDocumentUtilities.SetLastChange(_context, item);
                _systDb.UpdatePropertyValue(_context, item);
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

        public override EnumPropertyAction CommandType => EnumPropertyAction.ModifyPropertyValue;
    }
}