using NUnit.Framework;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    class GroupByTests
    {
        private static IGroupByBuilder Builder { get { return TSqlBuilder.Builder.Select.Columns().From("A"); } }

        [Test]
        public void Build_ReturnsCsvForGroupByColumns()
        {
            var result = Builder.GroupBy("A1", "A2", "A3").Build();

            Assert.That(result,Is.EqualTo("SELECT * FROM [A] GROUP BY [A1],[A2],[A3]"));
        }

        [Test]
        public void Build_ReturnsQuotedGroupBy_SingleColumn()
        {
            var result = Builder.GroupBy("A1").Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] GROUP BY [A1]"));
        }

        [Test]
        public void Build_DetecsQuotedColumns_ListContainsAlreadyQuotedColumns()
        {
            var result = Builder.GroupBy("A1","[A1]").Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] GROUP BY [A1],[A1]"));
        }

        [Test]
        public void Build_ReturnsGroupByForTwoCOlumns_GroupByPlusThenBy()
        {
            var result = Builder.GroupBy("A1").ThenBy("A2").Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] GROUP BY [A1],[A2]"));
        }

        [Test]
        public void Build_ReturnsCsvForMultipleColumns_GroupByPlusThenByAndThenByWithMultipleValues()
        {
            var result = Builder.GroupBy("A1").ThenBy("A2").ThenBy("A3","A4").Build();

            Assert.That(result, Is.EqualTo("SELECT * FROM [A] GROUP BY [A1],[A2],[A3],[A4]"));
        }
    }
}
