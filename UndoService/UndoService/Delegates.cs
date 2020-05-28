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
    public delegate void StateRecordedEventHandler(object sender, EventArgs e);
    public delegate void StateSetEventHandler(object sender, StateSetEventArgs e);
}
