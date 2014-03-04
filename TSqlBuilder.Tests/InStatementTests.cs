using NUnit.Framework;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    public class InStatementTests
    {
        [SetUp]
        public void Init()
        {
            _nonAliasedTableSelect = Builder.Select.Columns().From("Test");
        }

        private INonAliasedTableSelect _nonAliasedTableSelect;

        [Test]
        public void QueryShouldIncludeValidCondition_ForSimpleListOfValues()
        {
            var query = _nonAliasedTableSelect.Where("Id IN (1,2,3)").Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Test] WHERE Id IN (1,2,3)"));
        }

        [Test]
        public void QueryShouldIncludeValidQuotedCondition_ForInStatementCondition()
        {
            var query = _nonAliasedTableSelect.Where(Conditions.In("Id", new object[] { 1, 2, 3 })).Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Test] WHERE [Id] IN (1,2,3)"));
        }

        [Test]
        public void QueryShouldNotQuoteColumnTwice_WhenColumnAlreadyQuoted()
        {
            var query = _nonAliasedTableSelect.Where(Conditions.In("[Id]", new object[] { 1, 2, 3 })).Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Test] WHERE [Id] IN (1,2,3)"));
        }

    }
}