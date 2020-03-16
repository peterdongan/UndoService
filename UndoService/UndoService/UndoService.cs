// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;
using System.Collections.Generic;

namespace UndoService
{
    /// <summary>
    /// Generic Undo Service using delegates to access state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UndoService<T> : IUndoService
    {
        private readonly GetState<T> GetState;
        private readonly SetState<T> SetState;
        private readonly IStack<T> _undoStack;
        private readonly Stack<T> _redoStack;    //limited by undo stack capacity already
        
        private T _currentState;

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

        public UndoService(GetState<T> getState, SetState<T> setState, int? cap)
        {
            GetState = getState ?? throw new ArgumentNullException(nameof(getState));
            SetState = setState ?? throw new ArgumentNullException(nameof(setState));
            var stackFactory = new StackFactory<T>();
            _undoStack = stackFactory.MakeStack(cap);
            GetState(out _currentState);
            _redoStack = new Stack<T>();
        }

        public void ClearStacks()
        {
            _undoStack.Clear();
            GetState(out _currentState);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if(!CanUndo)
            {
                throw new Exception("Nothing to undo. Check CanUndo is true before invoking Undo().");
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
                throw new Exception("Nothing to redo. Check CanRedo is true before invoking Redo().");
            }

            var momento = _redoStack.Pop();
            SetState(momento);
            _undoStack.Push(_currentState);
            _currentState = momento;
        }

        public void RecordState()
        {
            GetState(out T momento);
            _undoStack.Push(_currentState);
            _currentState = momento;
            _redoStack.Clear();
        }
    }
}
