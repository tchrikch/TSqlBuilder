namespace TSqlBuilder.Builders.Select
{
    public interface IHavingBuilder : IOrderByBuilder
    {
        IComplexHavingBuilder Having(params string[] conditions);
    }
}