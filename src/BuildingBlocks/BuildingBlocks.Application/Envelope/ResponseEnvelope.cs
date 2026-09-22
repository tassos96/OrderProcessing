namespace BuildingBlocks.Application.Envelope;

public sealed record ExceptionResponse(
    string Code,
    string Message,
    IReadOnlyList<string>? Details = null);

public sealed record ResponseEnvelope<T>
{
    public T? Payload { get; init; }
    public ExceptionResponse? Exception { get; init; }

    public static ResponseEnvelope<T> Success(T payload) => new() { Payload = payload };

    public static ResponseEnvelope<T> Failure(string code, string message, IReadOnlyList<string>? details = null) =>
        new() { Exception = new ExceptionResponse(code, message, details) };

    public static ResponseEnvelope<T> Failure(ExceptionResponse exception) =>
        new() { Exception = exception };
}
