using System;
using System.Collections;
using System.Collections.Generic;

namespace TG.RollingStack
{
    /// <summary>
    /// A LIFO stack that rolls over when it reaches the specified capacity size.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the stack.</typeparam>
    public class RollingStack<T> : IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RollingStack{T}"/> class.
        /// </summary>
        /// <param name="capacity">The fixed capacity of the stack.</param>
        public RollingStack(int capacity)
        {
            _values = new T[capacity];
        }

        private readonly T[] _values;
        private int _zeroBasedIndex;
        private int _count;
        private int _current;

        /// <summary>
        /// Gets the element at the specified stack index, where 0 is the newest element.
        /// </summary>
        /// <param name="index">The zero-based stack index.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is out of range.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var actualIndex = (_zeroBasedIndex - index + _values.Length) % _values.Length;
                return _values[actualIndex];
            }
        }

        /// <summary>
        /// Pushes a value onto the stack, overwriting the oldest element when at capacity.
        /// </summary>
        /// <param name="value">The value to push.</param>
        public void Push(T value)
        {
            var index = _current % _values.Length;
            _values[index] = value;
            _zeroBasedIndex = index;
            _current = index + 1;
            _count = Math.Min(_count + 1, _values.Length);
        }

        /// <summary>
        /// Removes and returns the newest element in the stack.
        /// </summary>
        /// <returns>The removed element.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the stack is empty.</exception>
        public T Pop()
        {
            if (_count == 0)
                throw new InvalidOperationException("The stack is empty.");

            var index = (_current - 1 + _values.Length) % _values.Length;
            var value = _values[index];
            _values[index] = default;
            _current = index;
            _zeroBasedIndex = (_current - 1 + _values.Length) % _values.Length;
            if (_count > 0)
                _count--;
            return value;
        }

        /// <summary>
        /// Attempts to remove and return the newest element in the stack.
        /// </summary>
        /// <param name="value">When this method returns, contains the removed element if successful.</param>
        /// <returns><c>true</c> if an element was removed; otherwise, <c>false</c>.</returns>
        public bool TryPop(out T value)
        {
            if (_count == 0)
            {
                value = default;
                return false;
            }

            var index = (_current - 1 + _values.Length) % _values.Length;
            value = _values[index];
            _values[index] = default;
            _current = index;
            _zeroBasedIndex = (_current - 1 + _values.Length) % _values.Length;
            if (_count > 0)
                _count--;
            return true;
        }

        /// <summary>
        /// Returns the newest element in the stack without removing it.
        /// </summary>
        /// <returns>The newest element.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the stack is empty.</exception>
        public T Peek() => _count == 0 ? throw new InvalidOperationException("The stack is empty.") : this[0];

        /// <summary>
        /// Attempts to return the newest element in the stack without removing it.
        /// </summary>
        /// <param name="value">When this method returns, contains the newest element if successful.</param>
        /// <returns><c>true</c> if an element was returned; otherwise, <c>false</c>.</returns>
        public bool TryPeek(out T value)
        {
            if (_count == 0)
            {
                value = default;
                return false;
            }

            value = this[0];
            return true;
        }
        /// <summary>
        /// Returns an enumerator that iterates from newest to oldest.
        /// </summary>
        /// <returns>An enumerator over the stack contents.</returns>
        public IEnumerator<T> GetEnumerator() => new RollingStackEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the number of elements currently in the stack.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Gets the fixed capacity of the stack.
        /// </summary>
        public int Capacity => _values.Length;

        private class RollingStackEnumerator : IEnumerator<T>
        {
            private readonly RollingStack<T> _stack;
            private int _currentIndex;
            
            private T _current;

            internal RollingStackEnumerator(RollingStack<T> stack)
            {
                _stack = stack;
                _currentIndex = 0;
            }

            public T Current => _current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _current = default;
            }

            public bool MoveNext()
            {
                if (_currentIndex >= _stack._count)
                    return false;

                _current = _stack[_currentIndex];

                _currentIndex++;

                return true;
            }

            public void Reset()
            {
                _currentIndex = 0;
                _current = default;
            }
        }
    }
}