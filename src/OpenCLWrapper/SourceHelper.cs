using System;

namespace OpenCLWrapper
{
    static class SourceHelper
    {
        public static int ParseDown(string[] lines, string target, int start)
        {
            for (int i = start; i < lines.Length; i++)
            {
                if (lines[i].Contains(target)) return i;
            }

            return -1;
        }

        public static int ParseUp(string[] lines, string target, int start)
        {
            for (int i = start; i > 0; i--)
            {
                if (lines[i].Contains(target)) return i;
            }

            return -1;
        }

        public static string ExtractFunctionName(string function)
        {
            string[] functionSplit = function.Split(' ');

            for (int i = 0; i < functionSplit.Length; i++)
            {
                if (functionSplit[i].Contains("("))
                {
                    int headerStart = functionSplit[i].IndexOf("(", StringComparison.Ordinal);

                    return functionSplit[i].Substring(0, headerStart);
                }
            }

            throw new Exception($"Could not find a function name in definition '{function}'!");
        }

        public static string ExtractFunctionHeader(string function)
        {
            int parameterStart = function.IndexOf('(');
            int parameterEnd = function.LastIndexOf(')');

            return function.Substring(parameterStart, parameterEnd - parameterStart + 1);
        }
    }
}
