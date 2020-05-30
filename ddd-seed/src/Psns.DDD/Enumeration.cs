using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Psns.DDD
{
    [SuppressMessage("Microsoft.Design", "CA1036:Override methods on comparable types")]
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
    }
}
