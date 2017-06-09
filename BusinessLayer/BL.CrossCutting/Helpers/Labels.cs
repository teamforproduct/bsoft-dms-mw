namespace BL.CrossCutting.Helpers
{
    public static class Labels
    {
        public static string FirstSigns = "##l@";
        public static string LastSigns = "@l##";
        public static string Delimiter = ".";

        public static string GetEnumName<T>() => typeof(T).Name.Replace("Enum", "");

        public static string Get(string module) => FirstSigns + module.Trim() + LastSigns;

        public static string Get(string module, string item) => FirstSigns + module.Trim() + Delimiter + item.Trim() + LastSigns;

        public static string Get(string module, string item1, string item2) => FirstSigns + module.Trim() + Delimiter + item1.Trim() + Delimiter + item2.Trim() + LastSigns;
    }
}
