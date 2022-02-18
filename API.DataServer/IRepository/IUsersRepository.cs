using API.DataServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataServer.IRepository
{
    public interface IUsersRepository : IGenericRepository<UserInfoModel>
    {
        Task<UserInfoModel> GetUserByEmail(string email);
        Task<UserInfoModel> GetByIdentityIdAsync(Guid identityId);
        Task<bool> UpdateUserProfileAsync(UserInfoModel user);
    }
}
