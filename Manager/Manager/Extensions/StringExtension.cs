namespace Manager.Extensions
{
    public static class StringExtension
    {
        public static bool IsDigitsOnly(this string str)
        {
            foreach (char c in str)
            {
                if (!(IsNumber(c) || IsDotOrComma(c)))
                    return false;
            }
            return true;
        }

        private static bool IsNumber(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsDotOrComma(char c)
        {
            return c == ',' || c == '.';
        }
    }
}