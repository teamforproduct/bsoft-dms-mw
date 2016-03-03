using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface IDictionaryCommand:ICommand
    {
        void InitializeCommand(EnumDictionaryAction action, IContext ctx);
        void InitializeCommand(EnumDictionaryAction action, IContext ctx, object param);
        EnumDictionaryAction CommandType { get; }
    }
}