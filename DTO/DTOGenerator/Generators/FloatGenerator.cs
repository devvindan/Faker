using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator.Generators
{
    public class FloatGenerator : IGenerator
    {
        Random generator;

        public FloatGenerator()
        {
            generator = new Random();
        }

        public object Generate()
        {
            return (float)(generator.NextDouble() * float.MaxValue);
        }

        public Type GetGeneratorType()
        {
            return typeof(float);
        }
    }
}
