// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    internal interface ISubUndoService : IUndoService
    {
          int Id { get; set; }
    }
}
