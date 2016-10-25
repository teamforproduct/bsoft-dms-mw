namespace BL.Model.Enums
{
    public enum EnumSystemActions
    {
        SetSetting = 900001, 
        Login = 0, //Вход в систему

        // При добавлении действия не забудь добавить действие в ImportData: BL.Database.DatabaseContext.DmsDbImportData!!!
    }
}