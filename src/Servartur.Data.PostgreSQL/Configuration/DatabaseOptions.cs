namespace Servartur.Data.PostgreSQL.Configuration;

internal class DatabaseOptions
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public required string DatabaseName { get; init; }
    public required int CommandTimeoutSeconds { get; init; }
}
