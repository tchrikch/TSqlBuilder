using System.Collections.Generic;
using System.Linq;
using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Extensions
{
    public class Conditions
    {
        public static string In(string column,IEnumerable<object> values)
        {
            return string.Format("{0} {1} ({2})", column.TryQuote(), KeyWords.In, string.Join(KeyWords.Comma,values.Select(v=>v.ToString())));
        }

        public static string Like(string column ,string value,LikeMode mode)
        {
            string localValue;
            switch (mode)
            {
                case LikeMode.Left:
                    localValue = "'%" + value + "'";
                    break;
                case LikeMode.Right:
                    localValue = "'" + value + "%'";
                    break;
                default:
                    localValue = "'%" + value + "%'";
                    break;
            }

            return string.Format("{0} {1} ({2})", column.TryQuote(), KeyWords.Like, localValue);
        }
    }
}