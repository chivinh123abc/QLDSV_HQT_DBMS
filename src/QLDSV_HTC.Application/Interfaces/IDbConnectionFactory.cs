using System.Data;

namespace QLDSV_HTC.Application.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string? username = null, string? password = null);
    }
}
