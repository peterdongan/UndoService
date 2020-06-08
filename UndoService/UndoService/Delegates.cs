// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;

namespace StateManagement
{
    /// <summary>
    /// Gets the state of an object, to allow reversion later via undo/redo
    /// </summary>
    /// <typeparam name="T">The type used to record the object state</typeparam>
    /// <param name="state"></param>
    public delegate void GetState<T>(out T state);
    /// <summary>
    /// Sets the state of an object. Can be invoked via Undo/Redo
    /// </summary>
    /// <typeparam name="T">The type used to record the object state</typeparam>
    /// <param name="state"></param>
    public delegate void SetState<T>(T state);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void StateRecordedEventHandler(object sender, EventArgs e);
    /// <summary>
    /// Handler for StateSet event, which is raised when Undo() or Redo() is executed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">StateSetEventArgs. This indicates whether it was a Redo or Undo action that raised the event, and the value of the tag if one given to the recorded state</param>
    public delegate void StateSetEventHandler(object sender, StateSetEventArgs e);
}
