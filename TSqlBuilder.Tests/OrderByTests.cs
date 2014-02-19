using NUnit.Framework;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    class OrderByTests
    {
        private static IOrderByBuilder Builder { get { return TSqlBuilder.Builder.Select().From("A"); } }

        [Test]
        public void Build_ReturnsCsvForOrderByColumns()
        {
            var result = Builder.OrderBy("A1", "A2", "A3").Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] ORDER BY [A1],[A2],[A3]"));
        }

        [Test]
        public void Build_ReturnsQuotedOrderBy_SingleColumn()
        {
            var result = Builder.OrderBy("A1").Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] ORDER BY [A1]"));
        }

        [Test]
        public void Build_DetectsQuotedColumns_ListContainsAlreadyQuotedColumns()
        {
            var result = Builder.OrderBy("A1", "[A1]").Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] ORDER BY [A1],[A1]"));
        }

        [Test]
        public void Build_ReturnsOrderByForTwoColumns_OrderByPlusThenBy()
        {
            var result = Builder.OrderBy("A1").ThenBy("A2").Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] ORDER BY [A1],[A2]"));
        }

        [Test]
        public void Build_ReturnsColumns_ExplicitAscendingOrder()
        {
            var result = Builder.OrderBy("A1").ThenBy("A2").Ascending.Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] ORDER BY [A1],[A2] ASC"));
        }

        [Test]
        public void Build_ReturnsColumns_ExplicitDescendingOrder()
        {
            var result = Builder.OrderBy("A1").ThenBy("A2").Descending.Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] ORDER BY [A1],[A2] DESC"));
        }

        [Test]
        public void Build_ReturnsCsvForMultipleColumns_OrderByPlusThenByAndThenByWithMultipleValues()
        {
            var result = Builder.OrderBy("A1").ThenBy("A2").ThenBy("A3", "A4").Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] ORDER BY [A1],[A2],[A3],[A4]"));
        }
    }
}