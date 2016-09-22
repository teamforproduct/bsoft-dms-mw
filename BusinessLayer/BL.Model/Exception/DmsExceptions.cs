namespace BL.Model.Exception
{
    /*
    С точки зрения самой архитектуры важно чтобы соблюдался один закон - 
    все ошибки должны отлавливаться и обрабатываться на одном уровне (кроме возможно специфических каких-то случаев). 
    Это логический уровень - сервисы и команды. Уровень БД не генерирует ошибок, за исключением непредвиденных ситуаций. 
    На логическом уровне происходит отлов экспешенов нижнего уровня и преобразование их в человеческий формат (удобочитаемый для пользователя), 
    делаются все проверки и в случае неуспешности проверок генерируются наши ошибки.

    второй момент архитектуры: проверки тоже должны быть только на одном уровне. 
    на всех более глубоких уровнях мы считаем что данные туда приходят уже проверенные и достоверные и дополнительно входящие данные уже не проверяем. 
    это делается для того чтобы избежать многочисленных проверок одного и того же. 
    */

    public class DmsExceptions : System.Exception
    {

        public DmsExceptions(string message) : base(message)
        {
        }

        public DmsExceptions(string message, System.Exception ex) : base(message, ex)
        {

        }
    }


    #region [+] LicenceError ... 
    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// </summary>
    public class LicenceError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:LicenceInformationError@l##";
        public LicenceError() : base(_MESSAGE)
        {
        }

        public LicenceError(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// Лицензия просрочена
    /// </summary>
    public class LicenceExpired : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:LicenceExpired@l##";
        public LicenceExpired() : base(_MESSAGE)
        {
        }

        public LicenceExpired(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// Превышено количество зарегистрированных пользователей
    /// </summary>
    public class LicenceExceededNumberOfRegisteredUsers : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:LicenceExceededNumberOfRegisteredUsers@l##";
        public LicenceExceededNumberOfRegisteredUsers() : base(_MESSAGE)
        {
        }

        public LicenceExceededNumberOfRegisteredUsers(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение при ошибке проверки данных о лицензии
    /// Превышено количество подключенных пользователей
    /// </summary>
    public class LicenceExceededNumberOfConnectedUsers : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:LicenceExceededNumberOfConnectedUsers@l##";
        public LicenceExceededNumberOfConnectedUsers() : base(_MESSAGE)
        {
        }

        public LicenceExceededNumberOfConnectedUsers(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
    #endregion

    /// <summary>
    /// Сообщение при ошибке регистрации нового клиента
    /// Имя клиента уже существует
    /// Client Name already exists
    /// </summary>
    public class ClientNameAlreadyExists : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ClientNameAlreadyExists@l##";
        public ClientNameAlreadyExists() : base(_MESSAGE)
        {
        }

        public ClientNameAlreadyExists(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение при проверке проверочного кода клиента
    /// Проверочный код неверен
    /// Verification code is invalid
    /// </summary>
    public class ClientVerificationCodeIncorrect : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ClientVerificationCodeIncorrect@l##";
        public ClientVerificationCodeIncorrect() : base(_MESSAGE)
        {
        }

        public ClientVerificationCodeIncorrect(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение при ошибке работы с клиентом
    /// Клиент не найден
    /// Client not found
    /// </summary>
    public class ClientIsNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ClientIsNotFound@l##";
        public ClientIsNotFound() : base(_MESSAGE)
        {
        }

        public ClientIsNotFound(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение при ошибке регистрации нового клиента
    /// Имя пользователя уже существует
    /// User Name already exists
    /// </summary>
    public class UserNameAlreadyExists : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserNameAlreadyExists@l##";
        public UserNameAlreadyExists() : base(_MESSAGE)
        {
        }

        public UserNameAlreadyExists(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение при ошибке получения ключей
    /// </summary>
    public class CryptographicError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CryptographicError@l##";
        public CryptographicError() : base(_MESSAGE)
        {
        }

        public CryptographicError(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    #region [+] DatabaseError ...
    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DatabaseError@l##";
        public DatabaseError() : base(_MESSAGE)
        {
        }

        public DatabaseError(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseIsNotSet : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DatabaseIsNotSet@l##";
        public DatabaseIsNotSet() : base(_MESSAGE)
        {
        }

        public DatabaseIsNotSet(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение при ошибке работы с базой данных
    /// </summary>
    public class DatabaseIsNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DatabaseIsNotFound@l##";
        public DatabaseIsNotFound() : base(_MESSAGE)
        {
        }

        public DatabaseIsNotFound(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
    #endregion

    /// <summary>
    /// Искомой комманды не существует или она не описана
    /// </summary>
    public class CommandNotDefinedError : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CommandNotDefinedError@l##";
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
        private const string _MESSAGE = "##l@DmsExceptions:WrongParameterValueError@l##";
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
        private const string _MESSAGE = "##l@DmsExceptions:WrongParameterTypeError@l##";
        public WrongParameterTypeError() : base(_MESSAGE)
        {
        }

        public WrongParameterTypeError(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class AccessIsDenied : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AccessIsDenied@l##";
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
        private const string _MESSAGE = "##l@DmsExceptions:DocumentHasAlreadyHasLink@l##";
        //TODO:передавать параметры
        public DocumentHasAlreadyHasLink() : base(_MESSAGE)
        {
        }

        public DocumentHasAlreadyHasLink(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Параметр комманды неверного типа
    /// </summary>
    public class NobodyIsChosen : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:NobodyIsChosen@l##";
        public NobodyIsChosen() : base(_MESSAGE)
        {
        }

        public NobodyIsChosen(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentCannotBeModifiedOrDeleted : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentCannotBeModifiedOrDeleted@l##";
        public DocumentCannotBeModifiedOrDeleted() : base(_MESSAGE)
        {
        }

        public DocumentCannotBeModifiedOrDeleted(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserHasNoAccessToDocument : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserHasNoAccessToDocument@l##";
        public UserHasNoAccessToDocument() : base(_MESSAGE)
        {
        }

        public UserHasNoAccessToDocument(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CannotSaveFile : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CannotSaveFile@l##";
        public CannotSaveFile() : base(_MESSAGE)
        {
        }

        public CannotSaveFile(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserFileNotExists : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserFileNotExists@l##";
        public UserFileNotExists() : base(_MESSAGE)
        {
        }

        public UserFileNotExists(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UnknownDocumentFile : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UnknownDocumentFile@l##";
        public UnknownDocumentFile() : base(_MESSAGE)
        {
        }

        public UnknownDocumentFile(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CannotAccessToFile : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CannotAccessToFile@l##";
        public CannotAccessToFile() : base(_MESSAGE)
        {
        }

        public CannotAccessToFile(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentFileWasChangedExternally : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentFileWasChangedExternally@l##";
        public DocumentFileWasChangedExternally() : base(_MESSAGE)
        {
        }

        public DocumentFileWasChangedExternally(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class TaskNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TaskNotFoundOrUserHasNoAccess@l##";
        public TaskNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public TaskNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
    public class PaperNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:PaperNotFoundOrUserHasNoAccess@l##";
        public PaperNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public PaperNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
    public class PaperListNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:PaperListNotFoundOrUserHasNoAccess@l##";
        public PaperListNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public PaperListNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
    public class DocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentNotFoundOrUserHasNoAccess@l##";
        public DocumentNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public DocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class TemplateDocumentNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TemplateDocumentNotFoundOrUserHasNoAccess@l##";
        public TemplateDocumentNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public TemplateDocumentNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotModifyTemplateDocument : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotModifyTemplateDocument@l##";
        public CouldNotModifyTemplateDocument() : base(_MESSAGE)
        {
        }

        public CouldNotModifyTemplateDocument(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class TemplateDocumentIsNotValid : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TemplateDocumentIsNotValid@l##";
        public TemplateDocumentIsNotValid() : base(_MESSAGE)
        {
        }

        public TemplateDocumentIsNotValid(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentCouldNotBeRegistered : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentCouldNotBeRegistered@l##";
        public DocumentCouldNotBeRegistered() : base(_MESSAGE)
        {
        }

        public DocumentCouldNotBeRegistered(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotChangeAttributeLaunchPlan : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotChangeAttributeLaunchPlan@l##";
        public CouldNotChangeAttributeLaunchPlan() : base(_MESSAGE)
        {
        }

        public CouldNotChangeAttributeLaunchPlan(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotChangeFavourite : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotChangeFavourite@l##";
        public CouldNotChangeFavourite() : base(_MESSAGE)
        {
        }

        public CouldNotChangeFavourite(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotChangeIsInWork : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotChangeIsInWork@l##";
        public CouldNotChangeIsInWork() : base(_MESSAGE)
        {
        }

        public CouldNotChangeIsInWork(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentHasAlredyBeenRegistered : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentHasAlredyBeenRegistered@l##";
        public DocumentHasAlredyBeenRegistered() : base(_MESSAGE)
        {
        }

        public DocumentHasAlredyBeenRegistered(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class PlanPointHasAlredyBeenLaunched : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:PlanPointHasAlredyBeenLaunched@l##";
        public PlanPointHasAlredyBeenLaunched() : base(_MESSAGE)
        {
        }

        public PlanPointHasAlredyBeenLaunched(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserPositionIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserPositionIsNotDefined@l##";
        public UserPositionIsNotDefined() : base(_MESSAGE)
        {
        }

        public UserPositionIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class NeedInformationAboutCorrespondent : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:NeedInformationAboutCorrespondent@l##";
        public NeedInformationAboutCorrespondent() : base(_MESSAGE)
        {
        }

        public NeedInformationAboutCorrespondent(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserNameIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserNameIsNotDefined@l##";
        public UserNameIsNotDefined() : base(_MESSAGE)
        {
        }

        public UserNameIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class UserUnauthorized : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:UserUnauthorized@l##";
        public UserUnauthorized() : base(_MESSAGE)
        {
        }

        public UserUnauthorized(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentRestrictedSendListDuplication : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentRestrictedSendListDuplication@l##";
        public DocumentRestrictedSendListDuplication() : base(_MESSAGE)
        {
        }

        public DocumentRestrictedSendListDuplication(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class WrongDocumentSendListEntry : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:WrongDocumentSendListEntry@l##";
        public WrongDocumentSendListEntry() : base(_MESSAGE)
        {
        }

        public WrongDocumentSendListEntry(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class TargetIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TargetIsNotDefined@l##";
        public TargetIsNotDefined() : base(_MESSAGE)
        {
        }

        public TargetIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class ResponsibleExecutorIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ResponsibleExecutorIsNotDefined@l##";
        public ResponsibleExecutorIsNotDefined() : base(_MESSAGE)
        {
        }

        public ResponsibleExecutorIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class ResponsibleExecutorHasAlreadyBeenDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ResponsibleExecutorHasAlreadyBeenDefined@l##";
        public ResponsibleExecutorHasAlreadyBeenDefined() : base(_MESSAGE)
        {
        }

        public ResponsibleExecutorHasAlreadyBeenDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class ControlerHasAlreadyBeenDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ControlerHasAlreadyBeenDefined@l##";
        public ControlerHasAlreadyBeenDefined() : base(_MESSAGE)
        {
        }

        public ControlerHasAlreadyBeenDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class TaskIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:TaskIsNotDefined@l##";
        public TaskIsNotDefined() : base(_MESSAGE)
        {
        }

        public TaskIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class SubordinationHasBeenViolated : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:SubordinationHasBeenViolated@l##";
        public SubordinationHasBeenViolated() : base(_MESSAGE)
        {
        }

        public SubordinationHasBeenViolated(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentSendListNotFoundInDocumentRestrictedSendList : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentSendListNotFoundInDocumentRestrictedSendList@l##";
        public DocumentSendListNotFoundInDocumentRestrictedSendList() : base(_MESSAGE)
        {
        }

        public DocumentSendListNotFoundInDocumentRestrictedSendList(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentSendListDoesNotMatchTheTemplate@l##";
        public DocumentSendListDoesNotMatchTheTemplate() : base(_MESSAGE)
        {
        }

        public DocumentSendListDoesNotMatchTheTemplate(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DocumentRestrictedSendListDoesNotMatchTheTemplate : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DocumentRestrictedSendListDoesNotMatchTheTemplate@l##";
        public DocumentRestrictedSendListDoesNotMatchTheTemplate() : base(_MESSAGE)
        {
        }

        public DocumentRestrictedSendListDoesNotMatchTheTemplate(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class EventNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:EventNotFoundOrUserHasNoAccess@l##";
        public EventNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public EventNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotPerformOperation : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotPerformThisOperation@l##";
        public CouldNotPerformOperation() : base(_MESSAGE)
        {
        }

        public CouldNotPerformOperation(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class CouldNotPerformOperationWithPaper : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:CouldNotPerformOperationWithPaper@l##";
        public CouldNotPerformOperationWithPaper() : base(_MESSAGE)
        {
        }

        public CouldNotPerformOperationWithPaper(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class WaitNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:WaitNotFoundOrUserHasNoAccess@l##";
        public WaitNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public WaitNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class WaitHasAlreadyClosed : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:WaitHasAlreadyClosed@l##";
        public WaitHasAlreadyClosed() : base(_MESSAGE)
        {
        }

        public WaitHasAlreadyClosed(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class ExecutorAgentForPositionIsNotDefined : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:ExecutorAgentForPositionIsNotDefined@l##";
        public ExecutorAgentForPositionIsNotDefined() : base(_MESSAGE)
        {
        }

        public ExecutorAgentForPositionIsNotDefined(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    #region [+] Dictionary ...
    /// <summary>
    /// Сообщение об ошибке, когда невозможно добавить данные в справочник
    /// </summary>
    public class DictionaryRecordCouldNotBeAdded : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryRecordCouldNotBeAdded@l##";
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
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryRecordCouldNotBeDeleted@l##";
        public DictionaryRecordCouldNotBeDeleted() : base(_MESSAGE)
        {
        }

        public DictionaryRecordCouldNotBeDeleted(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение о невозможности удаления системной записи из справочника
    /// </summary>
    public class DictionarySystemRecordCouldNotBeDeleted : DmsExceptions
    {
        // pss локализация
        private const string _MESSAGE = "##l@DmsExceptions:DictionarySystemRecordCouldNotBeDeleted@l##";
        public DictionarySystemRecordCouldNotBeDeleted() : base(_MESSAGE)
        {
        }

        public DictionarySystemRecordCouldNotBeDeleted(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение об ошибке, когда пытаются обновить несуществующую строку справочника в БД
    /// </summary>
    public class DictionaryRecordWasNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryRecordWasNotFound@l##";
        public DictionaryRecordWasNotFound() : base(_MESSAGE)
        {
        }

        public DictionaryRecordWasNotFound(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Сообщение об ошибке, когда при добавлении или изменении строки в справочнике появится дубль
    /// </summary>
    public class DictionaryRecordNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryRecordNotUnique@l##";
        public DictionaryRecordNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryRecordNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }

    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать имя агента
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentNameNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentNameNotUnique@l##";
        public DictionaryAgentNameNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentNameNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }

    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать паспортные данные физлица
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentPersonPassportNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentPersonPassportNotUnique@l##";
        public DictionaryAgentPersonPassportNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentPersonPassportNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН физлица
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentPersonTaxCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentPersonTaxCodeNotUnique@l##";
        public DictionaryAgentPersonTaxCodeNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentPersonTaxCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать паспортные данные сотрудника
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentEmployeePassportNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentEmployeePassportNotUnique@l##";
        public DictionaryAgentEmployeePassportNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentEmployeePassportNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН сотрудника
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentEmployeeTaxCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentEmployeeTaxCodeNotUnique@l##";
        public DictionaryAgentEmployeeTaxCodeNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentEmployeeTaxCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН сотрудника
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentEmployeePersonnelNumberNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentEmployeePersonnelNumberNotUnique@l##";
        public DictionaryAgentEmployeePersonnelNumberNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentEmployeePersonnelNumberNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }


    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН сотрудника
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentBankMFOCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentBankMFOCodeNotUnique@l##";
        public DictionaryAgentBankMFOCodeNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentBankMFOCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать ИНН юрлица
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentCompanyTaxCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentCompanyTaxCodeNotUnique@l##";
        public DictionaryAgentCompanyTaxCodeNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentCompanyTaxCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать VATCode юрлица
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentCompanyVATCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentCompanyVATCodeNotUnique@l##";
        public DictionaryAgentCompanyVATCodeNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentCompanyVATCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    /// <summary>
    /// Ошибка возникает при попытке задублировать VATCode юрлица
    /// </summary>
    // pss ЛОКАЛИЗАЦИЯ
    public class DictionaryAgentCompanyOKPOCodeNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryAgentCompanyOKPOCodeNotUnique@l##";
        public DictionaryAgentCompanyOKPOCodeNotUnique() : base(_MESSAGE)
        {
        }

        public DictionaryAgentCompanyOKPOCodeNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class DictionaryTagNotFoundOrUserHasNoAccess : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:DictionaryTagNotFoundOrUserHasNoAccess@l##";
        public DictionaryTagNotFoundOrUserHasNoAccess() : base(_MESSAGE)
        {
        }

        public DictionaryTagNotFoundOrUserHasNoAccess(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
    /// <summary>
    /// Сообщение об ошибке, когда при добавлении или изменении строки  появится дубль
    /// </summary>
    #endregion

    #region [+] Admin ...
    // pss добавить сообщения ошибки в локали
    /// <summary>
    /// Сообщение об ошибке, когда при добавлении или изменении строки появится дубль
    /// </summary>
    public class AdminRecordNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AdminRecordNotUnique@l##";
        public AdminRecordNotUnique() : base(_MESSAGE) { }
        public AdminRecordNotUnique(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение об ошибке, когда невозможно добавить данные в админку
    /// </summary>
    public class AdminRecordCouldNotBeAdded : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AdminRecordCouldNotBeAdded@l##";
        public AdminRecordCouldNotBeAdded() : base(_MESSAGE) { }
        public AdminRecordCouldNotBeAdded(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение о невозможности удаления записи в админке
    /// </summary>
    public class AdminRecordCouldNotBeDeleted : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AdminRecordCouldNotBeDeleted@l##";
        public AdminRecordCouldNotBeDeleted() : base(_MESSAGE) { }
        public AdminRecordCouldNotBeDeleted(System.Exception ex) : base(_MESSAGE, ex) { }
    }

    /// <summary>
    /// Сообщение об ошибке, когда пытаются обновить несуществующую строку справочника в БД
    /// </summary>
    public class AdminRecordWasNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:AdminRecordWasNotFound@l##";
        public AdminRecordWasNotFound() : base(_MESSAGE) { }
        public AdminRecordWasNotFound(System.Exception ex) : base(_MESSAGE, ex) { }
    }
    #endregion

    public class RecordNotUnique : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:RecordNotUnique@l##";
        public RecordNotUnique() : base(_MESSAGE)
        {
        }

        public RecordNotUnique(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class IncomingModelIsNotValid : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:IncomingModelIsNotValid@l##";
        public IncomingModelIsNotValid() : base(_MESSAGE)
        {
        }

        public IncomingModelIsNotValid(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class NotFilledWithAdditionalRequiredAttributes : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:NotFilledWithAdditionalRequiredAttributes@l##";
        public NotFilledWithAdditionalRequiredAttributes() : base(_MESSAGE)
        {
        }

        public NotFilledWithAdditionalRequiredAttributes(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class SigningTypeNotAllowed : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:SigningTypeNotAllowed@l##";
        public SigningTypeNotAllowed() : base(_MESSAGE)
        {
        }

        public SigningTypeNotAllowed(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class EncryptionCertificateWasNotFound : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:EncryptionCertificateWasNotFound@l##";
        public EncryptionCertificateWasNotFound() : base(_MESSAGE)
        {
        }

        public EncryptionCertificateWasNotFound(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class EncryptionCertificateHasExpired : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:EncryptionCertificateHasExpired@l##";
        public EncryptionCertificateHasExpired() : base(_MESSAGE)
        {
        }

        public EncryptionCertificateHasExpired(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }

    public class EncryptionCertificatePrivateKeyСanNotBeExported : DmsExceptions
    {
        private const string _MESSAGE = "##l@DmsExceptions:EncryptionCertificatePrivateKeyСanNotBeExported@l##";
        public EncryptionCertificatePrivateKeyСanNotBeExported() : base(_MESSAGE)
        {
        }

        public EncryptionCertificatePrivateKeyСanNotBeExported(System.Exception ex) : base(_MESSAGE, ex)
        {
        }
    }
}