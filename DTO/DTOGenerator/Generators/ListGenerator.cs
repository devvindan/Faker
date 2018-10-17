using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator.Generators
{
    public class ListGenerator : ICollectionGenerator
    {
        public object Generate(Type t, Faker f)
        {
            object generated = Activator.CreateInstance(typeof(List<>).MakeGenericType(t));

            ((IList)generated).Add(f.Generate(t));

            return generated;
        }

        public Type GetGeneratorType()
        {
            return typeof(List<>);
        }
    }
}
