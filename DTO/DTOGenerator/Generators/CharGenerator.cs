using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator.Generators
{
    public class CharGenerator : IGenerator
    {
        Random generator;

        public CharGenerator()
        {
            generator = new Random();
        }

        public object Generate()
        {
            return Convert.ToChar(generator.Next(65, 122));
        }

        public Type GetGeneratorType()
        {
            return typeof(char);
        }
    }
}