using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DddSeed;

public abstract class BaseEnumeration : IComparable
{
    public string DisplayName { get; private set; }

    public string Name { get; private set; }

    public int Id { get; private set; }

    protected BaseEnumeration(int id, string name, string? displayName = default)
    {
        Id = id;
        Name = name;
        DisplayName = displayName ?? Name;
    }

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>() where T : BaseEnumeration
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        return fields.Select(f => f.GetValue(null)).Cast<T>();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseEnumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj?.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(BaseEnumeration firstValue, BaseEnumeration secondValue)
    {
        var first = firstValue?.Id ?? 0;
        var second = secondValue?.Id ?? 0;

        var absoluteDifference = Math.Abs(first - second);
        return absoluteDifference;
    }

    public static T FromValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(int value) where T : BaseEnumeration =>
        TryParse<T>(item => item.Id == value, out var result)
            ? result
            : throw new InvalidOperationException($"'{value}' is not a valid value in {typeof(T)}");

    public static T FromDisplayName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(string displayName) where T : BaseEnumeration =>
        TryParse<T>(item => item.DisplayName == displayName, out var result)
            ? result
            : throw new InvalidOperationException($"'{displayName}' is not a valid display name in {typeof(T)}");

    public static T FromName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(string name) where T : BaseEnumeration =>
        TryParse<T>(item => item.Name == name, out var result)
            ? result
            : throw new InvalidOperationException($"'{name}' is not a valid display name in {typeof(T)}");

    public static bool TryFromName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(string name, [MaybeNullWhen(false)] out T result) where T : BaseEnumeration
    {
        if (TryParse<T>(item => item.Name == name, out var t))
        {
            result = t;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    public static bool TryFromValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(int value, [MaybeNullWhen(false)] out T result) where T : BaseEnumeration
    {
        if (TryParse<T>(item => item.Id == value, out var t))
        {
            result = t;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    private static bool TryParse<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(Func<T, bool> predicate, [MaybeNullWhen(false)] out T result) where T : BaseEnumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);
        result = matchingItem;

        if (matchingItem is null)
        {
            return false;
        }
        else
        {
            result = matchingItem;
            return true;
        }
    }

    public int CompareTo(object? obj) => obj is null ? 1 : Id.CompareTo(((BaseEnumeration)obj).Id);

    public static bool operator ==(BaseEnumeration? left, BaseEnumeration? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(BaseEnumeration? left, BaseEnumeration? right) =>
        !(left == right);

    public static bool operator <(BaseEnumeration? left, BaseEnumeration? right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(BaseEnumeration? left, BaseEnumeration? right) =>
        left is null || left.CompareTo(right) <= 0;

    public static bool operator >(BaseEnumeration? left, BaseEnumeration? right) =>
        left?.CompareTo(right) > 0;

    public static bool operator >=(BaseEnumeration? left, BaseEnumeration? right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;
}
