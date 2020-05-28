// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;
using System.Collections.Generic;
using System.Text;

namespace StateManagement
{
    public class StateSetEventArgs : EventArgs
    {
        /// <summary>
        /// If the state object implements TaggedObject, then this will be its Tag. If it does not, then this will be null.
        /// </summary>
        public object Tag { get; set; }

        public StateSetAction SettingAction {get; set;}
        
    }
}
