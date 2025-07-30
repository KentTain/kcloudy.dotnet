using KC.Model.Job.Table;

namespace KC.Job.IRepository
{
    public interface IThreadConfigRepository
    {
        void DeleteAllWorkerRoleConfig();
        void DeleteUselessWorkerRoleConfig();
        void DeleteWorkerRoleConfigInfo(string rowKey);
        ThreadConfigInfo GetWorkerRoleConfigToRun(string workerRoleId, string hostName);
    }
}