using System;
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
        public bool IsBlocked => Blocker?.IsAlive ?? false;
        public bool IsProtected => Protector != null && Protector.IsAlive && !Protector.IsBlocked;
        public abstract bool RequiresTarget { get; }
        public bool CanAct
        {
            get
            {
                if (RequiresTarget && Target is null)
                    throw new NoTargetException(GetType());

                return !IsBlocked;
            }
        }
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

        protected void Kill(Agent killer)
        {
            IsAlive = false;
            Killer = killer;
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

        /// <summary>
        /// Visit this unit. This action is performed automatically by most other actions.
        /// </summary>
        /// <param name="visitor">The unit visiting this agent.</param>
        public void Visit(Agent visitor)
        {
            // Visitors is a HashSet, which means duplicates will automatically be filtered out
            Visitors.Add(visitor.Codename);
        }

        public void ActIfAble()
        {
            if (IsAlive && IsActing && CanAct)
            {
                Act();
            }
        }

        /// <summary>
        /// Causes the unit to perform their action.
        /// </summary>
        /// <remarks>
        /// What each type of agent does:
        ///      Allied units
        ///          Assassin:       Kills one target.
        ///          Convoy:         Protects one target from death. Sacrifices self if the target would have died.
        ///          Hacker:         Attempts to identify one target. Always able to identify the Android, but cannot identify the Mastermind.
        ///          Interrogator:   Attempts to identify one target.
        ///          Marshall:       Role-blocks one target.
        ///          Medic:          Protects one target from death unless killed first.
        ///          Sentry:         Surveils one target and reports who, if anyone, they visited.
        ///          Swallow:        Role-blocks one target.
        ///      Neutral units
        ///          Cut-out:        Harmlessly visits one agent per day.
        ///          Intern:         Does nothing. Kills one agent at random if executed.
        ///      Enemy units
        ///          Android:        Kills randomly (but will not kill the Mastermind). Automatically tries to kill the Swallow or Marshal if targeted by them.
        ///          Drudge:         Kills one Allied target every day.
        ///          Fabricator:     Plants false information on one Allied or Neutral target every day. This is more likely to fool the hacker than the interrogator. Will not target the target of the Mastermind or Drudge.
        ///          Mastermind:     Does nothing if the Drudge is alive. Kills one Allied target every day if the Drudge is dead.
        ///          Saboteur:       Role-blocks one target every day.
        /// </remarks>
        protected abstract void Act();

        /// <summary>
        /// Gets the unit's report. Only works for allied units.
        /// </summary>
        /// <returns>A string containing the report.</returns>
        /// <remarks>
        /// Unit         Self-targets    Self-identifies
        /// Marshal      No              Yes
        /// Swallow      No              Yes
        /// Convoy       Yes             No
        /// Medic        No              No
        /// Assassin     No              No
        /// Interrogator No              Yes
        /// Hacker       No              Yes
        /// Sentry       Yes             No
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
    }
}
