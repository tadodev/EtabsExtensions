using EtabsExtensions.Infrastructure.Import.Interface;
using System.Data.OleDb;
using System.Runtime.Versioning;

namespace EtabsExtensions.Infrastructure.Import;

[SupportedOSPlatform("windows")]
public class AccessConnectionProvider : IAccessConnectionProvider, IDisposable
{
    private OleDbConnection? _connection;
    public bool IsConnected => _connection != null && _connection.State == System.Data.ConnectionState.Open;

    public async Task<bool> OpenAsync(string filePath)
    {
        if (IsConnected)
            throw new InvalidOperationException("A connection is already open. Close it before opening a new one.");
        try
        {
            var connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;";
            _connection = new OleDbConnection(connectionString);
            await _connection.OpenAsync();
            return true;
        }
        catch (Exception)
        {
            _connection?.Dispose();
            _connection = null;
            return false;
        }
    }

    public async Task CloseAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
            _connection = null;
        }
    }

    public OleDbConnection? GetConnection() => _connection;

    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null;
        GC.SuppressFinalize(this);
    }
}
