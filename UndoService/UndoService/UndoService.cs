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
        private T _originalState;

        private StateRecord<T> _currentState;

        /// <summary>
        /// Create an UndoService.
        /// </summary>
        /// <param name="getState">Method to get the state of the tracked object</param>
        /// <param name="setState">Method to set the state of the tracked object</param>
        /// <param name="cap">Capacity of Undo history</param>
        public UndoService(GetState<T> getState, SetState<T> setState, int? cap = null)
        {
            GetState = getState ?? throw new ArgumentNullException(nameof(getState));
            SetState = setState ?? throw new ArgumentNullException(nameof(setState));
            var stackFactory = new StackFactory<StateRecord<T>>();
            _undoStack = stackFactory.MakeStack(cap);
           // T currentState;
            GetState(out T currentState);
            _originalState = currentState;
            _currentState = new StateRecord<T> { State = currentState };
            _redoStack = new Stack<StateRecord<T>>();
            _undoServiceValidator = new UndoServiceValidator<StateRecord<T>>(_undoStack, _redoStack);
        }

        /// <summary>
        /// Raised when RecordState() is executed.
        /// </summary>
        public event StateRecordedEventHandler StateRecorded;

        /// <summary>
        /// Raised when an Undo or Redo is executed.
        /// </summary>
        public event StateSetEventHandler StateSet;

        /// <summary>
        /// 
        /// </summary>
        public bool CanUndo => _undoServiceValidator.CanUndo;

        /// <summary>
        /// Indicates whether the state was changed from its original state or the last time ClearIsChangedFlag was invoked.
        /// </summary>
        public bool IsStateChanged
        {
            get
            {
                if (_currentState.State == null)
                {
                    return (_originalState != null);
                }
                else
                {
                    return (!_currentState.State.Equals(_originalState));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanRedo => _undoServiceValidator.CanRedo;


        /// <summary>
        /// Clear both the Undo and Redo stacks.
        /// </summary>
        public void ClearStacks()
        {
            _undoStack.Clear();
            GetState(out T currentState);
            _currentState = new StateRecord<T> { State = currentState };
            _redoStack.Clear();
        }

        /// <summary>
        /// Clear the Undo stack (but not the Redo stack).
        /// </summary>
        public void ClearUndoStack()
        {
            _undoStack.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearIsChangedFlag()
        {
            _originalState = _currentState.State;
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// Record the current state of the tracked object and add it to the Undo stack.
        /// </summary>
        /// <param name="tag">A tag associated with the recorded state</param>
        public void RecordState(object tag = null)
        {
            GetState(out T momento);
            _undoStack.Push(_currentState);
            _currentState = new StateRecord<T> { State = momento, Tag = tag };
            StateRecorded?.Invoke(this, new EventArgs());

            if(_redoStack.Count > 0)
            {
                _redoStack.Clear();
            }


        }
    }
}
