using KC.Enums.Doc;
using KC.Model.Doc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KC.DataAccess.Doc.Repository
{
    public interface IDocCategoryRepository : Database.IRepository.IDbTreeRepository<DocCategory>
    {
        List<DocCategory> FindFatherCategory(int? id, LableType? type);

        Task<DocCategory> GetDocCategoryByIdAsync(int id);
        DocCategory GetDocCategoryWithChildrenById(int id);

        Task<bool> SaveDocCategoryAsync(DocCategory model);
    }
}
