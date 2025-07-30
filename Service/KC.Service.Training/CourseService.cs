using AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using KC.Database.EFRepository;
using KC.Database.IRepository;
using KC.Framework.Extension;
using KC.Framework.Tenant;
using KC.Service.Base;
using KC.Service.Constants;
using KC.Service.EFService;
using KC.Service.Util;
using KC.Service.DTO.Training;
using KC.Service.DTO;
using KC.Service.WebApiService.Business;
using KC.Model.Training;
using KC.DataAccess.Training.Repository;

namespace KC.Service.Training
{
    public interface ICourseService : IEFService
    {
        #region Config

        PaginatedBaseDTO<TeacherDTO> GetConfigsByFilter(int pageIndex, int pageSize, string searchValue, int searchType, string sort, string order);

        TeacherDTO GetConfigByName(string configName);
        TeacherDTO GetConfigById(int configId);

        bool SaveConfig(TeacherDTO model);
        bool SoftRemoveTeacherById(int configId);

        #endregion

        #region Course
        List<CourseDTO> GetCoursesByConfigId(int configId);

        CourseDTO GetPropertyById(int propertyId);

        bool SaveCourses(List<Course> Courses);
        bool SaveCourse(CourseDTO data);

        bool RemoveCourseById(int propertyId);
        bool SoftRemoveCourseById(int propertyId);
        bool SoftRemoveCoursesByConfigId(int configId);
        #endregion
        
    }

    public class CourseService : EFServiceBase, ICourseService
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// ComAccountUnitOfWorkContext
        ///     use AccountUnitOfWorkContext.Commit() to save the context change(transaction).
        /// </summary>
        private EFUnitOfWorkContextBase _unitOfContext;

        private ITeacherRepository _teacherRepository;
        private IDbRepository<Course> _courseRepository;

        public CourseService(
            Tenant tenant,
            IMapper mapper,
            EFUnitOfWorkContextBase unitOfContext,

            ITeacherRepository teacherRepository,
            IDbRepository<Course> courseRepository,
            System.Net.Http.IHttpClientFactory clientFactory,
            Microsoft.Extensions.Logging.ILogger<CourseService> logger)
            : base(tenant, clientFactory, logger)
        {
            _mapper = mapper;
            _unitOfContext = unitOfContext;

            _teacherRepository = teacherRepository;
            _courseRepository = courseRepository;
        }

        #region Teacher

        public PaginatedBaseDTO<TeacherDTO> GetConfigsByFilter(int pageIndex, int pageSize, string searchValue, int searchType, string sort, string order)
        {
            Expression<Func<Teacher, bool>> predicate = m => true && !m.IsDeleted;
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                predicate = predicate.And(m => m.Name.Contains(searchValue));
            }

            var data = _teacherRepository.FindPagenatedListWithCount<Teacher>(pageIndex, pageSize, predicate, sort, order.Equals("asc"));

            var total = data.Item1;
            var rows = _mapper.Map<List<TeacherDTO>>(data.Item2);
            return new PaginatedBaseDTO<TeacherDTO>(pageIndex, pageSize, total, rows);
        }

        public TeacherDTO GetConfigByName(string configName)
        {
            var data = _teacherRepository.GetTeachersByName(configName);
            return _mapper.Map<TeacherDTO>(data);
        }

        public TeacherDTO GetConfigById(int configId)
        {
            var data = _teacherRepository.GetById(configId);
            return _mapper.Map<TeacherDTO>(data);
        }

        public bool SaveConfig(TeacherDTO model)
        {
            var data = _mapper.Map<Teacher>(model);
            if (data.TeacherId == 0)
            {
                return _teacherRepository.Add(data);
            }
            else
            {
                return _teacherRepository.Modify(data, true);
            }
        }
        public bool SaveCBSConfig(TeacherDTO model)
        {
            var data = _mapper.Map<Teacher>(model);
            if (data.TeacherId == 0)
            {
                return _teacherRepository.Add(data);
            }
            else
            {
                var Courses = data.Courses;
                return _courseRepository.Modify(Courses) > 0;
            }
        }
        public bool SoftRemoveTeacherById(int configId)
        {
            var data = _teacherRepository.GetById(configId);
            data.IsDeleted = true;
            return _teacherRepository.Modify(data, true);
        }

        #endregion

        #region Course

        public List<CourseDTO> GetCoursesByConfigId(int configId)
        {
            Expression<Func<Course, bool>> predicate = m => m.CourseId.Equals(configId) && !m.IsDeleted;

            var data = _courseRepository.FindAll(predicate, k => k.CreatedDate, false);
            return _mapper.Map<List<CourseDTO>>(data);
        }

        public CourseDTO GetPropertyById(int propertyId)
        {
            var data = _courseRepository.GetById(propertyId);
            return _mapper.Map<CourseDTO>(data);
        }

        public bool AddProperty(CourseDTO data)
        {
            var model = _mapper.Map<Course>(data);
            return _courseRepository.Add(model);
        }

        public bool SaveCourses(List<Course> Courses)
        {
            return _courseRepository.Modify(Courses) > 0;
        }

        public bool SaveCourse(CourseDTO data)
        {
            var model = _mapper.Map<Course>(data);
            if (model.CourseId != 0)
            {
                return _courseRepository.Modify(model, true);
            }
            else
            {
                return _courseRepository.Add(model);
            }
        }

        public bool RemoveCourseById(int propertyId)
        {
            return _courseRepository.RemoveById(propertyId);
        }

        public bool SoftRemoveCourseById(int propertyId)
        {
            var data = this.GetPropertyById(propertyId);
            data.IsDeleted = true;
            var res = _mapper.Map<Course>(data);
            return _courseRepository.Modify(res, true);
        }

        public bool RemovePropertiesByConfigId(int configId)
        {
            var list = _courseRepository.FindAll(m => m.CourseId == configId);
            int count = 0;
            if (list.Any())
            {
                for (int i = 0; i < list.Count; i++)
                {
                    _courseRepository.RemoveById(list[i].CourseId);
                    count++;
                }
            }
            if (count == list.Count)
            {
                return true;
            }
            return false;
        }

        public bool SoftRemoveCoursesByConfigId(int configId)
        {
            var list = _courseRepository.FindAll(m => m.CourseId == configId);
            if (list.Count != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].IsDeleted = true;
                    var data = _mapper.Map<Course>(list[i]);
                    _courseRepository.Modify(data, true);
                }
                var listAfter = _courseRepository.FindAll(m => m.CourseId == configId);
                if (listAfter.Count == 0)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
