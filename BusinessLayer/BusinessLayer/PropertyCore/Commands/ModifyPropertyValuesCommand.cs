using System;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Enums;
using System.Linq;

namespace BL.Logic.PropertyCore.Commands
{
    public class ModifyPropertyValuesCommand : BasePropertCommand
    {
        private readonly ISystemDbProcess _systDb;

        public ModifyPropertyValuesCommand(ISystemDbProcess systDb)
        {
            _systDb = systDb;
        }

        private InternalPropertyValues Model
        {
            get
            {
                if (!(_param is InternalPropertyValues))
                {
                    throw new WrongParameterTypeError();
                }
                return (InternalPropertyValues)_param;
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
                var propertyValues = Model.PropertyValues.ToList();
                foreach (var item in propertyValues)
                {
                    CommonDocumentUtilities.SetLastChange(_context, item);
                }
                Model.PropertyValues = propertyValues;
                
                _systDb.ModifyPropertyValues(_context, Model);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        public override EnumPropertyAction CommandType => EnumPropertyAction.ModifyPropertyValues;
    }
}