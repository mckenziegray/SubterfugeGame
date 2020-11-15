using System;
using System.Linq;
using Subterfuge.Agents;
using Subterfuge.Enums;

namespace Subterfuge
{
    public static class Extensions
    {
        public static int Count(this AgentList source, Func<Agent, bool> predicate)
        {
            return Enumerable.Count(source.OrderedList, predicate);
        }

        public static string ToCommonPronoun(this Gender source)
        {
            return source switch
            {
                Gender.Female => "She",
                Gender.Male => "He",
                _ => "They"
            };
        }
    }
}
