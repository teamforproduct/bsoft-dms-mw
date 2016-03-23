namespace BL.Model.Enums
{
    public enum EnumSendTypes
    {
        SendForResponsibleExecution = 10,
        SendForControl = 20,
        SendForExecution = 30,
        SendForInformation = 40,
        SendForInformationExternal = 45, //Для сведения внешнему агенту
        SendForConsideration = 50,
        SendForVisaing = 250,
        SendForАgreement = 260,
        SendForАpproval = 270,
        SendForSigning = 280
    }
}