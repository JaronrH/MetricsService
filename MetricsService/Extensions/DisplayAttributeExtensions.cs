using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// Extensions for getting name from <see cref="DisplayAttribute"/> and/or <see cref="DisplayNameAttribute"/>
/// </summary>
internal static class DisplayAttributeExtensions
{
    #region Name

    /// <summary>
    /// Get Display Name from a Class/Enumeration by looking first for the <see cref="DisplayAttribute"/> then <see cref="DisplayNameAttribute"/>.
    /// Defaults to the class's/enumeration's name.
    /// </summary>
    /// <param name="obj">Class/Enumeration to get name of (only used if Enum).</param>
    /// <typeparam name="TObject">Class/Enumeration</typeparam>
    /// <returns>Name.</returns>
    /// <exception cref="ArgumentNullException">Obj was null.</exception>
    public static string GetDisplayName<TObject>(this TObject obj)

    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        if (obj is ICustomAttributeProvider c)
            return c.GetDisplayName();
        var type = typeof(TObject);
        return type.IsEnum
            ? type
                .GetMember(obj.ToString()!)
                .First()
                .GetDisplayName()
            : (type as ICustomAttributeProvider).GetDisplayName();
    }

    /// <summary>
    /// Get Display Name from a class Field/Property as specified by the <paramref name="propertyExpression"/> param.
    /// </summary>
    /// <typeparam name="TObject">TObject Type</typeparam>
    /// <param name="obj">Object (not actually used)</param>
    /// <param name="propertyExpression">Expression to Object Property/Field</param>
    /// <returns>Name.</returns>
    public static string GetDisplayName<TObject>(this TObject obj, Expression<Func<TObject, object>> propertyExpression)
    {
        var expression = (MemberExpression)propertyExpression.Body;
        return expression.Member.GetDisplayName();
    }

    /// <summary>
    /// Get Display Name from <see cref="ICustomAttributeProvider"/> by looking first for the <see cref="DisplayAttribute"/> then <see cref="DisplayNameAttribute"/>.
    /// Defaults to the name.
    /// </summary>
    /// <param name="customAttributeProvider"><see cref="ICustomAttributeProvider"/> to try and get name from.</param>
    /// <returns>Name.</returns>
    /// <exception cref="ArgumentNullException"><see cref="ICustomAttributeProvider"/> was null.</exception>
    public static string GetDisplayName(this ICustomAttributeProvider customAttributeProvider)

    {
        if (customAttributeProvider == null) throw new ArgumentNullException(nameof(customAttributeProvider));
        return customAttributeProvider.GetCustomAttributes(typeof(DisplayAttribute), true).Cast<DisplayAttribute>().FirstOrDefault()?.Name ??
               customAttributeProvider.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ??
               (customAttributeProvider as MemberInfo)?.Name ??
               string.Empty;
    }

    #endregion
}
