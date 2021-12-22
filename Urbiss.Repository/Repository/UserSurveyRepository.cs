using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Urbiss.Domain.Exceptions;
using Urbiss.Domain.Interfaces;
using Urbiss.Domain.Models;

namespace Urbiss.Repository
{
    public class UserSurveyRepository : GenericRepository<UserSurvey>, IUserSurveyRepository
    {
        public UserSurveyRepository(UrbissDbContext context) : base(context)
        {
        }

        public override Task<UserSurvey> Update(UserSurvey entity)
        {
            throw new ApiException("Método não implementado!");
        }
    }
}
