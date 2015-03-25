Cflat is a provably Turing Complete object oriented language that compiles to the CIL.

# 1. Introduction and Motivation #
Cflat is a brand new language made to be run on the Common Language Infrastructure (CLI), and in its current implementation on Microsoft's Common Language Runtime (CLR) specifically.  The CLI is an open specification initially developed by Microsoft.  It describes both the code and the runtime environment that form the core of Microsoft's .NET platform.  Since this is an open specification, there have been open source projects, namely Mono and Portable.NET, that have also implemented runtime environments for the CLI.

The language is heavily based on modern object oriented languages such as Java and C# (from which the name is derived).  The language is full-featured and provides a majority of the features expected in a standard modern object oriented language such as classes, abstraction, inheritance, information hiding (encapsulation), and recursion, as well as several non-standard and even new concepts.

The primary motivation for Cflat was to provide a learning experience in language design and implementation, starting from nothing at all and ending with a full language specification and a functioning, if limited, compiler.  A secondary motivation was to provide a simpler and less verbose alternative to Visual Basic and C# for programmers who wish to use an object oriented language and run their code on the CLI.
This document will serve as an introduction to the language itself as well as documentation for the current version of the only Cflat compiler available.  In the Documentation appendix there are sample programs available as well as a full token and grammar specification.

# 2. About Cflat #

This section will introduce the Cflat language, from an overview of the structure and syntax to interesting (non-standard) features to the future of the language as seen at this point.

## 2.1 Language Structure and Syntax ##

The structure of a typical Cflat program is extremely similar if not identical to any other object oriented language that is on the market today.  A program is defined as a list of classes, which can appear in any order.  These classes are in turn made up of a list of declarations, which may be of two different types; member variables or method declarations.  Any variable defined within a class but not within a method is a member variable.  Method declarations consist of a list of statements or expressions.  These may be variable declarations, assignments, algebraic manipulations, looping constructs, return statements, method calls, and so on.  A formal definition of the Cflat grammar, including the syntax for each type of statement, can be found in section 5 of this document.

The syntax of Cflat will be instantly familiar to most programmers familiar with procedural or object oriented programming languages.  With the exception of the language-specific features described in detail below, many Cflat programs would with very minor modifications could be transformed into syntactically valid C++, Java, or C# programs.
The Example Programs section at the end of this document gives specific code examples that will show the syntax and structure of a common Cflat program in a more readable format than this description.

## 2.2 Standard Object-Oriented Features ##

Cflat currently includes most of the features that define object oriented programming languages.  Arbitrary levels of inheritance between classes is supported, including the ability to access public member variables and functions on one class's parent without instantiating the base class or calling the property of method on the base class explicitly.  Access modifiers of public and private provide information hiding, although not at a level as granular as many other modern languages.

Recursive function calls and mutually recursive functions are fully supported, as the example on recursion at the end of this document shows.  Data abstraction is certainly possible using Cflat, as it is in any object oriented language, although standard data structures are not shipped with the language as a base library.

There are several built-in types in Cflat that are similar to those in most other procedural or object oriented languages.  Primitive types are bool, int, real (floating point), and string.  As mentioned above, user-defined classes play a large role in the language.  Arrays of any type, including classes, are supported as well.

## 2.3 Language-specific Features ##

In addition to implementing many of the features found in common object oriented languages today, Cflat introduces several new and interesting programming concepts.  Several constructs are also borrowed from either non-object oriented or not-so-popular languages, possibly with adaptations.  Each of the topics below are shown by example in the Example Programs section of this document.

### 2.3.1 Necessary Functions ###

A new modifier available on functions is the necessary keyword.  Placing this keyword on a function means that any class which derives from the class containing this function must also have a method of the same signature.  This is verified during the semantic checking of the compilation, and if this condition is not satisfied then an error is printed and no code is generated.

This functionality serves as a kind of a proxy to an interface, which Cflat does not explicitly provide.  However the same behavior is possible by making a class full of necessary functions with empty bodies – this is essentially an interface.  This construct blends the concepts of base classes and interfaces that are available in many other languages together into a single entity.

### 2.3.2 Readonly Functions and Parameters ###

