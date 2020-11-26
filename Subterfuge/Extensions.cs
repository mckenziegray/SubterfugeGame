using System;
using System.Collections.Generic;
using System.Linq;
using Subterfuge.Enums;

namespace Subterfuge
{
    public static class Extensions
    {
        /// <summary>
        /// Randomizes the order of the items in the list.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list</typeparam>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="random">A <see cref="Random"/> object to be used for randomizing the order. A new object will be created if none is provided.</param>
        public static void Shuffle<T>(this IList<T> list, Random random = null)
        {
            random ??= new Random();

            for (int i = list.Count; i > 0; i--)
            {
                int r = random.Next(0, i);
                T temp = list[0];
                list[0] = list[r];
                list[r] = temp;
            }
        }

        /// <summary>
        /// Creates an instance of a given type.
        /// </summary>
        /// <typeparam name="T">The type of object to instantiate.</typeparam>
        /// <param name="source">A <see cref="Type"/> object representing the type to instantiate.</param>
        /// <param name="parameters">The parameters to pass to the constructor. Pass no parameters to call an empty constructor.</param>
        /// <returns>An object instance of the given type.</returns>
        public static T Instantiate<T>(this Type source, params object[] parameters)
        {
            try
            {
                return (T)source.GetConstructor(parameters.Select(p => p.GetType()).ToArray()).Invoke(parameters);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns the common pronoun (capitalized) corresponding to the given gender.
        /// </summary>
        /// <param name="source">The gender.</param>
        /// <returns>She, He, or They</returns>
        public static string ToCommonPronoun(this Gender source)
        {
            return source switch
            {
                Gender.Female => "She",
                Gender.Male => "He",
                _ => "They"
            };
        }

        /// <summary>
        /// Returns the objective pronoun (capitalized) corresponding to the given gender.
        /// </summary>
        /// <param name="source">The gender.</param>
        /// <returns>Her, Him, or Them</returns>
        public static string ToObjectivePronoun(this Gender source)
        {
            return source switch
            {
                Gender.Female => "Her",
                Gender.Male => "Him",
                _ => "Them"
            };
        }

        /// <summary>
        /// Returns the possessive pronoun (capitalized) corresponding to the given gender.
        /// </summary>
        /// <param name="source">The gender.</param>
        /// <returns>Her, His, or Their</returns>
        public static string ToPossessivePronoun(this Gender source)
        {
            return source switch
            {
                Gender.Female => "Her",
                Gender.Male => "His",
                _ => "Their"
            };
        }

        /// <summary>
        /// Returns the possessive plural pronoun (capitalized) corresponding to the given gender.
        /// </summary>
        /// <param name="source">The gender.</param>
        /// <returns>Hers, His, or Theirs</returns>
        public static string ToPossessivePluralPronoun(this Gender source)
        {
            return source switch
            {
                Gender.Female => "Hers",
                Gender.Male => "His",
                _ => "Theirs"
            };
        }
    }
}
