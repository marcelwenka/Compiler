
%namespace Compiler

%union
{
    public string       val;
    public char         type;
    public SyntaxTree   subtree;
}

%token Program
%token Comma Semicolon
%token IntCast DoubleCast
%token Int Double Bool
%token Assign
%token <val> Ident
%token <val> DoubleValue IntegerValue String
%token True False
%token If Else
%token While
%token Break Continue
%token Read Write
%token Plus Minus Multiply Divide
%token BitNeg BitOr BitAnd
%token Neg Or And
%token Equal NotEqual Greater Less GreaterEqual LessEqual
%token OpenPar ClosePar OpenCurly CloseCurly
%token Return
%token Eof Error

%type <subtree> start declarations declaration commands command write read return assign if else while expression logical relational additive multiplicative bit unary term

%%

start :
            Program
            OpenCurly
            declarations
            commands
            CloseCurly Eof { YYACCEPT; }
          | error { Compiler.syntaxErrors.Add(Compiler.lineno); YYABORT; }
          ;

declarations :
            declaration
            declarations
          | error Semicolon { Compiler.syntaxErrors.Add(Compiler.lineno); }
            declarations
          | error Eof { Compiler.syntaxErrors.Add(Compiler.lineno); YYABORT; }
          | /* empty */
          ;

declaration :
            Int
            Ident { Compiler.AddNewSymbol(new Symbol(Compiler.lineno, 'i', $2)); }
            intDeclarations
            Semicolon
          | Double
            Ident { Compiler.AddNewSymbol(new Symbol(Compiler.lineno, 'd', $2)); }
            doubleDeclarations
            Semicolon
          | Bool
            Ident { Compiler.AddNewSymbol(new Symbol(Compiler.lineno, 'b', $2)); }
            boolDeclarations
            Semicolon
          ;

intDeclarations:
            Comma
            Ident { Compiler.AddNewSymbol(new Symbol(Compiler.lineno, 'i', $2)); }
            intDeclarations
          | /* empty */
          ;

doubleDeclarations:
            Comma
            Ident { Compiler.AddNewSymbol(new Symbol(Compiler.lineno, 'd', $2)); }
            doubleDeclarations
          | /* empty */
          ;

boolDeclarations:
            Comma
            Ident { Compiler.AddNewSymbol(new Symbol(Compiler.lineno, 'b', $2)); }
            boolDeclarations
          | /* empty */
          ;

commands :
            command
            commands
          | error Semicolon { Compiler.syntaxErrors.Add(Compiler.lineno); }
            commands
          | error Eof { Compiler.syntaxErrors.Add(Compiler.lineno); YYABORT; }
          | /* empty */
          ;

command :
            OpenCurly commands CloseCurly
          | write
          | read
          | if
          | while
          | expression Semicolon { Compiler.AddNewNode(new StandaloneExpression(Compiler.lineno, $1)); }
          | return
          | Break Semicolon { Compiler.AddNewNode(new Break(Compiler.lineno, "1")); }
          | Break IntegerValue Semicolon { Compiler.AddNewNode(new Break(Compiler.lineno, $2)); }
          | Continue Semicolon { Compiler.AddNewNode(new Continue(Compiler.lineno)); }
          ;

write :
            Write
            String
            Semicolon { Compiler.AddNewNode(new WriteString(Compiler.lineno, $2)); }
          | Write
            expression
            Semicolon { Compiler.AddNewNode(new WriteExpression(Compiler.lineno, $2)); }
          ;

read :
            Read
            Ident
            Semicolon { Compiler.AddNewNode(new Read(Compiler.lineno, $2)); }
          ;

return :
            Return
            Semicolon { Compiler.AddNewNode(new Return(Compiler.lineno)); }
          ;

if :
            If
            OpenPar
            expression
            ClosePar { Compiler.AddNewNode(new If(Compiler.lineno, $3)); }
            command
            else
          ;

