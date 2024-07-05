using AutoMapper;
using Common.DTO;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserFormModel, UserForm>().ReverseMap(); //Kazemo mu da mapira Subject na SubjectDto i obrnuto

        }
    }
}
