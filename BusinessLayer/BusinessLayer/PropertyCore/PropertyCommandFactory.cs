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
        public static IPropertyCommand GetPropertyCommand(EnumPropertyActions act, IContext ctx, object param)
        {
            IPropertyCommand cmd;
            switch (act)
            {
                case EnumPropertyActions.AddProperty:
                    cmd = DmsResolver.Current.Get<AddPropertyCommand>();
                    break;
                case EnumPropertyActions.DeleteProperty:
                    cmd = DmsResolver.Current.Get<DeletePropertyCommand>();
                    break;
                case EnumPropertyActions.ModifyProperty:
                    cmd = DmsResolver.Current.Get<ModifyPropertyCommand>();
                    break;
                case EnumPropertyActions.AddPropertyLink:
                    cmd = DmsResolver.Current.Get<AddPropertyLinkCommand>();
                    break;
                case EnumPropertyActions.DeletePropertyLink:
                    cmd = DmsResolver.Current.Get<DeletePropertyLinkCommand>();
                    break;
                case EnumPropertyActions.ModifyPropertyLink:
                    cmd = DmsResolver.Current.Get<ModifyPropertyLinkCommand>();
                    break;
                default:
                    throw new CommandNotDefinedError(act.ToString());
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}