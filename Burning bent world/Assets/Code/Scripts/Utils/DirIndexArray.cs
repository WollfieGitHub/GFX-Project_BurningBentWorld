using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utils;
using static Utils.Utils;

namespace Code.Scripts.Utils
{
    public class DirIndexArray<T> : IEnumerable<T>
    {
        private T[] _array = new T[Constants.NbSidesInSquare];

        public T this[int i]
        {
            get => _array[i];
            set => _array[i] = value;
        }

        public T this[Direction direction]
        {
            get => _array[(int)direction];
            set => _array[(int)direction] = value;
        }

        public List<T> ToList() => _array.ToList();
        
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}