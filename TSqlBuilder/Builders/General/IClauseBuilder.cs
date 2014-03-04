using TSqlBuilder.Builders.Select;

namespace TSqlBuilder.Builders.General
{
    public interface IClauseBuilder : IWhereBuilder<ISelectComplexWhereBuilder> , IGroupByBuilder, IHavingBuilder
    {
        
    }
}