Another new modifier in Cflat is the readonly keyword, which is not quite the same as the same keyword in C#, Java's final keyword, or const from C++.  This keyword can be placed on a parameter in a function definition to mean that function cannot modify that variable in any way.  Placing the keyword on a function instead of a parameter implies that the readonly concept applies to all parameters passed to the function.

Modifications to readonly functions are checked in the semantic passes of the compiler.  Readonly parameters cannot be assigned to or passed as non-readonly parameters to other functions.  Class instances marked as readonly cannot have member variables on them assigned to, and arrays marked as readonly cannot be assigned to at any index.  As with the necessary keyword, violations to these rules will produce a compile-time error and no code will be generated.

### 2.3.3 For .. In Loops ###

A construct that Cflat has borrowed from other languages such as Python and Haskell is the concept of a for loop over a given range.  The syntax for(x in [-5..10]) denotes a loop that will iterate 14 times (inclusive lower bound, exclusive upper bound).  At each iteration of the loop, the variable x will be set to an increasing value, starting the first iteration at the lower bound and ending the loop with the value of one less than the upper bound.  Of important note is that no array or other internal representation is instantiated to represent this construct during the loop.  The loop written in this format is transformed by the compiler into a more standard for loop syntax – for the example above this would be for(int x=-5; x<10; x++).  Thus, this is for ease of use to the programmer and is pure syntactic sugar.

### 2.3.4 Bounded Arrays ###

Another construct that Cflat has borrowed from many other languages is that of bounded arrays, or arrays with a variable lower bound.  The syntax for instantiating one of these arrays would be for example int[5..12], for an array of integers that ha a lower bound 5 and an upper bound 12.  Again, this is merely for programmer ease and is treated exactly the same way as int[0..7] would be, except the indexes used in dereferencing will obviously differ.

## 2.4 Features of Future Versions ##

There are several features that most modern programmers expect to be in their object oriented language that are currently missing from Cflat, either due to design or because of time constraints.  Most notably missing from Cflat at this point are method overloading, variable shadowing, exception handling, delegates (function pointers), interaction with other .NET languages, and generics.  Also partially missing are interfaces, which can be proxied by using a carefully constructed class with necessary methods, but the single inheritance of Cflat restricts the usefulness of this approach.
Most of the above-mentioned features were left out of Cflat due to time constraints.  For example, method overloading can be relatively easily implemented in the current design, but it would require the use of a different internal data structure for methods.  The case is similar with variable shadowing.  This scope of refactoring was simply not feasible to do before the end of the semester, so these features were not implemented.
Generics were left out of the language design altogether, as we classified them as out of scope from the beginning.  However this does not mean that the current implementation of the grammar and infrastructure of the compiler could not support generics; the research simply has not been done.

More features that could be implemented in future iterations of the language would be better interaction with other languages in the .NET framework, more granular access modifiers, more primitive types and methods, and a base class library to ship along with the language to provide more built-in functionality.  Interaction with libraries written in other languages is achievable since these languages all use a common intermediate language, although that was clearly out of scope for this project.  A strong class library is key to the success of any programming language, and would probably be the first item on the list to do once the language became stable.

# 3. Compiler Implementation Specifics #

This section introduces and documents both the planned version of the Cflat compiler and its current state.  As this was done as a single-semester project by a team of three, certain features such as generics were known to be out of scope from the beginning; these are not included here.  The compiler description below is the version that we planned on trying to implement, and is mostly in agreement with the code.  Any known discrepancies are outlined in section 3.6.

## 3.1 Tools Used ##

The Cflat compiler is implemented using a scanner generator and parser generator.  This decision was made to allow the team to focus our efforts on defining the syntax of the language, performing the semantic analysis, and code generation.  Both the scanner generator and parser generator were developed by Queensland University of Technology and are licensed under the a Simplified BSD license.

## 3.2 Lexical Analysis ##

The scanner generator used by the Cflat compiler is the Gardens Point Scanner Generator (GPLEX).  GPLEX is used to generate a scanner in the C# language, and the input file in the similar format used with the traditional implementation of LEX, with the main difference being the small excerpts of C# within the .lex file that creates the instances of the tokens used by the language.

## 3.3 Syntax Analysis ##

The parser generator used by the Cflat compiler is the Gardens Point Parser Generator (GPPG).  GPPG was written specifically for use with the GPLEX scanner, although it can be used with other scanners as well.  GPPG takes a input file similar to that used in other popular parser generators, namely YACC and BISON.  GPPG generates a LALR(1) parser in the target language C#.

