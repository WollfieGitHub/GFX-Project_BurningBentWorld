namespace Code.Scripts.Utils
{
    /// <summary>
    /// Accessing multi dimensional array isn't efficient in C#,
    /// so we create this wrapper for a one dimensional array
    /// </summary>
    /// <typeparam name="T">The type of the array's content</typeparam>
    public class Efficient2DArray<T>
    {
        private T[] _array;

        public readonly int Width;
        public readonly int Height;

        public Efficient2DArray(int width, int height)
        {
            Width = width;
            Height = height;
            _array = new T[width * height];
        }

        public T this[int x, int y]
        {
            get => _array[y * Width + x];
            set => _array[y * Width + x] = value;
        }
    }
}