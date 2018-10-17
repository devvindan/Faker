using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.FakerTests
{
    public class SimpleClass
    {
        public int a;
        public int b;
    }

    public class FakerClass<T>
    {
        int intNum;
        uint uintNum;
        float floatNum;
        double doubleNum;
        long longNum;
        char charStr;
        bool boolValue;
        string str;
        DateTime date;
        List<T> list;
        SimpleClass simpl;

        public int GetInt => intNum;
        public uint GetUint => uintNum;
        public float GetFloat => floatNum;
        public double GetDouble => doubleNum;
        public long GetLong => longNum;
        public char GetChar => charStr;
        public bool GetBool => boolValue;
        public string GetString => str;
        public DateTime GetDate => date;
        public List<T> GetList => list;
        public SimpleClass GetAnotherClass => simpl;

        public FakerClass(int a, uint b, float c, double d, long e, char f, bool g, string h, DateTime k, List<T> l, SimpleClass m)
        {
            intNum = a;
            uintNum = b;
            floatNum = c;
            doubleNum = d;
            longNum = e;
            charStr = f;
            boolValue = g;
            str = h;
            date = k;
            list = l;
            simpl = m;
        }
    }
}
