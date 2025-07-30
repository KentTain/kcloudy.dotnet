using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;

namespace KC.Service.Extension
{
    public static class AutoMapperExtensions
    {
        //public static void Bidirectional<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        //{
        //    Mapper.CreateMap<TDestination, TSource>();
        //}

        public static List<TResult> MapTo<TResult>(this IEnumerable self)
        {
            if (self == null)
            {
                return null;
            }

            return (List<TResult>)Mapper.Map(self, self.GetType(), typeof(List<TResult>));
        }

        public static TResult MapTo<TResult>(this object self)
        {
            if (self == null)
            {
                return default(TResult);
            }

            return (TResult)Mapper.Map(self, self.GetType(), typeof(TResult));
        }

        public static TResult MapPropertiesToInstance<TResult>(this object self, TResult value)
        {
            if (self == null)
            {
                throw new ArgumentNullException();
            }

            return (TResult)Mapper.Map(self, value, self.GetType(), typeof(TResult));
        }

        //public static TResult DynamicMapTo<TResult>(this object self)
        //{
        //    if (self == null)
        //    {
        //        throw new ArgumentNullException();
        //    }

        //    return (TResult)Mapper.DynamicMap(self, self.GetType(), typeof(TResult));
        //}
    }
}
