# UndoService
This is a simple undo/redo service based on the momento pattern. There is not a requirement to write custom classes or methods, as it uses generic stacks to store state and delegate methods to access it. It can track changes to different parts of an application individually, while using one unified interface for performing undo/redo. 


## Features
* Multiple undo/redo stacks can be used concurrently, reducing memory imprint.
* Undo/Redo can be performed interchangeably on the state of the application whole or on specific parts.
* No requirement to write custom classes or methods as it uses generic stacks and delegate methods.
* Optional cap on the size of Undo stacks.
* Recorded states can be tagged. Tag values are present in the arguments of the StateSet event, which is raised on Undo or Redo.


## Usage
The simplest approach is to use a single UndoService for application state. Alternatively you can use separate UndoServices for different sections in conjunction with an UndoServiceAggregate. This means that the whole of the application state does not need to be recorded on each change.

To create an UndoService, pass the delegate methods that are used to get and set the state. To use it, invoke RecordState() **after** making changes to the state. (Note that the initial state is recorded automatically when the UndoService is initialized.) Use CanUndo and CanRedo to enable/disable Undo/Redo commands.

### Simple Undo Service Example

```csharp
    public class SimpleUndoServiceExample
    {
        /// <summary>
        /// We will demonstrate change tracking on this string. (You can track more complex objects, as long as you have methods to get and set their state.)
        /// </summary>
        private string _statefulString;     // (You can track changes of more complex objects.)

        /// <summary>
        /// This is the method to get the state of the tracked object, which will be passed as a delegate to the UndoService.
        /// If you have existing methods to access state, you can probably just put them in a wrapper to match the delegate signature.
        /// </summary>
        private void GetStringState(out string state)
        {
            state = _statefulString;
        }

        /// <summary>
        /// Method to set the state, conforming to the delegate signature.
        /// </summary>
        private void SetStringState(string value)
        {
            _statefulString = value;
        }

        [Test]
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
    }
```

To create an UndoServiceAggregate, pass a collection of UndoServices. To use it, invoke RecordState() in the child UndoServices to record changes. Generally undo and redo would be done via the UndoServiceAggregate. However, you can also do so in the child UndoServices directly to undo the last changes to specific elements.

### Simple Undo Service Aggregate Example

```csharp
public class SimpleUndoServiceAggregateExample
    {
        /// <summary>
        /// This will have an UndoService tracking its changes
        /// </summary>
        private string _statefulString;     

        /// <summary>
        /// This will have an UndoService tracking its changes
        /// </summary>
        private int _statefulInt;

        /// <summary>
        /// Test undo/redo in an AggregateUndoService
        /// </summary>
        [Test]
        public void AggregateUndoServiceUndoRedoTest()
        {
            // UndoServiceAggregate is created using an IUndoService array:
            var undoServiceForInt = new UndoService<int>(GetIntState, SetIntState, null);
            var undoServiceForString = new UndoService<string>(GetStringState, SetStringState, null);
            IUndoService[] subservices = { undoServiceForInt, undoServiceForString };
            var serviceAggregate = new UndoServiceAggregate(subservices);


           // Changes are recorded by the individual UndoServices
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


           /*
            * The UndoServiceAggregate provides a unified interface for performing undo/redo on the different tracked objects.
            * (You can also perform Undo/Redo on the individual services, which will undo the last change on the corresponding object.)
            */
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
    }
```

## Public Interfaces
* IStateTracker is used to record changes to state. It is implemented by UndoService.
* IUndoRedo is used to execute Undo and Redo operations. It is implemented by UndoService and UndoServiceAggregate.
* IUndoService is used to both record state and perform undo/redo. It is implemented by UndoService.

### IStateTracker
```csharp
    /// <summary>
    /// Tracks changes to a part of the application for Undo/Redo. Used in conjunction with IUndoRedo
    /// </summary>
    public interface IStateTracker
    {
        /// <summary>
        /// Raised when Undo or Redo is executed.
        /// </summary>
        event StateSetEventHandler StateSet;

        /// <summary>
        /// Raised when RecordState() is executed.
        /// </summary>
        event StateRecordedEventHandler StateRecorded;

        /// <summary>
        /// Records the current state of the tracked objects and puts it on the undo stack
        /// </summary>
        /// <param name="tag">When the tracked object is reverted to this state, a StateSet event will be thrown with this as a property in its arguments. </param>
        void RecordState(object tag = null);
    }
```

### IUndoRedo
```csharp
    /// <summary>
    /// Performs Undo/redo actions. Used in conjunction with object(s) that implement IStateTracker
    /// </summary>
    public interface IUndoRedo
    {
        bool CanUndo { get; }
        
        bool CanRedo { get; }

        /// <summary>
        /// Clear the Undo and Redo stacks.
        /// </summary>
        void ClearStacks();

        /// <summary>
        /// Clear the Undo stack (but not the redo stack).
        /// </summary>
        void ClearUndoStack();

        void Undo();

        void Redo();
    }
```

Refer to the unit test project in the source repository for further examples.

## Links
* [Home](https://peterdongan.github.io/UndoService/)
* [Source Repository](https://github.com/peterdongan/UndoService)
* [Nuget Package](https://www.nuget.org/packages/UndoService)

***
Copyright 2020 Peter Dongan. Licence: [MIT](https://licenses.nuget.org/MIT)