In the Cflat grammar, an instance of an abstract syntax tree (AST) node is created for each production in the grammar.  The AST was hand written and consists of approximately sixty different types of nodes.  Each node represents one important piece in the syntax of the language, such as a loop construct, variable reference, or method declaration.  The AST is the primary intermediate representation used in the Cflat compiler, and the tree is walked several times in both the semantic analysis and code generation phases.

## 3.4 Semantic Analysis ##

The semantic analysis for the Cflat compiler is performed by a series of three passes over the abstract syntax tree created by the parser.  By designing a multi-pass compiler, this allows for a more natural style of coding for developers, where classes and methods can appear in any order in the source code.  By implementing several passes, this removes the need for developers to stub out their method definitions in the beginning of the source file, like many early compilers required.

The goal of the semantic analysis is twofold.  The first object is to ensure the source file is a valid program.  Secondly, the semantic analysis fills in properties of the syntax tree with the details of the source code.  For example, the syntax tree node for a class will be populated with a “class descriptor” which defines the class's scope, as well as any parent class.  In the case of an identifier node, the semantic analysis will fill in the node's type.

The semantic analysis is performed using the visitor pattern, where each visitor only visits the nodes it is required to.  However, in the early passes, no action is required for many of the tree nodes.  Each of the semantic passes maintains a “typing environment” which maintains a tree of scopes, which allows the semantic analysis to correctly determine where variables and methods are defined.  When an error is detected by any of the three passes, and exception is thrown so the semantic analysis will stop.  At that point, the description of the offending code is shown, along with the line and column in the source code where the error has occurred.

In order for the team to focus efforts on the implementation of the compiler internals, it was decided that the fail fast model of the semantic analysis was most appropriate, rather than implementing an error recovery strategy to allow the semantic analysis to continue.

### 3.4.1 First Pass – Class-Level Analysis ###

The first semantic pass visits only class definitions in the source file.  This includes any subclass definitions as well.  When a class is visited, it's type is recorded in the global scope, and a new scope is created for the class's declarations, such as member variables and any methods.  The only other objective of the first pass is to define our language's built in methods in the global scope.  The built in functions are as follows:

  * void print(string)  –  Writes a string to the console window.
  * void println(string)  –  Writes a string to the console window and returns to the next line.
  * string readln()  –  Reads input text from the console and returns the input as a string.
  * int parseInt(string)  –  Accepts a string and converts it to an integer.

### 3.4.2 Second Pass – Method- and Member-Level Analysis ###

The second semantic pass visits again all class definitions and subclass definitions, in addition to visiting all member variable declarations and method declarations.  When a class definition is encountered, the second pass must restore the class's scope, so any member variables and methods are properly added to the class's scope.  Then, the pass simply visits the list of declarations that belong to the class.

When a member variable definition is visited, its' presence is recorded in the containing class's scope.  Any modifiers (i.e. public, readonly, etc) are collected and stored in the declaration node and in the scoping environment for later use.

When a method declaration is visited, the method's presence is recorded in the containing class as well, along with any modifiers (such as public or private) are recorded.  The parameters for the method, if any, are visited and stored for future use in the scoping environment and the syntax tree node.  At this point, a new scope is defined for use within the method body for any local variables.  The method body however is not visited yet, until the third pass.

Finally, once all of the member and method declarations have been visited, the semantic pass must implement one final check for subclasses.  The second pass must validate if any method in the parent class is marked as necessary.  If so, the pass must verify that the subclass provides an implementation of that method.  If not, an error is reported and the compilation process terminates.

### 3.4.3 Third Pass – Statement-Level Analysis ###

The third semantic pass is where the majority of compiler errors are caught and reported.  Up until the third semantic pass, the only errors that are reported are subclasses with undefined parent classes, and subclasses that do not implement necessary methods.  The third pass visits all method bodies, which is where the vast array of mistakes can happen.

