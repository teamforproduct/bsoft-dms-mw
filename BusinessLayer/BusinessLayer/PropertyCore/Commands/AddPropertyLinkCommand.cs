using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.Enums;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.PropertyCore.Commands
{
    public class AddPropertyLinkCommand : BasePropertyCommand
    {
        private readonly ISystemDbProcess _systDb;

        public AddPropertyLinkCommand(ISystemDbProcess systDb)
        {
            _systDb = systDb;
        }
        private ModifyPropertyLink Model { get { return GetModel<ModifyPropertyLink>(); } }

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

        public override EnumPropertyActions CommandType => EnumPropertyActions.AddPropertyLink;
    }
}