using KC.Database.EFRepository;
using KC.Database.Extension;
using KC.Enums.Portal;
using KC.Framework.Extension;
using KC.Model.Portal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KC.DataAccess.Portal.Repository
{
    public class RecommendCategoryRepository : EFTreeRepositoryBase<RecommendCategory>, IRecommendCategoryRepository
    {
        public RecommendCategoryRepository(EFUnitOfWorkContextBase unitOfWork)
            : base(unitOfWork)
        {
        }

        public List<RecommendCategory> FindFatherCategory(int? id)
        {
            Expression<Func<RecommendCategory, bool>> predicate = m => !m.IsDeleted && m.Level == 1;
            if (id.HasValue)
            {
                predicate = predicate.And(m => m.Id != id.Value);
            }
            return Entities
                .Where(predicate)
                .OrderByDescending(m => m.CreatedDate)
                .AsNoTracking()
                .ToList();
        }

        public override async Task<List<RecommendCategory>> FindTreeNodesWithNestParentAndChildByFilterAsync(Expression<Func<RecommendCategory, bool>> predicate)
        {
            var nodes = await EFContext.Set<RecommendCategory>()
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
            if (nodes == null || !nodes.Any())
                return new List<RecommendCategory>();

            var allIds = new HashSet<int>();
            var lastIds = new HashSet<int>();
            nodes.ForEach(m => {
                var newIds = m.TreeCode.ArrayFromCommaDelimitedIntegersBySplitChar('-').ToList();
                lastIds.Add(newIds.LastOrDefault());
                newIds.ForEach(n => allIds.Add(n));
            });

            var entities = await EFContext.Context.Set<RecommendCategory>()
                .Where(m => !m.IsDeleted && allIds.Contains(m.Id))
                .AsNoTracking()
                .ToListAsync();

            var result = new List<RecommendCategory>();
            foreach (var id in lastIds)
            {
                var child = await FindNodeListContainNestChildByParentIdAsync(id);
                foreach (var children in child)
                {
                    if (!entities.Any(m => m.Id == children.Id))
                    {
                        entities.Add(children);
                    }
                }
            }

            foreach (var parentNode in entities.Where(m => m.ParentId == null).OrderBy(m => m.Index))
            {
                NestTreeNodeWithChild(parentNode, entities);
                result.Add(parentNode);
            }

            return result;
        }

        public override async Task<List<RecommendCategory>> FindNodeListContainNestChildByParentIdAsync(int? parentId)
        {
            var result = await EFContext.Set<RecommendCategory>()
                .Where(o => !o.IsDeleted && o.ParentId == parentId)
                .AsNoTracking()
                .ToListAsync();

            var ids = result.OrderBy(m => m.Index).Select(m => m.Id).ToList();
            result.AddRange(FindNodeListContainNestChildByParentIds(ids));
            return result;
        }
        public override List<RecommendCategory> FindNodeListContainNestChildByParentIds(List<int> parentIds)
        {
            if (parentIds == null || !parentIds.Any()) return new List<RecommendCategory>();

            var result = EFContext.Set<RecommendCategory>()
                .Where(o => !o.IsDeleted
                        && o.ParentId != null
                        && parentIds.Contains(o.ParentId.Value))
                .AsNoTracking()
                .ToList();

            var pIds = result.OrderBy(m => m.Index).Select(m => m.Id).ToList();
            if (pIds == null || !pIds.Any()) return result;

            result.AddRange(FindNodeListContainNestChildByParentIds(pIds));
            return result;
        }

        public async Task<RecommendCategory> GetRecommendCategoryByIdAsync(int id)
        {
            var result = await EFContext.Set<RecommendCategory>()
                .Where(m => !m.IsDeleted && m.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            //EFContext.Context.Detach(result);
            return result;
        }

        public RecommendCategory GetRecommendCategoryWithChildrenById(int id)
        {
            return Entities
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ChildNodes)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public async Task<bool> SaveRecommendCategoryAsync(RecommendCategory model)
        {
            //新增记录时，先保存数据，后更新TreeCode、Level、Leaf字段
            if (model.Id == 0)
            {
                await EFContext.Context.Set<RecommendCategory>().AddAsync(model);
                await EFContext.Context.SaveChangesAsync();
            }

            //更新TreeCode、Level、Leaf字段，下面方法未更新字段？？
            //return await UpdateExtendFieldsByFilterAsync(m => m.Id == model.Id);

            if (model.ParentId.HasValue)
            {
                var dbPModel = await EFContext.Context.Set<RecommendCategory>().FirstOrDefaultAsync(m => m.Id == model.ParentId);
                model.Level = dbPModel.Level + 1;
                model.TreeCode = dbPModel.TreeCode + DatabaseExtensions.TreeCodeSplitIdWithChar + model.Id;
            }
            else
            {
                model.Level = 1;
                model.TreeCode = model.Id.ToString();
            }
            if (EFContext.Context.Entry(model).State == EntityState.Detached)
            {
                EFContext.Context.Set<RecommendCategory>().Attach(model);
                EFContext.Context.Entry(model).State = EntityState.Modified;
            }

            return await EFContext.Context.SaveChangesAsync() >= 0;
        }
    }
}
