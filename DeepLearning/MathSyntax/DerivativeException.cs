﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearning.MathSyntax
{
    class DerivativeException : Exception
    {
        public DerivativeException(string message) : base(message) { }
        public DerivativeException() { }
    }
}
