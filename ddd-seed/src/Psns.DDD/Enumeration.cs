using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Psns.DDD
{
    public abstract class Enumeration : IComparable
    {
        public int Id { get; }

        public string Name { get; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T)
                .GetFields(
                    BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null)).Cast<T>();

        public override bool Equals(object obj) =>
            !(obj is null)
                && obj is Enumeration other
                && GetType().Equals(obj.GetType())
                && Id.Equals(other.Id);

        public int CompareTo(object other) =>
            Id.CompareTo(((Enumeration)other)?.Id ?? default);

        public override int GetHashCode() =>
            Id.GetHashCode() ^ 31
                ^ Name.GetHashCode();

        public override string ToString() => Name;

        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var firstId = firstValue?.Id ?? 0;
            var secondId = secondValue?.Id ?? 0;
            var absoluteDifference = Math.Abs(firstId - secondId);

            return absoluteDifference;
        }

        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }
    }
}
