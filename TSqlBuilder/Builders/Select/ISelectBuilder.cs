namespace TSqlBuilder.Builders.Select
{
    public interface ISelectBuilder
    {
        INonAliasedTableSelect Columns(params string[] columns);
        INonAliasedTableSelect All();
    }
}