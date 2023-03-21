using Shared.DataTransferObjects.Create;

namespace Shared.DataTransferObjects.Update;

public record CompanyForUpdateDto(string Name, string Address, string Country,
    IEnumerable<EmployeeForCreationDto> Employees);