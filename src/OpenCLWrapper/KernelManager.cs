using System;
using System.Collections.Generic;
using System.IO;

namespace OpenCLWrapper
{
    public class KernelManager
    {
        private static Dictionary<string, KernelFile> KernelCache;

        static KernelManager()
        {
            KernelCache = new Dictionary<string, KernelFile>();
        }

        public static KernelFile LoadKernel(string kernelName)
        {
            // Load it straight from memory
            if (KernelCache.ContainsKey(kernelName))
            {
                return KernelCache[kernelName];
            }

            // Look for it on disk
            string kernelFile = string.Format("Kernels/{0}.kernel", kernelName);

            if (File.Exists(kernelFile))
            {
                var kernel = Serializer.Read<KernelFile>(kernelName + ".kernel");

                KernelCache.Add(kernelName, kernel);

                return kernel;
            }

            throw new Exception("Cannot load kernel " + kernelName);
        }

        public static void GenerateKernels(string kernelDirectory)
        {
            string symbolPath = string.Format("{0}/SymbolKernel.cs", kernelDirectory);

            if (!File.Exists(symbolPath))
            {
                throw new Exception("No symbol kernel detected!");
            }

            SymbolMapping symbolMapping = CreateSymbolMapping(symbolPath);

            string[] kerenelFiles = Directory.GetFiles(kernelDirectory);

            foreach (string kernelPath in kerenelFiles)
            {
                string kernelName = Path.GetFileNameWithoutExtension(kernelPath);

                if (kernelName == "SymbolKernel") continue;

                KernelFile kernelFile = CreateKernel(kernelPath, symbolMapping);

                KernelCache.Add(kernelName, kernelFile);
            }
        }

        private static SymbolMapping CreateSymbolMapping(string symbolKernelPath)
        {
            var symbolMapping = new SymbolMapping();

            string[] lines = File.ReadAllLines(symbolKernelPath);

            int symbolStart = ParseDown(lines, "#region", 0) + 1;
            int symbolEnd = ParseDown(lines, "#endregion", symbolStart);

            for (int i=symbolStart; i < symbolEnd; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;

                string cleanedLine = lines[i].Trim();

                string[] lineSplit = cleanedLine.Split(' ');

                string value = lineSplit[5].Substring(0, lineSplit[5].Length - 1);

                symbolMapping.AddSymbol(lineSplit[3], value);
            }

            return symbolMapping;
        }

        private static KernelFile CreateKernel(string codePath, SymbolMapping symbols)
        {
            string[] lines = File.ReadAllLines(codePath);

            int kernelIndex = ParseDown(lines, "public void Run", 0);

            string kernelHeader = lines[kernelIndex];

            int parameterStart = kernelHeader.IndexOf('(');
            int parameterEnd = kernelHeader.LastIndexOf(')');

            string kernelParameters = kernelHeader.Substring(parameterStart, parameterEnd - parameterStart + 1);

            kernelParameters = kernelParameters.Replace("int[]", "global int*");
            kernelParameters = kernelParameters.Replace("float[]", "global float*");

            string clCode = string.Empty;

            if (IsFP64(kernelHeader))
            {
                clCode += "#pragma OPENCL EXTENSION cl_khr_fp64 : enable\n";
            }

            clCode += "kernel void Run " + kernelParameters + "\n";

            int codeEnd = lines.Length;

            for (int i=0; i < 3; i++)
            {
                codeEnd = ParseUp(lines, "}", codeEnd - 1);
            }

            for (int i=kernelIndex + 1; i < codeEnd + 1; i++)
            {
                clCode += (lines[i].Trim() + "\n");
            }

            // Remove symbols
            clCode = symbols.ReplaceSymbols(clCode);

            return new KernelFile { Code = clCode};
        }

        private static bool IsFP64(string kernelHeader)
        {
            return kernelHeader.Contains("double");
        }

        private static int ParseDown(string[] lines, string target, int start)
        {
            for (int i = start; i < lines.Length; i++)
            {
                if (lines[i].Contains(target)) return i;
            }

            return -1;
        }

        private static int ParseUp(string[] lines, string target, int start)
        {
            for (int i = start; i > 0; i--)
            {
                if (lines[i].Contains(target)) return i;
            }

            return -1;
        }
    }
}
