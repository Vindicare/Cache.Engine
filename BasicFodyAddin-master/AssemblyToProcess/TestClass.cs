using BasicFodyAddin;

namespace AssemblyToProcess
{
    public class TestClass
    {
        [Cache]
        public int GetResult(int number)
        {
            return number * number;
        }
    }
}