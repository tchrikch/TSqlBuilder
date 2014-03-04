using NUnit.Framework;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    public class FullQueriesTests
    {
        [Test]
        public void QueryWithoutJoin()
        {
            var query = Builder.Select.Columns()
                .From("Test")
                .Where("Id=3").And("Id=5").Or("Id=7")
                .GroupBy("Id").ThenBy("Id")
                .Having("Id>1").And("Id<1")
                .OrderBy("Id").Ascending.Build();

            Assert.That(query,Is.EqualTo("SELECT * FROM [Test] WHERE Id=3 AND Id=5 OR Id=7 GROUP BY [Id],[Id] HAVING Id>1 AND Id<1 ORDER BY [Id] ASC"));
        }

        [Test]
        public void QueryWithJoin()
        {
            var query = Builder.Select.Columns()
                .From("Test").As("T1")
                .Join(JoinMode.Inner, "Test2").As("T2").On("T1.Id=T2.Id")
                .Where("Id=3")
                .OrderBy("Id")
                .Descending
                .Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Test] T1 INNER JOIN [Test2] T2 ON T1.Id=T2.Id WHERE Id=3 ORDER BY [Id] DESC"));

        }
    }
}