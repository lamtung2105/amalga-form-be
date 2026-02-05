namespace api.Provider;

public class ConnectionStringProvider(string connectionString)
{
    public string ConnectionString { get; } = connectionString;
}
