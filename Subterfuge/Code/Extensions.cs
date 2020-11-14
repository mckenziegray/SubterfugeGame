using Subterfuge.Enums;

namespace Subterfuge.Code
{
    public static class Extensions
    {
        public static string GetName (this AgentType source)
        {
            return source switch
            {
                AgentType.HoneyTrapper => "Honey Trapper",
                _ => source.ToString()
            };
        }
    }
}
