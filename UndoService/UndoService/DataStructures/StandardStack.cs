// Copyright (c) Peter Dongan. All rights reserved. 
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System.Collections.Generic;

namespace StateManagement.DataStructures
{
    /// <summary>
    /// This is just the standard c# stack marked as implementing IStack
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class StandardStack<T> : Stack<T>, IStack<T>
    { 
    }
}
