﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator.Generators
{
    public class LongGenerator : IGenerator
    {
        Random generator;

        public LongGenerator()
        {
            generator = new Random();
        }

        public object Generate()
        {
            return generator.Next(int.MinValue, int.MaxValue) + generator.Next(int.MinValue, int.MaxValue);
        }

        public Type GetGeneratorType()
        {
            return typeof(long);
        }
    }
}
