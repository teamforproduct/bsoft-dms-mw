using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.PropertyCore.Commands;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.PropertyCore
{
    public static class PropertyCommandFactory
    {
        public static IPropertyCommand GetPropertyCommand(EnumActions act, IContext ctx, object param)
        {
            IPropertyCommand cmd;
            switch (act)
            {
                #region Properties
                case EnumActions.AddProperty:
                    cmd = DmsResolver.Current.Get<AddPropertyCommand>();
                    break;
                case EnumActions.DeleteProperty:
                    cmd = DmsResolver.Current.Get<DeletePropertyCommand>();
                    break;
                case EnumActions.ModifyProperty:
                    cmd = DmsResolver.Current.Get<ModifyPropertyCommand>();
                    break;
                case EnumActions.AddPropertyLink:
                    cmd = DmsResolver.Current.Get<AddPropertyLinkCommand>();
                    break;
                case EnumActions.DeletePropertyLink:
                    cmd = DmsResolver.Current.Get<DeletePropertyLinkCommand>();
                    break;
                case EnumActions.ModifyPropertyLink:
                    cmd = DmsResolver.Current.Get<ModifyPropertyLinkCommand>();
                    break;
                #endregion Properties
                default:
                    throw new CommandNotDefinedError(act.ToString());
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}