using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.DTOGenerator;

namespace Plugins
{
    class IntGenerator : IGenerator, IPluginGenerator
    {
        Random generator;

        public IntGenerator()
        {
            generator = new Random();
        }

        public object Generate()
        {
            return generator.Next(int.MinValue, int.MaxValue);
        }

        public Type GetGeneratorType()
        {
            return typeof(int);
        }
    }
}
