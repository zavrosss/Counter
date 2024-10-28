namespace Testing
{
    public class Counter
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public Counter(string name)
        {
            Name = name;
            Value = 0;
        }
    }
}
