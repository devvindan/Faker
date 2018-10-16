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
        public object Generate(Type t)
        {
            object generated = Activator.CreateInstance(typeof(List<>).MakeGenericType(t));

            ((IList)generated).Add(Faker.Generate(t));

            return generated;
        }
    }
}
