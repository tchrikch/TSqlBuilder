using System;
using System.Collections.Generic;
using System.Linq;

namespace TSqlBuilder
{
    public class Builder
    {
        public static ISelectBuilder Sql { get { return new SelectBuilder(); } }
    }

    public class SelectBuilder : ISelectBuilder , ITableSelect, IClauseBuilder, IComplexWhereBuilder
    {
        private readonly IList<string> _columns = new List<string>();
        private string _table;
        private string _schema;
        private string _catalog;
        private readonly IList<string> _whereData = new List<string>(); 

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
            if (!string.IsNullOrEmpty(_catalog)) tablePart += _catalog.GetQuoted() + KeyWords.Dot;
            if (!string.IsNullOrEmpty(_schema)) tablePart += _schema.GetQuoted() + KeyWords.Dot;
            tablePart += _table.GetQuoted();

           var result = string.Format("{0} {1} {2} {3}",KeyWords.Select, columnsPart,KeyWords.From, tablePart);

           if (_whereData.Count > 0)
           {
               var whereStatement = string.Join(" ", _whereData);
               result += KeyWords.Where.Expand() + whereStatement;
           }

            return result;
        }

        public IComplexHavingBuilder Having(params string[] conditions)
        {
            throw new NotImplementedException();
        }


        public IComplexWhereBuilder Where(params string[] conditions)
        {
            return DoWhere(conditions, KeyWords.And);
        }

        public IOrderByBuilder OrderBy(params string[] conditions)
        {
            throw new NotImplementedException();
        }

        public IGroupByBuilder ThenBy(params string[] conditions)
        {
            throw new NotImplementedException();
        }

        public IGroupByBuilder GroupBy(params string[] conditions)
        {
            throw new NotImplementedException();
        }

        IOrderByBuilder IOrderByBuilder.ThenBy(params string[] conditions)
        {
            return ThenBy(conditions);
        }

        public IGroupByBuilder Ascending()
        {
            throw new NotImplementedException();
        }

        public IGroupByBuilder Descending()
        {
            throw new NotImplementedException();
        }

        public IComplexWhereBuilder And(params string[] conditions)
        {
            return Where(conditions);
        }

        public IComplexWhereBuilder Or(params string[] conditions)
        {
            return DoWhere(conditions, KeyWords.Or, true);
        }

        private IComplexWhereBuilder DoWhere(IList<string> conditions,string prefix,bool shouldWrapStatement = false)
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

    public interface IClauseBuilder : IWhereBuilder , IGroupByBuilder 
    {
        
    }

    public interface IWhereBuilder
    {
        IComplexWhereBuilder Where(params string[] conditions);
    }

    public interface IComplexWhereBuilder : IGroupByBuilder, IConditionBuilder<IComplexWhereBuilder>
    {

    }

    public interface IConditionBuilder<out T>
    {
        T And(params string[] conditions);
        T Or(params string[] conditions);
    }

    public interface IGroupByBuilder : IOrderByBuilder
    {
        IGroupByBuilder GroupBy(params string[] conditions);
        new IGroupByBuilder ThenBy(params string[] conditions);
    }

    public interface IOrderByBuilder : IHavingBuilder
    {
        IOrderByBuilder OrderBy(params string[] conditions);
        IOrderByBuilder ThenBy(params string[] conditions);
        IGroupByBuilder Ascending();
        IGroupByBuilder Descending();
    }

    public interface IHavingBuilder : ITSqlBuilder
    {
        IComplexHavingBuilder Having(params string[] conditions);
    }

    public interface IComplexHavingBuilder : ITSqlBuilder,IConditionBuilder<IHavingBuilder>
    {
        
    }

    public interface ITSqlBuilder
    {
        string Build();
    }

    public static class StringExtensions
    {
        public static string GetQuoted(this string @string)
        {
            return string.Format("[{0}]", @string);
        }

        public static string WrapWithParenthesis(this string @string)
        {
            return string.Format("({0})",@string);
        }

        public static string Expand(this string @string,string value=" ")
        {
            return string.Format("{0}{1}{0}", value, @string);
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
    }

}
