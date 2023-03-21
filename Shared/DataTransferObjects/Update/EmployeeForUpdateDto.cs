namespace Shared.DataTransferObjects.Update;

// This DTO contains the same properties as the DTO for creation, but
// there is a conceptual difference between those two DTO classes.
// One is for updating and the other is for creating. Furthermore, once
// we get to the validation part, we will understand the additional
// difference between those two.
public record EmployeeForUpdateDto(string Name, int Age, string Position);