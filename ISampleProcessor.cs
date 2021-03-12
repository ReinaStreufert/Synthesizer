using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesizer
{
    interface ISampleProcessor : ISampleProvider
    {
        ISampleProvider Input { get; set; }
    }
}
