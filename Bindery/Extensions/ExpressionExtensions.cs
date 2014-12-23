using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bindery.Expressions;

namespace Bindery.Extensions
{
    internal static class ExpressionExtensions
    {
        public static string GetAccessorName<T, TR>(this Expression<Func<T, TR>> expression)
        {
            var member = GetMemberExpression(expression);
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

        public static Type GetAccessorType<T, TR>(this Expression<Func<T, TR>> expression)
        {
            var member = GetMemberExpression(expression);
            return member.Type;
        }

        private static MemberExpression GetMemberExpression<T, TR>(Expression<Func<T, TR>> expression)
        {
            MemberExpression member = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var unary = expression.Body as UnaryExpression;
                if (unary != null)
                    member = unary.Operand as MemberExpression;
            }
            else
            {
                member = expression.Body as MemberExpression;
            }

            if (member == null)
                throw new ArgumentException(string.Format("Expression '{0}' is not a member access", expression.Body), "expression");
            return member;
        }

        public static Action<TR> GetPropertySetter<T, TR>(this Expression<Func<T, TR>> expression, T obj)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format("Expression '{0}' is not a member access", expression.Body), "expression");
            var lambdaParameter = expression.Parameters.Single();
            var assignTo = GetObjectToAssignTo(obj, member, lambdaParameter);
            var value = Expression.Parameter(typeof (TR), "value");
            var property = Expression.Property(assignTo, member.Member.Name);
            var assign = Expression.Assign(property, value);
            return Expression.Lambda<Action<TR>>(assign, value).Compile();
        }

        private static Expression GetObjectToAssignTo<T>(T obj, MemberExpression member, ParameterExpression lambdaParameter)
        {
            if (member.Expression is ParameterExpression)
                return Expression.Constant(obj);

            var subMember = member.Expression;
            var getSubMember = Expression.Lambda(Expression.GetFuncType(typeof (T), subMember.Type), subMember, lambdaParameter);
            var assignTo = Expression.Invoke(getSubMember, Expression.Constant(obj));
            return assignTo;
        }

        public static List<NotifyPropertySource> GetNotifyPropertyChangedSources<TIn, TOut>(this TIn objIn, Expression<Func<TIn, TOut>> exp)
        {
            return new NotifyPropertyChangedExpressionAnalyzer().GetSources(objIn, exp);
        }
    }
}