using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCLWrapper
{
    [Serializable]
    public class KernelFile : IKernel
    {
        public string Code { get; set; }
    }
}
