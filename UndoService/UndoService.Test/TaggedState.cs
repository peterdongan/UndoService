using StateManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace UndoService.Test
{
    class TaggedState 
    {
        public object Tag { get; set; }
        public string TheString { get; set; }
        public int TheInt { get; set; }
    }
}
