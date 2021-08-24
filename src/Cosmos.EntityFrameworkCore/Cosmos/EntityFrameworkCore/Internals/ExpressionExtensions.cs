using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cosmos.EntityFrameworkCore.Internals
{
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// GetMemberName
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <typeparam name="TMember">TMember</typeparam>
        /// <param name="memberExpression">get member expression</param>
        /// <returns></returns>
        public static string
            GetMemberName<TEntity, TMember>(this Expression<Func<TEntity, TMember>> memberExpression) =>
            GetMemberInfo(memberExpression).Name;

        /// <summary>
        /// GetMemberInfo
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <typeparam name="TMember">TMember</typeparam>
        /// <param name="expression">get member expression</param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo<TEntity, TMember>(this Expression<Func<TEntity, TMember>> expression)
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException($"{nameof(expression)} must be lambda expression.");
            }

            var lambda = (LambdaExpression)expression;

            var memberExpression = ExtractMemberExpression(lambda.Body);
            if (memberExpression == null)
            {
                throw new ArgumentException($"{nameof(expression)} must be lambda expression.");
            }

            return memberExpression.Member;
        }

        private static MemberExpression ExtractMemberExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return (MemberExpression)expression;
            }

            if (expression.NodeType == ExpressionType.Convert)
            {
                var operand = ((UnaryExpression)expression).Operand;
                return ExtractMemberExpression(operand);
            }

            throw new InvalidOperationException(nameof(ExtractMemberExpression));
        }
    }
}