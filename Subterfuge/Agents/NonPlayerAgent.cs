namespace Subterfuge.Agents
{
    public abstract class NonPlayerAgent : Agent
    {
        /// <summary>
        /// Selects an agent and sets them as this agent's <see cref="Agent.Target"/>.
        /// </summary>
        /// <param name="agents">The list of agents in the current game from which the target will be selected.</param>
        public abstract void SelectTarget(AgentList agents);
    }
}
