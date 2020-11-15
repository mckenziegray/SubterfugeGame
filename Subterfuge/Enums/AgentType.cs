/*
 * What each type of agent does:
 *      Allied units
 *          Assassin:       Kills one target.
 *          Convoy:         Protects one target from death. Sacrifices self if the target would have died.
 *          Hacker:         Attempts to identify one target. Always able to identify the Android, but cannot identify the Mastermind.
 *          Interrogator:   Attempts to identify one target.
 *          Marshall:       Role-blocks one target.
 *          Medic:          Protects one target from death unless killed first.
 *          Sentry:         Surveils one target and reports who, if anyone, they visited.
 *          Swallow:        Role-blocks one target.
 *      Neutral units
 *          Cut-out:        Harmlessly visits one agent per day.
 *          Intern:         Does nothing. Kills one agent at random if executed.
 *      Enemy units
 *          Android:        Kills randomly (but will not kill the Mastermind). Automatically tries to kill the Swallow or Marshal if targeted by them.
 *          Drudge:         Kills one Allied target every day.
 *          Fabricator:     Plants false information on one Allied or Neutral target every day. This is more likely to fool the hacker than the interrogator. Will not target the target of the Mastermind or Drudge.
 *          Mastermind:     Does nothing if the Drudge is alive. Kills one Allied target every day if the Drudge is dead.
 *          Saboteur:       Role-blocks one target every day.
 */

namespace Subterfuge.Enums
{
    // NOTE: The order of this enum is the order in which the agents will act
    public enum AgentType
    {
        Marshal,
        Swallow,
        Saboteur,
        Convoy,
        Medic,
        Android,
        Assassin,
        Drudge,
        Mastermind,
        Fabricator,
        Cutout,
        Interrogator,
        Hacker,
        Sentry,
        Intern
    }
}
