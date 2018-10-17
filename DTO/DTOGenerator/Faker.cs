using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DTO.DTOGenerator.Generators;

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

        public static void RemoveFromRecursionList(Type t)
        {
            recursionList.Remove(t);
        }

        public static void ClearRecursionList()
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
                RemoveFromRecursionList(type);
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

        // конструкторы, поля, свойства класса

        public static List<ConstructorInfo> GetClassConstructorsInfo(Type t)
        {
            return new List<ConstructorInfo>(t.GetConstructors());
        }

        public static List<PropertyInfo> GetClassPropertiesInfo(Type t)
        {
            return new List<PropertyInfo>(t.GetProperties());
        }

        public static List<FieldInfo> GetClassFieldsInfo(Type t)
        {
            return new List<FieldInfo>(t.GetFields());
        }

        // получаем конструктор с макс. количеством параметров
        public static ConstructorInfo GetMaxParameterizedConstructor(List<ConstructorInfo> allConstructors)
        {
            if (allConstructors.Count == 0)
            {
                throw new InvalidOperationException("Нет конструкторов");
            }

            allConstructors = allConstructors.OrderBy(item => item.GetParameters().Length).ToList();
            return allConstructors[0];
        }

        // создание по конструктору
        public static object CreateByConstructor(ConstructorInfo constructor)
        {

            
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

        // создание по полям и свойствам
        public static object CreateByFieldsAndProperties(Type t)
        {
            object generated = Activator.CreateInstance(t);
            List<FieldInfo> fields = GetClassFieldsInfo(t);
            List<PropertyInfo> settableProperties = GetSettableProperties(GetClassPropertiesInfo(t));
            foreach (FieldInfo field in fields)
            {
                field.SetValue(generated, Generate(field.FieldType));
            }

            foreach (PropertyInfo property in settableProperties)
            {
                property.SetValue(generated, Generate(property.PropertyType));
            }

            return generated;
        }

        public static object Create(Type t)
        {
            AddToRecursionList(t);
            Object result;
            List<ConstructorInfo> constructors = GetClassConstructorsInfo(t);
            if (constructors.Count > 0 && constructors[0].GetParameters().Length > 0)
            {
                ConstructorInfo bestConstructor = GetMaxParameterizedConstructor(constructors);
                result = CreateByConstructor(bestConstructor);
            }
            else
            {
                result = CreateByFieldsAndProperties(t);
            }
            return result;
        }

        public T Create<T>()
        {
            ClearRecursionList();
            return (T)Create(typeof(T));
        }

        public Faker()
        {
            Assembly asm = Assembly.LoadFrom("tobedone");

            collectionTypeGenerator = new Dictionary<string, ICollectionGenerator>();

            basicTypeGenerator = new Dictionary<Type, IGenerator>();
            basicTypeGenerator.Add(typeof(double), new DoubleGenerator());
            basicTypeGenerator.Add(typeof(uint), new UIntGenerator());
            basicTypeGenerator.Add(typeof(float), new FloatGenerator());
            basicTypeGenerator.Add(typeof(char), new CharGenerator());
            basicTypeGenerator.Add(typeof(string), new StringGenerator());
            basicTypeGenerator.Add(typeof(long), new LongGenerator());
            basicTypeGenerator.Add(typeof(DateTime), new DateGenerator());

            var types = asm.GetTypes().Where(t => t.GetInterfaces().Where(i => i.Equals(typeof(IGenerator))).Any());

            foreach (var type in types)
            {
                var plugin = asm.CreateInstance(type.FullName) as IPluginGenerator;
                Type t = plugin.GetGeneratorType();
                if (!basicTypeGenerator.ContainsKey(t))
                    basicTypeGenerator.Add(plugin.GetGeneratorType(), plugin as IGenerator);
            }

            collectionTypeGenerator.Add(typeof(List<>).Name, new ListGenerator());

            recursionList = new List<Type>();
        }



    }
}
