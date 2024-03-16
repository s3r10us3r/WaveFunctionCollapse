namespace WaveFunctionCollapse.WaveFunctionCollapse.SimpleTiledModel.XMLModels
{
    public class XMLException : Exception
    {
        public XMLException() { }

        public XMLException(string message) : base(message) { }

        public XMLException(string message, Exception inner) : base(message, inner) { }
    }
}
