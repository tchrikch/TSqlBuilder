namespace TSqlBuilder.Builders.Select
{
    public interface INonAliasedJoinBuilder
    {
        IJoinConditionBuilder As(string alias);
    }
}