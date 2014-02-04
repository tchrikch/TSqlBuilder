using System;
using System.Collections.Generic;
using System.Linq;

namespace TSqlBuilder
{
    public class Builder
    {
        public static ISelectBuilder Sql { get { return new SelectBuilder(); } }
    }

    public class SelectBuilder : ISelectBuilder, INonAliasedTableSelect, IJoinBuilder,IJoinConditionBuilder,INonAliasedJoinBuilder, IComplexWhereBuilder, IComplexGroupByBuilder, IComplexHavingBuilder, IComplexOrderByBuilder
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

        public INonAliasedTableSelect Select(string[] columns)
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

        public IComplexWhereBuilder Where(params string[] conditions)
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

        public IComplexWhereBuilder And(params string[] conditions)
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

        public IComplexWhereBuilder Or(params string[] conditions)
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

        private IComplexWhereBuilder DoWhere(IEnumerable<string> conditions,string prefix,bool shouldWrapStatement = false)
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
    }


    public interface ISelectBuilder
    {
        INonAliasedTableSelect Select(params string[] columns);
    }

    public interface INonAliasedTableSelect : IAliasedTableSelect
    {
        IJoinBuilder As(string alias);
    }

    public interface IAliasedTableSelect : IClauseBuilder
    {
        INonAliasedTableSelect From(string table);
        INonAliasedTableSelect From(string schema, string table);
        INonAliasedTableSelect From(string catalog, string schema, string table);
    }

    public interface INonAliasedJoinBuilder
    {
        IJoinConditionBuilder As(string alias);
    }

    public interface IJoinBuilder : IClauseBuilder
    {
        INonAliasedJoinBuilder Join(JoinMode mode, string table);
        INonAliasedJoinBuilder Join(JoinMode mode, string schema, string table);
        INonAliasedJoinBuilder Join(JoinMode mode, string catalog, string schema, string table);
    }


    public interface IJoinConditionBuilder : ITSqlBuilder
    {
        IJoinBuilder On(params string[] conditions);
    }

    public interface IClauseBuilder : IWhereBuilder , IGroupByBuilder, IHavingBuilder
    {
        
    }

    public interface IWhereBuilder : ITSqlBuilder
    {
        IComplexWhereBuilder Where(params string[] conditions);
    }

    public interface IComplexWhereBuilder : IGroupByBuilder, ILogicConditionBuilder<IComplexWhereBuilder>
    {

    }

    public interface ILogicConditionBuilder<out T>
    {
        T And(params string[] conditions);
        T Or(params string[] conditions);
    }

    public interface IOrderConditionBuilder<out T>
    {
        T ThenBy(params string[] conditions);
    }

    public interface IGroupByBuilder : ITSqlBuilder
    {
        IComplexGroupByBuilder GroupBy(params string[] conditions);
    }

    public interface IComplexGroupByBuilder : IHavingBuilder, IOrderConditionBuilder<IComplexGroupByBuilder>
    {
        
    }

    public interface IOrderByBuilder : ITSqlBuilder
    {
        IComplexOrderByBuilder OrderBy(params string[] conditions);
    }

    public interface IComplexOrderByBuilder : ITSqlBuilder,IOrderConditionBuilder<IComplexOrderByBuilder>
    {
        ITSqlBuilder Ascending { get; }
        ITSqlBuilder Descending { get; }
    }

    public interface IHavingBuilder : IOrderByBuilder
    {
        IComplexHavingBuilder Having(params string[] conditions);
    }

    public interface IComplexHavingBuilder : IOrderByBuilder,ILogicConditionBuilder<IComplexHavingBuilder>
    {
        
    }

    public interface ITSqlBuilder
    {
        string Build();
    }

    public static class StringExtensions
    {
        public static string Quote(this string @string)
        {
            return string.Format("[{0}]", @string);
        }

        public static string TryQuote(this string @string)
        {
            string result;

            if (@string.Length > 1 && @string[0] == '[' && @string[@string.Length - 1] == ']')
                result = @string;
            else result = string.Format("[{0}]", @string);

            return result;
        }

        public static string WrapWithParenthesis(this string @string)
        {
            return string.Format("({0})",@string);
        }

        public static string Expand(this string @string,string value=" ")
        {
            return string.Format("{0}{1}{0}", value, @string);
        }

        public static string ExpandLeft(this string @string, string value = " ")
        {
            return string.Format("{0}{1}", value, @string);
        }

    }

    public static class KeyWords
    {
        public static string And = "AND";
        public static string Or = "OR";
        public static string On = "ON";
        public static string Where = "WHERE";
        public static string Select = "SELECT";
        public static string From = "FROM";
        public static string Star = "*";
        public static string Comma = ",";
        public static string Dot = ".";
        public static string GroupBy = "GROUP BY";
        public static string OrderBy = "ORDER BY";
        public static string Empty = string.Empty;
        public static string Ascending = "ASC";
        public static string Descending = "DESC";
        public static string Space = " ";
        public static string Having = "HAVING";
    }

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

    public enum JoinMode
    {
        Inner,
        LeftOuter,
        RightOuter,
        FullOuter
    }

    class JoinModeConverter : IOneWayConverter<JoinMode>
    {
        private IDictionary<JoinMode, string> _values = new Dictionary<JoinMode, string>
            {
                {JoinMode.RightOuter, "RIGHT OUTER JOIN"},
                {JoinMode.Inner, "INNER JOIN"},
                {JoinMode.FullOuter, "FULL OUTER JOIN"},
                {JoinMode.LeftOuter, "LEFT OUTER JOIN"}
            };

        public string Convert(JoinMode data)
        {
            return _values[data];
        }
    }

    interface IOneWayConverter<in TType>
    {
        string Convert(TType data);
    }
}
