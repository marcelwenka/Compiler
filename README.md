# Compiler prepared for a Code Translation course

Its main objective is to validate a program written in a made-up language (simplified version of C) and translate it to CIL.
It uses GPLEX and GPPG which are like C# versions of LEX and YACC.

The process of compilation consists of the following steps:
* lexical analysis - translating source code to a list of tokens
* syntax analysis - parsing the list of tokens to a syntax tree (simultaneously checking for syntax errors)
* code check - semantic verification (e.g. verification of types)
* generating the output code

The basic language rules are as follows:
* every program starts with a word **program** and curly brackets
* the only allowed types are **int, double, bool**
* semicolon at the end of each statement
* all variable declarations should be placed at the beginning of the program
* statements **while, if, break and continue** are allowed
* additionally **break n** is allowed where **n** signifies a number of loops to break out of
* **write** followed by a text in double quotes prints the text to console
* **write** followed by an expression prints the result to console
* **read** followed by a variable name parses the console input to the variable 
* **return** finishes the program
* logical, relational, bitwise, additive, multiplicative and unary operations are allowed
* // signifies a comment
* whitespace characters are ignored

In order to compile a program run Compiler.exe and provide a path to the source code as the first parameter.
The compiler returns:
* 0 in case of successful compilation (compiled program might be found in the same location as the source code)
* 1 in case of file error (missing argument or unable to open)
* 2 in case of syntax error
* 3 in case of semantic error
* 4 in case of unexpected error

There's an example program in the file **example** printing all prime numbers smaller than the entered number.
