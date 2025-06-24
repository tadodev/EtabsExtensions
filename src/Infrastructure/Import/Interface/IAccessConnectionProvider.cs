using System.Data.OleDb;

namespace EtabsExtensions.Infrastructure.Import.Interface
{
    public interface IAccessConnectionProvider
    {
        bool IsConnected { get; }
        Task<bool> OpenAsync(string filePath);
        Task CloseAsync();
        OleDbConnection? GetConnection();
    }
}