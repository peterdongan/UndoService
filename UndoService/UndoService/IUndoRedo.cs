// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    /// <summary>
    /// Performs Undo/redo actions. Used in conjunction with object(s) that implement IStateTracker
    /// </summary>
    public interface IUndoRedo
    {
        /// <summary>
        /// 
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// 
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Clear the Undo and Redo stacks.
        /// </summary>
        void ClearStacks();

        /// <summary>
        /// Clear the Undo stack (but not the redo stack).
        /// </summary>
        void ClearUndoStack();

        /// <summary>
        /// 
        /// </summary>
        void Undo();

        /// <summary>
        /// 
        /// </summary>
        void Redo();
    }
}
