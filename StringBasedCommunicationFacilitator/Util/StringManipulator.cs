using StringBasedCommunicationFacilitator.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace StringBasedCommunicationFacilitator.Util
{
    public enum Alignment
    {
        LEFT,
        RIGHT
    }

    public static class StringManipulator
    {
        public static IEnumerable<string> SplitBy(this string str, int chunkLength)
        {
            if (str == null)
                throw new FacilitatorException(555);

            if (chunkLength < 1)
                throw new FacilitatorException(556);

            int remainder = str.Length % chunkLength;
            int totallen = str.Length + (remainder > 0 ? (chunkLength - remainder) : 0);
            str = str.PadRight(totallen, ' ');

            for (int i = 0; i < str.Length; i += chunkLength)
            {
                if (chunkLength + i > str.Length)
                    chunkLength = str.Length - i;

                yield return str.Substring(i, chunkLength);
            }
        }

        public static string ToCustomString(this ICustomType val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            string result = string.Empty;
            if (val != null)
            {
                result = val.ToCustomString();
            }
            return result;
        }

        public static string ToCustomString(this ICustomType[] val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            StringBuilder result = new StringBuilder();
            if (val != null)
            {
                int i = 0;
                foreach (var item in val)
                {
                    result.Append(item.ToCustomString());

                    if (++i >= length)
                        break;
                }
            }
            return result.ToString();
        }

        public static string ToCustomString(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            string result = val ?? string.Empty;
            result = alignment == Alignment.RIGHT ? result.PadLeft(length, paddingChar) : result.PadRight(length, paddingChar);
            result = result.Substring(0, length);
            return result;
        }

        public static string ToCustomString(this bool val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            string[] formatVals = format.Split(';');
            string result = val ? formatVals[0] : formatVals[1];
            return result;
        }

        // format = "000000000;-00000000"; sign for only negative numbers
        // format = "+00000000;-00000000"; sign for all numbers including zero
        // format = "+00000000;-00000000;000000000"; sign for all number excluding zero
        public static string ToCustomString(this int val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            string result = val.ToString(format, CultureInfo.InvariantCulture);
            return result;
        }

        // format = "000000.000;-00000.000"; sign for only negative numbers
        // format = "+00000.000;-00000.000"; sign for all numbers including zero
        // format = "+00000.000;-00000.000;000000.000"; sign for all number excluding zero
        public static string ToCustomString(this decimal val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            string result = val.ToString(format, CultureInfo.InvariantCulture);
            return ignoreDecimalPoint ? result = result.Replace(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator, "") : result;
        }

        public static string ToCustomString(this DateTime val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            return val.ToString(format, CultureInfo.InvariantCulture);
        }

        public static string ToCustomString(this TimeSpan val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            return val.ToString(format, CultureInfo.InvariantCulture);
        }

        public static string ToCustomString(this bool? val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            if (val.HasValue)
                return val.Value.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);

            string result = null;
            return result.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);
        }

        // format = "000000000;-00000000"; sign for only negative numbers
        // format = "+00000000;-00000000"; sign for all numbers including zero
        // format = "+00000000;-00000000;000000000"; sign for all number excluding zero
        public static string ToCustomString(this int? val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            if (val.HasValue)
                return val.Value.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);

            string result = null;
            return result.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);
        }

        // format = "000000.000;-00000.000"; sign for only negative numbers
        // format = "+00000.000;-00000.000"; sign for all numbers including zero
        // format = "+00000.000;-00000.000;000000.000"; sign for all number excluding zero
        public static string ToCustomString(this decimal? val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            if (val.HasValue)
                return val.Value.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);

            string result = null;
            return result.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);
        }

        public static string ToCustomString(this DateTime? val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            if (val.HasValue)
                return val.Value.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);

            string result = null;
            return result.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);
        }

        public static string ToCustomString(this TimeSpan? val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            if (val.HasValue)
                return val.Value.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);

            string result = null;
            return result.ToCustomString(length, alignment, paddingChar, format, ignoreDecimalPoint);
        }

        public static T ToCSharpType<T>(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            T result = default(T);

            object temp = null;

            var nullableType = Nullable.GetUnderlyingType(typeof(T));
            bool isNullableType = nullableType != null;
            string typeName = isNullableType ? nullableType.Name : typeof(T).Name;

            if (isNullableType)
            {
                switch (typeName)
                {
                    case "Boolean":
                        temp = ToBoolNullableExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    case "Int32":
                        temp = ToIntNullableExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    case "Decimal":
                        temp = ToDecimalNullableExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    case "DateTime":
                        temp = ToDateTimeNullableExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    case "TimeSpan":
                        temp = ToTimeSpanNullableExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    default:
                        throw new FacilitatorException(561, "type: [0]", typeName);
                }
            }
            else
            {
                switch (typeName)
                { 
                    case "String":
                        temp = ToStringExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    case "Boolean":
                        temp = ToBoolExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    case "Int32":
                        temp = ToIntExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    case "Decimal":
                        temp = ToDecimalExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    case "DateTime":
                        temp = ToDateTimeExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                    case "TimeSpan":
                        temp = ToTimeSpanExt(val, length, alignment, paddingChar, format, ignoreDecimalPoint);
                        break;
                }
            }

            if (temp != null)
            {
                result = (T)Convert.ChangeType(temp, isNullableType ? nullableType : typeof(T));
            }

            return result;
        }

        public static T ToCustomType<T>(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint) where T : ICustomType, new()
        {
            T result = default(T);
            if (val != null)
            {
                result = new T();
                result.FromCustomString(val);
            }
            return result;
        }

        public static T[] ToCustomTypeArray<T>(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint) where T : ICustomType, new()
        {
            T[] result = default(T[]);
            if (val != null)
            {
                List<T> items = new List<T>();
                T instance = new T();
                foreach (string str in val.SplitBy(instance.Length))
                {
                    if (string.IsNullOrWhiteSpace(str))
                        continue;

                    instance = new T();
                    instance.FromCustomString(str);
                    items.Add(instance);
                }
                result = items.ToArray();
            }
            return result;
        }

        public static string ToStringExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            string result = string.Empty;
            if (val != null)
            {
                result = val.Trim();
            }
            return result;
        }

        public static bool ToBoolExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(val))
            {
                string[] formatVals = format.Split(';');
                result = formatVals[0] == val;
            }
            return result;
        }

        public static int ToIntExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            int result = 0;
            int.TryParse(val, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static decimal ToDecimalExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            decimal result = 0;
            decimal.TryParse(val, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
            if (result % 1 == 0 && ignoreDecimalPoint && format.Split(';')[0].Contains('.'))
            {
                string[] formatVals = format.Split(';')[0].Split('.');
                int scale = formatVals[formatVals.Length - 1].Length;
                result = result / (decimal)Math.Pow(10, scale);
            }
            return result;
        }

        public static DateTime ToDateTimeExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            DateTime result;
            DateTime.TryParseExact(val, format, null, DateTimeStyles.None, out result);
            return result;
        }

        public static TimeSpan ToTimeSpanExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            TimeSpan result;
            TimeSpan.TryParseExact(val, format, null, TimeSpanStyles.None, out result);
            return result;
        }

        public static bool? ToBoolNullableExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            if (!string.IsNullOrWhiteSpace(val))
            {
                string[] formatVals = format.Split(';');
                if (val == formatVals[0] || val == formatVals[1])
                {
                    return val.ToBoolExt(length, alignment, paddingChar, format, ignoreDecimalPoint);
                }
            }
            return null;
        }

        public static int? ToIntNullableExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            int result = 0;
            if (!string.IsNullOrWhiteSpace(val))
            {
                if (int.TryParse(val, NumberStyles.None, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
            }
            return null;
        }

        public static decimal? ToDecimalNullableExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            decimal result = 0m;
            if (!string.IsNullOrWhiteSpace(val))
            {
                if (decimal.TryParse(val, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
            }
            return null;
        }

        public static DateTime? ToDateTimeNullableExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            DateTime result;
            if (DateTime.TryParseExact(val, format, null, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

        public static TimeSpan? ToTimeSpanNullableExt(this string val, int length, Alignment alignment, char paddingChar, string format, bool ignoreDecimalPoint)
        {
            TimeSpan result;
            if (TimeSpan.TryParseExact(val, format, null, TimeSpanStyles.None, out result))
            {
                return result;
            }
            return null;
        }
    }
}