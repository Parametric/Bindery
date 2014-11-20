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
            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException("Expression is not a member access", "expression");
            var assignTo = GetObjectToAssignTo(obj, member);
            var value = Expression.Parameter(typeof(TR), "value");
            var property = Expression.Property(assignTo, member.Member.Name);
            var assign = Expression.Assign(property, value);
            return Expression.Lambda<Action<TR>>(assign, value).Compile();
        }

        private static Expression GetObjectToAssignTo<T>(T obj, MemberExpression member)
        {
            if (member.Expression is ParameterExpression)
                return Expression.Constant(obj);

            var root = FindRootParameterExpression<T>(member);
            var subMember = member.Expression;
            var getSubMember = Expression.Lambda(Expression.GetFuncType(typeof(T), subMember.Type), subMember, root);
            var assignTo = Expression.Invoke(getSubMember, Expression.Constant(obj));
            return assignTo;
        }

        private static ParameterExpression FindRootParameterExpression<T>(MemberExpression member)
        {
            var parent = member.Expression;
            while (!(parent is ParameterExpression))
            {
                if (parent is MemberExpression)
                {
                    parent = ((MemberExpression) parent).Expression;
                    continue;
                }
                if (parent is MethodCallExpression)
                {
                    parent = ((MethodCallExpression) parent).Object;
                    continue;
                }
                throw new InvalidOperationException(string.Format("Unexpected expression type '{0}'.",
                    member.Expression.GetType().Name));
            }
            var root = parent as ParameterExpression;
            if (root == null)
                throw new InvalidOperationException("Could not find root parameter expression.");
            return root;
        }
    }
}
