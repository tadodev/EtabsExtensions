using System.Data;
using System.Data.OleDb;
using System.Runtime.Versioning;
using EtabsExtensions.Domain.Entities.JointDrift;
using EtabsExtensions.Core.JointDrift;
using EtabsExtensions.Infrastructure.Import.Interface;

namespace EtabsExtensions.Infrastructure.Import.Repositories.JointDrift;

[SupportedOSPlatform("windows")]
public class AccessJointDriftRepository : IJointDriftRepository
{
    private readonly IAccessConnectionProvider _connectionProvider;

    public AccessJointDriftRepository(IAccessConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IEnumerable<string>> GetUniqueCaseNamesAsync()
    {
        var caseNames = new List<string>();
        var sql = "SELECT DISTINCT [Output Case] FROM [Joint Drifts]";
        var connection = _connectionProvider.GetConnection();
        if (connection == null || connection.State != ConnectionState.Open)
            throw new InvalidOperationException("Access database connection is not open.");
        using var command = new OleDbCommand(sql, connection);
        try
        {
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (!reader.IsDBNull(0))
                    caseNames.Add(reader.GetString(0));
            }
        }
        catch (OleDbException ex)
        {
            Console.WriteLine($"Database error getting unique case names: {ex.Message}");
            throw;
        }
        return caseNames;
    }

    public async Task<IEnumerable<JointDriftItem>> GetEntriesByCaseAsync(string outputCase)
    {
        var entries = new List<JointDriftItem>();
        var sql = "SELECT * FROM [Joint Drifts] WHERE [Output Case] = ?";
        var connection = _connectionProvider.GetConnection();
        if (connection == null || connection.State != ConnectionState.Open)
            throw new InvalidOperationException("Access database connection is not open.");

        await using var command = new OleDbCommand(sql, connection);
        command.Parameters.AddWithValue("?", outputCase);
        try
        {
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var entry = new JointDriftItem(
                    story: reader["Story"]?.ToString(),
                    label: reader["Label"]?.ToString(),
                    uniqueName: reader["Unique Name"]?.ToString(),
                    outputCase: reader["Output Case"]?.ToString(),
                    caseType: reader["Case Type"]?.ToString(),
                    stepType: reader["Step Type"]?.ToString(),
                    stepNumber: reader["Step Number"] != DBNull.Value ? Convert.ToDouble(reader["Step Number"]) : 0.0,
                    stepLabel: reader["Step Label"]?.ToString(),
                    dispX: reader["Disp X"] != DBNull.Value ? Convert.ToDouble(reader["Disp X"]) : 0.0,
                    dispY: reader["Disp Y"] != DBNull.Value ? Convert.ToDouble(reader["Disp Y"]) : 0.0,
                    driftX: reader["Drift  X"] != DBNull.Value ? Convert.ToDouble(reader["Drift  X"]) : 0.0,
                    driftY: reader["Drift Y"] != DBNull.Value ? Convert.ToDouble(reader["Drift Y"]) : 0.0
                );
                entries.Add(entry);
            }
        }
        catch (OleDbException ex)
        {
            Console.WriteLine($"Database error fetching entries for case '{outputCase}': {ex.Message}");
            throw;
        }
        return entries;
    }
}