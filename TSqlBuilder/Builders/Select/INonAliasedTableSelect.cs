namespace TSqlBuilder.Builders.Select
{
    public interface INonAliasedTableSelect : IAliasedTableSelect
    {
        IJoinBuilder As(string alias);
    }
}