// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace UndoService
{
    public delegate void GetState<T>(out T state);
    public delegate void SetState<T>(T state);
}
