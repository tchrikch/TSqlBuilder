using TSqlBuilder.Builders.Update;

namespace TSqlBuilder.Builders.General
{
    public interface ITableBuilder 
    {
        ISetBuilder Table(string table);
        ISetBuilder Table(string schema,string table);
        ISetBuilder Table(string database,string schema,string table);
    }
}