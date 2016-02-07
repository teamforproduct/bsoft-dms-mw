namespace BL.Model.Exception
{
    public class DmsExceptions :System.Exception
    {

        public DmsExceptions(string message) : base(message)
        {
        }

        public DmsExceptions(string message, System.Exception ex) : base(message, ex)
        {

        }
    }


    public class UserHasNoAccessToDocument : DmsExceptions
    {
        private const string _MESSAGE = "User could not acceess this document!";
        public UserHasNoAccessToDocument():base(_MESSAGE)
        {
        }

        public UserHasNoAccessToDocument(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "User could not acceess this document!";
        public DocumentNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public DocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentCouldNotBeRegistered : DmsExceptions
    {
        private const string _MESSAGE = "Document registration has non been successfull! Try again!";
        public DocumentCouldNotBeRegistered() : base(_MESSAGE)
        {
        }

        public DocumentCouldNotBeRegistered(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserPositionIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "Position for the current user could not be defined!";
        public UserPositionIsNotDefined() : base(_MESSAGE)
        {
        }

        public UserPositionIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class WrongInformationAboutCorrespondent : DmsExceptions
    {
        private const string _MESSAGE = "Designation of information about correspondent is wrong!";
        public WrongInformationAboutCorrespondent() : base(_MESSAGE)
        {
        }

        public WrongInformationAboutCorrespondent(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserNameIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "Employee for the current user could not be defined!";
        public UserNameIsNotDefined() : base(_MESSAGE)
        {
        }

        public UserNameIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserUnauthorized : DmsExceptions
    {
        private const string _MESSAGE = "Authorization has been denied for this request.";
        public UserUnauthorized() : base(_MESSAGE)
        {
        }

        public UserUnauthorized(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentRestrictedSendListDuplication : DmsExceptions
    {
        private const string _MESSAGE = "Duplicate Entry DocumentRestrictSendList";
        public DocumentRestrictedSendListDuplication() : base(_MESSAGE)
        {
        }

        public DocumentRestrictedSendListDuplication(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentSendListDuplication : DmsExceptions
    {
        private const string _MESSAGE = "Duplicate Entry DocumentSendList";
        public DocumentSendListDuplication() : base(_MESSAGE)
        {
        }

        public DocumentSendListDuplication(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
    public class DocumentSendListNotFoundInDocumentRestrictedSendList : DmsExceptions
    {
        private const string _MESSAGE = "DocumentSendList not found in DocumentRestrictedSendList";
        public DocumentSendListNotFoundInDocumentRestrictedSendList() : base(_MESSAGE)
        {
        }

        public DocumentSendListNotFoundInDocumentRestrictedSendList(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
}