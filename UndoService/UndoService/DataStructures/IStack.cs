// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement.DataStructures
{
    public interface IStack<T>
    {
        int Count { get; }

        void Push(T item);

        T Pop();

        void Clear();
    }
}
