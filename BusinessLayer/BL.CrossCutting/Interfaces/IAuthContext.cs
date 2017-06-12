using BL.Model.Context;

namespace BL.CrossCutting.Interfaces
{
    public interface IAuthContext
    { 
        User User { get; set; }
        Session Session { get; set; }
    }
}