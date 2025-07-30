using AutoMapper;
using KC.Service.DTO.Training;
using KC.Framework.Base;
using KC.Model.Training;
using KC.Service.DTO.Account;
using KC.Service.DTO;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Extension;

namespace KC.Service.Training.AutoMapper.Profile
{
    public class TrainingMapperProfile : global::AutoMapper.Profile
    {
        public TrainingMapperProfile()
        {
            CreateMap<Entity, EntityDTO>();
            CreateMap<EntityDTO, Entity>();


            CreateMap<Teacher, TeacherDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.CourseIds,
                    config =>
                        config.MapFrom(
                            src =>
                                src.CourseSelects.Any()
                                    ? src.CourseSelects.Select(m => m.CourseId).ToList()
                                    : default(List<int>)))
                .ForMember(target => target.CourseName,
                    config =>
                        config.MapFrom(
                            src =>
                                src.Courses.Any()
                                    ? src.CourseSelects.Select(m => m.Course).ToCommaSeparatedStringByFilter(m => m.Name)
                                    : string.Empty));
            CreateMap<TeacherDTO, Teacher>()
                .ForMember(target => target.Courses, config => config.Ignore());

            CreateMap<Student, StudentDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore())
                .ForMember(target => target.CourseIds,
                    config =>
                        config.MapFrom(
                            src =>
                                src.CourseSelects.Any()
                                    ? src.CourseSelects.Select(m => m.CourseId).ToList()
                                    : default(List<int>)))
                .ForMember(target => target.CourseName,
                    config =>
                        config.MapFrom(
                            src =>
                                src.Courses.Any()
                                    ? src.CourseSelects.Select(m => m.Course).ToCommaSeparatedStringByFilter(m => m.Name)
                                    : string.Empty));
            CreateMap<StudentDTO, Student>()
                .ForMember(target => target.Courses, config => config.Ignore());

            CreateMap<ClassRoom, ClassRoomDTO>()
                .ForMember(target => target.IsEditMode, config => config.Ignore());
            CreateMap<ClassRoomDTO, ClassRoom>();

            CreateMap<CourseRecord, CourseRecordDTO>();
            CreateMap<CourseRecordDTO, CourseRecord>();
        }
    }
}
