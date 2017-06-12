using BL.CrossCutting.Interfaces;
using BL.Model.Context;

namespace BL.CrossCutting.Context
{
    /// <summary>
    /// Контекст пользователя - информационный класс, который НЕ должен содержать логику. Просто набор параметров, которые доступны на всех уровнях
    /// </summary>
    public class AuthContext : IAuthContext
    {
        public AuthContext() { }

        public AuthContext(IAuthContext ctx)
        {
            Session = new Session
            {
                Key = ctx.Session.Key,
                SignInId = ctx.Session.SignInId,
                LastUsage = ctx.Session.LastUsage,
            };
            User = new User
            {
                Id = ctx.User.Id,
                Name = ctx.User.Name,
                IsChangePasswordRequired = ctx.User.IsChangePasswordRequired,
                LanguageId = ctx.User.LanguageId,
            };
        }

        public User User { get; set; }
        public Session Session { get; set; }

    }
}