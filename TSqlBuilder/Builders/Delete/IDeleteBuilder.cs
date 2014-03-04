using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Delete
{
    public interface IDeleteBuilder : IFromClause<IWhereBuilder<IDeleteComplexWhereBuilder>>, ITSqlBuilder
    {
        
    }
}