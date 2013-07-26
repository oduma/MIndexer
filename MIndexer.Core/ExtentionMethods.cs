using System.Linq;

namespace MIndexer.Core
{
    public static class ExtentionMethods
    {
        public static string ReplaceAny(this string inString,string[] replace, string replaceWith)
        {
            if(replace==null || !replace.Any() || replaceWith==null)
                return inString;
            replace.ToList().ForEach((s) =>
                                         {
                                             inString = inString.Replace(s, replaceWith);
                                         });
            return inString;
        }
    }
}
