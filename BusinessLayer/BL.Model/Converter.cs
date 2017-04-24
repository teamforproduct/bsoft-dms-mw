namespace BL.Model
{
    public static class Converter
    {

        public static string ToBase64String(byte[] inArray)
        {
            string fileContect = string.Empty;

            if (inArray != null) fileContect = System.Convert.ToBase64String(inArray);

            return fileContect;
        }
    }
}
