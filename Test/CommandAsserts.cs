using Crawler.Commands;
using NUnit.Framework;

namespace Test
{
    public class CommandAsserts
    {
        public void AssertThatCommandFailed(Result result)
        {
            Assert.False(result.IsSucess, "The command should fail");
        }

        public void AssertCommandEndedWithSucess(Result result)
        {
            Assert.True(result.IsSucess, "The command should be successful");
        }

        public void AssertCorrectInstanceTypeOfResult<T>(Result result)
        {
            Assert.IsInstanceOf<T>(result);
        }
    }
}