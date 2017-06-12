using BL.CrossCutting.Interfaces;
using BL.Model.Context;

namespace BL.CrossCutting.Context
{
    /// <summary>
    /// Контекст пользователя - информационный класс, который НЕ должен содержать логику. Просто набор параметров, которые доступны на всех уровнях
    /// </summary>
    public class DefaultContext : ClientContext, IContext
    {

        public DefaultContext() { }
        
        /// <summary>
        /// Создает новый контекст пользователя и новое ПОДКЛЮЧЕНИЕ к базе
        /// </summary>
        /// <param name="ctx"></param>
        public DefaultContext(IContext ctx) : base (ctx)
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

        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            DefaultContext ctx = obj as DefaultContext;
            return ctx != null && string.Equals(this.Key, ctx.Key);
        }


        public User User { get; set; }

        public Session Session { get; set; }

    }
}