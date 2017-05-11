using BL.Model.Enums;
using System;

namespace BL.CrossCutting.Helpers
{
    public class TypedValues
    {

        public object GetTypedValue(string Value, EnumValueTypes ValueType)
        {
            object res;

            if (string.IsNullOrEmpty(Value)) return null;

            switch (ValueType)
            {
                case EnumValueTypes.Text:
                case EnumValueTypes.Api:
                case EnumValueTypes.Password:
                    res = Value;
                    break;
                case EnumValueTypes.Number:
                    res = Int32.Parse(Value);
                    break;
                case EnumValueTypes.Date:
                    res = DateTime.Parse(Value);
                    break;
                case EnumValueTypes.Bool:
                    res = Boolean.Parse(Value);
                    break;
                default:
                    res = Value;
                    break;
            }

            return res;
        }
    }
}
