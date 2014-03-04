namespace TSqlBuilder.Builders.General
{
    public interface ILogicConditionBuilder<out T>
    {
        T And(params string[] conditions);
        T Or(params string[] conditions);
    }
}