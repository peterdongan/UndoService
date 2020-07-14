// Copyright (c) Peter Dongan. All rights reserved. 
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UndoService.Test")]

namespace StateManagement.DataStructures
{
    /// <summary>
    /// Wrapper for standard stack which raises HasItemsChanged events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class StandardStack<T> : IStack<T>
    {
        private Stack<T> _stack;

        public StandardStack()
        {
            _stack = new Stack<T>();
        }

        public event HasItemsChangedEventHandler HasItemsChanged;

        public int Count { get { return _stack.Count; } }

        public void Push(T item)
        {
            _stack.Push(item);
            if(_stack.Count == 1)
            {
                HasItemsChanged?.Invoke(this, new EventArgs());
            }
        }

        public T Pop()
        {
            var item = _stack.Pop();
            if (_stack.Count == 0)
            {
                HasItemsChanged?.Invoke(this, new EventArgs());
            }
            return item;
        }

        public void Clear()
        {
            if(Count>0)
            {
                _stack.Clear();
                HasItemsChanged?.Invoke(this, new EventArgs());
            }
        }

        public T Peek()
        {
            return _stack.Peek();
        }

        public bool Contains(T item)
        {
            return _stack.Contains(item);
        }

        protected void InvokeHasItemsChanged()
        {
            HasItemsChanged?.Invoke(this, new EventArgs());
        }
    }
}
