using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.DTOGenerator;


namespace Plugins
{
    public class BoolGenerator : IGenerator
    {
        Random generator;

        public BoolGenerator()
        {
            generator = new Random();
        }

        public object Generate()
        {
            return generator.Next(1) == 1 ? true : false;
        }

        public Type GetGeneratorType()
        {
            return typeof(bool);
        }

    }
}
