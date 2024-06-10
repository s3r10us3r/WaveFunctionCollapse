namespace WaveFunctionCollapse.Interfaces.Errors
{
    //this error is thrown when algorithm runs into a contradiction meaning that, there are no states that we can collapse to and the image is not finished
    public class NoPossibleCollapseException : Exception
    {
        public NoPossibleCollapseException() { }

        public NoPossibleCollapseException(string message) : base(message) { }

        public NoPossibleCollapseException(string message, Exception? innerException) : base(message, innerException) { }
    }
}
