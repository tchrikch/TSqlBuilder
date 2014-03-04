namespace TSqlBuilder.Extensions
{
    public static class StringExtensions
    {
        public static string Quote(this string @string)
        {
            return string.Format("[{0}]", @string);
        }

        public static string TryQuote(this string @string)
        {
            string result;

            if (@string.Length > 1 && @string[0] == '[' && @string[@string.Length - 1] == ']')
                result = @string;
            else result = string.Format("[{0}]", @string);

            return result;
        }

        public static string WrapWithParenthesis(this string @string)
        {
            return string.Format("({0})",@string);
        }

        public static string Expand(this string @string,string value=" ")
        {
            return string.Format("{0}{1}{0}", value, @string);
        }

        public static string ExpandLeft(this string @string, string value = " ")
        {
            return string.Format("{0}{1}", value, @string);
        }

    }
}