using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Bindery.Expressions
{
    class NotifyPropertyChangedExpressionAnalyzer : ExpressionVisitor
    {
        private object _objIn;
        private Expression _exp;
        private readonly List<NotifyPropertySource> _sources = new List<NotifyPropertySource>();
        private ParameterExpression _param;

        public List<NotifyPropertySource> GetSources<TIn, TOut>(TIn objIn, Expression<Func<TIn, TOut>> exp)
        {
            _objIn = objIn;
            _exp = exp;
            _param = exp.Parameters[0];
            Visit(_exp);
            return _sources;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            var memberAccess = (MemberExpression)base.VisitMemberAccess(m);
            var objExp = memberAccess.Expression;
            if (!typeof (INotifyPropertyChanged).IsAssignableFrom(objExp.Type)) 
                return memberAccess;

            var lambdaType = Expression.GetFuncType(_objIn.GetType(), objExp.Type);
            var del = Expression.Lambda(lambdaType, objExp, _param).Compile();
            var obj = del.DynamicInvoke(_objIn) as INotifyPropertyChanged;
            var source = _sources.SingleOrDefault(x => x.Object == obj);
            if (source == null)
                _sources.Add(source = new NotifyPropertySource(obj));
            source.PropertyNames.Add(memberAccess.Member.Name);
            return memberAccess;
        }
    }
}
