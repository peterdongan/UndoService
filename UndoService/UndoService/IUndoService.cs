// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    /// <summary>
    /// Performs Undo/Redo operations and state recording
    /// </summary>
    public interface IUndoService : IUndoRedo, IStateTracker
    {
    }
}
