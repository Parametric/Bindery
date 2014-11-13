using System;
using System.Linq.Expressions;

namespace PerfectBound.WinForms.Binding
{
    public static class ExpressionExtensions
    {
        public static string GetAccessorName<T, TR>(this Expression<Func<T, TR>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException("Expression is not a member access", "expression");
            var dataMember = member.Member.Name;
            return dataMember;
        }
    }
}
