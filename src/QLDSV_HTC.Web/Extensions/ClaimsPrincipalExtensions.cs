using System.Security.Claims;
using QLDSV_HTC.Domain.Constants;

namespace QLDSV_HTC.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserGroup(this ClaimsPrincipal user)
        {
            return user.FindFirst(AppConstants.SessionKeys.Group)?.Value ?? string.Empty;
        }

        public static bool IsInGroup(this ClaimsPrincipal user, string group)
        {
            var userGroup = user.GetUserGroup();
            return string.Equals(userGroup, group, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsStudent(this ClaimsPrincipal user) => user.IsInGroup(AppConstants.Groups.SV);
        public static bool IsPGV(this ClaimsPrincipal user) => user.IsInGroup(AppConstants.Groups.PGV);
        public static bool IsKhoa(this ClaimsPrincipal user) => user.IsInGroup(AppConstants.Groups.KHOA);
        public static bool IsFaculty(this ClaimsPrincipal user) =>
            user.IsPGV() || user.IsKhoa();
    }
}
