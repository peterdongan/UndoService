# UndoService
This is a simple undo/redo service based on the momento pattern. It generally doesn't require custom classes or methods, as it uses generic stacks to store state and delegate methods to access it. It can track changes to different parts of an application individually, while using one unified interface for performing undo/redo. This reduces the memory imprint and facilitates modular design. 


## Features
* Multiple undo/redo stacks can be used concurrently, reducing memory imprint.
* Undo/Redo can be performed interchangeably on the state of the application whole or on specific parts.
* No requirement to write custom classes or methods as it uses generic stacks and delegate methods.
* Optional cap on the size of Undo stacks.
* Recorded states can be tagged. Tag values are present in the arguments of the StateSet event, which is raised on Undo or Redo.


## Usage
The simplest approach is to use a single UndoService for application state. Alternatively you can use separate UndoServices for different sections in conjunction with an UndoServiceAggregate. This means that the whole of the application state does not need to be recorded on each change.

To create an UndoService, pass the delegate methods that are used to get and set the state. To use it, invoke RecordState() **after** making changes to the state. (Note that the initial state is recorded automatically when the UndoService is initialized.) Use CanUndo and CanRedo to enable/disable Undo/Redo commands.

### Simple UndoService Example

```csharp

        // We will demonstrate change tracking on this string. 
        private string _statefulString;     
        
        // This is the method to get the state of the tracked object.
        // (If you have an existing method, you can just put it in a wrapper to match the delegate signature.)
        private void GetStringState(out string state)
        {
            state = _statefulString;
        }

        // Method to set the state.
        private void SetStringState(string value)
        {
            _statefulString = value;
        }

        public void UndoRedoTest()
        {
            var undoServiceForString = new UndoService<string>(GetStringState, SetStringState, null);

            _statefulString = "One";
            undoServiceForString.RecordState();
            _statefulString = "Two";
            undoServiceForString.RecordState();

            undoServiceForString.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));

            undoServiceForString.Redo();
            Assert.IsTrue(_statefulString.Equals("Two"));
        }
```

To create an UndoServiceAggregate, pass it an IUndoService array. To use it, invoke RecordState() in the child UndoServices to record changes. Generally undo and redo would be done via the UndoServiceAggregate. However, you can also do so in the child UndoServices directly to undo the last changes to specific elements.

### Simple UndoServiceAggregate Example

```csharp

        // We will use an UndoService to track changes to this
        private string _statefulString;     

        // We will use a second UndoService to track changes to this.
        private int _statefulInt;

        public void AggregateUndoServiceUndoRedoTest()
        {
            // Create the UndoServiceAggregate by passing an IUndoService array
            var undoServiceForInt = new UndoService<int>(GetIntState, SetIntState, null);
            var undoServiceForString = new UndoService<string>(GetStringState, SetStringState, null);
            IUndoService[] subservices = { undoServiceForInt, undoServiceForString };
            var serviceAggregate = new UndoServiceAggregate(subservices);


           // Use the Undo services to track changes to their associated objects.
            _statefulInt = 1;
            undoServiceForInt.RecordState();
            _statefulString = "One";
            undoServiceForString.RecordState();
            _statefulInt = 2;
            undoServiceForInt.RecordState();
            _statefulInt = 3;
            undoServiceForInt.RecordState();
            _statefulString = "Two";
            undoServiceForString.RecordState();


            // Use the Service Aggregate to undo/redo the most recent changes.
            serviceAggregate.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 3);

            serviceAggregate.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 2);

            serviceAggregate.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 1);

            serviceAggregate.Redo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 2);

            serviceAggregate.Redo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 3);

            serviceAggregate.Redo();
            Assert.IsTrue(_statefulString.Equals("Two"));
            Assert.IsTrue(_statefulInt == 3);
        }
        
        
        // These are the methods to get/access state, used by the UndoServices above.

        private void GetStringState(out string state)
        {
            state = _statefulString;
        }

        private void SetStringState(string value)
        {
            _statefulString = value;
        }

        private void GetIntState(out int state)
        {
            state = _statefulInt;
        }

        private void SetIntState(int value)
        {
            _statefulInt = value;
        }
```

Refer to the unit test project in the source repository for examples of other features.

### Checklist
If you run into problems, check the following:

* Invoke RecordState() after making changes, rather than before.
* Don't invoke RecordState() in the delegate method that sets the state. Otherwise Redo() will add an extra state. (For example, don't call RecordState() a property's set accessor, and then use that set accessor in the SetData delegate method.)
* Don't clear the stacks directly in an UndoService that is part of an UndoServiceAggregate. This can result in an exception trying to Undo/Redo.

## Public Interfaces
* IStateTracker is used to record changes to state. It is implemented by UndoService.
* IUndoRedo is used to execute Undo and Redo operations. It is implemented by UndoService and UndoServiceAggregate.
* IUndoService is used to both record state and perform undo/redo. It is implemented by UndoService.

### IStateTracker
```csharp
    // Tracks changes to a part of the application for Undo/Redo. Used in conjunction with IUndoRedo
    public interface IStateTracker
    {
        // Raised when Undo or Redo is executed.
        event StateSetEventHandler StateSet;

        // Raised when RecordState() is executed.
        event StateRecordedEventHandler StateRecorded;

        // Records the current state of the tracked objects and puts it on the undo stack
        // If you assign a tag value, it will be in the StateSet event arguments if this state is restored.
        void RecordState(object tag = null);
    }
```

### IUndoRedo
```csharp

    // Performs Undo/redo actions. Used in conjunction with object(s) that implement IStateTracker
    public interface IUndoRedo
    {
        bool CanUndo { get; }
        
        bool CanRedo { get; }

        // Clear the Undo and Redo stacks.
        void ClearStacks();

        // Clear the Undo stack (but not the redo stack).
        void ClearUndoStack();

        void Undo();

        void Redo();
    }
```


## Links
* [Home](https://peterdongan.github.io/UndoService/)
* [Source Repository](https://github.com/peterdongan/UndoService)
* [Nuget Package](https://www.nuget.org/packages/UndoService)

***
Copyright 2020 Peter Dongan. Licence: [MIT](https://licenses.nuget.org/MIT)
