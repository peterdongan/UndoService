// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;

namespace StateManagement
{
    public delegate void GetState<T>(out T state);
    public delegate void SetState<T>(T state);
    public delegate void StateRecordedEventHandler(object sender, EventArgs e);
    public delegate void StateSetEventHandler(object sender, StateSetEventArgs e);
}
