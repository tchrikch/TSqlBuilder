using NUnit.Framework;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    public class ToStringTests
    {
        [Test]
        public void ToString_ShouldBuildSameQuery()
        {
            var builder = Builder.Sql.Select().From("table").Where("Id>3");

            Assert.That(builder.ToString(),Is.EqualTo(builder.Build()));
        }
    }
}