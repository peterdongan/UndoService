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
    /// Blocks the undo/redo feature of a textbox from working. This is to allow it to be handled by our custom UndoService.
    /// </summary>
    public class TextBoxUndoBlocker
    {
        private readonly TextBox _textBox;

        public TextBoxUndoBlocker(TextBox textBox)
        {
            _textBox = textBox;
            _textBox.TextChanged += _textBox_TextChanged;
        }

        private void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _textBox.ClearUndoRedoHistory();
        }
    }
}
