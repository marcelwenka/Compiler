
%using QUT.Gppg;
%namespace Compiler

Integer [1-9][0-9]*|0
Real ([1-9][0-9]*|0)\.[0-9]+
Ident [a-zA-Z][a-zA-Z0-9]*
String \"(\\.|[^"\\])*\"

%%

"//".*		{ }
"program"	{ return (int)Tokens.Program; }
";"			{ return (int)Tokens.Semicolon; }
"(int)"		{ return (int)Tokens.IntCast; }
"(double)"	{ return (int)Tokens.DoubleCast; }
"int"		{ return (int)Tokens.Int; }
"double"	{ return (int)Tokens.Real; }
"bool"		{ return (int)Tokens.Bool; }
{Real}		{ yylval.val = yytext; return (int)Tokens.RealValue; }
{Integer}	{ yylval.val = yytext; return (int)Tokens.IntegerValue; }
{String}	{ yylval.val = yytext; return (int)Tokens.String; }
"true"		{ return (int)Tokens.True; }
"false"		{ return (int)Tokens.False; }
"if"		{ return (int)Tokens.If; }
"else"		{ return (int)Tokens.Else; }
"while"		{ return (int)Tokens.While; }
"read"		{ return (int)Tokens.Read; }
"write"		{ return (int)Tokens.Write; }
"!"			{ return (int)Tokens.Neg; }
"~"			{ return (int)Tokens.BitNeg; }
"|"			{ return (int)Tokens.BitOr; }
"&"			{ return (int)Tokens.BitAnd; }
"*"			{ return (int)Tokens.Multiply; }
"/"			{ return (int)Tokens.Divide; }
"+"			{ return (int)Tokens.Plus; }
"-"			{ return (int)Tokens.Minus; }
"=="		{ return (int)Tokens.Equal; }
"!="		{ return (int)Tokens.NotEqual; }
">"			{ return (int)Tokens.Greater; }
"<"			{ return (int)Tokens.Less; }
"<="		{ return (int)Tokens.GreaterEqual; }
">="		{ return (int)Tokens.LessEqual; }
"||"		{ return (int)Tokens.Or; }
"&&"		{ return (int)Tokens.And; }
"="			{ return (int)Tokens.Assign; }
"("			{ return (int)Tokens.OpenPar; }
")"			{ return (int)Tokens.ClosePar; }
"{"			{ return (int)Tokens.OpenCurly; }
"}"			{ return (int)Tokens.CloseCurly; }
"\r\n"		{ Compiler.lineno++; }
"\n"		{ Compiler.lineno++; }
" "			{ }
"\t"		{ }
"return"	{ return (int)Tokens.Return; }
{Ident}		{ yylval.val = yytext; return (int)Tokens.Ident; }
<<EOF>>		{ return (int)Tokens.Eof; }
.			{ return (int)Tokens.Error; }