using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    class SavedCommand
    {
        /*
         * This class provides a space to store registers and the operations to be done to them
         * in order to be saved and calculated at print time (lazy)         
         */

        public Register RegOne { get; set; }

        public string Operation { get; set; }

        public Register RegTwo { get; set; }
    }
}
