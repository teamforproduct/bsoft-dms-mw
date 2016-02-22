using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface IDictionaryCommand:ICommand
    {
        void InitializeCommand(IContext ctx);
        void InitializeCommand(IContext ctx, object param);
        EnumDictionaryAction CommandType { get; }
    }
}