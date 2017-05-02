using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace DMS_WebAPI.Utilities
{
    public static class StringExtentions
    {
        public static int ValueAsInt(this string data)
        {
            if (data.HasValue())
            {
                int result;
                if (int.TryParse(data, out result))
                {
                    return result;
                };
            }

            return default(int);
        }

        public static bool HasValue(this string source)
        {
            return !string.IsNullOrEmpty(source);
        }

        public static string NoNewLine(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            return source.Replace(Environment.NewLine, " ");
        }


        public static string NoQuotes(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            StringBuilder builder = new StringBuilder(source);
            builder.Replace("\"", "&#34;").Replace("'", "&#39;");
            return builder.ToString();
        }

        public static string Transliterate(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            Dictionary<string, string> comas = new Dictionary<string, string>()
                                                   {
                                                       {"-", "-"}
                                                   };

            Dictionary<string, string> dictionary = new Dictionary<string, string>()
                                                        {

                                                            {"-", "-"},
                                                            {" ", "-"},

			                                        		// цифры
			                                        		{"1", "1"},
                                                            {"2", "2"},
                                                            {"3", "3"},
                                                            {"4", "4"},
                                                            {"5", "5"},
                                                            {"6", "6"},
                                                            {"7", "7"},
                                                            {"8", "8"},
                                                            {"9", "9"},
                                                            {"0", "0"},

			                                        		// нижний регистр
			                                        		// однобуквенные замены
			                                        		{"а", "a"},
                                                            {"б", "b"},
                                                            {"в", "v"},
                                                            {"г", "g"},
                                                            {"д", "d"},
                                                            {"е", "e"},
                                                            {"є", "e"},
                                                            {"з", "z"},
                                                            {"и", "i"},
                                                            {"і", "i"},
                                                            {"ї", "i"},
                                                            {"й", "j"},
                                                            {"к", "k"},
                                                            {"л", "l"},
                                                            {"м", "m"},
                                                            {"н", "n"},
                                                            {"о", "o"},
                                                            {"п", "p"},
                                                            {"р", "r"},
                                                            {"с", "s"},
                                                            {"т", "t"},
                                                            {"у", "u"},
                                                            {"ф", "f"},
                                                            {"х", "h"},
                                                            {"э", "e"},
			                                        		// трехбуквенные замены
			                                        		{"щ", "sch"},
			                                        		// двухбуквенные замены
			                                        		{"ё", "yo"},
                                                            {"ж", "zh"},
                                                            {"ц", "ts"},
                                                            {"ч", "ch"},
                                                            {"ш", "sh"},
                                                            {"ы", "yi"},
                                                            {"ю", "yu"},
                                                            {"я", "ya"},

			                                        		// верхний регистр
			                                        		// однобуквенные замены
			                                        		{"А", "a"},
                                                            {"Б", "b"},
                                                            {"В", "v"},
                                                            {"Г", "g"},
                                                            {"Д", "d"},
                                                            {"Е", "e"},
                                                            {"Є", "e"},
                                                            {"З", "z"},
                                                            {"И", "i"},
                                                            {"І", "i"},
                                                            {"Ї", "i"},
                                                            {"Й", "j"},
                                                            {"К", "k"},
                                                            {"Л", "l"},
                                                            {"М", "m"},
                                                            {"Н", "n"},
                                                            {"О", "o"},
                                                            {"П", "p"},
                                                            {"Р", "r"},
                                                            {"С", "s"},
                                                            {"Т", "t"},
                                                            {"У", "u"},
                                                            {"Ф", "f"},
                                                            {"Х", "h"},
                                                            {"Э", "e"},
			                                        		// трехбуквенные замены
			                                        		{"Щ", "sch"},
			                                        		// двухбуквенные замены
			                                        		{"Ё", "yo"},
                                                            {"Ж", "zh"},
                                                            {"Ц", "ts"},
                                                            {"Ч", "ch"},
                                                            {"Ш", "sh"},
                                                            {"Ы", "yi"},
                                                            {"Ю", "yu"},
                                                            {"Я", "ya"},

			                                        		// английские буквы
			                                        		// нижний регистр
			                                        		{"a", "a"},
                                                            {"b", "b"},
                                                            {"c", "c"},
                                                            {"d", "d"},
                                                            {"e", "e"},
                                                            {"f", "f"},
                                                            {"g", "g"},
                                                            {"h", "h"},
                                                            {"i", "i"},
                                                            {"j", "j"},
                                                            {"k", "k"},
                                                            {"l", "l"},
                                                            {"m", "m"},
                                                            {"n", "n"},
                                                            {"o", "o"},
                                                            {"p", "p"},
                                                            {"q", "q"},
                                                            {"r", "r"},
                                                            {"s", "s"},
                                                            {"t", "t"},
                                                            {"u", "u"},
                                                            {"v", "v"},
                                                            {"w", "w"},
                                                            {"x", "x"},
                                                            {"y", "y"},
                                                            {"z", "z"},
			                                        		// верхний регистр
			                                        		{"A", "a"},
                                                            {"B", "b"},
                                                            {"C", "c"},
                                                            {"D", "d"},
                                                            {"E", "e"},
                                                            {"F", "f"},
                                                            {"G", "g"},
                                                            {"H", "h"},
                                                            {"I", "i"},
                                                            {"J", "j"},
                                                            {"K", "k"},
                                                            {"L", "l"},
                                                            {"M", "m"},
                                                            {"N", "n"},
                                                            {"O", "o"},
                                                            {"P", "p"},
                                                            {"Q", "q"},
                                                            {"R", "r"},
                                                            {"S", "s"},
                                                            {"T", "t"},
                                                            {"U", "u"},
                                                            {"V", "v"},
                                                            {"W", "w"},
                                                            {"X", "x"},
                                                            {"Y", "y"},
                                                            {"Z", "z"}
                                                        };

            string res = Regex.Replace(source, "(" + String.Join("|", dictionary.Keys.ToArray()) + ")", (m) => dictionary[m.Value]);
            res = Regex.Replace(res, "[^" + String.Join("", dictionary.Keys.ToArray()) + "\\.]", string.Empty);
            foreach (KeyValuePair<string, string> item in comas)
            {
                res = Regex.Replace(res, item.Key + "{2,}", item.Value);
            }
            res = res.Replace('.', '-').Trim('-');
            return res;
        }

        private static Regex _rgReplaceQuote = new Regex(String.Format("(&#34;)"
                                                                       + "|(»)|(&raquo;)|(&#187;)" + "|(«)|(&laquo;)|(&#171;)"
                                                                       + "|(‘)|(&lsquo;)|(&#8216;)" + "|(?:(’){0})|(?:(&rsquo;){0})|(?:(&#8217;){0})" + "|(‚)|(&sbquo;)|(&#8218;)"
                                                                       + "|(“)|(&ldquo;)|(&#8220;)" + "|(”)|(&rdquo;)|(&#8221;)" + "|(„)|(&bdquo;)|(&#8222;)"
                                                                       , "(?:(?: )|&nbsp;|&#160;)"));

        public static string ReplaceQuote(this string s)
        {
            return _rgReplaceQuote.Replace(s, "&quot;");
        }

        public static string RemoveQuotes(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return s.Replace("'", string.Empty).Replace("&quot;", string.Empty);
            }
            return s;
        }

        public static string Truncate(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            source = source.Trim();

            source = HttpUtility.HtmlDecode(source);


            if (source.Length <= length)
            {
                return source;
            }

            var words = source.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
            {
                return source;
            }

            string join = string.Join(" ", words);
            if (join.Length <= length)
            {
                return join;
            }

            StringBuilder stringBuilder = new StringBuilder(words[0]);
            for (int i = 1; i < words.Length; i++)
            {
                if (stringBuilder.Length + words[i].Length + 1 < length)
                {
                    stringBuilder.Append(" " + words[i]);
                }
                else
                {
                    break;
                }
            }
            stringBuilder.Append(" " + "...");
            return stringBuilder.ToString();
        }

        public static string TruncateHard(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            source = source.Trim();

            //source = HttpUtility.HtmlDecode(source);

            if (source.Length <= length)
            {
                return source;
            }

            return source.Substring(0, length - 1);

        }

        public static string ToQueryString(this Dictionary<string, string> qs)
        {
            return string.Join("&", qs.Select(e => string.Format("{0}={1}", HttpUtility.UrlEncode(e.Key), HttpUtility.UrlEncode(e.Value))));
        }

        public static string md5(this string text)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] bSignature = md5.ComputeHash(Encoding.ASCII.GetBytes(text));
            return string.Join(string.Empty, bSignature.Select(e => string.Format("{0:x2}", e)).ToArray());
        }

        public static bool IsEmpty(this string aString)
        {
            return string.IsNullOrEmpty(aString);
        }

        public static string DecodeReturnUrl(this string returnUrl)
        {
            return HttpUtility.UrlDecode(returnUrl.Replace("_", "%"));
        }

        public static string EncodeReturnUrl(this string returnUrl)
        {
            return HttpUtility.UrlEncode(returnUrl)
                .Replace("_", "%5F")
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("*", "%2A")
                .Replace("+", "%2B")
                .Replace("!", "%21")
                .Replace("-", "%2D")
                .Replace(":", "%3A")
                .Replace(";", "%3B")
                .Replace("<", "%3C")
                .Replace("=", "%3D")
                .Replace(">", "%3E")
                .Replace("?", "%3F")
                .Replace("@", "%40")
                .Replace("%", "_");
        }

        public static string ReadAllText(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                return sr.ReadToEnd();
            }
        }

        public static string ToXml(this object obj)
        {
            var serializer = new DataContractSerializer(obj.GetType());
            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(stringBuilder))
            {
                serializer.WriteObject(xmlWriter, obj);
            }

            return stringBuilder.ToString();
        }

        public static string ToXPathClassSelector(this string className)
        {
            return "[contains(concat(' ', normalize-space(@class), ' '), ' " + className + " ')]";
        }
        public static string ToXPathClassSelectorNot(this string className)
        {
            return "[not(contains(concat(' ', normalize-space(@class), ' '), ' " + className + " '))]";
        }

        //public static string UserNameFormatByClientCode(this string userName, string clientCode)
        //{
        //    if (!string.IsNullOrEmpty(clientCode))
        //    {
        //        var dbProc = new WebAPIDbProcess();
        //        var client = dbProc.GetClient(clientCode);
        //        if (client != null)
        //            userName = userName.UserNameFormatByClientId(client.Id);
        //    }

        //    return userName;
        //}

        //public static string UserNameFormatByClientId(this string userName, int clientId)
        //{

        //    if (clientId > 0)
        //    {
        //        userName = $"Client_{clientId}_{userName}";
        //    }

        //    return userName;
        //}
    }
}