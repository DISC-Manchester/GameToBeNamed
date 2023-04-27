namespace SquareSmash.share
{
    internal class ErrorOrData

    {
        bool Errored { get; }
        string Message { get; }
        object Value { get; }

        public ErrorOrData(object value)
        {
            Errored = false;
            Message = "";
            Value = value;
        }

        public ErrorOrData(string msg)
        {
            Errored = true;
            Message = msg;
            Value = new object();
        }

        public object Unwrap()
        {
            return Errored ? Message : Value;
        }
    }
}
