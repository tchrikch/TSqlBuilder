using TSqlBuilder.Builders.Delete;
using TSqlBuilder.Builders.Select;
using TSqlBuilder.Builders.Update;

namespace TSqlBuilder.Builders
{
    public class CommandBuilder
    {
        public static ISelectBuilder Select { get{return new SelectBuilder();}}

        public static IDeleteBuilder Delete { get { return new DeleteBuilder(); } }

        public static IUpdateBuilder Update {get{return new UpdateBuilder();}}
    }
}
