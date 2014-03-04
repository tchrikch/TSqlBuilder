using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Select
{
    public interface IComplexOrderByBuilder : ITSqlBuilder,IOrderConditionBuilder<IComplexOrderByBuilder>
    {
        ITSqlBuilder Ascending { get; }
        ITSqlBuilder Descending { get; }
    }
}