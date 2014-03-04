using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Update
{
    public interface ISetBuilder
    {
        IWhereBuilder<IUpdateComplexWhereBuilder> Set(params string[] conditions);
    }
}