using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Select
{
    public interface IGroupByBuilder : ITSqlBuilder
    {
        IComplexGroupByBuilder GroupBy(params string[] conditions);
    }
}