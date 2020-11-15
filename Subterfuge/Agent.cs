using System;
using System.Collections.Generic;
using Subterfuge.Enums;
using Subterfuge.Exceptions;

namespace Subterfuge
{
    public class Agent
    {
        public string Codename { get; protected set; }
        public AgentType AgentType { get; protected set; }
        public Gender Gender { get; protected set; }
        public bool IsAlive { get; protected set; }
        public Allegiance Allegiance {
            get
            {
                return AgentType switch
                {
                    AgentType.Assassin      => Allegiance.Ally,
                    AgentType.Convoy        => Allegiance.Ally,
                    AgentType.Hacker        => Allegiance.Ally,
                    AgentType.Interrogator  => Allegiance.Ally,
                    AgentType.Marshal       => Allegiance.Ally,
                    AgentType.Medic         => Allegiance.Ally,
                    AgentType.Swallow       => Allegiance.Ally,
                    AgentType.Sentry        => Allegiance.Ally,

                    AgentType.Cutout        => Allegiance.Neutral,
                    AgentType.Intern        => Allegiance.Neutral,

                    AgentType.Android       => Allegiance.Enemy,
                    AgentType.Drudge        => Allegiance.Enemy,
                    AgentType.Fabricator    => Allegiance.Enemy,
                    AgentType.Mastermind    => Allegiance.Enemy,
                    AgentType.Saboteur      => Allegiance.Enemy,

                    _ => throw new NotImplementedException()
                };
            }
        }
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
        public HashSet<string> Visitors { get; set; }
        /// <summary>
        /// The agent blocking this agent
        /// </summary>
        protected Agent Blocker { get; set; }
        /// <summary>
        /// The agent protecting this agent
        /// </summary>
        protected Agent Protector { get; set; }
        /// <summary>
        /// The agent who killed this agent
        /// </summary>
        protected Agent Killer { get; set; }
        public bool WasAttacked { get; protected set; }
        public bool IsBlocked => Blocker?.IsAlive ?? false;
        public bool IsProtected => Protector != null && Protector.IsAlive && !Protector.IsBlocked;
        public bool RequiresTarget => AgentType switch
        {
            AgentType.Marshal       => true,
            AgentType.Swallow       => true,
            AgentType.Saboteur      => true,
            AgentType.Convoy        => true,
            AgentType.Medic         => true,
            AgentType.Android       => true,
            AgentType.Assassin      => true,
            AgentType.Drudge        => true,
            AgentType.Mastermind    => true,
            AgentType.Fabricator    => true,
            AgentType.Cutout        => true,
            AgentType.Interrogator  => true,
            AgentType.Hacker        => true,
            AgentType.Sentry        => true,
            AgentType.Intern        => true,
            _ => false
        };
        public string Name => AgentType switch
        {
            AgentType.Swallow => Gender switch
            {
                Gender.Male => "Raven",
                Gender.Female => "Swallow",
                _ => "Swallow"
            },
            _ => AgentType.ToString()
        };

        protected bool RedirectKill { get; set; }

        public Agent(AgentType type)
        {
            AgentType = type;
            Codename = GameService.GenerateUniqueCodename();
            Gender = (Gender)GameService.Random.Next(2);
            IsAlive = true;
            Reset();
        }

