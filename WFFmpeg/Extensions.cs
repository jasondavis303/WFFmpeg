using System;
using System.Collections.Generic;
using System.Linq;

namespace WFFmpeg
{
    static class Extensions
    {
        public static bool ICEquals(this string s, string compare)
        {
            if (s == null && compare == null)
                return true;

            if (s == null || compare == null)
                return false;

            return s.Equals(compare, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool ICStartsWith(this string s, string text)
        {
            if (s == null && text == null)
                return true;

            if (s == null || text == null)
                return false;

            return s.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool ICEndsWith(this string s, string text)
        {
            if (s == null && text == null)
                return true;

            if (s == null || text == null)
                return false;

            return s.EndsWith(text, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool ICContains(this string s, string text)
        {
            if (s == null && text == null)
                return true;

            if (s == null || text == null)
                return false;

            return s.ToLower().Contains(text.ToLower());
        }

        public static bool ICContains(this IEnumerable<string> lst, string text)
        {
            if (lst == null)
                return false;

            return lst.Any(item => item.ICEquals(text));
        }

    }
}
