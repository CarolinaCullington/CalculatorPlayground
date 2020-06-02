using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public class Register
    {
        /*
         * This class provides a model for the type Register and the means to create one
         */

        public string Name { get; set; }

        public decimal Value { get; set; } = 0; 

        public Register() { } 

        public Register(string name)
        {
            Name = name;
        }
    }
}
