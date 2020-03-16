// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. See LICENCE file in the project root for full licence information.

namespace UndoService
{
    public delegate void GetState<T>(out T state);
    public delegate void SetState<T>(T state);
}
