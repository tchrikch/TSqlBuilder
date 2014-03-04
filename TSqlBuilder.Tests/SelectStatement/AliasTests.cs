using NUnit.Framework;
using TSqlBuilder.Builders;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    public class AliasTests
    {
        private const string SomeTable = "Test";
        private const string EmptyAlias = "";
        private const string SomeAlias = "SomeAlias";

        [Test]
        public void ShouldSkipAlias_IfNull()
        {
            var query = CommandBuilder.Select.Columns().From(SomeTable).As(null).Build();

            Assert.That(query,Is.EqualTo("SELECT * FROM [Test]"));
        }

        [Test]
        public void ShouldSkipAlias_IfEmpty()
        {
            var query = CommandBuilder.Select.Columns().From(SomeTable).As(EmptyAlias).Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Test]"));
        }

        [Test]
        public void ShouldAddAliasToTable_WhenRequested()
        {
            var query = CommandBuilder.Select.Columns().From(SomeTable).As(SomeAlias).Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Test] SomeAlias"));
        }
    }

    [TestFixture]
    public class ToStringTests
    {
        [Test]
        public void ToString_ShouldBuildSameQuery()
        {
            var builder = CommandBuilder.Select.Columns().From("table").Where("Id>3");

            Assert.That(builder.ToString(),Is.EqualTo(builder.Build()));
        }
    }
}