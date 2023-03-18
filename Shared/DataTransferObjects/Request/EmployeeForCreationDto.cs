namespace Shared.DataTransferObjects.Request;

// We don’t have the Id property because we are going to create that Id on 
// the server-side. But additionally, we don’t have the CompanyId because 
// we accept that parameter through the route: 
// [Route("api/companies/{companyId}/employees")]
public record EmployeeForCreationDto(string Name, int Age, string Position);