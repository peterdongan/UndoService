// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System.Collections.Generic;

namespace StateManagement.DataStructures
{
    /// <summary>
    /// Stack<Int> with DeleteLast(int) method added.</Int>
    /// </summary>
    class IntStackWithDelete : Stack<int> , IStack<int>
    {
        /// <summary>
        /// Deletes the last instance of a value from the stack. Does nothing if the value is not in the stack.
        /// </summary>
        /// <param name="item"></param>
        public void DeleteLast(int item)
        {
            if (Contains(item))
            {
                var tempStack = new Stack<int>();

                var nextItem = Pop();

                while (nextItem != item)
                {
                    tempStack.Push(nextItem);
                    nextItem = Pop();
                }

                while (tempStack.Count > 0)
                {
                    Push(tempStack.Pop());
                }
            }
        }
    }
}
