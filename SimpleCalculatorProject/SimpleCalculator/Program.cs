using System;

namespace Calculator
{
    class Program
    {
        /* 
         *  A simple calculator program.  Please read the included READEME.txt file for instructions for installation and operation.
         *  Futher comments are available within the SimpleCalculator class
         */        

        static void Main(string[] args)
        {
            Console.WriteLine("Simple Calculator!\r");
            Console.WriteLine("Please enter either: <register><operation><value>, print <register>, or quit.\n");

            SimpleCalculator calc = new SimpleCalculator();
            calc.RunCalculator();

            Console.WriteLine("Press enter to close this window...");
            Console.ReadLine();
        }
        
    }
}
