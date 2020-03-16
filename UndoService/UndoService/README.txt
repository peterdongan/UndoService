Generic undo service using delegates to access state.


PROJECT: https://github.com/peterdongan/UndoService


USAGE:

UndoService(GetState<T> getState, SetState<T> setState, int? cap)

bool CanUndo
bool CanRedo

void RecordState()  //adds an item to the history.
void Undo()
void Redo()
void Clear()


LICENCE: MIT



