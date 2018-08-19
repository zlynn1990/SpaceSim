using System.Collections.Generic;

namespace OpenCLWrapper
{
    class ConstantMapping
    {
        private readonly Dictionary<string, string> _constantTable;

        public ConstantMapping(string[] code)
        {
            _constantTable = new Dictionary<string, string>();

            int constantStart = SourceHelper.ParseDown(code, "CONSTANTS START", 0) + 1;
            int constantEnd = SourceHelper.ParseDown(code, "CONSTANTS END", constantStart);

            for (int i = constantStart; i < constantEnd; i++)
            {
                if (string.IsNullOrEmpty(code[i])) continue;

                string cleanedLine = code[i].Trim();

                string[] lineSplit = cleanedLine.Split(' ');

                string value = lineSplit[5].Substring(0, lineSplit[5].Length - 1);

                AddConstant(lineSplit[3], value);
            }
        }

        public void AddConstant(string constant, string value)
        {
            _constantTable.Add(constant, value);
        }

        public void Replace(List<string> code)
        {
            for (var i = 0; i < code.Count; i++)
            {
                foreach (string constant in _constantTable.Keys)
                {
                    if (code[i].Contains(constant))
                    {
                        code[i] = code[i].Replace(constant, _constantTable[constant]);
                    }
                }
            }
        }
    }
}
