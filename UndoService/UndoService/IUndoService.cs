// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. See LICENCE file in the project root for full licence information.

namespace UndoService
{
    public interface IUndoService
    {
        bool CanUndo { get; }

        bool CanRedo { get; }

        void RecordState();

        void ClearStacks();

        void Undo();

        void Redo();
    }
}
