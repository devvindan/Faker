using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTO.DTOGenerator;

namespace DTO.FakerTests
{
    public class A
    {
        public B b;
        public C c;
    }

    public class B
    {
        public C c;
    }

    public class C
    {
        public int intNum;
    }

    [TestClass]
    public class FakerTests
    {
        private Faker faker;
        private FakerClass<int> testClass;

        [TestInitialize]
        public void SetUp()
        {
            faker = new Faker();
            testClass = faker.Create<FakerClass<int>>();
        }

        [TestMethod]
        public void TestMethod1()
        {

        }

        [TestMethod]
        public void TestIntGenerator()
        {
            Assert.IsTrue(testClass.GetInt != default(int));
        }

        [TestMethod]
        public void TestUintGenerator()
        {
            Assert.IsTrue(testClass.GetUint != default(uint));
        }

        [TestMethod]
        public void TestFloatGenerator()
        {
            Assert.IsTrue(testClass.GetFloat != default(float));
        }

        [TestMethod]
        public void TestDoubleGenerator()
        {
            Assert.IsTrue(testClass.GetDouble != default(double));
        }

        [TestMethod]
        public void TestLongGenerator()
        {
            Assert.IsTrue(testClass.GetLong != default(long));
        }

        [TestMethod]
        public void TestCharGenerator()
        {
            Assert.IsTrue(testClass.GetChar != default(char));
        }

        [TestMethod]
        public void TestStringGenerator()
        {
            Assert.IsTrue(testClass.GetString != default(string));
        }

        [TestMethod]
        public void TestBoolGenerator()
        {
            Assert.IsTrue(testClass.GetBool == false || testClass.GetBool == true);
        }

        [TestMethod]
        public void TestDateGenerator()
        {
            Assert.IsTrue(testClass.GetDate != null);
        }

        [TestMethod]
        public void TestListGenerator()
        {
            Assert.IsTrue(testClass.GetList != null && testClass.GetList.Count != 0);
        }

        [TestMethod]
        public void TestClassGenerator()
        {
            Assert.IsTrue(testClass.GetAnotherClass != null);
        }

        [TestMethod]
        public void TestRecursionCreation()
        {
            A a = faker.Create<A>();
            Assert.IsTrue(a.b != null);
            Assert.IsTrue(a.b.c != null);
            Assert.IsTrue(a.b.c.intNum != default(int));
            Assert.IsTrue(a.c != null);
        }
    }
}