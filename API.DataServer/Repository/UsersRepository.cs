using API.DataServer.Data;
using API.DataServer.IConfiguration;
using API.DataServer.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataServer.Repository
{
    public class UsersRepository : GenericRepository<UserModel>, IUsersRepository 
    {
        IUnitOfWork unitOfWork;

        public UsersRepository(ApiDbContext context, ILogger logger)
            :base(context, logger)
        {

        }

        public override async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            try
            {
                return await dbSet.Where(x => x.Alive == true)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll has generated an error", typeof(UsersRepository));
                return new List<UserModel>();
            }
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            try
            {
                var user = dbSet.Where(x => x.EmailAddress == email)
                    .FirstOrDefault();

                if (user == null)
                    return new UserModel();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetuserByEmail has generated an error", typeof(UsersRepository));
                return new UserModel();
            }
        }
    }
}
