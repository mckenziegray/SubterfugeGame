using System.Collections.Generic;
using Subterfuge.Enums;
using Subterfuge.Exceptions;

namespace Subterfuge.Agents
{
    /// <summary>
    /// Base class for all agents.
    /// </summary>
    public abstract class Agent
    {
        /// <summary>
        /// The agent's role name.
        /// </summary>
        public virtual string Name => GetType().Name;
        /// <summary>
        /// The agent's codename.
        /// </summary>
        public string Codename { get; init; }
        /// <summary>
        /// The agent's gender.
        /// </summary>
        public Gender Gender { get; init; }
        /// <summary>
        /// This agent's allegiance.
        /// </summary>
        public abstract Allegiance Allegiance { get; }
        /// <summary>
        /// Whether this agent must have a target in order to act.
        /// </summary>
        public abstract bool RequiresTarget { get; }
        /// <summary>
        /// Whether this agent is still in the game. This will be set to false if an agent dies or deserts.
        /// </summary>
        public bool IsActive { get; protected set; }
        /// <summary>
        /// Whether this agent is acting this round.
        /// </summary>
        public bool IsActing { get; set; }
        /// <summary>
        /// The agent that this agent is targeting.
        /// </summary>
        public Agent Target { get; set; }
        /// <summary>
        /// The codenames of the units that visited this agent today.
        /// </summary>
        public HashSet<string> Visitors { get; set; } = new();
        /// <summary>
        /// The agent blocking this agent.
        /// </summary>
        public Agent Blocker { get; protected set; }
        /// <summary>
        /// The agent protecting this agent.
        /// </summary>
        public Agent Protector { get; protected set; }
        /// <summary>
        /// The agent who killed this agent.
        /// </summary>
        public Agent Killer { get; protected set; }
        /// <summary>
        /// Whether this agent was framed this round.
        /// </summary>
        public bool WasFramed { get; protected set; }
        /// <summary>
        /// Whether this agent was attacked this round.
        /// </summary>
        public bool WasAttacked { get; protected set; }
        /// <summary>
        /// Whether this agent died this round (includes execution).
        /// </summary>
        public bool WasKilled { get; protected set; }
        /// <summary>
        /// Whether this agent was executed this round.
        /// </summary>
        public bool WasExecuted => WasKilled && Killer is null;
        /// <summary>
        /// Whether this agent is role-blocked (prevented from acting).
        /// </summary>
        public bool IsBlocked => Blocker?.IsActive == true;
        /// <summary>
        /// Whether this agent is being protected from death.
        /// </summary>
        public bool IsProtected => Protector != null && Protector.IsActive && !Protector.IsBlocked;
        /// <summary>
        /// Whether this agent is able to act.
        /// </summary>
        public bool CanAct => IsActive && !(RequiresTarget && Target is null) && !IsBlocked;

        /// <summary>
        /// If true, the <see cref="Protector"/> will be killed instead of this agent.
        /// </summary>
        protected bool RedirectKill { get; set; }

        /// <summary>
        /// Initializes non-abstract members.
        /// </summary>
        public Agent()
        {
            Codename = GameService.GenerateUniqueCodename();
            Gender = (Gender)GameService.Random.Next(2);
            IsActive = true;
            Reset();
        }

        /// <summary>
        /// Attack this agent, killing them if they are not under protection. The attacker will be marked as having visited this agent.
        /// </summary>
        /// <param name="attacker">The agent attacking this agent.</param>
        public void Attack(Agent attacker)
        {
            Visit(attacker);
            WasAttacked = true;

            if (IsProtected)
            {
                if (RedirectKill)
                {
                    Protector.Kill(attacker);
                    RedirectKill = false;
                }
            }
            else
            {
                Kill(attacker);
            }
        }

        /// <summary>
        /// Role-block this agent, preventing them from attacking. The blocker will be marked as having visited this agent.
        /// </summary>
        /// <param name="blocker">The agent blocking this agent.</param>
        public void Block(Agent blocker)
        {
            Visit(blocker);

            Blocker = blocker;
        }

        /// <summary>
        /// Protect this agent from death. The protector will be marked as having visited this agent.
        /// </summary>
        /// <param name="protector">The agent providing protection.</param>
        /// <param name="redirect">Whether the protector should die if an attempt is made to kill this agent.</param>
        public void Protect(Agent protector, bool redirect = false)
        {
            Visit(protector);

            Protector = protector;
            RedirectKill = redirect;
        }

        /// <summary>
        /// Frame this agent, causing them to be identified as an enemy by the <see cref="Hacker"/>. The framer will be marked as having visited this agent.
        /// </summary>
        /// <param name="framer">The agent framing this agent.</param>
        public void Frame(Agent framer)
        {
            Visit(framer);

            WasFramed = true;
        }

        /// <summary>
        /// Visit this agent. This action is performed automatically by most other actions.
        /// </summary>
        /// <param name="visitor">The agent visiting this agent.</param>
        public void Visit(Agent visitor)
        {
            // NOTE: Visitors is a HashSet, which means duplicates will automatically be filtered out
            Visitors.Add(visitor.Codename);
        }

        /// <summary>
        /// Executes this agent, killing them immediately.
        /// </summary>
        public void Execute()
        {
            Kill(null);
        }

        /// <summary>
        /// Perform this agent's action if they are flagged to act and able to perform their action.
        /// </summary>
        /// <exception cref="NoTargetException">Thrown if this agent requires a target to act but has no target.</exception>
        public void ActIfAble()
        {
            if (IsActing)
            {
                if (RequiresTarget && Target is null)
                    throw new NoTargetException(GetType());

                if (CanAct)
                    Act();
            }
        }

        /// <summary>
        /// Reset this agent for a new round.
        /// </summary>
        public virtual void Reset()
        {
            IsActing = false;
            WasFramed = false;
            WasAttacked = false;
            WasKilled = false;
            RedirectKill = false;

            Target = null;
            Blocker = null;
            Protector = null;

            Visitors.Clear();
        }

        /// <summary>
        /// Perform this agent's action.
        /// </summary>
        protected abstract void Act();

        /// <summary>
        /// Kill this agent.
        /// </summary>
        /// <param name="killer">The agent killing this agent.</param>
        protected void Kill(Agent killer)
        {
            IsActive = false;
            Killer = killer;
            WasKilled = true;
        }
    }
}
