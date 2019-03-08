using Yahvol.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yahvol.Linq.Expressions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LinqComposerTests
    {
        [TestMethod]
        public void AndTest()
        {
            IQueryable<TestData> queryable = GenerateTestdata().AsQueryable();

            Expression<Func<TestData, bool>> expression = td => td.Id < 6;
            Expression<Func<TestData, bool>> expression1 = td => td.SignIsTrue;
            
            Assert.AreEqual(3, queryable.Where(expression.And(expression1)).Count());
        }

        [TestMethod]
        public void And_ExpressionsReversable_ReturnsSameResultTest()
        {
            IQueryable<TestData> queryable = GenerateTestdata().AsQueryable();

            Expression<Func<TestData, bool>> expression = td => td.Id < 6;
            Expression<Func<TestData, bool>> expression1 = td => td.SignIsTrue;

            Assert.AreEqual(3, queryable.Where(expression.And(expression1)).Count());
            Assert.AreEqual(3, queryable.Where(expression1.And(expression)).Count());
        }

        [TestMethod]
        public void AndAllTest()
        {
            var testdata = GenerateTestdata();

            Expression<Func<TestData, bool>> expression = td => td.Name == "John";
            Expression<Func<TestData, bool>> expression2 = td => td.Id < 3;
            Expression<Func<TestData, bool>> expression3 = td => td.SignIsTrue ;
            var expressions = new List<Expression<Func<TestData, bool>>> { expression, expression2, expression3 };

            IQueryable<TestData> queryable = testdata.AsQueryable();

            Assert.AreEqual(4, testdata.Count(td => td.Name == "John"));

            Assert.AreEqual(1, queryable.Where(expressions.AndAll()).Count());
        }

        [TestMethod]
        public void AndAll_CombineMultipleExpressions_ReturnsCorrectResults()
        {
            var testdata = GenerateTestdata2();

            Expression<Func<TestData, bool>> expression = td => td.Name == "John";
            Expression<Func<TestData, bool>> expression2 = td => td.Id < 3;
            Expression<Func<TestData, bool>> expression3 = td => td.SignIsTrue;
            var expressions = new List<Expression<Func<TestData, bool>>> { expression, expression2, expression3 };

            IQueryable<TestData> queryable = testdata.AsQueryable();

            Assert.AreEqual(3, testdata.Count(td => td.Name == "John"));

            Assert.AreEqual(1, queryable.Where(expressions.AndAll()).Count());
        }

        [TestMethod]
        public void Or_ExpressionsReversable_ReturnsSameResultTest()
        {
            IQueryable<TestData> queryable = GenerateTestdata().AsQueryable();

            Expression<Func<TestData, bool>> expression = td => td.Name == "Steve";
            Expression<Func<TestData, bool>> expression1 = td => td.SignIsTrue == false;

            Assert.AreEqual(3, queryable.Where(expression.Or(expression1)).Count());
            Assert.AreEqual(3, queryable.Where(expression1.Or(expression)).Count());
        }

        [TestMethod]
        public void AndORTest()
        {
            var testdata = GenerateTestdata();

            Expression<Func<TestData, bool>> expression = td => td.Name == "John";
            Expression<Func<TestData, bool>> expression2 = td => td.Id == 1;
            Expression<Func<TestData, bool>> expression3 = td => td.Name == "Wilson";

            var combinedExpression = expression.And(expression2);
            var combinedExpression3 = expression.Or(expression3);
            
            IQueryable<TestData> queryable = testdata.AsQueryable();

            Assert.AreEqual(4, testdata.Count(td => td.Name == "John"));
            Assert.AreEqual(4, queryable.Count(expression));
            Assert.AreEqual(1, queryable.Where(expression2).Count());
            Assert.AreEqual(1, queryable.Where(combinedExpression).Count());
            Assert.AreEqual(5, queryable.Where(combinedExpression3).Count());
        }

        private static List<TestData> GenerateTestdata()
        {
            var testdata = new List<TestData>();
            testdata.Add(new TestData() { Id = 1, Name = "John", RandomData = Guid.NewGuid().ToString(), SignIsTrue = true });
            testdata.Add(new TestData() { Id = 2, Name = "John", RandomData = Guid.NewGuid().ToString(), SignIsTrue = false }); 
            testdata.Add(new TestData() { Id = 3, Name = "John", RandomData = Guid.NewGuid().ToString(), SignIsTrue = true });
            testdata.Add(new TestData() { Id = 4, Name = "John", RandomData = Guid.NewGuid().ToString(), SignIsTrue = false }); 
            testdata.Add(new TestData() { Id = 5, Name = "Steve", RandomData = Guid.NewGuid().ToString(), SignIsTrue = true });
            testdata.Add(new TestData() { Id = 6, Name = "Wilson", RandomData = Guid.NewGuid().ToString(), SignIsTrue = true });
            return testdata;
        }

        private static List<TestData> GenerateTestdata2()
        {
            var testdata = new List<TestData>();
            testdata.Add(new TestData() { Id = 1, Name = "John", RandomData = Guid.NewGuid().ToString(), SignIsTrue = true });
            testdata.Add(new TestData() { Id = 2, Name = "Bob", RandomData = Guid.NewGuid().ToString(), SignIsTrue = true });
            testdata.Add(new TestData() { Id = 3, Name = "John", RandomData = Guid.NewGuid().ToString(), SignIsTrue = true });
            testdata.Add(new TestData() { Id = 4, Name = "John", RandomData = Guid.NewGuid().ToString(), SignIsTrue = false });
            testdata.Add(new TestData() { Id = 5, Name = "Steve", RandomData = Guid.NewGuid().ToString(), SignIsTrue = true });
            testdata.Add(new TestData() { Id = 6, Name = "Wilson", RandomData = Guid.NewGuid().ToString(), SignIsTrue = true });
            return testdata;
        }

        public class TestData 
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string RandomData { get; set; }

            public bool SignIsTrue { get; set; }
        }
    }
}
