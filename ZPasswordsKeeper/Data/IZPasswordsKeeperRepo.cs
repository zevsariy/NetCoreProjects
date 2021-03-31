using System.Collections.Generic;
using ZPasswordsKeeper.Models;

namespace ZPasswordsKeeper.Data
{
    public interface IZPasswordsKeeperRepo
    {
        IEnumerable<PasswordInfoItem> GetAllPasswordsInfos();

        PasswordInfoItem GetPasswordInfoById(int Id);
    }
}
