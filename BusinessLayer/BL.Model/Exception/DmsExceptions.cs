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
        private const string _MESSAGE = "[lang]DmsExceptions@DatabaseError[lang]";
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
        private const string _MESSAGE = "[lang]DmsExceptions@CommandNotDefinedError[lang]";
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
    public class WrongParameterValueError : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@WrongParameterValueError[lang]";
        public WrongParameterValueError() : base(_MESSAGE)
        {
        }

        public WrongParameterValueError(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Параметр комманды неверного типа
    /// </summary>
    public class WrongParameterTypeError : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@WrongParameterTypeError[lang]";
        public WrongParameterTypeError() : base(_MESSAGE)
        {
        }

        public WrongParameterTypeError(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class AccessIsDenied : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@AccessIsDenied[lang]";
        //TODO:передавать параметры
        public AccessIsDenied() : base(_MESSAGE)
        {
        }

        public AccessIsDenied(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentHasAlreadyHasLink : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DocumentHasAlreadyHasLink[lang]";
        //TODO:передавать параметры
        public DocumentHasAlreadyHasLink() : base(_MESSAGE)
        {
        }

        public DocumentHasAlreadyHasLink(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentCannotBeModifiedOrDeleted : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DocumentCannotBeModifiedOrDeleted[lang]";
        public DocumentCannotBeModifiedOrDeleted() : base(_MESSAGE)
        {
        }

        public DocumentCannotBeModifiedOrDeleted(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserHasNoAccessToDocument : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@UserHasNoAccessToDocument[lang]";
        public UserHasNoAccessToDocument() : base(_MESSAGE)
        {
        }

        public UserHasNoAccessToDocument(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CannotSaveFile : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@CannotSaveFile[lang]";
        public CannotSaveFile() : base(_MESSAGE)
        {
        }

        public CannotSaveFile(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserFileNotExists : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@UserFileNotExists[lang]";
        public UserFileNotExists() : base(_MESSAGE)
        {
        }

        public UserFileNotExists(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UnknownDocumentFile : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@UnknownDocumentFile[lang]";
        public UnknownDocumentFile() : base(_MESSAGE)
        {
        }

        public UnknownDocumentFile(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CannotAccessToFile : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@CannotAccessToFile[lang]";
        public CannotAccessToFile() : base(_MESSAGE)
        {
        }

        public CannotAccessToFile(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DocumentNotFoundOrUserHasNoAccess[lang]";
        public DocumentNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public DocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class TemplateDocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@TemplateDocumentNotFoundOrUserHasNoAccess[lang]";
        public TemplateDocumentNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public TemplateDocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentCouldNotBeRegistered : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DocumentCouldNotBeRegistered[lang]";
        public DocumentCouldNotBeRegistered() : base(_MESSAGE)
        {
        }

        public DocumentCouldNotBeRegistered(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotChangeAttributeLaunchPlan : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@CouldNotChangeAttributeLaunchPlan[lang]";
        public CouldNotChangeAttributeLaunchPlan() : base(_MESSAGE)
        {
        }

        public CouldNotChangeAttributeLaunchPlan(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotChangeFavourite : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@CouldNotChangeFavourite[lang]";
        public CouldNotChangeFavourite() : base(_MESSAGE)
        {
        }

        public CouldNotChangeFavourite(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotChangeIsInWork : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@CouldNotChangeIsInWork[lang]";
        public CouldNotChangeIsInWork() : base(_MESSAGE)
        {
        }

        public CouldNotChangeIsInWork(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentHasAlredyBeenRegistered : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DocumentHasAlredyBeenRegistered[lang]";
        public DocumentHasAlredyBeenRegistered() : base(_MESSAGE)
        {
        }

        public DocumentHasAlredyBeenRegistered(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class PlanPointHasAlredyBeenLaunched : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@PlanPointHasAlredyBeenLaunched[lang]";
        public PlanPointHasAlredyBeenLaunched() : base(_MESSAGE)
        {
        }

        public PlanPointHasAlredyBeenLaunched(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserPositionIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@UserPositionIsNotDefined[lang]";
        public UserPositionIsNotDefined() : base(_MESSAGE)
        {
        }

        public UserPositionIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class NeedInformationAboutCorrespondent : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@NeedInformationAboutCorrespondent[lang]";
        public NeedInformationAboutCorrespondent() : base(_MESSAGE)
        {
        }

        public NeedInformationAboutCorrespondent(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserNameIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@UserNameIsNotDefined[lang]";
        public UserNameIsNotDefined() : base(_MESSAGE)
        {
        }

        public UserNameIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserUnauthorized : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@UserUnauthorized[lang]";
        public UserUnauthorized() : base(_MESSAGE)
        {
        }

        public UserUnauthorized(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentRestrictedSendListDuplication : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DocumentRestrictedSendListDuplication[lang]";
        public DocumentRestrictedSendListDuplication() : base(_MESSAGE)
        {
        }

        public DocumentRestrictedSendListDuplication(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class WrongDocumentSendListEntry : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@WrongDocumentSendListEntry[lang]";
        public WrongDocumentSendListEntry() : base(_MESSAGE)
        {
        }

        public WrongDocumentSendListEntry(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentSendListNotFoundInDocumentRestrictedSendList : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DocumentSendListNotFoundInDocumentRestrictedSendList[lang]";
        public DocumentSendListNotFoundInDocumentRestrictedSendList() : base(_MESSAGE)
        {
        }

        public DocumentSendListNotFoundInDocumentRestrictedSendList(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DocumentSendListDoesNotMatchTheTemplate[lang]";
        public DocumentSendListDoesNotMatchTheTemplate() : base(_MESSAGE)
        {
        }

        public DocumentSendListDoesNotMatchTheTemplate(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentRestrictedSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DocumentRestrictedSendListDoesNotMatchTheTemplate[lang]";
        public DocumentRestrictedSendListDoesNotMatchTheTemplate() : base(_MESSAGE)
        {
        }

        public DocumentRestrictedSendListDoesNotMatchTheTemplate(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class EventNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@EventNotFoundOrUserHasNoAccess[lang]";
        public EventNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public EventNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotPerformThisOperation : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@CouldNotPerformThisOperation[lang]";
        public CouldNotPerformThisOperation() : base(_MESSAGE)
        {
        }

        public CouldNotPerformThisOperation(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class WaitNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@WaitNotFoundOrUserHasNoAccess[lang]";
        public WaitNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public WaitNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class WaitHasAlreadyClosed : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@WaitHasAlreadyClosed[lang]";
        public WaitHasAlreadyClosed() : base(_MESSAGE)
        {
        }

        public WaitHasAlreadyClosed(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DictionaryTagNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DictionaryTagNotFoundOrUserHasNoAccess[lang]";
        public DictionaryTagNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public DictionaryTagNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class ExecutorAgentForPositionIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@ExecutorAgentForPositionIsNotDefined[lang]";
        public ExecutorAgentForPositionIsNotDefined() : base(_MESSAGE)
        {
        }

        public ExecutorAgentForPositionIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение об ошибке, когда невозможно добавить данные в справочник
    /// </summary>
    public class DictionaryRecordCouldNotBeAdded : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DictionaryRecordCouldNotBeAdded[lang]";
        public DictionaryRecordCouldNotBeAdded() : base(_MESSAGE)
        {
        }

        public DictionaryRecordCouldNotBeAdded(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение о невозможности удаления записи из справочника
    /// </summary>
    public class DictionaryRecordCouldNotBeDeleted : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DictionaryRecordCouldNotBeDeleted[lang]";
        public DictionaryRecordCouldNotBeDeleted() : base(_MESSAGE)
        {
        }

        public DictionaryRecordCouldNotBeDeleted(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }


    /// <summary>
    /// Сообщение об ошибке, когда пытаются обновить несуществующую строку справочника в БД
    /// </summary>
    public class DictionaryRecordWasNotFound : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@DictionaryRecordWasNotFound[lang]";
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
        private const string _MESSAGE = "[lang]DmsExceptions@DictionaryRecordNotUnique[lang]";
        public DictionaryRecordNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryRecordNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class IncomingModelIsNotValid : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@IncomingModelIsNotValid[lang]";
        public IncomingModelIsNotValid() : base(_MESSAGE)
        {
        }

        public IncomingModelIsNotValid(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class NotFilledWithAdditionalRequiredAttributes : DmsExceptions
    {
        private const string _MESSAGE = "[lang]DmsExceptions@NotFilledWithAdditionalRequiredAttributes[lang]";
        public NotFilledWithAdditionalRequiredAttributes() : base(_MESSAGE)
        {
        }

        public NotFilledWithAdditionalRequiredAttributes(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
}