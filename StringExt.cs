namespace OpenTx2Dashware
{
    static class StringExt
    {
        public static string SubstringSafe(this string s, int start, int length)
        {
            try
            {
                return s.Substring(start, length);
            }
            catch
            {
                return null;
            }
        }
    }
}