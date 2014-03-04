using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Select
{
    public interface IJoinConditionBuilder : ITSqlBuilder
    {
        IJoinBuilder On(params string[] conditions);
    }
}