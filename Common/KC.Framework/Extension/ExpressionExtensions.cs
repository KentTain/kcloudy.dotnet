using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace KC.Framework.Extension
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// Expression<Func<Cat, bool>> GetMappedSelector(Expression<Func<Dog, bool>> selector)
        ///     {
        ///         Expression<Func<Cat, Dog>> mapper = Mapper.CreateMapExpression<Cat, Dog>();
        ///         Expression<Func<Cat, bool>> mappedSelector = selector.Compose(mapper);
        ///         return mappedSelector;
        ///     }
        /// </summary>
        /// <typeparam name="X"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <typeparam name="Z"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        public static Expression<Func<X, Y>> Compose<X, Y, Z>(this Expression<Func<Z, Y>> outer, Expression<Func<X, Z>> inner)
        {
            return Expression.Lambda<Func<X, Y>>(
                ParameterReplacer.Replace(outer.Body, outer.Parameters[0], inner.Body),
                inner.Parameters[0]);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

    }
    
}
