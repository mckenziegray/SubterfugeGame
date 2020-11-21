using System.Collections.Generic;
using Subterfuge.Enums;
using Subterfuge.Exceptions;

namespace Subterfuge.Agents
{
    public abstract class Agent
    {
        public string Codename { get; protected set; }
        public Gender Gender { get; protected set; }
        public bool IsAlive { get; protected set; }
        public abstract Allegiance Allegiance { get; }
        /// <summary>
        /// Whether this agent is acting this round
        /// </summary>
        public bool IsActing { get; set; }
        /// <summary>
        /// The agent that this agent is targeting
        /// </summary>
        public Agent Target { get; set; }
        /// <summary>
        /// The codenames of the units that visited this agent today
        /// </summary>
        public HashSet<string> Visitors { get; set; } = new();
        /// <summary>
        /// The agent blocking this agent
        /// </summary>
        public Agent Blocker { get; protected set; }
        /// <summary>
        /// The agent protecting this agent
        /// </summary>
        public Agent Protector { get; protected set; }
        /// <summary>
        /// The agent who killed this agent
        /// </summary>
        public Agent Killer { get; protected set; }
        public bool WasAttacked { get; protected set; }
        public bool WasKilled => WasAttacked && !IsAlive;
        public bool WasFramed { get; protected set; }
        public bool IsBlocked => Blocker?.IsAlive == true;
        public bool IsProtected => Protector != null && Protector.IsAlive && !Protector.IsBlocked;
        public abstract bool RequiresTarget { get; }
        public bool CanAct => IsAlive && !(RequiresTarget && Target is null) && !IsBlocked;
        public virtual string Name => GetType().Name;

        protected bool RedirectKill { get; set; }

        public Agent()
        {
            Codename = GameService.GenerateUniqueCodename();
            Gender = (Gender)GameService.Random.Next(2);
            IsAlive = true;
            Reset();
        }

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

        public void Block(Agent blocker)
        {
            Visit(blocker);

            Blocker = blocker;
        }

        /// <summary>
        /// Protect this unit from death.
        /// </summary>
        /// <param name="protector">The unit providing protection.</param>
        /// <param name="redirect">Whether the protector should die if an attempt is made to kill this unit.</param>
        public void Protect(Agent protector, bool redirect = false)
        {
            Visit(protector);

            Protector = protector;
            RedirectKill = redirect;
        }

        public void Frame(Agent framer)
        {
            Visit(framer);

            WasFramed = true;
        }

        /// <summary>
        /// Visit this unit. This action is performed automatically by most other actions.
        /// </summary>
        /// <param name="visitor">The unit visiting this agent.</param>
        public void Visit(Agent visitor)
        {
            // Visitors is a HashSet, which means duplicates will automatically be filtered out
            Visitors.Add(visitor.Codename);
        }

        public void Execute()
        {
            Kill(null);
        }

        public void ActIfAble()
        {
            if (RequiresTarget && Target is null)
                throw new NoTargetException(GetType());

            if (CanAct && IsActing)
                Act();
        }

        public abstract void SelectTarget(AgentList agents);

        /// <summary>
        /// Causes the unit to perform their action.
        /// </summary>
        protected abstract void Act();

        /// <summary>
        /// Gets the unit's report. Only works for allied units.
        /// </summary>
        /// <returns>A string containing the report.</returns>
        /// <remarks>
        /// Unit            Self-targets    Self-identifies
        /// Assassin        No              No
        /// Convoy          Yes             No
        /// Hacker          No              Yes
        /// Interrogator    No              Yes
        /// Marshal         No              Yes
        /// Medic           No              No
        /// Sentry          Yes             No
        /// Swallow/Raven   No              Yes
        /// </remarks>
        public abstract string GetReport();

        public void Reset()
        {
            IsActing = false;
            WasAttacked = false;
            RedirectKill = false;

            Target = null;
            Blocker = null;
            Protector = null;

            Visitors.Clear();
        }

        protected void Kill(Agent killer)
        {
            IsAlive = false;
            Killer = killer;
        }
    }
}
