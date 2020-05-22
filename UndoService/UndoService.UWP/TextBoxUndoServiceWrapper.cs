// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using StateManagement.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using StateManagement;

namespace StateManagement.UWP
{
    /// <summary>
    /// This blocks the Undo/Redo functionality of a TextBox and adds an implementation of UndoService that is suitable for use in an AggregateUndoService. 
    /// If you are using a single UndoService to record application state, then you should not use this, but use TextBoxUndoBlocker directly on the TextBox.
    /// </summary>
    public class TextBoxUndoServiceWrapper : IUndoService
    { 
        private readonly TextBox _textBox;
        private readonly UndoService<string> _undoService;
        private readonly TextBoxUndoBlocker _textBoxUndoBlocker;

        /// <summary>
        /// If the textbox is changed by the undoservice - used to avoid recording state changes 
        /// </summary>
        private bool _stateSetByUndoServiceFlag;

        public int Id { get => ((IUndoService)_undoService).Id; set => ((IUndoService)_undoService).Id = value; }

        public bool CanUndo => ((IUndoService)_undoService).CanUndo;

        public bool CanRedo => ((IUndoService)_undoService).CanRedo;

        public TextBoxUndoServiceWrapper(TextBox textBox, int? cap)
        {
            _textBox = textBox;
            _stateSetByUndoServiceFlag = false;
            _textBoxUndoBlocker = new TextBoxUndoBlocker(textBox);
            _textBox.TextChanged += _textBox_TextChanged;
            _undoService = new UndoService<string>(GetState, SetState, cap);
        }

        /// <summary>
        /// Record the state when the textbox content changes if it was not changed by the undoservice. This approach is obviously very broken as wedon't want to save the state on every keypress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(_stateSetByUndoServiceFlag)
            {
                _stateSetByUndoServiceFlag = false;
            }
            else
            {
                RecordState();
            }
        }

        public event StateRecordedEventHandler StateRecorded
        {
            add
            {
                ((IUndoService)_undoService).StateRecorded += value;
            }

            remove
            {
                ((IUndoService)_undoService).StateRecorded -= value;
            }
        }

        public event StateSetEventHandler StateSet
        {
            add
            {
                ((IUndoService)_undoService).StateSet += value;
            }

            remove
            {
                ((IUndoService)_undoService).StateSet -= value;
            }
        }  

        private void GetState(out string state)
        {
            state = _textBox.Text;
        }

        private void SetState(string state)
        {
            _textBox.Text = state;
        }

        public void RecordState()
        {
            ((IUndoService)_undoService).RecordState();
        }

        public void ClearStacks()
        {
            ((IUndoService)_undoService).ClearStacks();
        }

        public void ClearUndoStack()
        {
            ((IUndoService)_undoService).ClearUndoStack();
        }

        public void Undo()
        {
            ((IUndoService)_undoService).Undo();
        }

        public void Redo()
        {
            ((IUndoService)_undoService).Redo();
        }
    }
}
