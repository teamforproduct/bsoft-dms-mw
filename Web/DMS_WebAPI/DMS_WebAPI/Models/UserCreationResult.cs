using DMS_WebAPI.DBModel;

namespace DMS_WebAPI.Models
{
    public class UserCreationResult : AspNetUsers
    {
        public UserCreationResult() { }

        public UserCreationResult(AspNetUsers user)
        {
            SetMergeUserResult(user);
        }

        public UserCreationResult(AspNetUsers user, bool isNew)
        {
            SetMergeUserResult(user);
            IsNew = isNew;
        }
        private void SetMergeUserResult(AspNetUsers user)
        {
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;

            PhoneNumber = user.PhoneNumber;
            PhoneNumberConfirmed = user.PhoneNumberConfirmed;

            UserName = user.UserName;

            IsChangePasswordRequired = user.IsChangePasswordRequired;
            IsEmailConfirmRequired = user.IsEmailConfirmRequired;
            IsLockout = user.IsLockout;
            IsFingerprintEnabled = user.IsLockout;

            ControlQuestionId = user.ControlQuestionId;
            ControlAnswer = user.ControlAnswer;

            CreateDate = user.CreateDate;
            LastChangeDate = user.LastChangeDate;

            PasswordHash = user.PasswordHash;
            SecurityStamp = user.SecurityStamp;
            TwoFactorEnabled = user.TwoFactorEnabled;
            LockoutEndDateUtc = user.LockoutEndDateUtc;
            LockoutEnabled = user.LockoutEnabled;
            AccessFailedCount = user.AccessFailedCount;
        }


        public bool IsNew { set; get; }
    }
}