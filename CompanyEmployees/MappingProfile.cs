using AutoMapper;
using AutoMapper.Configuration;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Create;
using Shared.DataTransferObjects.Response;
using Shared.DataTransferObjects.Update;

namespace CompanyEmployees;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        /*
        CreateMap<Company, CompanyDto>()
            .ForMember((CompanyDto cdto) => cdto.FullAddress, 
                (IMemberConfigurationExpression<Company, CompanyDto, string> opt) => 
                    opt.MapFrom((Company c) => $"{c.Address} {c.Country}"));
        */
        CreateMap<Company, CompanyDto>()
            .ForCtorParam("FullAddress", 
                (ICtorParamConfigurationExpression<Company> opt) =>
                opt.MapFrom((Company c) => $"{c.Address} - {c.Country}"));
        
        CreateMap<CompanyForCreationDto, Company>();
        
        CreateMap<Employee, EmployeeDto>();
        
        CreateMap<EmployeeForCreationDto, Employee>();
        
        CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
        
        CreateMap<CompanyForUpdateDto, Company>();
    }
}