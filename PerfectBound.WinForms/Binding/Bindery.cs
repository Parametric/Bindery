using System;
using System.Linq.Expressions;

namespace PerfectBound.WinForms.Binding
{
    public static class Bindery
    {
        public static BindingSource<T> Source<T>(T source) where T : class
        {
            return new BindingSource<T>(source);
        }

        internal static string GetAccessorName<T, TR>(Expression<Func<T, TR>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException("Expression is not a member access", "expression");
            var dataMember = member.Member.Name;
            return dataMember;
        }
    }
}
