// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;
using System.Collections.Generic;
using System.Text;

namespace StateManagement
{
    /// <summary>
    /// The type of Action (Undo or Redo) that caused the state to be set
    /// </summary>
    public enum StateSetAction { 
        /// <summary>
        /// </summary>
        Undo, 
        /// <summary>
        /// </summary>
        Redo }
}
