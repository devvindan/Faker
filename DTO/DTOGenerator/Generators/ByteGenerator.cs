using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator.Generators
{
    public class ByteGenerator : IGenerator
    {
        Random generator;

        public ByteGenerator()
        {
            generator = new Random();
        }

        public object Generate()
        {
            return (byte)generator.Next(0, 255);
        }


    }
}
