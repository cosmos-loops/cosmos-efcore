using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

/*
 * reference to:
 *     PomeloFoundation/Pomelo.EntityFrameworkCore.Extensions.ToSql
 *     Author: Yuko & PomeloFoundation
 *     URL: https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.Extensions.ToSql
 *     MIT
 */

namespace Pomelo.EntityFrameworkCore.ToSql.Internals
{
    internal static class QueryModelVisitorUtils
    {
        internal static IEnumerable<string> VisitExpression(Expression expression, dynamic caller)
        {
            var ret = new List<string>();
            dynamic exp = expression;

            if (expression.NodeType == ExpressionType.Lambda)
            {
                var resultBuilder = new StringBuilder();
                if (caller != null)
                {
                    resultBuilder.Append(caller.Method.Name.Replace("_", "."));
                    resultBuilder.Append("(");
                }

                resultBuilder.Append(exp.ToString());

                if (caller != null)
                {
                    resultBuilder.Append(")");
                }

                ret.Add(resultBuilder.ToString());
            }

            try
            {
                if (exp.Arguments.Count > 0)
                {
                    foreach (var x in exp.Arguments)
                    {
                        ret.AddRange(VisitExpression(x, exp));
                    }
                }
            }
            catch
            {
                // ignored
            }

            return ret;
        }
    }
}