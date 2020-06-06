using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    class SavedInput
    {
        /*
         * This class provides a space to store registers and the operations to be done to them
         * in order to be saved and calculated at print time (lazy)         
         */

        public Register RegisterOne { get; set; }

        public string Operator { get; set; }

        public Register RegisterTwo { get; set; }
    }
}
