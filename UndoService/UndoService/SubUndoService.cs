// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;

namespace StateManagement 
{
    /* The current structure of this class achieves two things: It avoids being a generic so that be referred to without a type sepcified. The Index member is internal.
     * If the project is updated to .net standard 2.1 or later then it will use c# 8.0, which allows internal memebers in interfaces.
     * At that point it might be preferable to create an ISubUndoService interface with an internal Index member and to change this class to inherit from UndoService and implement that interface.
     */

    /// <summary>
    /// This is used to track changes to a particular section of the application. It is used in conjunction with AggregateUndoService.
    /// </summary>
    public class SubUndoService : IStateTracker
    {
        private readonly IUndoService _undoService;

        public SubUndoService(IUndoService undoService)
        {
            _undoService = undoService ?? throw new NullReferenceException();

            _undoService.StateRecorded += UndoService_StateRecorded;
            _undoService.StateSet += UndoService_StateSet;
        }

        private void UndoService_StateSet(object sender, StateSetEventArgs e)
        {
            StateSet?.Invoke(this, e);
        }

        public event StateRecordedEventHandler StateRecorded;

        public event StateSetEventHandler StateSet;

        /// <summary>
        /// This is used by the AggregateUndoService to keep track of where changes were made.
        /// </summary>
        internal int Index { get; set; }

        public bool CanUndo => _undoService.CanUndo;

        public bool CanRedo => _undoService.CanRedo;

        public static SubUndoService CreateSubUndoService<T>(GetState<T> getState, SetState<T> setState, int? cap)
        {
            return new SubUndoService(new UndoService<T>(getState, setState, cap));
        }

        public void RecordState(object tag = null) => _undoService.RecordState( tag);

        internal void ClearStacks() => _undoService.ClearStacks();

        internal void ClearUndoStack() => _undoService.ClearUndoStack();

        public void Undo() => _undoService.Undo();

        public void Redo() => _undoService.Redo();

        private void UndoService_StateRecorded(object sender, EventArgs e)
        {
            StateRecorded?.Invoke(this, new EventArgs());
        }
    }
}
