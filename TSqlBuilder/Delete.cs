using System;
using System.Collections.Generic;
using System.Linq;

namespace TSqlBuilder
{
    public class DeleteBuilder : IDeleteBuilder, IDeleteComplexWhereBuilder, IWhereBuilder<IDeleteComplexWhereBuilder>
    {
        private string _table;
        private string _schema;
        private string _catalog;
        private readonly IList<string> _whereData = new List<string>();

        public IWhereBuilder<IDeleteComplexWhereBuilder> From(string table)
        {
            return From(null, null, table);
        }

        public IWhereBuilder<IDeleteComplexWhereBuilder> From(string schema, string table)
        {
            return From(null, schema, table);
        }

        public IWhereBuilder<IDeleteComplexWhereBuilder> From(string catalog, string schema, string table)
        {
            _catalog = catalog;
            _schema = schema;
            _table = table;

            return this;
        }

        public IDeleteComplexWhereBuilder Where(params string[] conditions)
        {
            return DoWhere(conditions, KeyWords.And);
        }

        private IDeleteComplexWhereBuilder DoWhere(IEnumerable<string> conditions, string prefix, bool shouldWrapStatement = false)
        {
            var existingConditions = conditions.Where(condition => !string.IsNullOrEmpty(condition)).ToList();
            if (existingConditions.Count == 0) return this;
            if (_whereData.Count > 0) _whereData.Add(prefix);
            var joinedCondition = string.Join(KeyWords.And.Expand(), existingConditions);
            if (shouldWrapStatement && existingConditions.Count > 1) joinedCondition = joinedCondition.WrapWithParenthesis();
            _whereData.Add(joinedCondition);

            return this;
        }

        public string Build()
        {
            var tablePart = string.Empty;
            if (!string.IsNullOrEmpty(_catalog)) tablePart += _catalog.TryQuote() + KeyWords.Dot;
            if (!string.IsNullOrEmpty(_schema)) tablePart += _schema.TryQuote() + KeyWords.Dot;
            tablePart += _table.TryQuote();

            var result = string.Format("{0} {1} {2}", KeyWords.Delete, KeyWords.From, tablePart);

            if (_whereData.Count > 0)
            {
                var whereStatement = string.Join(KeyWords.Space, _whereData);
                result += KeyWords.Where.Expand() + whereStatement;
            }

            return result;
        }

        public IDeleteComplexWhereBuilder And(params string[] conditions)
        {
            return Where(conditions);
        }

        public IDeleteComplexWhereBuilder Or(params string[] conditions)
        {
            return DoWhere(conditions, KeyWords.Or, true);
        }
    }

    public interface IDeleteBuilder : IFromClause<IWhereBuilder<IDeleteComplexWhereBuilder>>, ITSqlBuilder
    {
        
    }

    public interface IDeleteComplexWhereBuilder : ILogicConditionBuilder<IDeleteComplexWhereBuilder> ,ITSqlBuilder
    {
        
    }
}

    
