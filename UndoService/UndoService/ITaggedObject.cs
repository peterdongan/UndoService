// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    public class StateRecord<T>
    {
        public object Tag { get; set; }
        public T State { get; set; }
    }
}
