using System;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.PropertyCore.Commands
{
    public class AddPropertyLinkCommand : BasePropertCommand
    {
        private readonly ISystemDbProcess _systDb;

        public AddPropertyLinkCommand(ISystemDbProcess systDb)
        {
            _systDb = systDb;
        }

        private ModifyPropertyLink Model
        {
            get
            {
                if (!(_param is ModifyPropertyLink))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyPropertyLink)_param;
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
                var item = new InternalPropertyLink
                {
                    PropertyId = Model.PropertyId,
                    Object = Model.Object,
                    Filers = Model.Filers,
                    IsMandatory = Model.IsMandatory,
                };
                CommonDocumentUtilities.SetLastChange(_context, item);
                return _systDb.AddPropertyLink(_context, item);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        public override EnumPropertyActions CommandType => EnumPropertyActions.AddPropertyLink;
    }
}