using System.Collections.Generic;
using System.Linq;
using TSqlBuilder.Builders.General;
using TSqlBuilder.Converters;
using TSqlBuilder.Extensions;

namespace TSqlBuilder.Builders.Select
{
    public class SelectBuilder : ISelectBuilder, INonAliasedTableSelect, IJoinBuilder, IJoinConditionBuilder, INonAliasedJoinBuilder, ISelectComplexWhereBuilder, IComplexGroupByBuilder, IComplexHavingBuilder, IComplexOrderByBuilder
    {
        private readonly IList<string> _columns = new List<string>();
        private string _table;
        private string _schema;
        private string _catalog;
        private string _alias;
        private readonly IList<string> _whereData = new List<string>();
        private readonly IList<string> _havingData = new List<string>();
        private readonly IList<string> _groupByData = new List<string>(); 
        private readonly IList<string> _orderByData = new List<string>();
        private string _orderMode = KeyWords.Empty;

        private readonly IList<JoinData> _joinDatas = new List<JoinData>();

        public INonAliasedTableSelect Columns(string[] columns)
        {
            if(columns==null)
            {
                _columns.Clear();
            }
            else 
                foreach (var column in columns)
                {
                    _columns.Add(column);
                }

            return this;
        }

        public INonAliasedTableSelect All()
        {
            return Columns(new string[0]);
        }

        public INonAliasedTableSelect From(string table)
        {
            return From(null, null, table);
        }

        public INonAliasedTableSelect From(string schema, string table)
        {
            return From(null, schema, table);
        }

        public INonAliasedTableSelect From(string catalog, string schema, string table)
        {
            _catalog = catalog;
            _schema = schema;
            _table = table;

            return this;
        }

        public IJoinBuilder As(string alias)
        {
            _alias = alias;

            return this;
        }

        public string Build()
        {
            var columnsPart = _columns.Count == 0 ? KeyWords.Star : string.Join(KeyWords.Comma, _columns);
            var tablePart = string.Empty;
            if (!string.IsNullOrEmpty(_catalog)) tablePart += _catalog.Quote() + KeyWords.Dot;
            if (!string.IsNullOrEmpty(_schema)) tablePart += _schema.Quote() + KeyWords.Dot;
            tablePart += _table.Quote();
            if(!string.IsNullOrEmpty(_alias)) tablePart+=KeyWords.Space+_alias;

            var result = string.Format("{0} {1} {2} {3}",KeyWords.Select, columnsPart,KeyWords.From, tablePart);

            foreach (var joinData in _joinDatas)
            {
                var converter = new JoinModeConverter();
                var stmt = converter.Convert(joinData.Mode);

                var tableNameParts = new[] { joinData.Catalog, joinData.Schema, joinData.Table }.Where(item => !string.IsNullOrEmpty(item)).Select(item => item.Quote());
                var tableName = string.Join(KeyWords.Dot, tableNameParts);
                stmt += KeyWords.Space + tableName;
                if (!string.IsNullOrEmpty(joinData.Alias)) stmt += KeyWords.Space + joinData.Alias;
                if (joinData.Conditions.Count > 0) stmt += KeyWords.On.Expand();

                for (var i = 0; i < joinData.Conditions.Count; ++i)
                {
                    stmt += joinData.Conditions[i];
                    if (i != joinData.Conditions.Count - 1) stmt += KeyWords.And.Expand();
                }

                result += KeyWords.Space + stmt;
            }

            if (_whereData.Count > 0)
            {
                var whereStatement = string.Join(KeyWords.Space, _whereData);
                result += KeyWords.Where.Expand() + whereStatement;
            }

            if (_groupByData.Count > 0)
            {
                var groupStatement = string.Join(KeyWords.Comma, _groupByData);
                result += KeyWords.GroupBy.Expand() + groupStatement;
            }

            if (_havingData.Count > 0)
            {
                var havingStatement = string.Join(KeyWords.Space, _havingData);
                result += KeyWords.Having.Expand() + havingStatement;
            }

            if (_orderByData.Count > 0)
            {
                var groupStatement = string.Join(KeyWords.Comma, _orderByData);
                result += KeyWords.OrderBy.Expand() + groupStatement;
                if (_orderMode != KeyWords.Empty)
                {
                    result += _orderMode.ExpandLeft();
                }
            }

            return result;
        }

