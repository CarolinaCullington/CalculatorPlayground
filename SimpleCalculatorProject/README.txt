****************************************
* Simple Calculator README file *
****************************************

This program has been produced as part of the interview process for the role of Software Engineer at ***.
Completed on 10th May 2020.
All work my own.

---------------------------------------------------------------------------------------------------------------------------

INSTALLATION:
	This application has been created in Visual Studio 2019 with .NET Core 3.1

	To ensure this application does not need any extra files installed to run, I have created an exe file located in
	the /SimpleCalculator/PublishExe folder in this directory.  Please double click the Calculator.exe file to run.

	This has created a rather large file as it incorporates the .NET Core runtime!

	Alternatively, please install the .NET Core 3.1 runtime and either:
	*	open the solution file 'SimpleCalculatorProject' in Visual Studio, ensure the 'Calculator' project is loaded and click the 'Run' button
	*	open a command window in the directory /SimpleCalculator and type 'dotnet run'

OPERATION:
	Enter either commands manually into the application or provide the location of a txt file with the relevant commands.*
	The output will show the calculated result.

	Commands should be in the form of:
		<register> <operation> <value>
		print <register>
		quit

	Register names are case insensitive and can be made up of letters or letters and numbers.
	Operations currently provided are 'add', 'subtract' and 'multiply'.
	Results are computed at print time.

* Please refer to the original test requirements file (UglyCalc.pdf) for examples of input files.

---------------------------------------------------------------------------------------------------------------------------

TESTS:
	This application comes with a small set of unit tests to ensure the basic calculator funtionality is working.
	Please run the tests within Visual Studio, by changing the project over to the 'SimpleCalcTestProject', selecting 'Tests'
	from the top menu and 'Run all tests'.

---------------------------------------------------------------------------------------------------------------------------

NOTES:
•	The definition of an alphanumeric character is ambiguous when it comes to containing only numbers e.g. 
	the following register names would qualify: ‘a’, ‘a5’, ‘5’.  The register name ‘5’ will be a problem for 
	this simple calculator, given it’s limited user interface.  As the requirements do not specify any other 
	identifier of a register apart from it’s location in the input, with its possible use as a value, and the 
	fact that it can be used without being initialised, the following statement would cause an exception: ‘A add 5’. 
	Is 5 a number or is it a register?
	Therefore, I will limit the requirement of ‘any mane consisting of alphanumeric characters should be allowed 
	as register names’ to numbers AND letters, or just letters.  Not numbers on their own.
	I am aware this may be a significant requirement, in which case I would suggest identifying register names in 
	some way at input e.g. using quote, a symbol ‘@’ or ‘r:’.
*	Due to needing to provide extra functionality in the future such as divide and basing requirements on the
	specification provided here, I've chosen to use 'decimal' as the base number type.  This means it can take
	numbers with decimal points, can show fractions when divided and can handle much large numbers than ints
	or floats. It does have its drawbacks as it's heavy on memory and not as scalable as a double, but as one
	of the examples showed calculations on money, decimals are more accurate, so decided this would be 
	most suitable to this application.
	Therefore the number range is: 79228162514264337593543950335 to -79228162514264337593543950335.  If a 
	larger number is attempted, a try/catch block with tackle it.
*	'divide' as an operation will work as one word, not ‘divide by’ as the initial input string is split on spaces.  
	To work with ‘divide by’, extra processing should be done to identify this string combination.

ASSUMPTIONS:
*	All new registers are intialised to zero.

SOLUTION NOTE: 
*	In GetFinalResult() aiming to implement 'lazy evaluation', looping through the saved commands twice does not 
	feel at all right here and I firmly believe a better solution is still 'out there'.
	Similarly in SaveCommand() and the SavedCommand object I used to store not-yet-calculated commands, I'm not 
	satisfied this is the best solution either.	
    Looking at functional programming may get a better solution, but that's not in my skill set yet, so didn't feel
	that was a route I could investigate.  Also looked at Lazy<T> but don't really understand it enough to implement it
    or explain/defend it.  I thought about using a Tree data structure and a traversal algorithm to store the Registers 
	and their operations to Traverse the tree at print time but again, not a strong point of mine yet, do decided against it.
    It does appear to be producing the correct result based on the specifications, but I suspect there is a much
	better way of doing things.
    I COULD say this is an intentional example of creating technical debt in order to show my ability at identifying it!

--------------------------------------------------------------------------------------------------------------------------
