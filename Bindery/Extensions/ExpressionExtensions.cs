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
            var retVal = member.Member.Name;
            while (true)
            {
                member = member.Expression as MemberExpression;
                if (member == null) break;
                var dataMember = member.Member.Name;
                retVal = dataMember + "." + retVal;
            }
            return retVal;
        }

        public static Action<TR> GetPropertySetter<T, TR>(this Expression<Func<T, TR>> expression, T obj)
        {
            var memberName = expression.GetAccessorName().Split('.');
            var value = Expression.Parameter(typeof (TR), "value");
            var property  = Expression.Property(Expression.Constant(obj), memberName[0]);
            for (var i = 1; i < memberName.Length; i++)
                property = Expression.Property(property, memberName[i]);
            var assign = Expression.Assign(property, value);
            return Expression.Lambda<Action<TR>>(assign, value).Compile();
        }
    }
}
