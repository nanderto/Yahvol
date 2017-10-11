namespace Yahvol.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Extension methods to combine LINQ Predicates
    /// </summary>
    public static class LinqComposer
    {
        /// <summary>
        /// Joins the specified first expression, with the second expression using the And operator.
        /// </summary>
        /// <typeparam name="T">The Type that the Expression refers too</typeparam>
        /// <param name="firstExpression">The first expression.</param>
        /// <param name="secondExpression">The second expression.</param>
        /// <returns>Expression that has been joined with the Expression.And operator</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> firstExpression, Expression<Func<T, bool>> secondExpression)
        {
            return firstExpression.Compose(secondExpression, Expression.And);
        }

        /// <summary>
        /// Joins all of the expressions provided in the list using the And operator and returns a single expression .
        /// </summary>
        /// <typeparam name="T">The Type that the Expression refers too</typeparam>
        /// <param name="expressions">The list of expressions.</param>
        /// <returns>Expression that has been joined with the Expression.And operator for all provided expressions</returns>
        public static Expression<Func<T, bool>> AndAll<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
        {
            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            var listOfExpressions = expressions.ToList();
            if (!listOfExpressions.Any())
            {
                return t => true;
            }

            Expression<Func<T, bool>> combined = listOfExpressions[0];

            for (int i = 0; i < listOfExpressions.Count() - 1; i++)
            {
                var left = combined;
                var right = listOfExpressions[i + 1];
                combined = left.And(right);
            }

            return combined;
        }

        /// <summary>
        /// Allows for the Composition of Expressions into a single expression.
        /// </summary>
        /// <typeparam name="T">The Type that the Expression refers too</typeparam>
        /// <param name="first">The first Expression.</param>
        /// <param name="second">The second Expression.</param>
        /// <param name="merge">The type of operation to use when mergeing.</param>
        /// <returns>Expression that has been joined with the Expression.Or operator</returns>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// Joins the specified first expression, with the second expression using the Or operator.
        /// </summary>
        /// <typeparam name="T">The Type that the Expression refers too</typeparam>
        /// <param name="first">The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <returns>Expression that has been joined with the Expression.Or operator</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }
}