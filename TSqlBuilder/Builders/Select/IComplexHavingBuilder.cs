using TSqlBuilder.Builders.General;

namespace TSqlBuilder.Builders.Select
{
    public interface IComplexHavingBuilder : IOrderByBuilder,ILogicConditionBuilder<IComplexHavingBuilder>
    {
        
    }
}