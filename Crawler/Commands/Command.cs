using System.Threading.Tasks;

namespace Crawler.Commands
{
    public interface Command  
    {
        Task<Result> Execute();
    }
}