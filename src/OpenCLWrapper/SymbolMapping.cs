using System.Collections.Generic;

namespace OpenCLWrapper
{
    public class SymbolMapping
    {
        private Dictionary<string, string> symbolTable;

        public SymbolMapping()
        {
            symbolTable = new Dictionary<string, string>();
        }

        public void AddSymbol(string symbol, string value)
        {
            symbolTable.Add(symbol, value);
        }

        public string ReplaceSymbols(string code)
        {
            foreach (string symbol in symbolTable.Keys)
            {
                code = code.Replace(symbol, symbolTable[symbol]);
            }

            return code;
        }
    }
}
