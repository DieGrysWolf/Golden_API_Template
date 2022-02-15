using API.DataServer.IConfiguration;
using API.DataServer.IRepository;
using API.Entities.Models.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DataServer.Repository
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        IUnitOfWork unitOfWork;

        public RefreshTokenRepository(ApiDbContext context, ILogger logger)
            : base(context, logger)
        {

        }

        public override async Task<IEnumerable<RefreshToken>> GetAllAsync()
        {
            try
            {
                return await dbSet.Where(x => x.Alive == true)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll has generated an error", typeof(RefreshTokenRepository));
                return new List<RefreshToken>();
            }
        }

        public async Task<RefreshToken> GetByRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var result = await dbSet.Where(x => x.Token.ToLower() == refreshToken.ToLower())
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (result == null)
                    return null;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll has generated an error", typeof(RefreshTokenRepository));
                return null;
            }
        }
    }
}
