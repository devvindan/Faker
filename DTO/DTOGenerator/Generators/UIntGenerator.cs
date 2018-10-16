using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator.Generators
{
    public class UIntGenerator : IGenerator
    {
        Random generator;

        public UIntGenerator()
        {
            generator = new Random();
        }

        public object Generate()
        {
            uint res = (uint)generator.Next(int.MaxValue) + (uint)generator.Next(int.MaxValue);
            return res;
        }
    }
}
