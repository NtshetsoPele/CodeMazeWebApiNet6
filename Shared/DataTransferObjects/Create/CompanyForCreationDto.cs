namespace Shared.DataTransferObjects.Create;

// The input and output DTO classes may be the same, but we still recommend
// separating them for easier maintenance and refactoring of our code.
// Furthermore, when we start talking about validation, we don’t want to
// validate the output objects — but we definitely want to validate the input
// ones.
public record CompanyForCreationDto(string Name, string Address, string Country,
    IEnumerable<EmployeeForCreationDto> Employees);