        public void Kill(Agent killer)
        {
            Visit(killer);

            if (RedirectKill && IsProtected)
            {
                Protector.Kill(killer);
                RedirectKill = false;
            }
            else
            {
                IsAlive = false;
                Killer = killer;
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

        /// <summary>
        /// Visit this unit. This action is performed automatically by most other actions.
        /// </summary>
        /// <param name="visitor">The unit visiting this agent.</param>
        public void Visit(Agent visitor)
        {
            // Visitors is a HashSet, which means duplicates will automatically be filtered out
            Visitors.Add(visitor.Codename);
        }

        public void Act()
        {
            if (RequiresTarget && Target is null)
                throw new NoTargetException(AgentType);

            // If this unit is role-blocked, they are prevented from acting
            if (IsBlocked)
                return;

            // If this unit requires a target and their target is dead, there's nothing to do
            if (RequiresTarget && !Target.IsAlive)
                return;

            // Act according to role
            switch (AgentType)
            {
                case AgentType.Marshal:
                case AgentType.Swallow:
                    if (Target.AgentType == AgentType.Android)
                    {
                        // The Android is immune to being role-blocked and will instead try to kill the unit targeting it
                        Target.Target = this;
                        Target.IsActing = true;
                    }
                    else
                    {
                        Target.Block(this);
                        Target.Protect(this);
                    }
                    break;
                case AgentType.Saboteur:
                    Target.Block(this);
                    break;
                case AgentType.Convoy:
                    Target.Protect(this, true);
                    break;
                case AgentType.Medic:
                    Target.Protect(this, false);
                    break;
                case AgentType.Android:
                case AgentType.Assassin:
                case AgentType.Drudge:
                case AgentType.Mastermind:
                    if (!Target.IsProtected)
                        Target.Kill(this);
                    break;
                case AgentType.Fabricator:
                    // TODO
                    break;
                case AgentType.Cutout:
                    Target.Visit(this);
                    break;
                case AgentType.Interrogator:
                    // TODO
                    break;
                case AgentType.Hacker:
                    // TODO
                    break;
                case AgentType.Sentry:
                    // Technically nothing to do here
                    break;
                case AgentType.Intern:
                    // Does nothing
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public string GetReport()
        {
            /*
             * Unit         Self-targets            Self-identifies
             * Marshal      No                      Yes
             * Swallow      No                      Yes
             * Convoy       Yes                     No
             * Medic        No                      No
             * Assassin     No                      No
             * Interrogator No                      Yes
             * Hacker       No                      Yes
             * Sentry       Yes                     No
             */

            string report;

            switch (AgentType)
            {
                case AgentType.Marshal:

                    break;
                case AgentType.Swallow:
                    report = "I received your orders to carry out the \"special\" operation.";
                    if (!Target.IsAlive || IsBlocked)
                        report += " I'm sorry to say that I was unsuccessful.";
                    else
                        report += $" The mission was a success. {Target.Codename} was tied up all night.";
                    break;
                case AgentType.Saboteur:
                    break;
                case AgentType.Convoy:
                    break;
                case AgentType.Medic:
                    report = $"I got your orders to look after {Target.Codename}.";
                    if (!Target.IsAlive || IsBlocked)
                    {
                        report += " Unfortunately, I was unable to perform my duties. Apologies.";
                    }
                    else if (Target.WasAttacked)
                    {
                        report += " The target was attacked but is alive and recovering.";
                    }
                    else
                    {
                        report += " Thankfully, my services were not required.";
                    }
                    break;
                case AgentType.Android:
                    break;
                case AgentType.Assassin:
                    if (Target.Killer == this)
                        report = $"Orders received. Target {Target.Codename} neutralized.";
                    else
                        report = $"Mission compromised. Target {Target.Codename} still active. Standing by for further orders.";
                    break;
                case AgentType.Drudge:
                    break;
                case AgentType.Mastermind:
                    break;
                case AgentType.Fabricator:
                    break;
                case AgentType.Cutout:
                    break;
                case AgentType.Interrogator:
                    if (!Target.IsAlive || IsBlocked)
                    {
                        report = "Due to unforeseen circumstances, I was unable to question the target. Don't worry, we'll get 'em next time.";
                    }
                    else
                    {
                        report = $"Per your orders, {Target.Codename} was detained and interrogated.";
                        // TODO: Get information
                    }
                    break;
                case AgentType.Hacker:
                    report = "I got your dead drop, boss.";
                    if (!Target.IsAlive || IsBlocked)
                    {
                        report += " Unfortunately, I hit a bit of a snag. Sorry. Maybe next time.";
                    }
                    else
                    {
                        report += $" I did some digging into {Target.Codename}.";
                        // TODO: Get information
                    }
                    break;
                case AgentType.Sentry:
                    break;
                case AgentType.Intern:
                    break;
                default:
                    throw new NotImplementedException();
            }

            return report;
        }

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
