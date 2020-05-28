// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    /// <summary>
    /// Full UndoService interface. Implements IStateTracker and IUndoRedo
    /// </summary>
    public interface IUndoService : IStateTracker, IUndoRedo
    {

    }
}
