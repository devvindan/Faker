﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator
{
    public interface IGenerator
    {
        object Generate();

        Type GetGeneratorType();
    }
}
