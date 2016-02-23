namespace BL.Logic.Helpers
{
    public static class DmsUtilities
    {
        public static string RegistrationFullNumber(bool IsRegistered, int? RegistrationNumber, string RegistrationNumberPrefix, string RegistrationNumberSuffix, int Id)
        {
            return
                !IsRegistered
                        ? "#"
                        : "" +
                          RegistrationNumber != null
                            ? RegistrationNumberPrefix + RegistrationNumber.ToString() +
                              RegistrationNumberSuffix
                            : "#" + Id.ToString();
        }
    }
}