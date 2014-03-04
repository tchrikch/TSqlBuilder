using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Select
{
    public interface IOrderByBuilder : ITSqlBuilder
    {
        IComplexOrderByBuilder OrderBy(params string[] conditions);
    }
}