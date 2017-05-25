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
            //IsLockout = model.IsLockout;
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
        /// Признак блокировки
        /// </summary>
        public bool IsLockout { get; set; }

        /// <summary>
        /// Основной имейл, на который высылается письмо с приглашением
        /// </summary>
        public string UserEmail { get; set; }

    }
}
