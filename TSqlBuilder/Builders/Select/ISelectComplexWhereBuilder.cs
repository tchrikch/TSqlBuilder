using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Select
{
    public interface ISelectComplexWhereBuilder : IGroupByBuilder, IOrderByBuilder, ILogicConditionBuilder<ISelectComplexWhereBuilder>
    {
        
    }
}