using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Select
{
    public interface IAliasedTableSelect : IFromClause<INonAliasedTableSelect>, IClauseBuilder
    {
        
    }
}