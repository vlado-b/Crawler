namespace Test.Commands
{
    public class Result
    {
        public Result()
        {
                
        }

        public Result( bool isSucess )
        {
            IsSucess = isSucess;
        }

        public bool IsSucess { get; protected set; }
    }
}