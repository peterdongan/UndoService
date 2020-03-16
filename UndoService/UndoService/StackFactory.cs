// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. See LICENCE file in the project root for full licence information.

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
