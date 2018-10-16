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

        // обработка информации

        public static List<ParameterInfo> GetParametersInfo(ConstructorInfo constructor)
        {
            return constructor.GetParameters().ToList();
        }

        public static bool IsParameterSimple(ParameterInfo parameter)
        {
            Type type = parameter.GetType();
            return type.IsPrimitive || type.Equals(typeof(string));
        }

        public static List<PropertyInfo> GetSettableProperties(List<PropertyInfo> allProperties)
        {
            return new List<PropertyInfo>(allProperties.Where(item => !item.GetSetMethod().IsPrivate));
        }

        public static ConstructorInfo GetMaxParameterizedConstructor(List<ConstructorInfo> allConstructors)
        {
            if (allConstructors.Count == 0)
            {
                throw new InvalidOperationException("Нет конструкторов");
            }

            allConstructors = allConstructors.OrderBy(item => item.GetParameters().Length).ToList();
            return allConstructors[0];
        }



        public object CreateByConstructor(ConstructorInfo constructor)
        {

            // получаем список параметров
            List<ParameterInfo> parameters = GetParametersInfo(constructor);
            object[] tmpParams = new object[parameters.Count];
            int i = 0;

            foreach (ParameterInfo parameterInfo in parameters)
            {
                tmpParams[i] = Generate(parameterInfo.ParameterType);
                i++;
            }

            return constructor.Invoke(tmpParams);
        }



    }
}
