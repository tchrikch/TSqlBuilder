using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Script
{
    public class Script : IScript
    {
        private readonly IList<string> _statements = new List<string>(); 

        public IScript RegisterSql(string sql)
        {
            _statements.Add(sql);
            return this;
        }

        public IScript RegisterBuilder(ITSqlBuilder builder)
        {
            _statements.Add(builder.Build());
            return this;
        }

        public string Build()
        {
            return string.Join("\n", _statements);
        }
    }

    public interface IScript : ITSqlBuilder
    {
        IScript RegisterSql(string sql);
        IScript RegisterBuilder(ITSqlBuilder builder);

        
    }




}
