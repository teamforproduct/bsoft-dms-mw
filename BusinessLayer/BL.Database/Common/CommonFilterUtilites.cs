namespace BL.Database.Common
{
    public static class CommonFilterUtilites
    {
        public static string[] GetWhereExpressions(string SearchExpression)
        {
            var res = SearchExpression.Trim().ToLower();

            while (res.Contains("  "))
            {
                res = res.Replace("  ", " ");
            }

            return res.Split(' ');
        }

    }
}
