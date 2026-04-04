using System.Data;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class FacultyRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), IFacultyRepository
    {
        public async Task<IEnumerable<FacultyDto>> GetFacultiesAsync()
        {
            var dt = await ExecuteQueryAsync(
                SqlQueries.Faculty.SelectAll,
                CommandType.Text
            );

            return dt.AsEnumerable().Select(row => new FacultyDto
            {
                FacultyId = row[DbConstants.Columns.Faculty.Id]?.ToString() ?? string.Empty,
                FacultyName = row[DbConstants.Columns.Faculty.Name]?.ToString() ?? string.Empty,
            });
        }
    }
}