        public IComplexHavingBuilder Having(params string[] conditions)
        {
            return DoHaving(conditions, KeyWords.And);
        }

        public ISelectComplexWhereBuilder Where(params string[] conditions)
        {
            return DoWhere(conditions, KeyWords.And);
        }

        public IComplexOrderByBuilder OrderBy(params string[] conditions)
        {
            foreach (var condition in conditions)
            {
                if (!string.IsNullOrEmpty(condition))
                    _orderByData.Add(condition.TryQuote());
            }

            return this;
        }

        public IComplexGroupByBuilder ThenBy(params string[] conditions)
        {
            return GroupBy(conditions);
        }

        public IComplexGroupByBuilder GroupBy(params string[] conditions)
        {
            foreach (var condition in conditions)
            {
                if(!string.IsNullOrEmpty(condition))
                    _groupByData.Add(condition.TryQuote());
            }

            return this;
        }

        public ITSqlBuilder Ascending
        {
            get
            {
                _orderMode = KeyWords.Ascending;

                return this;
            }
        }

        public ITSqlBuilder Descending
        {
            get
            {
                _orderMode = KeyWords.Descending;

                return this;
            }
        }

        public ISelectComplexWhereBuilder And(params string[] conditions)
        {
            return Where(conditions);
        }

        IComplexHavingBuilder ILogicConditionBuilder<IComplexHavingBuilder>.Or(params string[] conditions)
        {
            return DoHaving(conditions, KeyWords.Or, true);
        }

        IComplexHavingBuilder ILogicConditionBuilder<IComplexHavingBuilder>.And(params string[] conditions)
        {
            return DoHaving(conditions, KeyWords.And);
        }

        public ISelectComplexWhereBuilder Or(params string[] conditions)
        {
            return DoWhere(conditions, KeyWords.Or, true);
        }

        private IComplexHavingBuilder DoHaving(IEnumerable<string> conditions, string prefix, bool shouldWrapStatement = false)
        {
            var existingConditions = conditions.Where(condition => !string.IsNullOrEmpty(condition)).ToList();
            if (existingConditions.Count == 0) return this;
            if (_havingData.Count > 0) _havingData.Add(prefix);
            var joinedCondition = string.Join(KeyWords.And.Expand(), existingConditions);
            if (shouldWrapStatement && existingConditions.Count > 1) joinedCondition = joinedCondition.WrapWithParenthesis();
            _havingData.Add(joinedCondition);

            return this;
        }

        private ISelectComplexWhereBuilder DoWhere(IEnumerable<string> conditions, string prefix, bool shouldWrapStatement = false)
        {
            var existingConditions = conditions.Where(condition => !string.IsNullOrEmpty(condition)).ToList();
            if (existingConditions.Count == 0) return this;
            if (_whereData.Count > 0) _whereData.Add(prefix);
            var joinedCondition = string.Join(KeyWords.And.Expand(), existingConditions);
            if (shouldWrapStatement && existingConditions.Count > 1) joinedCondition = joinedCondition.WrapWithParenthesis();
            _whereData.Add(joinedCondition);

            return this;
        }

        IComplexOrderByBuilder IOrderConditionBuilder<IComplexOrderByBuilder>.ThenBy(params string[] conditions)
        {
            return OrderBy(conditions);
        }

        public INonAliasedJoinBuilder Join(JoinMode mode, string table)
        {
            return Join(mode, null, null, table);
        }

        public INonAliasedJoinBuilder Join(JoinMode mode, string schema, string table)
        {
            return Join(mode, null, schema, table);
        }

        public INonAliasedJoinBuilder Join(JoinMode mode, string catalog, string schema, string table)
        {
            var data = new JoinData(catalog, schema, table, mode);
            _joinDatas.Add(data);

            return this;
        }

        public IJoinBuilder On(params string[] conditions)
        {
            var lastJoinData = _joinDatas.Last();

            foreach (var condition in conditions)
            {
                if (!string.IsNullOrEmpty(condition))
                {
                    lastJoinData.Conditions.Add(condition);
                }
            }

            return this;
        }

        IJoinConditionBuilder INonAliasedJoinBuilder.As(string alias)
        {
            _joinDatas.Last().Alias = alias;

            return this;
        }

        public override string ToString()
        {
            return Build();
        }
    }
}