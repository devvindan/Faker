using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOGenerator
{
    public class Faker
    {
        // Список с доступными генераторами

        private static Dictionary<Type, IGenerator> basicTypeGenerator;
        private static Dictionary<string, ICollectionGenerator> collectionTypeGenerator;

        // Для случая вложенности

        private static List<Type> recursionList;

        public static void AddToRecursionList(Type t)
        {
            recursionList.Add(t);
        }

        public static void RemoveFromRecursionControlList(Type t)
        {
            recursionList.Remove(t);
        }

        public static void ClearRecursionControlList()
        {
            recursionList.Clear();
        }

        public static bool checkIfDTO(Type t)
        {
            MethodInfo[] classMethods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);

            if (classMethods.Length > 0)
            {
                return false;
            }
            return true;
        }

        public static object Generate(Type type)
        {
            if (basicTypeGenerator.ContainsKey(type))
            {
                return basicTypeGenerator[type].Generate();
            }
            if (collectionTypeGenerator.ContainsKey(type.Name))
            {
                return collectionTypeGenerator[type.Name].Generate(type.GenericTypeArguments[0]);
            }

            if (recursionList.Contains(type))
            {
                throw new InvalidOperationException("DTO содержат друг друга в качестве полей.");
            }


            if (checkIfDTO(type))
            {
                object tmp = Create(type);
                RemoveFromRecursionControlList(type);
                return tmp;
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;

        }
    }
}
