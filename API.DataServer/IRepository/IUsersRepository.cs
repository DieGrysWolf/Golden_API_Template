using API.DataServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataServer.IRepository
{
    public interface IUsersRepository : IGenericRepository<UserModel>
    {
        Task<UserModel> GetUserByEmail(string email);
    }
}
