using System.Threading.Tasks;

namespace KC.Database.IRepository
{
    public interface IUnitOfWork<T> where T : class
    {
        T Context { get; }
        /// <summary>  
        ///     获取 当前单元操作是否已被提交  
        /// </summary>  
        bool IsCommitted { get; }
        /// <summary>  
        ///     提交当前单元操作的结果  
        /// </summary>  
        /// <returns></returns>  
        int Commit();
        Task<int> CommitAsync();
        /// <summary>  
        ///     把当前单元操作回滚成未提交状态  
        /// </summary>  
        void Rollback();  
    }
}
