// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using StateManagement.DataStructures;
using System;
using System.Collections.Generic;

namespace StateManagement
{
    /// <summary>
    /// Undo operation attempted with empty stack
    /// </summary>
    public class EmptyStackException : Exception
    {
        public EmptyStackException(string message) : base(message)
        {
        }
    }
}
