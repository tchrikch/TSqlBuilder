using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Select
{
    public interface IJoinBuilder : IClauseBuilder
    {
        INonAliasedJoinBuilder Join(JoinMode mode, string table);
        INonAliasedJoinBuilder Join(JoinMode mode, string schema, string table);
        INonAliasedJoinBuilder Join(JoinMode mode, string catalog, string schema, string table);
    }
}