// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. See LICENCE file in the project root for full licence information.

namespace UndoService
{
    interface IStack<T>
    {
        int Count { get; }

        void Push(T item);

        T Pop();

        void Clear();


    }
}