When the third pass revisits a class or a method, the class and method's scope is restored to ensure that all variables and methods are available again, and are no longer available one control leaves from the class or method.  When visiting a method, the concept of blocks are introduced, which allows the compiler to track all code paths of a method.  This is crucial to ensuring that a method with a return type other than void actually returns a value in all cases.  Simply checking if a return statement appears in the method body is not enough. Blocks also allow for scoping of variables declared inside a for loop for example.  These variables will go out of scope at the end of the block, and therefore attempting to access them will result in a compiler error.
The main types of errors caught by the third pass are the following:
  * Use of variables that have not been defined
  * Invoking methods that do not exist in the current scope
  * Invoking a method with incorrect parameters
  * Modifying a readonly field
  * Type mismatches in assignments, binary operators, and control loops
  * Field dereferences that do not exist and array dereferences that do not evaluate to the integer type.

In some cases, the job of the third pass is very easy.  For example, when visiting an if statement node, the third pass simply must visit the condition statement and ensure it evaluates to a boolean.  Then, the third pass must record the presence of the control branch block and visit the block body. Other syntax tree nodes are more complex, for example invoking a method on a class.  First, the pass must ensure the class instance actually exists in the current scope.  Then, the third pass must lookup the class type of the instance and ensure it has a method with the given name, and we then need to ensure the parameters match.

If there are no errors encountered during the third semantic pass, then the compilation process continues to the code generation phase.

## 3.5 Code Generation ##

The code generation phase takes in an abstract syntax tree and targets Common Intermediate Language (CIL).  Cflat's code generation phase consists of three passes over the abstract syntax tree.  The first collects class information, the second method information, and the last phase generates the intermediate language.

### 3.5.1 First Pass – Type and Inheritance Analysis ###

This pass walks the tree checking any node that declares a class or subclass.  The types are then collected in a dictionary to determine type hierarchy.  This allows classes to be defined in any order since we can tell if the current node directly depends on a type defined in another node.

### 3.5.2 Second Pass – Method and Type/Method Association Analysis ###

This pass goes over all of the types collected in the first pass and generates a collection of the methods within the classes.  Each method's declaration is saved with it's containing type information.

### 3.5.3 Third Pass – Common Intermediate Language Generation ###

The third pass actually generates the CIL, but there are a few things to take care of before entering the actual IL generation.

First, the type collection from the first pass is iterated over to stub out classes and define types.  This also handles implementing the actual type inheritance.  This also allows types to be accessed by name at any point in the third pass.
Second, the method collection is iterated over and each method is stubbed out and given a signature.  If this step is omitted, we would have to generate methods in order, since once a method is called, it's signature can no longer be changed.  For example if function a takes an integer as an argument and it is called before it is given its proper signature, the code will generate a void argument for the function a and passing an integer to this function will have no effect.  For this reason, this iteration collects modifier and formal information for arguments, as well as return types, and generates the method signature for invocation.

The largest part of the third pass is the actual CIL generation, which utilizes the .NET framework's System.Reflection.Emit libraries and the ILGenerator class within.  The Emit() method of ILGenerator takes an OpCode from a list of OpCodes in System.Reflection.Emit and optional arguments, depending on the instruction.  Instructions either operate on the stack, take argument, or operate on their own.
Binary operators generate stack operations where the expression on the left and the expression on the right are evaluated and the desired OpCode is written to the IL to perform the operation.

Unary operators walk the expression and then write the appropriate OpCode to perform their operations.

Branching is handled by the set of br OpCodes and takes arguments to labels marked in the IL.

The final part of CIL generation is actually creating the types and finalizing the IL being written to the dynamic assembly module.

## 3.6 Current State ##

The following section outlines the current state of Cflat and lists the known bugs and limitations.

### 3.6.1 Known Limitations and Bugs ###
  1. Access modifiers are currently non-functional – the grammar will accept and generate AST nodes for access modifiers like private, public, static, final, etc.  These are not taken into account beyond semantic analysis at this point.
  1. Class fields are only partially implemented – a Cflat programmer can currently define fields on a class, but accessing the fields is not completely implemented.
  1. Class instantiation – classes can be instantiated but only with a default, 0 argument constructor, and even then the newobj OpCode may generate incorrect CIL and fail.
  1. Variable shadowing and method overloading – due to a hasty design decision to use key/value pairs for local and method storage, these concepts do not work.  Rather than introduce a dictonary key “hack” to create unique values depending on arguments, the full implementation has been put off until it can be properly implemented.
  1. Utilization of the .NET framework – The framework can be used from Cflat, but it is not completely implemented.  Being that this is one of the most attractive features of .NET languages, this is definitely a high priority future feature.
  1. Optimization – The current cflat compiler provides little to no optimization at compile time.  Dead code elimination, constants and loops, and elimination of redundant declarations/initializations would be simple features to implement that would greatly improve the compiler.

