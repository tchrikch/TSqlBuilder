using System.Collections.Generic;

namespace TSqlBuilder.Builders.Select
{
    class JoinData
    {
        public JoinData(string catalog, string schema, string table,JoinMode mode)
        {
            Catalog = catalog;
            Schema = schema;
            Table = table;
            Mode = mode;
            Conditions = new List<string>();
        }

        public string Alias { get; set; }
        public string Table { get; private set; }
        public string Schema { get; private set; }
        public string Catalog { get; private set; }
        public JoinMode Mode { get; private set; }

        public IList<string> Conditions { get; private set; }
    }
}