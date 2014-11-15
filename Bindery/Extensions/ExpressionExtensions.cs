using System;
using System.Linq.Expressions;

namespace Bindery.Extensions
{
    internal static class ExpressionExtensions
    {
        public static string GetAccessorName<T, TR>(this Expression<Func<T, TR>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException("Expression is not a member access", "expression");
            var dataMember = member.Member.Name;
            return dataMember;
        }

        public static Action<TR> GetPropertySetter<T, TR>(this Expression<Func<T, TR>> expression, T obj)
        {
            var memberName = expression.GetAccessorName();
            var value = Expression.Parameter(typeof (TR), "value");
            var property  = Expression.Property(Expression.Constant(obj), memberName);
            var assign = Expression.Assign(property, value);
            return Expression.Lambda<Action<TR>>(assign, value).Compile();
        }
    }
}