# 4. Conclusion #

Overall the implementation of the Cflat compiler during a single semester was a success.  The team demonstrated the implementation of a compiler from the front end, which included a custom designed grammar and syntax, as well as a syntax tree, all the way to code generation in an existing intermediate language.  Utilizing the Gardens Point compiler tools proved to have extremely good results, as using them allowed the team to focus efforts on semantic analysis and code generation.

One of the goals for Cflat was to implement a language with a similar syntax to popular object oriented languages today, like that of Java or C#, and provide a subset of the features available in these popular languages.  However, we also brought new ideas to the language with the use of different keywords such as readonly and necessary, which both compile into working programs in CIL.

While it is true that the compiler has known issues (see Section 3.6), this does not take away from what the team has learned.  We have a much greater understanding and appreciation for the compilers we use today.  In addition, we have a solid understanding for CIL, which is the language that Microsoft's .NET languages are compiled to.

# 5. Documentation #

## 5.1 Compiler User's Manual ##
There are currently no optimization or other flags to pass into the compiler.  The only input then is the source code of the program.  Currently this needs to be in a single file – if you have multiple files these can simply be concatenated together into a single file.  The compiler can be invoked from a command prompt or alternatively built from C# sources and run in your favorite IDE.

The syntax for calling the compiler from a command prompt is: Cflat /path/sourceFile.cf

## 5.2 Token Definition ##
Token definitions are located [here](http://cflat.googlecode.com/svn/trunk/GeneratorInputFiles/CFlat.lex).

## 5.3 The Official Grammar – LALR(1) ##
Grammar is located [here](http://cflat.googlecode.com/svn/trunk/GeneratorInputFiles/CFlat.y).

## 5.4 Example Programs ##

### 5.4.1 Hello World ###
```
class helloWorld
{
	void main() 
	{
		print("Hello, world"); 
	}
}
```
### 5.4.2 Looping ###
```
class forLoop
{
	void main()
	{
		int x=0;

		for(int i = 0; i < 10; i++)
			x++;

		println(x & " should be 10");

		for(a in [0..9])
			x--;
		

		println(x & " should be 0");
	}
}
```
### 5.4.3 Inheritance ###
```
class fact
{
	int factorial(int n) 
	{
		return 0;
	}
}

class derivedFact is fact
{
	void main() 
	{
		int x = factorial(5);
		int y = base.factorial(5);
		int z = self.factorial(5);

		println("factorial: " & x & ", base: " & y & ", self: " & z);
	}

	int factorial(int n) 
	{
		if (n == 0)
			return 1;
		else
			return n * factorial(n-1);
	}
}
```
### 5.4.3 Recursion ###
```
class fact
{
	int factorial(int n) 
	{
		if (n == 0)
			return 1;
		else
			return n * factorial(n-1);
	}
}
```
### 5.4.4 Readonly Parameters ###
```
class holder
{
	public int x;
}
class foo
{
	void main()
	{
		holder h = new holder();
		h.x = 10;

		bar b = new bar();
		b.change(holder, 5);
		print(holder.x);

		b.changeAgain(readonly holder, 12);
		print(holder.x);
	}
}
class bar
{
	public void change(holder h, int n) 
	{
		h.x = n;
	}

	/* readonly can also be a method modifier.  
	That is the same as setting all parameters to readonly */
	public void changeAgain(readonly holder h, int n)
	{
		/* Both the call to sneakyFunc and the property setter on h.x 
		are semantic errors since h is a readonly variable*/
		sneakyFunc(h,n);
		h.x = n;
	}
	
	public void sneakyFunc(holder h, int n)
	{
		h.x = n;
	}
}
```
### 5.4.5 Necessary Methods ###
```
class animal
{
	necessary public void speak()
	{
		println("Hello");
	}
}

class cat is animal
{
	necessary public void speak()
	{
		println("meow");
	}
}

class dog is animal
{
	void main()
	{
		speak();
		base.speak();
	}
	
	/* Class dog will not compile since it does not implement the method
	'void speak()' marked necessary by parent class 'animal' */
	public void sayHi()
	{
		println("woof");
	}
}
```