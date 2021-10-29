using System;
using System.Collections.Generic;
using System.Reflection;

namespace BaselessJumping.Internals.Common
{
    public static class StringComparator
    {
        public static float CompareTo_GetSimilarity(string input, string comparingTo)
        {
            var percent = 0f;
            //float percent = 1f;
            var invariant = (float)(1f / comparingTo.Length);

            foreach (var cChar in comparingTo)
            {
                if (input.Contains(cChar) && comparingTo.Contains(cChar))
                {
                    percent += invariant;
                }
            }
            return (float)Math.Round(percent * 100, 2);
        }
        public static string[] FindMatches(string input, string[] comparisons)
        {
            List<string> strings = new();
            /*foreach (var str in comparisons)
            {
                if (CompareTo_GetSimilarity(input, str) > 0.25f)
                {
                    strings.Add(str);
                }
            }*/
            foreach (var str in comparisons)
            {
                if (str.Contains(input))
                    strings.Add(str);

                /*for (int i = 0; i < str.Length; i++)
                {
                    for (int j = 0; j < input.Length; j++)
                    {

                    }
                }*/

            }
            return strings.ToArray();
        }
    }
}