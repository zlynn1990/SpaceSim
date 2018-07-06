using System;
using System.Collections.Generic;

namespace OpenCLWrapper
{
    // Assumes a rigid function structure where all functions are protected void and must only have var out parameters
    // Functions must follow standard spacing and line indentation conventions and have out parameters above
    class FunctionMapping
    {
        private readonly Dictionary<string, string[]> _functionTable;

        public FunctionMapping(string[] code)
        {
            _functionTable = new Dictionary<string, string[]>();

            int blockStart = SourceHelper.ParseDown(code, "FUNCTIONS START", 0) + 1;
            int blockEnd = SourceHelper.ParseDown(code, "FUNCTIONS END", blockStart);

            for (int i = blockStart; i < blockEnd; i++)
            {
                if (string.IsNullOrEmpty(code[i])) continue;

                // Found a function opening
                if (code[i].Contains("protected void"))
                {
                    string functionName = SourceHelper.ExtractFunctionName(code[i]);

                    List<FunctionParameter> outParams = ParseOutParams(code[i]);

                    if (outParams.Count == 0)
                    {
                        throw new Exception($"Function '{code[i]}' must contain out parameters!");
                    }

                    var functionCode = new List<string>();
                    var paramsReplaced = new HashSet<string>();

                    int functionEnd = DetectFunctionEnd(code, i + 2, blockEnd);

                    // Assume the function starts two lines below the function declaration
                    for (int lineIndex = i + 2; lineIndex < functionEnd; lineIndex++)
                    {
                        foreach (FunctionParameter outParam in outParams)
                        {
                            // Only replace the first usage of the parameter with it's type definition
                            if (code[lineIndex].Contains(outParam.Name) && !paramsReplaced.Contains(outParam.Name))
                            {
                                code[lineIndex] = code[lineIndex].Replace(outParam.Name, $"{outParam.Type} {outParam.Name}");

                                paramsReplaced.Add(outParam.Name);
                                break;
                            }
                        }

                        functionCode.Add(code[lineIndex].Trim());
                    }

                    AddFunction(functionName, functionCode.ToArray());
                }
            }
        }

        private int DetectFunctionEnd(string[] code, int functionStart, int codeEnd)
        {
            int indentationCounter = 0;

            for (int i = functionStart; i < codeEnd; i++)
            {
                int openBracket = SourceHelper.ParseDown(code, "{", i);
                int closeBracket = SourceHelper.ParseDown(code, "}", i);

                if (openBracket > 0 && openBracket < closeBracket)
                {
                    i = openBracket;

                    indentationCounter++;
                }

                if (closeBracket > 0)
                {
                    i = closeBracket;

                    if (indentationCounter == 0)
                    {
                        return closeBracket;
                    }

                    indentationCounter--;
                }
            }

            throw new Exception("Could not find the end of the function!");
        }

        private List<FunctionParameter> ParseOutParams(string function)
        {
            var outParams = new List<FunctionParameter>();

            string functionHeader = SourceHelper.ExtractFunctionHeader(function);

            string[] headerSplit = functionHeader.Split(',');

            foreach (string variable in headerSplit)
            {
                string[] variableSplit = variable.Trim().Split(' ');

                if (variableSplit.Length == 3 && variableSplit[0] == "out")
                {
                    outParams.Add(new FunctionParameter
                    {
                        // Fix any parameter names that come at the end of the function
                        Name = variableSplit[2].Replace(")", string.Empty),
                        Type = variableSplit[1]
                    });
                }
            }

            return outParams;
        }

        public void AddFunction(string name, string[] code)
        {
            _functionTable.Add(name, code);
        }

        public void Inline(List<string> code)
        {
            for (int i = 0; i < code.Count; i++)
            {
                foreach (string function in _functionTable.Keys)
                {
                    if (code[i].Contains(function))
                    {
                        // Remove the current line and line above it
                        code.RemoveAt(i - 1);
                        code.RemoveAt(i - 1);

                        // Inline the function directly
                        code.InsertRange(i - 1, _functionTable[function]);
                    }
                }
            }
        }

        private class FunctionParameter
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }
    }
}
