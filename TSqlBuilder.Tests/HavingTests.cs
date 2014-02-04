using NUnit.Framework;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    internal sealed class HavingTests
    {
        private static IHavingBuilder Builder { get { return TSqlBuilder.Builder.Sql.Select().From("Table1"); } }

        [Test]
        public void Build_ReturnsWhereForSingleColumn()
        {
            var query = Builder.Having("A1='test'").Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] HAVING A1='test'"));
        }

        [Test]
        public void Build_ReturnsWhereForMultipleColumnsColumn_ProvidedAsOneBatch()
        {
            var query = Builder.Having("A1='test'", "A2='test'").Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] HAVING A1='test' AND A2='test'"));
        }

        [Test]
        public void Build_ReturnsWhereForMultipleColumnsColumn_ProvidedAsMultipleBatches()
        {
            var query = Builder.Having("A1='test'", "A2='test'").And("A3='test'").Build();
            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] HAVING A1='test' AND A2='test' AND A3='test'"));
        }

        [Test]
        public void Build_ReturnsWhereForMultipleAndConditions()
        {
            var query = Builder.Having("A1='test'").And("A2='test'").And("A3='test'").Build();
            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] HAVING A1='test' AND A2='test' AND A3='test'"));
        }

        [Test]
        public void Build_ReturnsMultipleLogicalConditionsWithoutParenthesis()
        {
            var query = Builder.Having("A1='test'").Or("A2='test'").And("A3='test'").Build();
            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] HAVING A1='test' OR A2='test' AND A3='test'"));
        }

        [Test]
        public void Build_EmbeddsMultipleConditionsInsideOrClause_WithParenthesis()
        {
            var query = Builder.Having("A1='test'").Or("A2='test'", "A3='test'").Build();
            Assert.That(query, Is.EqualTo("SELECT * FROM [Table1] HAVING A1='test' OR (A2='test' AND A3='test')"));
        }
    }

    [TestFixture]
    public class FullQueriesTests
    {
        [Test]
        public void T1()
        {
            var query = Builder.Sql.Select()
                .From("Test")
                .Where("Id=3").And("Id=5").Or("Id=7")
                .GroupBy("Id").ThenBy("Id")
                .Having("Id>1").And("Id<1")
                .OrderBy("Id").Ascending.Build();

            Assert.That(query,Is.EqualTo("SELECT * FROM [Test] WHERE Id=3 AND Id=5 OR Id=7 GROUP BY [Id],[Id] HAVING Id>1 AND Id<1 ORDER BY [Id] ASC"));
        }
    }

    [TestFixture]
    public class JoinTests
    {
        private IJoinBuilder JoinBuilder { get { return Builder.Sql.Select().From("Table").As(TableAlias); } }
        private const string SomeTable = "SomeTable";
        private const string TableAlias = "T1";
        private const string JoinedTableAlias = "T2";
        private const string SomeCondition = "SomeCondition";
        private const string SomeSchema = "SomeSchema";
        private const string SomeCatalog = "SomeCatalog";

        [TestCase(JoinMode.FullOuter, ExpectedResult = "SELECT * FROM [Table] T1 FULL OUTER JOIN [SomeTable] T2 ON SomeCondition")]
        [TestCase(JoinMode.Inner, ExpectedResult = "SELECT * FROM [Table] T1 INNER JOIN [SomeTable] T2 ON SomeCondition")]
        [TestCase(JoinMode.RightOuter, ExpectedResult = "SELECT * FROM [Table] T1 RIGHT OUTER JOIN [SomeTable] T2 ON SomeCondition")]
        [TestCase(JoinMode.LeftOuter, ExpectedResult = "SELECT * FROM [Table] T1 LEFT OUTER JOIN [SomeTable] T2 ON SomeCondition")]
        public string ShouldReturnJoinWithTableOnCondition_WhenConditionProvided_ForDifferentModes(JoinMode mode)
        {
            var query = JoinBuilder.Join(mode, SomeTable).As(JoinedTableAlias).On(SomeCondition).Build();

            return query;
        }

        [TestCase(JoinMode.FullOuter, ExpectedResult = "SELECT * FROM [Table] T1 FULL OUTER JOIN [SomeSchema].[SomeTable] T2 ON SomeCondition")]
        [TestCase(JoinMode.Inner, ExpectedResult = "SELECT * FROM [Table] T1 INNER JOIN [SomeSchema].[SomeTable] T2 ON SomeCondition")]
        [TestCase(JoinMode.RightOuter, ExpectedResult = "SELECT * FROM [Table] T1 RIGHT OUTER JOIN [SomeSchema].[SomeTable] T2 ON SomeCondition")]
        [TestCase(JoinMode.LeftOuter, ExpectedResult = "SELECT * FROM [Table] T1 LEFT OUTER JOIN [SomeSchema].[SomeTable] T2 ON SomeCondition")]
        public string ShouldReturnJoinWithTableAndSchemaOnCondition_WhenConditionProvided_ForDifferentModes(JoinMode mode)
        {
            var query = JoinBuilder.Join(mode, SomeSchema, SomeTable).As(JoinedTableAlias).On(SomeCondition).Build();

            return query;
        }

        [TestCase(JoinMode.FullOuter, ExpectedResult = "SELECT * FROM [Table] T1 FULL OUTER JOIN [SomeCatalog].[SomeSchema].[SomeTable] T2 ON SomeCondition")]
        [TestCase(JoinMode.Inner, ExpectedResult = "SELECT * FROM [Table] T1 INNER JOIN [SomeCatalog].[SomeSchema].[SomeTable] T2 ON SomeCondition")]
        [TestCase(JoinMode.RightOuter, ExpectedResult = "SELECT * FROM [Table] T1 RIGHT OUTER JOIN [SomeCatalog].[SomeSchema].[SomeTable] T2 ON SomeCondition")]
        [TestCase(JoinMode.LeftOuter, ExpectedResult = "SELECT * FROM [Table] T1 LEFT OUTER JOIN [SomeCatalog].[SomeSchema].[SomeTable] T2 ON SomeCondition")]
        public string ShouldReturnJoinWithTableAndSchemaAndCatalogOnCondition_WhenConditionProvided_ForDifferentModes(JoinMode mode)
        {
            var query = JoinBuilder.Join(mode, SomeCatalog, SomeSchema, SomeTable).As(JoinedTableAlias).On(SomeCondition).Build();

            return query;
        }
    }

    [TestFixture]
    public class AliasTests
    {
        private const string SomeTable = "Test";
        private const string EmptyAlias = "";
        private const string SomeAlias = "SomeAlias";

        [Test]
        public void ShouldSkipAlias_IfNull()
        {
            var query = Builder.Sql.Select().From(SomeTable).As(null).Build();

            Assert.That(query,Is.EqualTo("SELECT * FROM [Test]"));
        }

        [Test]
        public void ShouldSkipAlias_IfEmpty()
        {
            var query = Builder.Sql.Select().From(SomeTable).As(EmptyAlias).Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Test]"));
        }

        [Test]
        public void ShouldAddAliasToTable_WhenRequested()
        {
            var query = Builder.Sql.Select().From(SomeTable).As(SomeAlias).Build();

            Assert.That(query, Is.EqualTo("SELECT * FROM [Test] SomeAlias"));
        }
    }
}