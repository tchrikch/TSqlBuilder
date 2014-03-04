using System.Collections.Generic;
using System.Linq;
using TSqlBuilder.Builders.General;
using TSqlBuilder.Extensions;

namespace TSqlBuilder.Builders.Update
{
    class UpdateBuilder : IUpdateBuilder, IUpdateComplexWhereBuilder , ISetBuilder,IWhereBuilder<IUpdateComplexWhereBuilder>
    {
        private readonly IList<string> _conditions = new List<string>();
        private string _table;
        private string _schema;
        private string _catalog;
        private readonly IList<string> _whereData = new List<string>();

        public ISetBuilder Table(string table)
        {
            return Table(null, null, table);
        }

        public ISetBuilder Table(string schema, string table)
        {
            return Table(null, schema, table);
        }

        public ISetBuilder Table(string database, string schema, string table)
        {
            _catalog = database;
            _schema = schema;
            _table = table;

            return this;
        }

        public IWhereBuilder<IUpdateComplexWhereBuilder> Set(params string[] conditions)
        {
            if (conditions == null)
            {
                _conditions.Clear();
            }
            else
                foreach (var column in conditions)
                {
                    _conditions.Add(column);
                }

            return this;
        }

        public IUpdateComplexWhereBuilder Where(params string[] conditions)
        {
            return DoWhere(conditions, KeyWords.And);
        }

        public string Build()
        {
            var tablePart = string.Empty;
            if (!string.IsNullOrEmpty(_catalog)) tablePart += _catalog.Quote() + KeyWords.Dot;
            if (!string.IsNullOrEmpty(_schema)) tablePart += _schema.Quote() + KeyWords.Dot;
            tablePart += _table.Quote();

            var result = string.Format("{0} {1} {2} {3}", KeyWords.Update, tablePart, KeyWords.Set, string.Join(KeyWords.Comma,_conditions));
            if (_whereData.Count > 0)
            {
                var whereStatement = string.Join(KeyWords.Space, _whereData);
                result += KeyWords.Where.Expand() + whereStatement;
            }

            return result;
        }

        public IUpdateComplexWhereBuilder And(params string[] conditions)
        {
            return Where(conditions);
        }

        public IUpdateComplexWhereBuilder Or(params string[] conditions)
        {
            return DoWhere(conditions, KeyWords.Or, true);
        }

        private IUpdateComplexWhereBuilder DoWhere(IEnumerable<string> conditions, string prefix, bool shouldWrapStatement = false)
        {
            var existingConditions = conditions.Where(condition => !string.IsNullOrEmpty(condition)).ToList();
            if (existingConditions.Count == 0) return this;
            if (_whereData.Count > 0) _whereData.Add(prefix);
            var joinedCondition = string.Join(KeyWords.And.Expand(), existingConditions);
            if (shouldWrapStatement && existingConditions.Count > 1) joinedCondition = joinedCondition.WrapWithParenthesis();
            _whereData.Add(joinedCondition);

            return this;
        }
    }
}
