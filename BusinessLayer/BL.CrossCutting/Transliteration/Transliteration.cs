using System.Collections.Generic;

namespace BL.CrossCutting.Transliteration
{
    public enum TransliterationType
    {
        Gost,
        ISO
    }
    public static class Transliteration
    {
        private static Dictionary<string, string> _Gost =  //ГОСТ 16876-71
            new Dictionary<string, string> {{"Є", "EH"},{"І", "I"},{"і", "i"},{"№", "#"},{"є", "eh"},{"А", "A"},
            {"Б", "B"},{"В", "V"},{"Г", "G"},{"Д", "D"},{"Е", "E"},{"Ё", "JO"},{"Ж", "ZH"},{"З", "Z"},{"И", "I"},
            {"Й", "JJ"},{"К", "K"},{"Л", "L"},{"М", "M"},{"Н", "N"},{"О", "O"},{"П", "P"},{"Р", "R"},{"С", "S"},
            {"Т", "T"},{"У", "U"},{"Ф", "F"},{"Х", "KH"},{"Ц", "C"},{"Ч", "CH"},{"Ш", "SH"},{"Щ", "SHH"},{"Ъ", "'"},
            {"Ы", "Y"},{"Ь", ""},{"Э", "EH"},{"Ю", "YU"},{"Я", "YA"},{"а", "a"},{"б", "b"},{"в", "v"},{"г", "g"},
            {"д", "d"},{"е", "e"},{"ё", "jo"},{"ж", "zh"},{"з", "z"},{"и", "i"},{"й", "jj"},{"к", "k"},{"л", "l"},
            {"м", "m"},{"н", "n"},{"о", "o"},{"п", "p"},{"р", "r"},{"с", "s"},{"т", "t"},{"у", "u"},{"ф", "f"},
            {"х", "kh"},{"ц", "c"},{"ч", "ch"},{"ш", "sh"},{"щ", "shh"},{"ъ", ""},{"ы", "y"},{"ь", ""},{"э", "eh"},
            {"ю", "yu"},{"я", "ya"},{"«", ""},{"»", ""},{"—", "-"}};
        private static Dictionary<string, string> _ISO =  //ISO 9-95
            new Dictionary<string, string> {{"Є", "YE"},{"І", "I"},{"Ѓ", "G"},{"і", "i"},{"№", "#"},{"є", "ye"},
            {"ѓ", "g"},{"А", "A"},{"Б", "B"},{"В", "V"},{"Г", "G"},{"Д", "D"},{"Е", "E"},{"Ё", "YO"},{"Ж", "ZH"},
            {"З", "Z"},{"И", "I"},{"Й", "J"},{"К", "K"},{"Л", "L"},{"М", "M"},{"Н", "N"},{"О", "O"},{"П", "P"},
            {"Р", "R"},{"С", "S"},{"Т", "T"},{"У", "U"},{"Ф", "F"},{"Х", "X"},{"Ц", "C"},{"Ч", "CH"},{"Ш", "SH"},
            {"Щ", "SHH"},{"Ъ", "'"},{"Ы", "Y"},{"Ь", ""},{"Э", "E"},{"Ю", "YU"},{"Я", "YA"},{"а", "a"},{"б", "b"},
            {"в", "v"},{"г", "g"},{"д", "d"},{"е", "e"},{"ё", "yo"},{"ж", "zh"},{"з", "z"},{"и", "i"},{"й", "j"},
            {"к", "k"},{"л", "l"},{"м", "m"},{"н", "n"},{"о", "o"},{"п", "p"},{"р", "r"},{"с", "s"},{"т", "t"},
            {"у", "u"},{"ф", "f"},{"х", "x"},{"ц", "c"},{"ч", "ch"},{"ш", "sh"},{"щ", "shh"},{"ъ", ""},{"ы", "y"},
            {"ь", ""},{"э", "e"},{"ю", "yu"},{"я", "ya"},{"«", ""},{"»", ""},{"—", "-"} };
        public static string ToTransliteration(this string text)
        {
            return text.ToTransliteration(TransliterationType.ISO);
        }
        public static string ToTransliteration(this string text, TransliterationType type)
        {
            foreach (var key in GetDictionary(type))
                text = text.Replace(key.Key, key.Value);
            return text;
        }
        public static string BackTransliteration(this string text)
        {
            return text.BackTransliteration(TransliterationType.ISO);
        }
        public static string BackTransliteration(this string text, TransliterationType type)
        {
            foreach (var key in GetDictionary(type))
                text = text.Replace(key.Value, key.Key);
            return text;
        }

        private static Dictionary<string, string> GetDictionary(TransliterationType type)
        {
            switch (type)
            {
                case TransliterationType.Gost:
                    return _Gost;
                case TransliterationType.ISO:
                default:
                    return _ISO;
            }
        }
    }
}
