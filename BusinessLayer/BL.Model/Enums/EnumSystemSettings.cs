namespace BL.Model.Enums
{
    public enum EnumSystemSettings
    {
        CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE_FOR_CLEAR,
        DIGITAL_SIGNATURE_IS_USE_CERTIFICATE_SIGN,
        DIGITAL_SIGNATURE_IS_USE_INTERNAL_SIGN,
        FILE_STORE_TEMPLATE_REPORT_FILE_DocumentForDigitalSignature,
        FILE_STORE_TEMPLATE_REPORT_FILE_RegisterTransmissionDocuments,
        FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardIncomingDocument,
        FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardInternalDocument,
        FILE_STORE_TEMPLATE_REPORT_FILE_RegistrationCardOutcomingDocument,
        FULLTEXTSEARCH_WAS_INITIALIZED,
        RUN_AUTOPLAN_TIMEOUT_MINUTE,
        RUN_CLEARTRASHDOCUMENTS_TIMEOUT_MINUTE,
        CLEARTRASHFILES_TIMEOUT_DAY_FOR_CLEAR,
        SUBORDINATIONS_SEND_ALL_FOR_EXECUTION,
        SUBORDINATIONS_SEND_ALL_FOR_INFORMING,
        OLDPDFDELETEPERIOD,

        // При добавлении новых настроек не забудь добавить значения по умолчанию в SettingsFactory
    }
}