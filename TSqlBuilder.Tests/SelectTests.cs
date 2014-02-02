using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    internal sealed class SelectTests
    {
        [Test]
        public void Build_ReturnsSimpleSelect_ForTable()
        {
            var query = Builder.Sql.Select().From("Table1").Build();

            Assert.That(query,Is.EqualTo("SELECT * FROM [Table1]"));
        }
        
        [Test]
        public void Build_ReturnSimpleSelect_ForTableAndSchema()
        {
            var query = Builder.Sql.Select().From("Schema1", "Table1").Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Schema1].[Table1]"));
        }

        [Test]
        public void Build_ReturnSimpleSelect_ForTableAndSchemaAndCatalog()
        {
            var query = Builder.Sql.Select().From("Catalog1", "Schema1", "Table1").Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Catalog1].[Schema1].[Table1]"));
        }

        [Test]
        public void Build_ReturnColumSelect_ForTable()
        {
            var query = Builder.Sql.Select("C1","C2","C3").From("Table1").Build();

            Assert.That(query, Is.EqualTo("SELECT C1,C2,C3 FROM [Table1]"));
        }

        [Test]
        public void Build_ReturnJoinedComplextColumsSelect_ForTable()
        {
            var query = Builder.Sql.Select("C1,A1,A2", "C2", "C3,B5").From("Table1").Build();

            Assert.That(query, Is.EqualTo("SELECT C1,A1,A2,C2,C3,B5 FROM [Table1]"));
        }
    }


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
