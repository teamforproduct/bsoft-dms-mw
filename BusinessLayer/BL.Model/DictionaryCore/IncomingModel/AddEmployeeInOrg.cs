using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddEmployeeInOrg: AddAgentPeople
    {
        /// <summary>
        /// Название организации (будет создана новая)
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// Id организации
        /// </summary>
        public int? OrgId { get; set; }

        /// <summary>
        /// Название отдела (будет создан новый)
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// Индекс отдела (будет создан новый)
        /// </summary>
        public string DepartmentIndex { get; set; }
        /// <summary>
        /// ID отдела
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// Название должности, на которую будет назначен сотрудник (будет создана новая)
        /// </summary>
        public string PositionName { get; set; }
        /// <summary>
        ///  Id должности, на которую будет назначен сотрудник 
        /// </summary>
        public int? PositionId { get; set; }

        /// <summary>
        /// Имейл, на который высылается письмо с приглашением
        /// </summary>
        [Required]
        //[EmailAddress]
        public string Login { get; set; }
        [IgnoreDataMember]
        public string Password { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// Профиль пользователя. Язык интерфейса.
        /// </summary>
        [Required]
        public int LanguageId { get; set; }

        [IgnoreDataMember]
        public Roles Role { get; set; }

        /// <summary>
        /// Тип исполнения
        /// </summary>
        [Required]
        public EnumPositionExecutionTypes ExecutorType { get; set; }

        /// <summary>
        /// Уровень доступа
        /// </summary>
        [Required]
        public EnumAccessLevels AccessLevel { get; set; }


        [IgnoreDataMember]
        public bool IsChangePasswordRequired { get; set; } = true;
        [IgnoreDataMember]
        public bool IsEmailConfirmRequired { get; set; } = true;
        [IgnoreDataMember]
        public bool EmailConfirmed { get; set; }

    }
}
