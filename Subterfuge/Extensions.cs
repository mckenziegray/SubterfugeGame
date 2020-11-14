using System;
using System.Linq;

namespace Subterfuge
{
    public static class Extensions
    {
        public static int Count(this AgentList source, Func<Agent, bool> predicate)
        {
            return Enumerable.Count(source.OrderedList, predicate);
        }
    }
}
