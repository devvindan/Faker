using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator
{
    public interface ICollectionGenerator
    {
        object Generate(Type t, Faker f);
        Type GetGeneratorType();
    }


}

//statics methods, Iplugin generator -> IGenerator, assembly