else :
            Else { Compiler.AddNewNode(new Else(Compiler.lineno)); }
            command { Compiler.AddNewNode(new EndElse(Compiler.lineno)); }
          | /* empty */ { Compiler.AddNewNode(new EndIf(Compiler.lineno)); }
          ;

while :
            While
            OpenPar
            expression
            ClosePar { Compiler.AddNewNode(new While(Compiler.lineno, $3)); }
            command { Compiler.AddNewNode(new EndWhile(Compiler.lineno)); }
          ;

expression :
            assign { $$ = $1; }
          ;

assign :
            Ident
            Assign
            assign { $$ = new Assign(Compiler.lineno, $1, $3); }
          | logical { $$ = $1; }
          ;

logical :
            logical Or relational { $$ = new Logical(Compiler.lineno, Tokens.Or, $1, $3); }
          | logical And relational { $$ = new Logical(Compiler.lineno, Tokens.And, $1, $3); }
          | relational { $$ = $1; }
          ;

relational :
            relational Equal additive { $$ = new Relational(Compiler.lineno, Tokens.Equal, $1, $3); }
          | relational NotEqual additive { $$ = new Relational(Compiler.lineno, Tokens.NotEqual, $1, $3); }
          | relational Greater additive { $$ = new Relational(Compiler.lineno, Tokens.Greater, $1, $3); }
          | relational Less additive { $$ = new Relational(Compiler.lineno, Tokens.Less, $1, $3); }
          | relational GreaterEqual additive { $$ = new Relational(Compiler.lineno, Tokens.GreaterEqual, $1, $3); }
          | relational LessEqual additive { $$ = new Relational(Compiler.lineno, Tokens.LessEqual, $1, $3); }
          | additive { $$ = $1; }
          ;

additive :
            additive Plus multiplicative { $$ = new AdditiveMultiplicative(Compiler.lineno, Tokens.Plus, $1, $3); }
          | additive Minus multiplicative { $$ = new AdditiveMultiplicative(Compiler.lineno, Tokens.Minus, $1, $3); }
          | multiplicative { $$ = $1; }
          ;

multiplicative :
            multiplicative Multiply bit { $$ = new AdditiveMultiplicative(Compiler.lineno, Tokens.Multiply, $1, $3); }
          | multiplicative Divide bit { $$ = new AdditiveMultiplicative(Compiler.lineno, Tokens.Divide, $1, $3); }
          | bit { $$ = $1; }
          ;

bit :
            bit BitOr unary { $$ = new Bit(Compiler.lineno, Tokens.BitOr, $1, $3); }
          | bit BitAnd unary { $$ = new Bit(Compiler.lineno, Tokens.BitAnd, $1, $3); }
          | unary { $$ = $1; }
          ;

unary :   
            Minus unary { $$ = new Unary(Compiler.lineno, Tokens.Minus, $2); }
          | BitNeg unary { $$ = new Unary(Compiler.lineno, Tokens.BitNeg, $2); }
          | Neg unary { $$ = new Unary(Compiler.lineno, Tokens.Neg, $2); }
          | OpenPar Int ClosePar unary { $$ = new Unary(Compiler.lineno, Tokens.IntCast, $4); }
          | OpenPar Double ClosePar unary { $$ = new Unary(Compiler.lineno, Tokens.DoubleCast, $4); }
          | OpenPar expression ClosePar { $$ = $2; }
          | term { $$ = $1; }
          ;

term : 
            Ident { $$ = new Symbol(Compiler.lineno, $1); }
          | DoubleValue { $$ = new NumericValue(Compiler.lineno, 'd', $1); }
          | IntegerValue  { $$ = new NumericValue(Compiler.lineno, 'i', $1); }
          | True { $$ = new BoolValue(Compiler.lineno, true); }
          | False { $$ = new BoolValue(Compiler.lineno, false); }
          ;

%%

public Parser(Scanner scanner) : base(scanner) { }