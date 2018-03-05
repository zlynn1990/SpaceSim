using System;
using System.Collections.Generic;
using Cloo;

namespace OpenCLWrapper
{
    public class OpenCLProxy : IDisposable
    {
        public bool HardwareAccelerationEnabled { get; private set; }

        public string AcceleratorName { get; private set; }

        private ComputeContext _context;

        private ComputeCommandQueue _commands;

        private Dictionary<string, int> _intArguments;
        private Dictionary<string, int[]> _intBuffers;
        private Dictionary<string, ComputeBuffer<int>> _intComputeBuffers;

        private Dictionary<string, float> _floatArguments;
        private Dictionary<string, float[]> _floatBuffers;
        private Dictionary<string, ComputeBuffer<float>> _floatComputeBuffers;

        private Dictionary<string, double> _doubleArguments;

        public OpenCLProxy(bool useSoftware = false)
        {
            HardwareAccelerationEnabled = ComputePlatform.Platforms.Count != 0 && !useSoftware;

            if (HardwareAccelerationEnabled)
            {
                ComputePlatform platform = ComputePlatform.Platforms[0];
                var devices = new List<ComputeDevice> { platform.Devices[0] };
                var properties = new ComputeContextPropertyList(platform);

                _context = new ComputeContext(devices, properties, null, IntPtr.Zero);
                _commands = new ComputeCommandQueue(_context, _context.Devices[0], ComputeCommandQueueFlags.None);

                _intComputeBuffers = new Dictionary<string, ComputeBuffer<int>>();
                _floatComputeBuffers = new Dictionary<string, ComputeBuffer<float>>();

                AcceleratorName = platform.Name;
            }
            else
            {
                AcceleratorName = "CPU";
            }

            _intArguments = new Dictionary<string, int>();
            _intBuffers = new Dictionary<string, int[]>();

            _floatArguments = new Dictionary<string, float>();
            _floatBuffers = new Dictionary<string, float[]>();

            _doubleArguments = new Dictionary<string, double>();
        }

        public void CreateIntBuffer(string key, int length, ComputeMemoryFlags flags)
        {
            CreateIntBuffer(key, new int[length], flags);
        }

        public void CreateIntBuffer(string key, int[] hostBuffer, ComputeMemoryFlags flags)
        {
            _intBuffers.Add(key, hostBuffer);

            if (HardwareAccelerationEnabled)
            {
                _intComputeBuffers.Add(key, new ComputeBuffer<int>(_context, flags, hostBuffer));
            }
        }

        public void CreateFloatBuffer(string key, float[] hostBuffer, ComputeMemoryFlags flags)
        {
            _floatBuffers.Add(key, hostBuffer);

            if (HardwareAccelerationEnabled)
            {
                _floatComputeBuffers.Add(key, new ComputeBuffer<float>(_context, flags, hostBuffer));
            }
        }

        public void CreateIntArgument(string key, int argumentValue)
        {
            _intArguments.Add(key, argumentValue);
        }

        public void UpdateIntArgument(string key, int updatedValue)
        {
            _intArguments[key] = updatedValue;
        }

        public void CreateFloatArgument(string key, float argumentValue)
        {
            _floatArguments.Add(key, argumentValue);
        }

        public void UpdateFloatArgument(string key, float updatedValue)
        {
            _floatArguments[key] = updatedValue;
        }

        public void CreateDoubleArgument(string key, double argumentValue)
        {
            _doubleArguments.Add(key, argumentValue);
        }

        public void UpdateDoubleArgument(string key, double updatedValue)
        {
            _doubleArguments[key] = updatedValue;
        }

        public int[] ReadIntBuffer(string key, int length)
        {
            int[] rawBuffer = _intBuffers[key];

            if (HardwareAccelerationEnabled)
            {
                _commands.ReadFromBuffer(_intComputeBuffers[key], ref rawBuffer, true, 0, 0, length, null);

                _commands.Finish();   
            }

            return rawBuffer;
        }

        public float[] ReadFloatBuffer(string key, int length)
        {
            float[] rawBuffer = _floatBuffers[key];

            if (HardwareAccelerationEnabled)
            {
                _commands.ReadFromBuffer(_floatComputeBuffers[key], ref rawBuffer, true, 0, 0, length, null);

                _commands.Finish();
            }

            return rawBuffer;
        }

        public ComputeKernel CreateKernel(object kernelInstance)
        {
            string kernelName = kernelInstance.GetType().Name;

            if (HardwareAccelerationEnabled)
            {
                IKernel program = KernelManager.LoadKernel(kernelName);

                // Create and build the opencl program.
                var computeProgram = new ComputeProgram(_context, program.Code);
                computeProgram.Build(null, null, null, IntPtr.Zero);

                // Create the kernel function and set its arguments.
                ComputeKernel kernel = computeProgram.CreateKernel("Run");

                int index = 0;

                foreach (string key in _intComputeBuffers.Keys)
                {
                    kernel.SetMemoryArgument(index, _intComputeBuffers[key]);

                    index++;
                }

                foreach (string key in _floatComputeBuffers.Keys)
                {
                    kernel.SetMemoryArgument(index, _floatComputeBuffers[key]);

                    index++;
                }

                return kernel;
            }

            return null;
        }

        public void RunKernel(ComputeKernel kernel, int count)
        {
            int argOffset = _intComputeBuffers.Count + _floatComputeBuffers.Count;

            foreach (string key in _intArguments.Keys)
            {
                kernel.SetValueArgument(argOffset, _intArguments[key]);

                argOffset++;
            }

            foreach (string key in _floatArguments.Keys)
            {
                kernel.SetValueArgument(argOffset, _floatArguments[key]);

                argOffset++;
            }

            foreach (string key in _doubleArguments.Keys)
            {
                kernel.SetValueArgument(argOffset, _doubleArguments[key]);

                argOffset++;
            }

            _commands.Execute(kernel, count);

            _commands.Finish();
        }

        public void Dispose()
        {
            if (HardwareAccelerationEnabled)
            {
                _commands.Finish();
            }
        }
    }
}
