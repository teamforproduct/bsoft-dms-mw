using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface IDictionaryCommand:ICommand
    {
        void InitializeCommand(EnumDictionaryActions action, IContext ctx);
        void InitializeCommand(EnumDictionaryActions action, IContext ctx, object param);
        EnumDictionaryActions CommandType { get; }
    }
}