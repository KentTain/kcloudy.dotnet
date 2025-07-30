using KC.Service.DTO.Account;

namespace KC.Service.Account
{
    public interface ISettingService
    {
        UserSettingDTO GetUserSettingDetailByCode(string code);
    }
}