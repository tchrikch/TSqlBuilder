namespace TSqlBuilder.Converters
{
    interface IOneWayConverter<in TType>
    {
        string Convert(TType data);
    }
}