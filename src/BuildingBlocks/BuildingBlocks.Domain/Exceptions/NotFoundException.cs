namespace BuildingBlocks.Domain.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} with identifier '{id}' was not found.") { }
}
