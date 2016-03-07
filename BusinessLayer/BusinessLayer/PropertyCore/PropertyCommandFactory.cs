using BL.CrossCutting.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Logic.PropertyCore.Commands;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.PropertyCore
{
    public static class PropertyCommandFactory
    {
        public static IPropertyCommand GetPropertyCommand(EnumPropertyAction act, IContext ctx, object param)
        {
            IPropertyCommand cmd;
            switch (act)
            {
                case EnumPropertyAction.AddProperty:
                    cmd = DmsResolver.Current.Get<AddPropertyCommand>();
                    break;
                case EnumPropertyAction.DeleteProperty:
                    cmd = DmsResolver.Current.Get<DeletePropertyCommand>();
                    break;
                case EnumPropertyAction.ModifyProperty:
                    cmd = DmsResolver.Current.Get<ModifyPropertyCommand>();
                    break;
                case EnumPropertyAction.AddPropertyLink:
                    cmd = DmsResolver.Current.Get<AddPropertyLinkCommand>();
                    break;
                case EnumPropertyAction.DeletePropertyLink:
                    cmd = DmsResolver.Current.Get<DeletePropertyLinkCommand>();
                    break;
                case EnumPropertyAction.ModifyPropertyLink:
                    cmd = DmsResolver.Current.Get<ModifyPropertyLinkCommand>();
                    break;
                case EnumPropertyAction.AddPropertyValue:
                    cmd = DmsResolver.Current.Get<AddPropertyValueCommand>();
                    break;
                case EnumPropertyAction.DeletePropertyValue:
                    cmd = DmsResolver.Current.Get<DeletePropertyValueCommand>();
                    break;
                case EnumPropertyAction.ModifyPropertyValue:
                    cmd = DmsResolver.Current.Get<ModifyPropertyValueCommand>();
                    break;
                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}