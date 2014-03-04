using System.Collections.Generic;
using TSqlBuilder.Builders.Select;

namespace TSqlBuilder.Converters
{
    class JoinModeConverter : IOneWayConverter<JoinMode>
    {
        private readonly IDictionary<JoinMode, string> _values = new Dictionary<JoinMode, string>
            {
                {JoinMode.RightOuter, "RIGHT OUTER JOIN"},
                {JoinMode.Inner, "INNER JOIN"},
                {JoinMode.FullOuter, "FULL OUTER JOIN"},
                {JoinMode.LeftOuter, "LEFT OUTER JOIN"}
            };

        public string Convert(JoinMode data)
        {
            return _values[data];
        }
    }
}