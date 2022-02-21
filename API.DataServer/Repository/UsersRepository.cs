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
    public class UsersRepository : GenericRepository<UserInfoModel>, IUsersRepository 
    {
        public UsersRepository(ApiDbContext context, ILogger logger)
            :base(context, logger)
        {

        }

        public override async Task<IEnumerable<UserInfoModel>> GetAllAsync()
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
                return new List<UserInfoModel>();
            }
        }

        public async Task<IEnumerable<UserInfoModel>> GetAllDeadAsync()
        {
            try
            {
                return await dbSet.Where(x => x.Alive == false)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll has generated an error", typeof(UsersRepository));
                return new List<UserInfoModel>();
            }
        }

        public async Task<UserInfoModel> GetUserByEmail(string email)
        {
            try
            {
                var user = await dbSet.Where(x => x.EmailAddress == email)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return new UserInfoModel();

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetuserByEmail has generated an error", typeof(UsersRepository));
                return new UserInfoModel();
            }
        }

        public async Task<UserInfoModel> GetByIdentityIdAsync(Guid identityId)
        {
            try
            {
                var result = await dbSet.Where(x => x.Alive == true && x.IdentityId == identityId)
                    .FirstOrDefaultAsync();

                if (result == null)
                    return new UserInfoModel();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetByIdentityId method has generated an error", typeof(UsersRepository));
                return new UserInfoModel();
            }
        }

        public async Task<bool> UpdateUserProfileAsync(UserInfoModel user)
        {
            try
            {
                var existingUser = await dbSet.Where(x => x.Alive == true && x.IdentityId == user.IdentityId)
                    .FirstOrDefaultAsync();

                if (existingUser == null)
                    return false;

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;
                existingUser.DateUpdated = DateTime.UtcNow;

                dbSet.Update(existingUser);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} UpdateUserProfile method has generated an error", typeof(UsersRepository));
                return false;
            }
        }
    }
}
