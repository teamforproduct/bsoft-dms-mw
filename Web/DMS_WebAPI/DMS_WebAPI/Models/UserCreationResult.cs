using DMS_WebAPI.DBModel;

namespace DMS_WebAPI.Models
{
    public class UserCreationResult
    {
        public UserCreationResult() { }

        public UserCreationResult(UserCreationResult user)
        {
            SetMergeUserResult(user);
        }

        public UserCreationResult(AspNetUsers user, bool isNew)
        {
            SetMergeUserResult(user);
            IsNew = isNew;
        }
        private void SetMergeUserResult(UserCreationResult user)
        {
            Id = user.Id;
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;

            UserName = user.UserName;

            IsChangePasswordRequired = user.IsChangePasswordRequired;
            IsEmailConfirmRequired = user.IsEmailConfirmRequired;

        }

        private void SetMergeUserResult(AspNetUsers user)
        {
            Id = user.Id;
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;

            UserName = user.UserName;

            IsChangePasswordRequired = user.IsChangePasswordRequired;
            IsEmailConfirmRequired = user.IsEmailConfirmRequired;
        }

        public string Id { set; get; }
        public string Email { set; get; }
        public string UserName { get; set; }

        public bool EmailConfirmed { get; set; }
        public bool IsChangePasswordRequired { get; set; }
        public bool IsEmailConfirmRequired { get; set; }


        public bool IsNew { set; get; }
    }
}