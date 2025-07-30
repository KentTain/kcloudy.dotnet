using System.Collections.Generic;

namespace KC.Framework.Extension
{
    public abstract class MapperBase<T1, T2>
        where T2 : class
        where T1 : class
    {
        protected abstract T2 MapInternal(T1 source);
        protected abstract T1 MapInternal(T2 source);

        public virtual T2 Map(T1 source, params object[] moreSources)
        {
            if (source == null)
                return null;

            T2 result = MapInternal(source);

            return result;
        }

        public virtual T1 Map(T2 source, params object[] moreSources)
        {
            return source == null ? null : MapInternal(source);
        }

        public virtual IEnumerable<T2> Map(IEnumerable<T1> sourceItems, params object[] moreSources)
        {
            if (sourceItems == null)
                yield break;

            foreach (T1 item in sourceItems)
            {
                yield return MapInternal(item);
            }
        }

        public virtual IEnumerable<T1> Map(IEnumerable<T2> sourceItems, params object[] moreSources)
        {
            if (sourceItems == null)
                yield break;

            foreach (T2 item in sourceItems)
            {
                yield return MapInternal(item);
            }
        }
    }
}
