# This project is intended to provide a way to use UndoService with UWP controls that have their own undo/redo features.

## Requirements:

* Block Undo() and Redo() from being performed directly on the control. This behaviour is not compatible with AggregateUndoService. [It would be possible to handle it in AggregateUndoService, but it would mean inconsistent behaviour of Undo(), which would be confusing and AFAIK unconventional.]
* Optionally provide an implementation of StateManagement.IUndoService on the targetted control.

## Usage

If you are using a single UndoService to record application state, then use a TextBoxUndoBlocker on a textbox to block its own undo features from working.

If you are using an AggregateUndoService to record application state, then use a TextBoxUndoServiceWrapper on a textbox, which implements IUndoService for the textbox to be compatible with the AggregateService.

## Discussion 

If it is possible to block the native Undo() and Redo() operations of a control being accessible to the user then it would be possible to provide a wrapper to expose that via the IUndoService interface rather than to replace it. This might be neater but also might lead to issues. One thing that might be problematic with that approach would be the need to synchronise the StateRecorded event with the changes as they are recorded by the control's undo feature.

The wrapper approach used in this project strikes me as safer than writing custom versions of the controls, and doesn't require you to replace your framework controls. I think of it as being safer because it should remove the possibility of the hidden Undo/Redo methods being called directly.

There are potential usability issues with undoing changes to a textbox that is not currently visible - for example on a different tab. A way of avoiding that would be to provide events to Undo and Redo that could be used to display the controls concerned.