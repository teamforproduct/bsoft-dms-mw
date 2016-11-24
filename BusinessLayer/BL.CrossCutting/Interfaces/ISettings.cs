using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;
using System;

namespace BL.CrossCutting.Interfaces
{
    public interface ISettings
    {
        //TValue GetSetting<TValue>(IContext ctx, string settingName) where TValue : IConvertible;
        //TValue GetSetting<TValue>(IContext ctx, string settingName, TValue defaulValue) where TValue : IConvertible;
        //TValue GetSetting<TValue>(IContext ctx, string settingKey, InternalSystemSetting defaulValue) where TValue : IConvertible;
        void SaveSetting(IContext ctx, InternalSystemSetting val);
        void ClearCache(IContext ctx);
        void TotalClear();
        object GetTypedValue(string Value, EnumValueTypes ValueType);



        bool GetSubordinationsSendAllForExecution(IContext ctx);
        bool GetSubordinationsSendAllForInforming(IContext ctx);
        int GetMailTimeoutMin(IContext ctx);
        MailServerType GetMailInfoServerType(IContext ctx);


        string GetMailInfoSystemMail(IContext ctx);

        string GetMailInfoName(IContext ctx);

        string GetMailInfoLogin(IContext ctx);

        string GetMailInfoPassword(IContext ctx);

        int GetMailInfoPort(IContext ctx);

        bool GetDigitalSignatureIsUseCertificateSign(IContext ctx);

        bool GetDigitalSignatureIsUseInternalSign(IContext ctx);

        string GetFulltextDatastorePath(IContext ctx);

        int GetFulltextRefreshTimeout(IContext ctx);

        bool GetFulltextWasInitialized(IContext ctx);

        string GetFileStorePath(IContext ctx);

        string GetReportDocumentForDigitalSignature(IContext ctx);

        string GetReportRegisterTransmissionDocuments(IContext ctx);

        string GetReportRegistrationCardIncomingDocument(IContext ctx);

        string GetReportRegistrationCardInternalDocument(IContext ctx);

        string GetReportRegistrationCardOutcomingDocument(IContext ctx);


        int GetAutoplanTimeoutMinute(IContext ctx);

        int GetClearTrashDocumentsTimeoutMinute(IContext ctx);

        int GetClearTrashDocumentsTimeoutMinuteForClear(IContext ctx);

    }
}