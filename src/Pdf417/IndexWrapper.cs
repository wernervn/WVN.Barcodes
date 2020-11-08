namespace WVN.Barcodes.Pdf417
{
    internal sealed class IndexWrapper
    {
        public static int Increment(int current, int count) => current++ % count;
        private readonly int _count;
        public int Current { get; private set; }
        public IndexWrapper(int count) : this(count, 0) { }
        public IndexWrapper(int count, int start)
        {
            _count = count;
            Current = start;
        }
        public int Increment() => (Current = ++Current % _count);
        public void Set(int value) => Current = value % _count;
    }
}
