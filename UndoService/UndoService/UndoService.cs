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
    public class UndoService<T> : IUndoService, IStateTracker, IUndoRedo
    {
        private readonly GetState<T> GetState;
        private readonly SetState<T> SetState;
        private readonly IStack<StateRecord<T>> _undoStack;
        private readonly Stack<StateRecord<T>> _redoStack;    //limited by undo stack capacity already
        private readonly UndoServiceValidator<StateRecord<T>> _undoServiceValidator;

        private StateRecord<T> _currentState;

        public UndoService(GetState<T> getState, SetState<T> setState, int? cap)
        {
            GetState = getState ?? throw new ArgumentNullException(nameof(getState));
            SetState = setState ?? throw new ArgumentNullException(nameof(setState));
            var stackFactory = new StackFactory<StateRecord<T>>();
            _undoStack = stackFactory.MakeStack(cap);
           // T currentState;
            GetState(out T currentState);
            _currentState = new StateRecord<T> { State = currentState };
            _redoStack = new Stack<StateRecord<T>>();
            _undoServiceValidator = new UndoServiceValidator<StateRecord<T>>(_undoStack, _redoStack);
        }

        public event StateRecordedEventHandler StateRecorded;
        public event StateSetEventHandler StateSet;

        public bool CanUndo => _undoServiceValidator.CanUndo;

        public bool CanRedo => _undoServiceValidator.CanRedo;

        public void ClearStacks()
        {
            _undoStack.Clear();
            GetState(out T currentState);
            _currentState = new StateRecord<T> { State = currentState };
            _redoStack.Clear();
        }

        public void ClearUndoStack()
        {
            _undoStack.Clear();
        }

        public void Undo()
        {
            _undoServiceValidator.ValidateUndo();

            //If tagging is used, the part of the state that will be changed by this will be tagged in _currentState (the change there will be undone).
            var args = new StateSetEventArgs { Tag = _currentState.Tag, SettingAction = StateSetAction.Undo };

            var momento = _undoStack.Pop();
            SetState(momento.State);
            _redoStack.Push(_currentState);
            _currentState = momento;

            StateSet?.Invoke(this, args);
        }

        public void Redo()
        {
            _undoServiceValidator.ValidateRedo();

            var momento = _redoStack.Pop();
            SetState(momento.State);
            _undoStack.Push(_currentState);
            _currentState = momento;

            //If tagging is used, the part of the state that will be changed by this will be tagged in momento (the change there will be applied).
            var args = new StateSetEventArgs { Tag = momento.Tag, SettingAction = StateSetAction.Redo };

            StateSet?.Invoke(this, args);
        }


        public void RecordState(object tag = null)
        {
            GetState(out T momento);
            _undoStack.Push(_currentState);
            _currentState = new StateRecord<T> { State = momento, Tag = tag };
            _redoStack.Clear();
            StateRecorded?.Invoke(this, new EventArgs());
        }
    }
}
