using System;
using BL.Logic.Common;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.PropertyCore.Commands
{
    public class ModifyPropertyLinkCommand : BasePropertCommand
    {
        private readonly ISystemDbProcess _systDb;

        public ModifyPropertyLinkCommand(ISystemDbProcess systDb)
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
                    Id = Model.Id,
                    Filers = Model.Filers,
                    IsMandatory = Model.IsMandatory,
                };
                CommonDocumentUtilities.SetLastChange(_context, item);
                _systDb.UpdatePropertyLink(_context, item);
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

        public override EnumPropertyAction CommandType => EnumPropertyAction.ModifyPropertyLink;
    }
}