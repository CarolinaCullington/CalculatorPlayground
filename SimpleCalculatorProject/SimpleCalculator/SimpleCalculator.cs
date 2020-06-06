using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    public class SimpleCalculator
    {
        /*  
         * To add a new operator such as 'divide' add it to the 'operators' array then add a new 
         * dictionary element to the CalculateTwoDecimals() method to cater for a new operation.
         * 
         * To add a new program command such as 'share' or 'send', add it to the 'commands' array
         * and provide a new method to cater for a new type of command.
         */

        // Static collections used for storage while the program is running
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
                string input;

                input = Console.ReadLine();                

                if (input.Length > 0)
                {
                    if (input == "quit")
                    {
                        quit = true;
                        break;
                    }

                    string[] splitInput = ToTidyStringArray(input);

                    // If there's only one string, it's a filename 
                    if (splitInput.Length == 1)
                    {                        
                        string fileName = splitInput[0];
                        quit = ProcessFileName(fileName);      
                        //if (quit)
                        //{
                        //    break; // not sure this is necessary...
                        //}
                    }   
                    else // If there are more than one string in the array, it's a manual input
                    {
                        DoWork(input);
                    }
                }
                else 
                {
                    Console.WriteLine("Please enter either: <register><operation><value>, print <register>, or quit.");
                }
            }
        }

        private bool ProcessFileName(string fileName)
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

        // Separate out the individual strings from the input based on spaces between them, removing any trailing or leading spaces and convert to lower case
        private string[] ToTidyStringArray(string input)
        {
            input = input.Trim().ToLower();
            string[] split = input.Split(' ');
            return split;
        }

        private void DoWork(string input)
        {
            string[] splitInput = ToTidyStringArray(input);

            // If the input string is two, the first should be a command
            if (splitInput.Length == 2) 
            {
                string command = splitInput[0];
                string register = splitInput[1];

                if (commands.Contains(command))  
                {
                    if (command == "print")
                    {
                        if (allRegisters.Exists(r => r.Name == register))
                        {
                            PrintAnswerToConsole(allRegisters.Where(r => r.Name == register).First());
                        }
                        else
                        {
                            Console.WriteLine($"\"{register}\" is not a valid register name.");
                        }
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
            else if (splitInput.Length == 3) // If the input string is three strings, we hope it's the required <register><operation><value>
            {
                string registerOne = splitInput[0];
                string operation = splitInput[1];
                string valueOrRegisterTwo = splitInput[2];

                if (operators.Contains(operation))
                {
                    // If the third input is a number, it will parse and we can use it as inputValue.
                    // If not, it's an alphanumeric and is referring to an existing or new register
                    decimal inputValue = 0;
                    bool inputIsNumber = Decimal.TryParse(valueOrRegisterTwo, out inputValue);

                    // Create the initial register to apply the operation to
                    Register regOne = AddRegister(registerOne);

                    // If the input 'value' is a register, store the request in the list of saved commands for lazy evaluation at print
                    // if not, do the calculation now and update the value of the initial register
                    if (!inputIsNumber)
                    {
                        Register regTwo = AddRegister(valueOrRegisterTwo);
                        SaveInput(regOne, operation, regTwo);
                    }
                    else
                    {
                        regOne.Value = CalculateTwoDecimals(operation, regOne.Value, inputValue);
                    }
                }
                else
                {
                    OutputErrorMessage(operation);
                }
            }
            else
            {
                // An input format not known to the application such as a length of four strings
                OutputErrorMessage(input); 
            }
        }

        // This method takes a string '@operator' and two decimals, performs the matching dictionary operation and returns the decimal result
        // The method is 'public' in order to run the unit tests. I feel there may be a better way for testing and/or disucssion on encapsulation
        public decimal CalculateTwoDecimals(string @operator, decimal firstValue, decimal secondValue)
        {
            // To add further functionality, add a new dictionary item of 'operation' and the corresponding function
            var operations = 
                new Dictionary<string, Func<decimal, decimal, decimal>>
                            {
                                {"add", (x, y) => x + y },
                                {"subtract", (x, y) => x - y },
                                {"multiply", (x, y) => x * y }
                            };

            try
            {
                return operations[@operator](firstValue, secondValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error attempting the calculation: {ex.Message}. \nZero has been returned as the value.");
                return 0;
            }
        }

        private Register AddRegister(string registerName)  // TODO: this method is doing two things, split it up
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

        // Keep a record of all the inputs where they were two registers, in order to process at print time
        private void SaveInput(Register regOne, string @operator, Register regTwo)
        {
            var saved = new SavedInput();
            saved.RegisterOne = regOne;
            saved.Operator = @operator;
            saved.RegisterTwo = regTwo;
            savedInputs.Add(saved);
        }

        private decimal GetFinalResult(Register registerToEvaluate)
        {
            // Process the saved commands OTHER THAN the one we want to print, so that all sub-calculations are done before the final one
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

            // Now run through again, looking only for the final register to determine its final value
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

        // Output the final result to the console
        private void PrintAnswerToConsole(Register regToPrint) => Console.WriteLine(GetFinalResult(regToPrint).ToString());
    }
}
