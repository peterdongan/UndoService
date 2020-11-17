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
        private readonly IStack<StateRecord<T>> _redoStack;    //limited by undo stack capacity already
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
            _redoStack = new StandardStack<StateRecord<T>>();
            _undoServiceValidator = new UndoServiceValidator<StateRecord<T>>(_undoStack, _redoStack);
            _undoStack.HasItemsChanged += UndoStack_HasItemsChanged;
            _redoStack.HasItemsChanged += RedoStack_HasItemsChanged;
        }

        private void RedoStack_HasItemsChanged(object sender, EventArgs e)
        {
            CanRedoChanged?.Invoke(this, new EventArgs());
        }

        private void UndoStack_HasItemsChanged(object sender, EventArgs e)
        {
            CanUndoChanged?.Invoke(this, new EventArgs());
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
        /// Raised when CanUndo changes.
        /// </summary>
        public event CanUndoChangedEventHandler CanUndoChanged;

        /// <summary>
        /// Raised when CanRedo changes.
        /// </summary>
        public event CanRedoChangedEventHandler CanRedoChanged;

        /// <summary>
        /// Raised when one or both of the undo and redo stacks is cleared.
        /// </summary>
        public event StackClearInvokedEventHandler ClearStackInvoked;

        /// <summary>
        /// 
        /// </summary>
        public bool CanUndo => _undoServiceValidator.CanUndo;

        /// <summary>
        /// Indicates whether the state was changed from its original state or the last time ClearIsStateChangedFlag was invoked.
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
        /// Clears the undo and redo stacks, clears IsStateChanged flag, considers the current state to be the original state.
        /// </summary>
        public void Reset()
        {
            _undoStack.Clear();

            GetState(out T currentState);
            _currentState = new StateRecord<T> { State = currentState };

            _redoStack.Clear();
            ClearIsStateChangedFlag();

            ClearStackInvoked?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Clears the Undo and Redo stacks.
        /// </summary>
        [Obsolete("ClearStacks() is deprecated. Please use Reset() instead.")]
        public void ClearStacks()
        {
            _undoStack.Clear();

            GetState(out T currentState);
            _currentState = new StateRecord<T> { State = currentState };

            _redoStack.Clear();

            ClearStackInvoked?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Clear the Undo stack (but not the Redo stack).
        /// </summary>
        public void ClearUndoStack()
        {
            _undoStack.Clear();

            ClearStackInvoked?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Clear the Redo stack (but not the Undo stack).
        /// </summary>
        public void ClearRedoStack()
        {
            _redoStack.Clear();

            ClearStackInvoked?.Invoke(this, new EventArgs());
        }


        /// <summary>
        /// 
        /// </summary>
        public void ClearIsStateChangedFlag()
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

            var memento = _undoStack.Pop();
            SetState(memento.State);
            _redoStack.Push(_currentState);
            _currentState = memento;
            StateSet?.Invoke(this, args);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Redo()
        {
            _undoServiceValidator.ValidateRedo();

            var memento = _redoStack.Pop();
            SetState(memento.State);
            _undoStack.Push(_currentState);
            _currentState = memento;

            //If tagging is used, the part of the state that will be changed by this will be tagged in memento (the change there will be applied).
            var args = new StateSetEventArgs { Tag = memento.Tag, SettingAction = StateSetAction.Redo };

            StateSet?.Invoke(this, args);
        }

        /// <summary>
        /// Record the current state of the tracked object and add it to the Undo stack.
        /// </summary>
        /// <param name="tag">A tag associated with the recorded state</param>
        public void RecordState(object tag = null)
        {
            GetState(out T memento);
            _undoStack.Push(_currentState);
            _currentState = new StateRecord<T> { State = memento, Tag = tag };
            StateRecorded?.Invoke(this, new EventArgs());

            if(_redoStack.Count > 0)
            {
                _redoStack.Clear();
            }

        }
    }
}
