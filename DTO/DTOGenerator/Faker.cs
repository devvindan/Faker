using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DTO.DTOGenerator.Generators;
using System.IO;

namespace DTO.DTOGenerator
{
    public class Faker
    {
        // Список с доступными генераторами

        private Dictionary<Type, IGenerator> basicTypeGenerator;
        private Dictionary<string, ICollectionGenerator> collectionTypeGenerator;

        // Для случая вложенности

        private List<Type> recursionList;

        public void AddToRecursionList(Type t)
        {
            recursionList.Add(t);
        }

        public void RemoveFromRecursionList(Type t)
        {
            recursionList.Remove(t);
        }

        public void ClearRecursionList()
        {
            recursionList.Clear();
        }

        public bool checkIfDTO(Type t)
        {
            MethodInfo[] classMethods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static);

            if (classMethods.Length > 0)
            {
                return false;
            }
            return true;
        }

        public object Generate(Type type)
        {
            if (basicTypeGenerator.ContainsKey(type))
            {
                return basicTypeGenerator[type].Generate();
            }
            if (collectionTypeGenerator.ContainsKey(type.Name))
            {
                return collectionTypeGenerator[type.Name].Generate(type.GenericTypeArguments[0], this);
            }

            // защита от зацикливания
            if (recursionList.Contains(type))
            {
                return null;
            }


            if (checkIfDTO(type))
            {
                object tmp = Create(type);
                RemoveFromRecursionList(type);
                return tmp;
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;

        }

        // обработка информации, вспомогательные функции

        public List<ParameterInfo> GetParametersInfo(ConstructorInfo constructor)
        {
            return constructor.GetParameters().ToList();
        }

        public bool IsParameterSimple(ParameterInfo parameter)
        {
            Type type = parameter.GetType();
            return type.IsPrimitive || type.Equals(typeof(string));
        }

        public List<PropertyInfo> GetSettableProperties(List<PropertyInfo> allProperties)
        {
            return new List<PropertyInfo>(allProperties.Where(item => !item.GetSetMethod().IsPrivate));
        }

        // конструкторы, поля, свойства класса

        public List<ConstructorInfo> GetClassConstructorsInfo(Type t)
        {
            return new List<ConstructorInfo>(t.GetConstructors());
        }

        public List<PropertyInfo> GetClassPropertiesInfo(Type t)
        {
            return new List<PropertyInfo>(t.GetProperties());
        }

        public List<FieldInfo> GetClassFieldsInfo(Type t)
        {
            return new List<FieldInfo>(t.GetFields());
        }

        // получаем конструктор с макс. количеством параметров
        public ConstructorInfo GetMaxParameterizedConstructor(List<ConstructorInfo> allConstructors)
        {
            if (allConstructors.Count == 0)
            {
                throw new InvalidOperationException("Нет конструкторов");
            }

            allConstructors = allConstructors.OrderBy(item => item.GetParameters().Length).ToList();
            return allConstructors[0];
        }

        // создание по конструктору
        public object CreateByConstructor(ConstructorInfo constructor)
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
        public object CreateByFieldsAndProperties(Type t)
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

        public object Create(Type t)
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

        public Faker(string dirPath)
        {

            List<Assembly> allAssemblies = new List<Assembly>();


            foreach (string dll in Directory.GetFiles(dirPath, "*.dll"))
            {
                allAssemblies.Add(Assembly.LoadFile(dll));
            }

            collectionTypeGenerator = new Dictionary<string, ICollectionGenerator>();

            basicTypeGenerator = new Dictionary<Type, IGenerator>();

            var doubleGenerator = new DoubleGenerator();
            basicTypeGenerator.Add(doubleGenerator.GetGeneratorType(), doubleGenerator);

            var uintGenerator = new UIntGenerator();
            basicTypeGenerator.Add(uintGenerator.GetGeneratorType(), uintGenerator);

            var floatGenerator = new FloatGenerator();
            basicTypeGenerator.Add(floatGenerator.GetGeneratorType(), floatGenerator);

            var charGenerator = new CharGenerator();
            basicTypeGenerator.Add(charGenerator.GetGeneratorType(), charGenerator);

            var stringGenerator = new StringGenerator();
            basicTypeGenerator.Add(stringGenerator.GetGeneratorType(), stringGenerator);

            var longGenerator = new LongGenerator();
            basicTypeGenerator.Add(longGenerator.GetGeneratorType(), longGenerator);

            var datetimeGenerator = new DateGenerator();
            basicTypeGenerator.Add(datetimeGenerator.GetGeneratorType(), datetimeGenerator);

            foreach (var asm in allAssemblies)
            {
                Console.WriteLine(asm.FullName);
                var types = asm.GetTypes().Where(t => t.GetInterfaces().Where(i => i.Equals(typeof(IGenerator))).Any());

                foreach (var type in types)
                {
                    var plugin = asm.CreateInstance(type.FullName) as IGenerator;
                    Type t = plugin.GetGeneratorType();
                    if (!basicTypeGenerator.ContainsKey(t))
                        basicTypeGenerator.Add(plugin.GetGeneratorType(), plugin);
                }
            } 

            var listGenerator = new ListGenerator();
            collectionTypeGenerator.Add(listGenerator.GetGeneratorType().Name, listGenerator);

            recursionList = new List<Type>();
        }



    }
}
