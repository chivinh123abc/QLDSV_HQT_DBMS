namespace QLDSV_HTC.Application.Interfaces
{
    public interface IAuthRepository
    {
        bool ValidateUser(string username, string password);
    }
}
