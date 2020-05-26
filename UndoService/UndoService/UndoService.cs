// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using StateManagement.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StateManagement
{
    /// <summary>
    /// Generic Undo Service using delegates to access state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UndoService<T> : IUndoService, IStateTracker
    {
        private readonly GetState<T> GetState;
        private readonly SetState<T> SetState;
        private readonly IStack<T> _undoStack;
        private readonly Stack<T> _redoStack;    //limited by undo stack capacity already
        private readonly UndoServiceValidator<T> _undoServiceValidator;

        private T _currentState;

        public UndoService(GetState<T> getState, SetState<T> setState, int? cap)
        {
            GetState = getState ?? throw new ArgumentNullException(nameof(getState));
            SetState = setState ?? throw new ArgumentNullException(nameof(setState));
            var stackFactory = new StackFactory<T>();
            _undoStack = stackFactory.MakeStack(cap);
            GetState(out _currentState);
            _redoStack = new Stack<T>();
            _undoServiceValidator = new UndoServiceValidator<T>(_undoStack, _redoStack);
        }

        public event StateRecordedEventHandler StateRecorded;
        public event StateSetEventHandler StateSet;

        public bool CanUndo => _undoServiceValidator.CanUndo;

        public bool CanRedo => _undoServiceValidator.CanRedo;

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
            _undoServiceValidator.ValidateUndo();

            var momento = _undoStack.Pop();
            SetState(momento);
            _redoStack.Push(_currentState);
            _currentState = momento;
            var args = new StateSetEventArgs();
            if(typeof(T).GetInterfaces().Contains(typeof(ITaggedObject)))
            {
                args.Tag = ((ITaggedObject)momento).Tag;
            }
            StateSet?.Invoke(this, args);
        }

        public void Redo()
        {
            _undoServiceValidator.ValidateRedo();

            var momento = _redoStack.Pop();
            SetState(momento);
            _undoStack.Push(_currentState);
            _currentState = momento;
            var args = new StateSetEventArgs();
            if (typeof(T).GetInterfaces().Contains(typeof(ITaggedObject)))
            {
                args.Tag = ((ITaggedObject)momento).Tag;
            }
            StateSet?.Invoke(this, args);
        }

        public void RecordState()
        {
            GetState(out T momento);
            _undoStack.Push(_currentState);
            _currentState = momento;
            _redoStack.Clear();
            StateRecorded?.Invoke(this, new EventArgs());
        }

    }
}
