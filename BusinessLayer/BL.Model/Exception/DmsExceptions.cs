namespace BL.Model.Exception
{
    public class DmsExceptions : System.Exception
    {

        public DmsExceptions(string message) : base(message)
        {
        }

        public DmsExceptions(string message, System.Exception ex) : base(message, ex)
        {

        }
    }

    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseError : DmsExceptions
    {
        private const string _MESSAGE = "Ошибка при обращении к базе данных!";
        public DatabaseError() : base(_MESSAGE)
        {
        }

        public DatabaseError(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Искомой комманды не существует или она не описана
    /// </summary>
    public class CommandNotDefinedError : DmsExceptions
    {
        private const string _MESSAGE = "Искомой комманды не найдено";
        public CommandNotDefinedError() : base(_MESSAGE)
        {
        }

        public CommandNotDefinedError(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Параметр комманды неверного типа
    /// </summary>
    public class WrongParameterTypeError : DmsExceptions
    {
        private const string _MESSAGE = "Тип параметра комманды указан неверно!";
        public WrongParameterTypeError() : base(_MESSAGE)
        {
        }

        public WrongParameterTypeError(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class AccessIsDenied : DmsExceptions
    {
        private const string _MESSAGE = "Access is Denied!";
        //TODO:передавать параметры
        public AccessIsDenied() : base(_MESSAGE)
        {
        }

        public AccessIsDenied(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentHasAlreadyHadLink : DmsExceptions
    {
        private const string _MESSAGE = "Document Has Already Had Link!";
        //TODO:передавать параметры
        public DocumentHasAlreadyHadLink() : base(_MESSAGE)
        {
        }

        public DocumentHasAlreadyHadLink(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentCannotBeModifiedOrDeleted : DmsExceptions
    {
        private const string _MESSAGE = "Document Can't Be Modified Or Deleted!";
        public DocumentCannotBeModifiedOrDeleted() : base(_MESSAGE)
        {
        }

        public DocumentCannotBeModifiedOrDeleted(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserHasNoAccessToDocument : DmsExceptions
    {
        private const string _MESSAGE = "User could not acceess this document!";
        public UserHasNoAccessToDocument() : base(_MESSAGE)
        {
        }

        public UserHasNoAccessToDocument(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CannotSaveFile : DmsExceptions
    {
        private const string _MESSAGE = "Error when save user file!";
        public CannotSaveFile() : base(_MESSAGE)
        {
        }

        public CannotSaveFile(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserFileNotExists : DmsExceptions
    {
        private const string _MESSAGE = "User file does not exists on Filestore!";
        public UserFileNotExists() : base(_MESSAGE)
        {
        }

        public UserFileNotExists(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UnknownDocumentFile : DmsExceptions
    {
        private const string _MESSAGE = "Could not find appropriate document file!";
        public UnknownDocumentFile() : base(_MESSAGE)
        {
        }

        public UnknownDocumentFile(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CannotAccessToFile : DmsExceptions
    {
        private const string _MESSAGE = "Cannot accss to user file!";
        public CannotAccessToFile() : base(_MESSAGE)
        {
        }

        public CannotAccessToFile(System.Exception ex) : base(_MESSAGE, ex)
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

    public class TemplateDocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "User could not acceess this template document!";
        public TemplateDocumentNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public TemplateDocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
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

    public class DocumentHasAlredyBeenRegistered : DmsExceptions
    {
        private const string _MESSAGE = "Document has already been registered!";
        public DocumentHasAlredyBeenRegistered() : base(_MESSAGE)
        {
        }

        public DocumentHasAlredyBeenRegistered(System.Exception ex) : base(_MESSAGE, ex)
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

    public class NeedInformationAboutCorrespondent : DmsExceptions
    {
        private const string _MESSAGE = "Need information about correspondent!";
        public NeedInformationAboutCorrespondent() : base(_MESSAGE)
        {
        }

        public NeedInformationAboutCorrespondent(System.Exception ex) : base(_MESSAGE, ex)
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

    public class DocumentSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        private const string _MESSAGE = "Document SendList does not match the template";
        public DocumentSendListDoesNotMatchTheTemplate() : base(_MESSAGE)
        {
        }

        public DocumentSendListDoesNotMatchTheTemplate(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentRestrictedSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        private const string _MESSAGE = "Document Restricted SendList does not match the template";
        public DocumentRestrictedSendListDoesNotMatchTheTemplate() : base(_MESSAGE)
        {
        }

        public DocumentRestrictedSendListDoesNotMatchTheTemplate(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class EventNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "User could not acceess this event!";
        public EventNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public EventNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class WaitNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "User could not acceess this wait!";
        public WaitNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public WaitNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DictionaryTagNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "User could not acceess this tag!";
        public DictionaryTagNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public DictionaryTagNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }



    /// <summary>
    /// Сообщение об ошибке, когда невозможно добавить данные в справочник
    /// </summary>
    public class DictionaryRecordCouldNotBeAdded : DmsExceptions
    {
        private const string _MESSAGE = "You could not add this dictionary data!";
        public DictionaryRecordCouldNotBeAdded() : base(_MESSAGE)
        {
        }

        public DictionaryRecordCouldNotBeAdded(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение об ошибке, когда пытаются обновить несуществующую строку справочника в БД
    /// </summary>
    public class DictionaryRecordWasNotFound : DmsExceptions
    {
        private const string _MESSAGE = "Dictionary record was not found!";
        public DictionaryRecordWasNotFound() : base(_MESSAGE)
        {
        }

        public DictionaryRecordWasNotFound(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение об ошибке, когда пытаются обновить несуществующую строку справочника в БД
    /// </summary>
    public class DictionaryRecordNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "Dictionary record should be unique!";
        public DictionaryRecordNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryRecordNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
}