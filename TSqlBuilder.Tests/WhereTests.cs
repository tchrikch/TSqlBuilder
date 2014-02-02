using NUnit.Framework;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    internal sealed class WhereTests
    {
        private IClauseBuilder GetBuilder()
        {
            return Builder.Sql.Select().From("Table1");
        } 

        [Test]
        public void Build_ReturnsWhereForSingleColumn()
        {
            var query = GetBuilder().Where("A1='test'").Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] WHERE A1='test'"));
        }

        [Test]
        public void Build_ReturnsWhereForMultipleColumnsColumn_ProvidedAsOneBatch()
        {
            var query = GetBuilder().Where("A1='test'","A2='test'").Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] WHERE A1='test' AND A2='test'"));
        }

        [Test]
        public void Build_ReturnsWhereForMultipleColumnsColumn_ProvidedAsMultipleBatches()
        {
            var query = GetBuilder().Where("A1='test'", "A2='test'").And("A3='test'").Build();
            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] WHERE A1='test' AND A2='test' AND A3='test'"));
        }

        [Test]
        public void Build_ReturnsWhereForMultipleAndConditions()
        {
            var query = GetBuilder().Where("A1='test'").And("A2='test'").And("A3='test'").Build();
            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] WHERE A1='test' AND A2='test' AND A3='test'"));
        }

        [Test]
        public void Build_ReturnsMultipleLogicalConditionsWithoutParenthesis()
        {
            var query = GetBuilder().Where("A1='test'").Or("A2='test'").And("A3='test'").Build();
            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] WHERE A1='test' OR A2='test' AND A3='test'"));
        }

        [Test]
        public void Build_EmbeddsMultipleConditionsInsideOrClause_WithParenthesis()
        {
            var query = GetBuilder().Where("A1='test'").Or("A2='test'", "A3='test'").Build();
            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] WHERE A1='test' OR (A2='test' AND A3='test')"));
        }
    }
}