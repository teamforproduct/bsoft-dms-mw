using BL.Model.Common;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Сведения об авторизации сотрудника - пользователя
    /// </summary>
    public class FrontAuthInfo
    {
        /// <summary>
        /// Id 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Login 
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Email 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// EmailConfirmed 
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// IsLockout - блокирован 
        /// </summary>
        public bool IsLockout { get; set; }




        //Id = user.Id, UserName = user.Email, IsLockout = user.IsLockout, Email = user.Email, EmailConfirmed = user.EmailConfirmed, IsEmailConfirmRequired = user.IsEmailConfirmRequired, IsChangePasswordRequired = user.IsChangePasswordRequired,  AccessFailedCount = user.AccessFailedCount
    }
}