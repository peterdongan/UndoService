using System;
using System.Collections.Generic;
using System.Text;

namespace UndoService.Test
{
    class StatefulClass
    {
        public int TheInt { get; set; }
        public string TheString { get; set; }

        public void GetData(out StatefulClassDto dto)
        {
            dto = new StatefulClassDto { TheInt = TheInt, TheString = TheString };
        }

        public void SetData(StatefulClassDto dto)
        {
            TheInt = dto.TheInt;
            TheString = dto.TheString;
        }
    }
}
