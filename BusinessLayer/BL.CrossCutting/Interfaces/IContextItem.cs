using System.Collections.Generic;

namespace BL.CrossCutting.Interfaces
{
    // Контекст - элемент коллекции контекстов.
    public interface IContextItem : IAuthContext 
    {
        string Key { get; set; }
        Dictionary<string, IClientContext> ClientContexts { get; }
    }
}