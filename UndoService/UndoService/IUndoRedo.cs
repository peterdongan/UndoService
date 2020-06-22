// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;

namespace StateManagement
{
    /// <summary>
    /// Performs Undo/redo actions. Used in conjunction with an IStateTracker.
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
        /// Indicates whether the state was changed from its original state or the last time ClearIsChangedFlag was invoked.
        /// </summary>
        bool IsStateChanged { get; }

        /// <summary>
        /// Resest the IsStateChanged flag to false.
        /// </summary>
        void ClearIsChangedFlag();

        /// <summary>
        /// Clear the Undo and Redo stacks.
        /// </summary>
        void ClearStacks();

        /// <summary>
        /// Clear the Undo stack (but not the redo stack).
        /// </summary>
        void ClearUndoStack();

        /// <summary>
        /// Clear the redo stack (but not the undo stack). This is done automatically when RecordState() is invoked.
        /// </summary>
        void ClearRedoStack();

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
