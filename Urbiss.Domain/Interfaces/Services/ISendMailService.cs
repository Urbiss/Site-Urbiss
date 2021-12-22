using System.Threading.Tasks;
using Urbiss.Domain.Dtos;

namespace Urbiss.Domain.Interfaces
{
    public interface ISendMailService
    {
        void Send(MailRequestDto request);
    }
}
