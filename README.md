UndoService
========
  
Generic undo service using delegates to access state.  
  
[https://github.com/peterdongan/UndoService](https://github.com/peterdongan/UndoService)  
  
   
## Usage ##
  
_//The constructor takes delegate methods to get/set state, and an optional cap on the number of states stored._  
UndoService(GetState<T> getState, SetState<T> setState, int? cap)	  
  
_//Check these before allowing Undo/Redo operations to be performed._  
bool CanUndo  
bool CanRedo  
  
void RecordState()  _//Invoke this whenever you want to add an item to the Undo history._  
void Undo()   
void Redo()  
void Clear()  
  
  
## LICENCE ##
  
MIT
