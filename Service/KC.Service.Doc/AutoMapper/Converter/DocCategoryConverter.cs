using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KC.Framework.Extension;
using KC.Model.Doc;
using KC.Service.DTO.Doc;

namespace KC.Service.Doc.AutoMapper.Converter
{
    public class DocCategoryConverter : ITypeConverter<DocCategory, DocCategoryDTO>
    {
        public DocCategoryDTO Convert(DocCategory source, DocCategoryDTO destination, ResolutionContext context)
        {
            var menu = source;
            if (menu == null)
            {
                return null;
            }

            var result = new DocCategoryDTO()
            {
                IsDeleted = menu.IsDeleted,
                CreatedBy = menu.CreatedBy,
                CreatedName = menu.CreatedName,
                CreatedDate = menu.CreatedDate,
                ModifiedBy = menu.ModifiedBy,
                ModifiedDate = menu.ModifiedDate,
                ModifiedName = menu.ModifiedName,

                Id = menu.Id,
                Text = menu.Name,
                ParentId = menu.ParentId,
                TreeCode = menu.TreeCode,
                Leaf = menu.Leaf,
                Level = menu.Level,
                Index = menu.Index,
                Children = context.Mapper.Map<List<DocCategoryDTO>>(menu.ChildNodes.Where(m => !m.IsDeleted).OrderBy(m => m.Index)),

                Type = menu.Type,
                Comment = menu.Comment
            };

            return result;
        }

    }
    public class DocCategoryDTOConverter : ITypeConverter<DocCategoryDTO, DocCategory>
    {
        public DocCategory Convert(DocCategoryDTO source, DocCategory destination, ResolutionContext context)
        {
            var menu = source;
            if (menu == null)
            {
                return null;
            }

            var result = new DocCategory()
            {
                IsDeleted = menu.IsDeleted,
                CreatedBy = menu.CreatedBy,
                CreatedName = menu.CreatedName,
                CreatedDate = menu.CreatedDate,
                ModifiedBy = menu.ModifiedBy,
                ModifiedDate = menu.ModifiedDate,
                ModifiedName = menu.ModifiedName,

                Id = menu.Id,
                Name = menu.Text,
                ParentId = menu.ParentId,
                TreeCode = menu.TreeCode,
                Leaf = menu.Leaf,
                Level = menu.Level,
                Index = menu.Index,
                ChildNodes = context.Mapper.Map<List<DocCategory>>(menu.Children),

                Type = menu.Type,
                Comment = menu.Comment
            };

            return result;
        }

    }
}

