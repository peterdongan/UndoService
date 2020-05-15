// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using StateManagement.DataStructures;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StateManagement
{
    /// <summary>
    /// Generic Undo Service using delegates to access state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UndoService<T> : IUndoService
    {
        protected readonly GetState<T> GetState;
        protected readonly SetState<T> SetState;
        protected readonly IStack<T> _undoStack;
        protected readonly Stack<T> _redoStack;    //limited by undo stack capacity already
        
        protected T _currentState;

        public UndoService(GetState<T> getState, SetState<T> setState, int? cap)
        {
            GetState = getState ?? throw new ArgumentNullException(nameof(getState));
            SetState = setState ?? throw new ArgumentNullException(nameof(setState));
            var stackFactory = new StackFactory<T>();
            _undoStack = stackFactory.MakeStack(cap);
            GetState(out _currentState);
            _redoStack = new Stack<T>();
        }


        public event StateRecordedEventHandler StateRecorded;
        
        /// <summary>
        /// This is used by the AggregateUndoService to keep track of where changes were made.
        /// </summary>
        public int Id { get; set; }

        public bool CanUndo
        {
            get
            {
                return _undoStack.Count > 0;
            }
        }

        public bool CanRedo
        {
            get
            {
                return _redoStack.Count > 0;
            }
        }

        public void ClearStacks()
        {
            _undoStack.Clear();
            GetState(out _currentState);
            _redoStack.Clear();
        }

        public void ClearUndoStack()
        {
            _undoStack.Clear();
        }

        public void Undo()
        {
            if (!CanUndo)
            {
                throw new EmptyStackException("Nothing to undo. Check CanUndo is true before invoking Undo().");
            }
            var momento = _undoStack.Pop();
            SetState(momento);
            _redoStack.Push(_currentState);
            _currentState = momento;
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                throw new EmptyStackException("Nothing to redo. Check CanRedo is true before invoking Redo().");
            }

            var momento = _redoStack.Pop();
            SetState(momento);
            _undoStack.Push(_currentState);
            _currentState = momento;
        }

        public virtual void RecordState()
        {
            GetState(out T momento);
            _undoStack.Push(_currentState);
            _currentState = momento;
            _redoStack.Clear();
            StateRecorded?.Invoke(this, new EventArgs());
        }




    }
}
