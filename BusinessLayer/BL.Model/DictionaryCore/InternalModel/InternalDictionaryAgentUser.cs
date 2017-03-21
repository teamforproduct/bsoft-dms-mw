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

        public InternalDictionaryAgentUser(InternalDictionaryAgentEmployee model)
        {
            Id = model.Id;
            IsActive = model.IsActive;
            LanguageId = model.LanguageId;
            UserId = model.UserId;
            UserName = model.UserName;
            UserEmail = model.UserEmail;
            LastChangeDate = model.LastChangeDate;
            LastChangeUserId = model.LastChangeUserId;
        }

        public int Id { get; set; }

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        public int LanguageId { get; set; }

        public string LastPositionChose { get; set; }        

        public string UserId { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Основной имейл, на который высылается письмо с приглашением
        /// </summary>
        public string UserEmail { get; set; }

    }
}
