using System.Collections.Generic;
using System.Linq;

namespace TSqlBuilder
{
    public class Builder
    {
        public static ISelectBuilder Sql { get { return new SelectBuilder(); } }
    }

    public class SelectBuilder : ISelectBuilder, ITableSelect, IClauseBuilder, IComplexWhereBuilder, IComplexGroupByBuilder, IComplexHavingBuilder, IComplexOrderByBuilder
    {
        private readonly IList<string> _columns = new List<string>();
        private string _table;
        private string _schema;
        private string _catalog;
        private readonly IList<string> _whereData = new List<string>();
        private readonly IList<string> _havingData = new List<string>();
        private readonly IList<string> _groupByData = new List<string>(); 
        private readonly IList<string> _orderByData = new List<string>();
        private string _orderMode = KeyWords.Empty;

        public ITableSelect Select(string[] columns)
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

        public IClauseBuilder From(string table)
        {
            return From(null, null, table);
        }

        public IClauseBuilder From(string schema, string table)
        {
            return From(null, schema, table);
        }

        public IClauseBuilder From(string catalog, string schema, string table)
        {
            _catalog = catalog;
            _schema = schema;
            _table = table;

            return this;
        }

        public string Build()
        {
            var columnsPart = _columns.Count == 0 ? KeyWords.Star : string.Join(KeyWords.Comma, _columns);
            var tablePart = string.Empty;
            if (!string.IsNullOrEmpty(_catalog)) tablePart += _catalog.Quote() + KeyWords.Dot;
            if (!string.IsNullOrEmpty(_schema)) tablePart += _schema.Quote() + KeyWords.Dot;
            tablePart += _table.Quote();

           var result = string.Format("{0} {1} {2} {3}",KeyWords.Select, columnsPart,KeyWords.From, tablePart);

           if (_whereData.Count > 0)
           {
               var whereStatement = string.Join(" ", _whereData);
               result += KeyWords.Where.Expand() + whereStatement;
           }

           if (_groupByData.Count > 0)
           {
               var groupStatement = string.Join(KeyWords.Comma, _groupByData);
               result += KeyWords.GroupBy.Expand() + groupStatement;
           }

           if (_havingData.Count > 0)
           {
               var havingStatement = string.Join(" ", _havingData);
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
    }


    public interface ISelectBuilder
    {
        ITableSelect Select(params string[] columns);
    }

    public interface ITableSelect
    {
        IClauseBuilder From(string table);
        IClauseBuilder From(string schema, string table);
        IClauseBuilder From(string catalog, string schema, string table);
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
        public static string Having = "HAVING";
    }

}
