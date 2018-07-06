using System;
using System.Collections.Generic;
using System.IO;

namespace OpenCLWrapper
{
    public static class KernelManager
    {
        private static readonly Dictionary<string, KernelFile> KernelCache;

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
            string kernelFile = $"Kernels/{kernelName}.kernel";

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
            string baseKernelPath = $"{kernelDirectory}/BaseKernel.cs";

            if (!File.Exists(baseKernelPath))
            {
                throw new Exception("No base kernel detected!");
            }

            string[] code = File.ReadAllLines(baseKernelPath);

            var constantMapping = new ConstantMapping(code);
            var functionMappings = new FunctionMapping(code);

            string[] kerenelFiles = Directory.GetFiles(kernelDirectory);

            foreach (string kernelPath in kerenelFiles)
            {
                string kernelName = Path.GetFileNameWithoutExtension(kernelPath);

                if (kernelName == "BaseKernel") continue;

                KernelFile kernelFile = CreateKernel(kernelPath, constantMapping, functionMappings);

                KernelCache.Add(kernelName, kernelFile);
            }
        }

        private static KernelFile CreateKernel(string codePath, ConstantMapping constants, FunctionMapping functions)
        {
            string[] lines = File.ReadAllLines(codePath);

            int kernelIndex = SourceHelper.ParseDown(lines, "public void Run", 0);

            string kernelFunction = lines[kernelIndex];

            string kernelParameters = SourceHelper.ExtractFunctionHeader(kernelFunction);

            kernelParameters = kernelParameters.Replace("int[]", "global int*");
            kernelParameters = kernelParameters.Replace("float[]", "global float*");

            var clCode = new List<string>();

            if (IsFP64(kernelFunction))
            {
                clCode.Add("#pragma OPENCL EXTENSION cl_khr_fp64 : enable");
            }

            clCode.Add("kernel void Run " + kernelParameters);

            int codeEnd = lines.Length;

            for (int i=0; i < 3; i++)
            {
                codeEnd = SourceHelper.ParseUp(lines, "}", codeEnd - 1);
            }

            for (int i=kernelIndex + 1; i < codeEnd + 1; i++)
            {
                clCode.Add(lines[i].Trim());
            }

            // Replace constants and inline functions
            functions.Inline(clCode);
            constants.Replace(clCode);

            return new KernelFile { Code = string.Join("\n", clCode)};
        }

        private static bool IsFP64(string kernelHeader)
        {
            return kernelHeader.Contains("double");
        }
    }
}
