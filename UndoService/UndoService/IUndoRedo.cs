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
        /// Raised when CanUndo changes.
        /// </summary>
        event CanUndoChangedEventHandler CanUndoChanged;

        /// <summary>
        /// Raised when CanRedo changes.
        /// </summary>
        event CanRedoChangedEventHandler CanRedoChanged;

        /// <summary>
        /// 
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// 
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Indicates whether the state was changed from its original state or the last time ClearIsStateChangedFlag was invoked. The intended use is to flag if there are unsaved changes to the state.
        /// </summary>
        bool IsStateChanged { get; }

        /// <summary>
        /// Resets the IsStateChanged flag to false. This is intended to be invoked after saving state, to show that there are no longer unsaved changes. 
        /// </summary>
        void ClearIsStateChangedFlag();

        /// <summary>
        /// Clears the Undo and Redo stacks and sets the IsStateChanged flag to false.
        /// </summary>
        void Reset();

        /// <summary>
        /// Clears the Undo and Redo stacks.
        /// </summary>
        [Obsolete("ClearStacks() is deprecated. Please use Reset() instead.")]
        void ClearStacks();

        /// <summary>
        /// Clears the Undo stack (but not the redo stack).
        /// </summary>
        void ClearUndoStack();

        /// <summary>
        /// Clears the redo stack (but not the undo stack). This is done automatically when RecordState() is invoked.
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
