// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement.DataStructures
{
    //based on https://stackoverflow.com/questions/384042/can-i-limit-the-depth-of-a-generic-stack
    class DropoutStack<T> : IStack<T>
    {
        private T[] items;
        private int top = 0;

        public int Count
        {
            get; private set;
        }

        public DropoutStack(int capacity)
        {
            items = new T[capacity];
        }

        public void Clear()
        {
            items = new T[items.Length];
            top = 0;
            Count = 0;
        }

        public void Push(T item)
        {
            items[top] = item;
            top = (top + 1) % items.Length;

            if (Count < items.Length)
            {
                Count++;
            }
        }

        public T Pop()
        {
            top = (items.Length + top - 1) % items.Length;
            Count--;
            return items[top];
        }

        public T Peek()
        {
            top = (items.Length + top - 1) % items.Length;
            return items[top];
        }
    }
}
