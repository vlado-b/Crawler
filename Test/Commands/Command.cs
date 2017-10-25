using System.Threading.Tasks;

namespace Test.Commands
{
    public interface Command //where R : Result
    {
        Task<Result> Execute();
    }
}