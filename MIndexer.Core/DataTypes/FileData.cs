namespace MIndexer.Core.DataTypes
{
    public class FileData
    {
        public bool IsFolder { get; set; }

        public string Name { get; set; }

        public State State { get; set; }

        public string LName { get; set; }

        public State LState { get; set; }
    }

    public enum State
    {
        NotTouched=0,
        Downloaded,
        Indexed
    }
}
