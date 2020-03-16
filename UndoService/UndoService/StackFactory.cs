// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace UndoService
{
     class StackFactory<T>
    {
        internal IStack<T> MakeStack (int? cap)
        {
            if(cap == null)
            {
                return new StackWrapper<T>();
            }
            else
            {
                return new DropoutStack<T>(cap.Value);
            }
        }
    }
}
