using NUnit.Framework;

namespace TSqlBuilder.Tests
{
    [TestFixture]
    public class JoinTests
    {
        private IJoinBuilder JoinBuilder { get { return Builder.Select().From("Table").As(TableAlias); } }
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
}