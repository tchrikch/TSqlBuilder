namespace TSqlBuilder.Builders.General
{
    public interface IWhereBuilder<out T> : ITSqlBuilder
    {
        T Where(params string[] conditions);
    }
}