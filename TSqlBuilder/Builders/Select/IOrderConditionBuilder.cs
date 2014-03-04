namespace TSqlBuilder.Builders.Select
{
    public interface IOrderConditionBuilder<out T>
    {
        T ThenBy(params string[] conditions);
    }
}