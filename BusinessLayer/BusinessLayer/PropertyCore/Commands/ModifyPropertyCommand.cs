using System;
using BL.Logic.Common;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.PropertyCore.Commands
{
    public class ModifyPropertyCommand : BasePropertCommand
    {
        private readonly ISystemDbProcess _systDb;

        public ModifyPropertyCommand(ISystemDbProcess systDb)
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
                    Id = Model.Id,
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
                    SelectTable = Model.SelectTable,
                };
                CommonDocumentUtilities.SetLastChange(_context, item);
                _systDb.UpdateProperty(_context, item);
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

        public override EnumPropertyActions CommandType => EnumPropertyActions.ModifyProperty;
    }
}