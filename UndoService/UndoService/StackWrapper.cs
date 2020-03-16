// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. See LICENCE file in the project root for full licence information.

using System.Collections.Generic;

namespace UndoService
{
    class StackWrapper<T> : IStack<T>
    {
        private readonly Stack<T> _stack;

        public int Count
        { 
            get { return _stack.Count; } 
        }

        public StackWrapper()
        {
            _stack = new Stack<T>();
        }

        public void Push(T item)
        {
            _stack.Push(item);
        }

        public T Pop()
        {
            return _stack.Pop();
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}
