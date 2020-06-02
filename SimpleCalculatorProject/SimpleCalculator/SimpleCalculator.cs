using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    public class SimpleCalculator
    {
        /*  
         * To extend funtionality of the program, please first enter new operation and command names into the string arrays 'operations'
         * and 'commands'.  Then add a new dictionary element to the CalculateTwoDecimals() method to cater for a new operation, or
         * provide a new method to cater for a new type of command e.g. 'share' or 'send'.
         */

        // Static collections used for storage while the program is running
        private static List<Register> registers;
        private static List<SavedCommand> savedCommands;

        // These two string arrays hold the strings that reflect the operation and command required inputs to the Simple Calculator.        
        private string[] operations;
        private string[] commands;

        public void RunCalculator()
        {    
            // Initialise the list of registers running in memory while the program is running.
            registers = new List<Register>();
            savedCommands = new List<SavedCommand>();

            // Initialise the operations and commands available.
            operations = new string[] { "add", "subtract", "multiply" };
            commands = new string[] { "print" };

            bool quit = false;            

            while (!quit)
            {
                string input = "";

                input = Console.ReadLine();                

                if (input.Length > 0)
                {
                    if (input == "quit")
                    {
                        quit = true;
                        break;
                    }

                    string[] splitInput = ToTidyStringArray(input);

                    // If there's only one string, it should be a filename 
                    if (splitInput.Length == 1)
                    {                        
                        string fileName = splitInput[0];

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

                            quit = true;
                            break;
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine($"There was an error reading that file: {ex.Message}");
                        }
                    }   
                    else // If there are more than one string in the array, it's a manual input
                    {
                        DoWork(input);
                    }
                }
                else // If there was no input string
                {
                    Console.WriteLine("Please enter either: <register><operation><value>, print <register>, or quit.");
                }
            }
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

                // If we have any further commands in the future, this block can be extended
                if (commands.Contains(command))  
                {
                    if (command == "print")
                    {
                        // If the register exists, call the PrintAnswerToConsole method
                        // if not, log it to the console.
                        if (registers.Exists(r => r.Name == register))
                        {
                            PrintAnswerToConsole(registers.Where(r => r.Name == register).First());
                        }
                        else
                        {
                            Console.WriteLine($"\"{register}\" is not a valid register name.");
                        }
                    }
                    else 
                    {
                        // If the command exists, but no functionality is yet provided
                        Console.WriteLine($"No functionality yet provided for the existing \"{command}\" command.");
                    }
                }
                else
                {
                    // If the command does not exist, log an error to the console
                    OutputErrorMessage(command);
                }
            }
            else if (splitInput.Length == 3) // If the input string is three strings, we hope it's the required <register><operation><value>
            {
                string registerOne = splitInput[0];
                string operation = splitInput[1];
                string valueOrRegisterTwo = splitInput[2];

                if (operations.Contains(operation))
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
                        SaveCommand(regOne, operation, regTwo);
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

        // This method takes a string 'operation' and two decimals, performs the matching dictionary operation and returns the decimal result
        // The method is 'public' in order to run the unit tests. I feel there may be a better way for testing and/or disucssion on encapsulation
        public decimal CalculateTwoDecimals(string operation, decimal a, decimal b)
        {
            // To add further functionality, add a new dictionary item of 'operation' and the corresponding function
            var operations = new Dictionary<string, Func<decimal, decimal, decimal>>
                            {
                                {"add", (x, y) => x + y },
                                {"subtract", (x, y) => x - y },
                                {"multiply", (x, y) => x * y }
                            };

            // To protect against mathematical e.g. divide by zero or overflow errors, we'll put this into a try/catch block
            try
            {
                return operations[operation](a, b);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error attempting the calculation: {ex.Message}. \nZero has been returned as the value.");
                return 0;
            }
        }

        // checks for the existence of the register in the static collection, add it if not there and return it.
        private Register AddRegister(string regName)
        {
            Register reg = new Register(regName);

            if (registers.Exists(r => r.Name == regName))
            {
                reg = registers.Where(r => r.Name == regName).First();
            }
            else
            {
                registers.Add(reg); 
            }

            return reg;
        }

        // Keep a record of all the commands where they were two registers, in order to process at print time
        private void SaveCommand(Register regOne, string operation, Register regTwo)
        {
            var saved = new SavedCommand();
            saved.RegOne = regOne;
            saved.Operation = operation;
            saved.RegTwo = regTwo;
            savedCommands.Add(saved);
        }

        private decimal GetFinalResult(Register regToEvaluate)
        {
            // Process the saved commands OTHER THAN the one we want to print, so that all sub-calculations are done before the final one
            foreach (var item in savedCommands)
            {
                if (item.RegOne.Name != regToEvaluate.Name)
                {
                    decimal value1 = item.RegTwo.Value;
                    item.RegOne.Value = CalculateTwoDecimals(item.Operation, item.RegOne.Value, value1);
                }
            }

            // Now run through again, looking only for the final register to determine its final value
            foreach (var item in savedCommands)
            {
                if (item.RegOne.Name == regToEvaluate.Name)
                {
                    decimal value1 = item.RegTwo.Value;
                    item.RegOne.Value = CalculateTwoDecimals(item.Operation, item.RegOne.Value, value1);
                    regToEvaluate.Value = item.RegOne.Value;
                }
            }

            return regToEvaluate.Value;
        }

        private void OutputErrorMessage(string input) => Console.WriteLine($"The command \"{input}\" is an invalid command.\r");

        // Output the final result to the console
        private void PrintAnswerToConsole(Register regToPrint) => Console.WriteLine(GetFinalResult(regToPrint).ToString());
    }
}
