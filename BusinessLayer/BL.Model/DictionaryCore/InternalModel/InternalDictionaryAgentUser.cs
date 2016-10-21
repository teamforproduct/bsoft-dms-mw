using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контрагент - пользователь
    /// </summary>
    public class InternalDictionaryAgentUser : LastChangeInfo
    {

        public InternalDictionaryAgentUser()
        { }

        public InternalDictionaryAgentUser(ModifyDictionaryAgentUser model)
        {
            Id = model.Id;
            LanguageId = model.LanguageId;
            UserId = model.UserId;
            Login = model.Login;
            PasswordHash = model.PasswordHash;
            IsActive = model.IsActive;
        }

        public InternalDictionaryAgentUser(InternalDictionaryAgentEmployee model)
        {
            Id = model.Id;
            IsActive = model.IsActive;
            LanguageId = model.LanguageId;
            //UserId = model.UserId;
            Login = model.Login;
            PasswordHash = model.PasswordHash;
            LastChangeDate = model.LastChangeDate;
            LastChangeUserId = model.LastChangeUserId;
        }

        public int Id { get; set; }

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        public int? LanguageId { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Основной имейл, на который высылается письмо с приглашением
        /// </summary>
        public string Login { get; set; }

    }
}
