using AutoMapper;
using Chino.IdentityServer.Models.User;
using Chino.IdentityServer.SeedData;

namespace Chino.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SeedDataJsonConfig.User, ChinoUser>();
            //CreateMap<Meow, MeowDto>();
            //......
        }
    }
}
