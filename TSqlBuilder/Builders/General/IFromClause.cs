namespace TSqlBuilder.Builders.General
{
    public interface IFromClause<out T>
    {
        T From(string table);
        T From(string schema, string table);
        T From(string catalog, string schema, string table);
    }
}