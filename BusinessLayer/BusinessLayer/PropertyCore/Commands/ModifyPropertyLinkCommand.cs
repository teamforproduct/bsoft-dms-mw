using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.Enums;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.PropertyCore.Commands
{
    public class ModifyPropertyLinkCommand : BasePropertyCommand
    {
        private readonly ISystemDbProcess _systDb;

        public ModifyPropertyLinkCommand(ISystemDbProcess systDb)
        {
            _systDb = systDb;
        }
        private ModifyPropertyLink Model { get { return GetModel<ModifyPropertyLink>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            //TODO: Проверка
            return true;
        }

        public override object Execute()
        {
            var item = new InternalPropertyLink
            {
                Id = Model.Id,
                Filers = Model.Filers,
                IsMandatory = Model.IsMandatory,
            };
            CommonDocumentUtilities.SetLastChange(_context, item);
            _systDb.UpdatePropertyLink(_context, item);
            return null;
        }

        public override EnumPropertyActions CommandType => EnumPropertyActions.ModifyPropertyLink;
    }
}