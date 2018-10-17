using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using DTO.DTOGenerator;

namespace DTO
{

    public class B
    {
        public char a;
        public string b;
    }
    public class A
    {
        public int a;
        public uint b;
        public string c;
        public byte d;
        public char e;
        public double f;
        public float g;
        public bool h;
        public List<B> list;
    }
    class Program
    {
        static void Main(string[] args)
        {

            Faker faker = new Faker();
            A c = faker.Create<A>();

            Console.WriteLine(c.a);
            Console.WriteLine(c.h);
            Console.WriteLine(c.c);

            foreach (B element in c.list)
            {
                Console.WriteLine("Inner char: " + element.a);
            }

            Console.ReadKey();
        }
    }
}
