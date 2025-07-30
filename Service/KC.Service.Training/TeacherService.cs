using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KC.Service.EFService;
using KC.Framework.Base;
using KC.DataAccess.Training.Repository;
using KC.Framework.Tenant;
using KC.Service.DTO;
using KC.Framework.Extension;
using KC.Database.IRepository;
using KC.Model.Training;
using KC.Service.DTO.Training;
using AutoMapper;
using KC.Service.WebApiService.Business;

namespace KC.Service.Training
{
    public interface ITeacherService : IEFService
    {
        PaginatedBaseDTO<TeacherDTO> GetPaginatedTeachersByName(int pageIndex, int pageSize, string name);

        TeacherDTO GetTeacherByName(string name);

        bool SaveTeacher(TeacherDTO model);
        bool RemoveTeacherById(string name);

    }

    public class TeacherService : EFServiceBase, ITeacherService
    {
        private readonly IMapper _mapper;

        private ITeacherRepository _teacherRepository;

        public TeacherService(
            Tenant tenant,
            IMapper mapper,

            ITeacherRepository seedEntityRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<TeacherService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _teacherRepository = seedEntityRepository;
        }

        public PaginatedBaseDTO<TeacherDTO> GetPaginatedTeachersByName(int pageIndex, int pageSize, string name)
        {
            Expression<Func<Teacher, bool>> predicate = m => true;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(m => m.Name.Contains(name));
            }

            var data = _teacherRepository.FindPagenatedListWithCount(pageIndex, pageSize, predicate, m => m.Name, true);

            var total = data.Item1;
            var rows = _mapper.Map<List<TeacherDTO>>(data.Item2);
            return new PaginatedBaseDTO<TeacherDTO>(pageIndex, pageSize, total, rows);
        }

        public TeacherDTO GetTeacherByName(string name)
        {
            var data = _teacherRepository.GetById(name);
            return _mapper.Map<TeacherDTO>(data);
        }

        public bool SaveTeacher(TeacherDTO model)
        {
            var data = _mapper.Map<Teacher>(model);
            var sequence = _teacherRepository.GetById(model.TeacherId);
            if (sequence == null)
            {
                return _teacherRepository.Add(data);
            }
            else
            {
                return _teacherRepository.Modify(data, true);
            }
        }
        public bool RemoveTeacherById(string name)
        {
            var data = _teacherRepository.GetById(name);
            return _teacherRepository.Modify(data, true);
        }
    }
}
