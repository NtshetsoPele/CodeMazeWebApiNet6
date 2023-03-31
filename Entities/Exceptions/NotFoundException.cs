namespace Entities.Exceptions;

// Create custom exceptions that we can call from
// the service methods and interrupt the flow.
// Then our error handling middleware can catch
// the exception, process the response, and return
// it to the client. This is a great way of
// handling invalid requests inside a service layer
// without having additional checks in our
// controllers.

// Base class for all the individual not found
// exception classes.
public abstract class NotFoundException : Exception
{
    protected NotFoundException(string message)
        : base(message)
    { }
}