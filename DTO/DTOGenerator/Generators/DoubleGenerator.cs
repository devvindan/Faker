using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator.Generators
{
    public class DoubleGenerator : IGenerator
    {
        Random generator;

        public DoubleGenerator()
        {
            generator = new Random();
        }

        public object Generate()
        {
            return generator.NextDouble() * double.MaxValue;
        }
    }
}
