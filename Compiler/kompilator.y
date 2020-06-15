
%namespace Compiler

%union
{
    public string       val;
    public char         type;
    public SyntaxTree   subtree;
}

%token Program
%token Semicolon
%token IntCast DoubleCast
%token Int Real Bool
%token Assign
%token <val> Ident
%token <val> RealValue IntegerValue String
%token True False
%token If Else
%token While
%token Read Write
%token Plus Minus Multiply Divide
%token BitNeg BitOr BitAnd
%token Neg Or And
%token Equal NotEqual Greater Less GreaterEqual LessEqual
%token OpenPar ClosePar OpenCurly CloseCurly
%token Return
%token Eof Error

%type <subtree> start declarations declaration commands command write read return assign if while expression logical relational additive multiplicative bit unary term

%%

start :
            Program
            OpenCurly
            declarations
            commands
            CloseCurly Eof { YYACCEPT; }
          | error { Compiler.errorLines.Add(Compiler.lineno); YYABORT; }
          ;

declarations :
            declaration
            declarations
          | /* empty */
          ;

declaration :
            Int
            Ident { new IntIdent($2); }
            Semicolon
          | Real
            Ident { new RealIdent($2); }
            Semicolon
          | Bool
            Ident { new BoolIdent($2); }
            Semicolon
          | error Semicolon declaration { Compiler.errorLines.Add(Compiler.lineno); }
          | error Eof { Compiler.errorLines.Add(Compiler.lineno); YYABORT; }
          ;

commands :
            command
            commands
          | /* empty */
          ;

command :
            OpenCurly commands CloseCurly
          | write
          | read
          | assign
          | if
          | while
          | return
          | error Semicolon command { Compiler.errorLines.Add(Compiler.lineno); }
          | error Eof { Compiler.errorLines.Add(Compiler.lineno); YYABORT; }
          ;

write :
            Write
            String
            Semicolon { new WriteString($2); }
          | Write
            expression
            Semicolon { new WriteExpression($2); }
          ;

read :
            Read
            Ident
            Semicolon { new Read($2); }
          ;

return :
            Return
            Semicolon
          ;

assign :
            Ident
            Assign
            expression
            Semicolon
          ;

if :
            If
            OpenPar
            expression
            ClosePar
            command
            Else
            command
          | If
            OpenPar
            expression
            ClosePar
            command
          ;

while :
            While
            OpenPar
            expression
            ClosePar
            command
          ;

expression :
            logical { $$ = new BoolIdent("qwe"); }
          ;

logical :
            logical Or relational
          | logical And relational
          | relational
          ;

relational :
            relational Equal additive
          | relational NotEqual additive
          | relational Greater additive
          | relational Less additive
          | relational GreaterEqual additive
          | relational LessEqual additive
          | additive
          ;

additive :
            additive Plus multiplicative
          | additive Minus multiplicative
          | multiplicative
          ;

multiplicative :
            multiplicative Multiply bit
          | multiplicative Divide bit
          | bit
          ;

bit :
            bit BitOr unary
          | bit BitAnd unary
          | unary
          ;

unary :   
            Minus unary
          | BitNeg unary
          | Neg unary
          | IntCast unary
          | DoubleCast unary
          | OpenPar expression ClosePar
          | term
          ;

term : 
            Ident
          | RealValue
          | IntegerValue
          | True
          | False
          ;

%%

public Parser(Scanner scanner) : base(scanner) { }