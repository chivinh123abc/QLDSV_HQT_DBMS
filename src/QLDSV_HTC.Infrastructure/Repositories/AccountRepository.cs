using System.Data;
using Microsoft.Data.SqlClient;
using QLDSV_HTC.Application.DTOs;
using QLDSV_HTC.Application.Interfaces;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Infrastructure.Repositories
{
    public class AccountRepository(IDbConnectionProvider connectionProvider)
        : BaseSqlRepository(connectionProvider), IAccountRepository
    {
        public async Task<IEnumerable<AccountDto>> GetAccountListAsync()
        {
            var dt = await ExecuteQueryAsync(
                AppConstants.SpNames.GetAccountList,
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(MapRow);
        }

        public async Task CreateAccountAsync(CreateAccountDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.CreateAccount,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.AccountCrud.LoginName, dto.LoginName),
                new SqlParameter(StoredProcedureConstants.AccountCrud.Password, dto.Password),
                new SqlParameter(StoredProcedureConstants.AccountCrud.UserName, dto.UserName),
                new SqlParameter(StoredProcedureConstants.AccountCrud.Role, dto.Role)
            );
        }

        public async Task UpdateAccountAsync(UpdateAccountDto dto)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.UpdateAccount,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.AccountCrud.OldLoginName, dto.OldLoginName),
                new SqlParameter(StoredProcedureConstants.AccountCrud.NewLoginName, dto.NewLoginName ?? (object)DBNull.Value),
                new SqlParameter(StoredProcedureConstants.AccountCrud.NewPassword, dto.NewPassword ?? (object)DBNull.Value),
                new SqlParameter(StoredProcedureConstants.AccountCrud.NewUserName, dto.NewUserName ?? (object)DBNull.Value),
                new SqlParameter(StoredProcedureConstants.AccountCrud.NewRole, dto.NewRole ?? (object)DBNull.Value)
            );
        }

        public async Task DeleteAccountAsync(string loginName)
        {
            await ExecuteNonQueryAsync(
                AppConstants.SpNames.DeleteAccount,
                CommandType.StoredProcedure,
                new SqlParameter(StoredProcedureConstants.AccountCrud.LoginName, loginName)
            );
        }

        private static AccountDto MapRow(DataRow row) => new()
        {
            LoginName = row[DbConstants.Columns.Account.LoginName]?.ToString() ?? string.Empty,
            UserName = row[DbConstants.Columns.Account.UserName]?.ToString() ?? string.Empty,
            GroupName = row[DbConstants.Columns.Account.GroupName]?.ToString() ?? string.Empty,
            LecturerId = row[DbConstants.Columns.Account.LecturerId] == DBNull.Value ? null : row[DbConstants.Columns.Account.LecturerId]?.ToString()?.Trim(),
            LecturerFullName = row[DbConstants.Columns.Account.LecturerFullName] == DBNull.Value ? null : row[DbConstants.Columns.Account.LecturerFullName]?.ToString(),
            IsDisabled = row[DbConstants.Columns.Account.IsDisabled] != DBNull.Value && Convert.ToBoolean(row[DbConstants.Columns.Account.IsDisabled]),
        };
    }
}
