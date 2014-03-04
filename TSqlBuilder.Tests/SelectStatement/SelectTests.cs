using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TSqlBuilder.Builders;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    internal sealed class SelectTests
    {
        [Test]
        public void Build_ReturnsSimpleSelect_ForTable()
        {
            var query = CommandBuilder.Select.Columns().From("Table1").Build();

            Assert.That(query,Is.EqualTo("SELECT * FROM [Table1]"));
        }
        
        [Test]
        public void Build_ReturnSimpleSelect_ForTableAndSchema()
        {
            var query = CommandBuilder.Select.Columns().From("Schema1", "Table1").Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Schema1].[Table1]"));
        }

        [Test]
        public void Build_ReturnSimpleSelect_ForTableAndSchemaAndCatalog()
        {
            var query = CommandBuilder.Select.Columns().From("Catalog1", "Schema1", "Table1").Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Catalog1].[Schema1].[Table1]"));
        }

        [Test]
        public void Build_ReturnColumSelect_ForTable()
        {
            var query = CommandBuilder.Select.Columns("C1", "C2", "C3").From("Table1").Build();

            Assert.That(query, Is.EqualTo("SELECT C1,C2,C3 FROM [Table1]"));
        }

        [Test]
        public void Build_ReturnJoinedComplextColumsSelect_ForTable()
        {
            var query = CommandBuilder.Select.Columns("C1,A1,A2", "C2", "C3,B5").From("Table1").Build();

            Assert.That(query, Is.EqualTo("SELECT C1,A1,A2,C2,C3,B5 FROM [Table1]"));
        }
    }

}
