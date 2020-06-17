using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    public class SimpleCalculator
    {
        /*  
         *  The Simple Calculator takes the following as inputs:
         *      1. <register> <operation> <value> - where operations are either 'add', 'subtract' and 'multiply'
         *      2. <command> <register> - initially only 'print'
         *      3. 'quit'
         *      4. A file location containing the above commands.
         *  
         *  If the final <value> can be a number or a <register>, if it is a <register>, the line is saved to
         *  savedInputs in the saveInput() method to be evaluated at print time (lazy evaluation)
         *  
         *  Lists of all <registers> and saved inputs are placed in static collections to be accessed at any point
         *  in the program.
         *      
         *  To add a new operator such as 'divide' add it to the 'operators' array then add a new 
         *  dictionary element to the CalculateTwoDecimals() method to cater for a new operation.
         * 
         *  To add a new program command such as 'share' or 'send', add it to the 'commands' array
         *  and provide a new method to cater for a new type of command.
         */

        private static List<Register> allRegisters;
        private static List<SavedInput> savedInputs;

        private string[] operators;
        private string[] commands;

        public void RunCalculator()
        {    
            allRegisters = new List<Register>();
            savedInputs = new List<SavedInput>();
            operators = new string[] { "add", "subtract", "multiply" };
            commands = new string[] { "print" };

            bool quit = false;            
            while (!quit)
            {
                quit = ProcessInput();                
            }
        }

        /*  Inputs vary in length:
         *      One string - it's a filename or it's 'quit'
         *      More than one string - it's a manual input      
         */
        private bool ProcessInput()
        {
            bool quit = false;
            string input;
            input = Console.ReadLine();
            if (input.Length > 0)
            {
                if (input.ToLower().Trim() == "quit")
                {
                    quit = true;
                }
                else
                {
                    quit = ProcessFileOrString(input);                    
                }                
            }
            else
            {
                Console.WriteLine("Please enter either: <register> <operation> <value>, <command> <register>, or quit.\n Alternatively, enter a file location of saved commands.");
            }
            return quit;
        }

        private bool ProcessFileOrString(string input)
        {
            bool finished = false;
            string[] splitInput = ToTidyStringArray(input);
            if (splitInput.Length == 1)
            {
                finished = ProcessFile(splitInput[0]);
            }
            else
            {
                DoWork(input);
            }
            return finished;
        }

        private string[] ToTidyStringArray(string input)
        {
            input = input.Trim().ToLower();
            string[] split = input.Split(' ');
            return split;
        }

        private bool ProcessFile(string fileName)
        {
            bool success = false;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);
                foreach (var line in lines)
                {
                    if (line != "quit")
                    {
                        DoWork(line);
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error reading that file: {ex.Message}");
            }
            return success;
        }

        /*  Valid inputs should be:
         *      length of 2: <command> <register>
         *      length of 3: <register> <operation> <value>
         *  Any longer length is not recognised and will output an error message.
         */
        private void DoWork(string input)
        {
            string[] splitInput = ToTidyStringArray(input);
            if (splitInput.Length == 2) 
            {
                ProcessCommand(splitInput);                
            }
            else if (splitInput.Length == 3) 
            {
                ProcessRegisterOperationValue(splitInput);                
            }
            else
            {
                OutputErrorMessage(input); 
            }
        }

        private void ProcessCommand(string[] splitInput)
        {
            string command = splitInput[0];
            string register = splitInput[1];
            if (commands.Contains(command))
            {
                if (command == "print")
                {
                    PrintAnswerToConsole(register);                    
                }
                else
                {
                    Console.WriteLine($"No functionality yet provided for the existing \"{command}\" command.");
                }
            }
            else
            {
                OutputErrorMessage(command);
            }
        }

        /*  If the input 'value' is a register, store the request in the list of saved commands 
         *  if not, do the calculation now and update the value of the initial register.
         *  TryParse() is used to determine if the value is a number or alphanumeric and therefore a register.
         */
        private void ProcessRegisterOperationValue(string[] splitInput)
        {
            string regOne = splitInput[0];
            string operation = splitInput[1];
            string valueOrRegisterTwo = splitInput[2];
            if (operators.Contains(operation))
            {
                decimal inputValue = 0;
                bool inputIsNumber = Decimal.TryParse(valueOrRegisterTwo, out inputValue);
                Register registerOne = ReturnRegister(regOne);
                if (!inputIsNumber)
                {
                    Register registerTwo = ReturnRegister(valueOrRegisterTwo);
                    SaveInput(registerOne, operation, registerTwo);
                }
                else
                {
                    registerOne.Value = CalculateTwoDecimals(operation, registerOne.Value, inputValue);
                }
            }
            else
            {
                OutputErrorMessage(operation);
            }
        }

        /*  The method is 'public' in order to run the unit tests. I feel there may be a better way for testing 
         *  and/or disucssion on encapsulation? TODO.
         */
        public decimal CalculateTwoDecimals(string operation, decimal firstValue, decimal secondValue)
        {
            var operations = new Dictionary<string, Func<decimal, decimal, decimal>>
                                    {
                                        {"add", (x, y) => x + y },
                                        {"subtract", (x, y) => x - y },
                                        {"multiply", (x, y) => x * y }
                                    };
            try
            {
                return operations[operation](firstValue, secondValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error attempting the calculation: {ex.Message}. \nZero has been returned as the value.");
                return 0;
            }
        }

        private Register ReturnRegister(string registerName)  
        {
            Register register = new Register(registerName);
            if (allRegisters.Exists(r => r.Name == registerName))
            {
                register = allRegisters.Where(r => r.Name == registerName).First();
            }
            else
            {
                allRegisters.Add(register); 
            }
            return register;
        }

        /*  Keep a record of all the inputs where they were two registers, in order to process at print time
         */
        private void SaveInput(Register regOne, string operation, Register regTwo)
        {
            var saved = new SavedInput();
            saved.RegisterOne = regOne;
            saved.Operator = operation;
            saved.RegisterTwo = regTwo;
            savedInputs.Add(saved);
        }

        /*  A foreach will run over the savedInputs twice in order to:
         *      Process the saved commands OTHER THAN the one we want to print, so that all sub-calculations are done before the final one.
         *      Process all instances of the final registerToEvaluate to determine its final value.
         */
        private decimal GetFinalResult(Register registerToEvaluate)
        {
            foreach (var savedItem in savedInputs)
            {
                if (savedItem.RegisterOne.Name != registerToEvaluate.Name)
                {
                    savedItem.RegisterOne.Value = 
                        CalculateTwoDecimals(
                            savedItem.Operator,
                            savedItem.RegisterOne.Value,
                            savedItem.RegisterTwo.Value
                            );
                }
            }
            foreach (var savedItem in savedInputs)
            {
                if (savedItem.RegisterOne.Name == registerToEvaluate.Name)
                {
                    savedItem.RegisterOne.Value =
                        CalculateTwoDecimals(
                            savedItem.Operator,
                            savedItem.RegisterOne.Value,
                            savedItem.RegisterTwo.Value
                            );
                    registerToEvaluate.Value = savedItem.RegisterOne.Value;
                }
            }
            return registerToEvaluate.Value;
        }

        private void OutputErrorMessage(string input) => Console.WriteLine($"The command \"{input}\" is an invalid command.\r");

        private void PrintAnswerToConsole(string registerName) 
        {
            if (allRegisters.Exists(r => r.Name == registerName))
            {
                Register register = allRegisters.Where(r => r.Name == registerName).First();
                Console.WriteLine(GetFinalResult(register).ToString());
            }
            else
            {
                Console.WriteLine($"\"{registerName}\" is not a valid register name.");
            }            
        }
    }
}
