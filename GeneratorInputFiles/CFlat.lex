%namespace LexicalAnalysis
%using SyntaxAnalysis;
%using QUT.Gppg;
%using CFlat;
%visibility internal


WhiteSpace  \r|\n|\r\n|\t\f
Integer		0|[1-9][0-9]*
Exponent	[eE]("+"|"-")?{Integer}
Real		{Integer}("."{Integer})?{Exponent}?
Comment		(\/\*)([^\*]*|\*+[^\/\*])*(\*+\/)
Identifier	[a-zA-Z][a-zA-Z0-9]*
String		\"(\\.|[^"])*\"


%%

";"                	{ yylval.Token = new Token(Tokens.SEMI, yytext, yyline, yycol); return (int)Tokens.SEMI; }
"+"          	    { yylval.Token = new Token(Tokens.PLUS, yytext, yyline, yycol); return (int)Tokens.PLUS; }
"-"          	    { yylval.Token = new Token(Tokens.MINUS, yytext, yyline, yycol); return (int)Tokens.MINUS; }
"*"          	    { yylval.Token = new Token(Tokens.TIMES, yytext, yyline, yycol); return (int)Tokens.TIMES; }
"/"          	    { yylval.Token = new Token(Tokens.DIVIDE, yytext, yyline, yycol); return (int)Tokens.DIVIDE; }
"<"          	    { yylval.Token = new Token(Tokens.SMALLER, yytext, yyline, yycol); return (int)Tokens.SMALLER; }
">"          	    { yylval.Token = new Token(Tokens.GREATER, yytext, yyline, yycol); return (int)Tokens.GREATER; }
"<="        	    { yylval.Token = new Token(Tokens.SMEQ, yytext, yyline, yycol); return (int)Tokens.SMEQ; }
">="        	    { yylval.Token = new Token(Tokens.GTEQ, yytext, yyline, yycol); return (int)Tokens.GTEQ; }
"=="               	{ yylval.Token = new Token(Tokens.EQ, yytext, yyline, yycol); return (int)Tokens.EQ; }
"!="               	{ yylval.Token = new Token(Tokens.NEQ, yytext, yyline, yycol); return (int)Tokens.NEQ; }
"="                	{ yylval.Token = new Token(Tokens.ASSIGN, yytext, yyline, yycol); return (int)Tokens.ASSIGN; }
"&&"               	{ yylval.Token = new Token(Tokens.AND, yytext, yyline, yycol); return (int)Tokens.AND; }
"||"               	{ yylval.Token = new Token(Tokens.OR, yytext, yyline, yycol); return (int)Tokens.OR; }
"!"                	{ yylval.Token = new Token(Tokens.NOT, yytext, yyline, yycol); return (int)Tokens.NOT; }
"%"                	{ yylval.Token = new Token(Tokens.MOD, yytext, yyline, yycol); return (int)Tokens.MOD; }
"("                	{ yylval.Token = new Token(Tokens.LPAREN, yytext, yyline, yycol); return (int)Tokens.LPAREN; }
")"                	{ yylval.Token = new Token(Tokens.RPAREN, yytext, yyline, yycol); return (int)Tokens.RPAREN; }
"{"                	{ yylval.Token = new Token(Tokens.LBRACE, yytext, yyline, yycol); return (int)Tokens.LBRACE; }
"}"                	{ yylval.Token = new Token(Tokens.RBRACE, yytext, yyline, yycol); return (int)Tokens.RBRACE; }
"["                	{ yylval.Token = new Token(Tokens.LBRACKET, yytext, yyline, yycol); return (int)Tokens.LBRACKET; }
"]"                	{ yylval.Token = new Token(Tokens.RBRACKET, yytext, yyline, yycol); return (int)Tokens.RBRACKET; }
"[]"				{ yylval.Token = new Token(Tokens.PBRACKET, yytext, yyline, yycol); return (int)Tokens.PBRACKET; }
"++"				{ yylval.Token = new Token(Tokens.INCREMENT, yytext, yyline, yycol); return (int)Tokens.INCREMENT; }
"--"				{ yylval.Token = new Token(Tokens.DECREMENT, yytext, yyline, yycol); return (int)Tokens.DECREMENT; }
"^"					{ yylval.Token = new Token(Tokens.EXP, yytext, yyline, yycol); return (int)Tokens.EXP; }
"**"				{ yylval.Token = new Token(Tokens.EXP, yytext, yyline, yycol); return (int)Tokens.EXP; }
"."					{ yylval.Token = new Token(Tokens.DOT, yytext, yyline, yycol); return (int)Tokens.DOT; }
".."				{ yylval.Token = new Token(Tokens.DOTDOT, yytext, yyline, yycol); return (int)Tokens.DOTDOT;}
","					{ yylval.Token = new Token(Tokens.COMMA, yytext, yyline, yycol); return (int)Tokens.COMMA; }
"&"					{ yylval.Token = new Token(Tokens.AMP, yytext, yyline, yycol); return (int)Tokens.AMP; }

"int"              	{ yylval.Token = new Token(Tokens.TINT, yytext, yyline, yycol); return (int)Tokens.TINT; }
"real"              { yylval.Token = new Token(Tokens.TREAL, yytext, yyline, yycol); return (int)Tokens.TREAL; }
"string"			{ yylval.Token = new Token(Tokens.TSTRING, yytext, yyline, yycol); return (int)Tokens.TSTRING; }
"bool"             	{ yylval.Token = new Token(Tokens.TBOOL, yytext, yyline, yycol); return (int)Tokens.TBOOL; }
"void"             	{ yylval.Token = new Token(Tokens.TVOID, yytext, yyline, yycol); return (int)Tokens.TVOID; }
"while"            	{ yylval.Token = new Token(Tokens.WHILE, yytext, yyline, yycol); return (int)Tokens.WHILE; }
"for"				{ yylval.Token = new Token(Tokens.FOR, yytext, yyline, yycol); return (int)Tokens.FOR; }
"in"				{ yylval.Token = new Token(Tokens.IN, yytext, yyline, yycol); return (int)Tokens.IN; }
"if"				{ yylval.Token = new Token(Tokens.IF, yytext, yyline, yycol); return (int)Tokens.IF; }   
"else"             	{ yylval.Token = new Token(Tokens.ELSE, yytext, yyline, yycol); return (int)Tokens.ELSE; }
"self"				{ yylval.Token = new Token(Tokens.SELF, yytext, yyline, yycol); return (int)Tokens.SELF; }
"base"				{ yylval.Token = new Token(Tokens.BASE, yytext, yyline, yycol); return (int)Tokens.BASE; }
"class"				{ yylval.Token = new Token(Tokens.CLASS, yytext, yyline, yycol); return (int)Tokens.CLASS; }
"is"				{ yylval.Token = new Token(Tokens.EXTENDS, yytext, yyline, yycol); return (int)Tokens.EXTENDS; }
"new"				{ yylval.Token = new Token(Tokens.NEW, yytext, yyline, yycol); return (int)Tokens.NEW; }
"return"			{ yylval.Token = new Token(Tokens.RETURN, yytext, yyline, yycol); return (int)Tokens.RETURN; }
"true"		   		{ yylval.Token = new Token(Tokens.TRUE, yytext, yyline, yycol); return (int)Tokens.TRUE; }
"false"		   		{ yylval.Token = new Token(Tokens.FALSE, yytext, yyline, yycol); return (int)Tokens.FALSE; }
"private"			{ yylval.Token = new Token(Tokens.FALSE, yytext, yyline, yycol); return (int)Tokens.PRIVATE; }
"public"			{ yylval.Token = new Token(Tokens.FALSE, yytext, yyline, yycol); return (int)Tokens.PUBLIC; }
"necessary"			{ yylval.Token = new Token(Tokens.FALSE, yytext, yyline, yycol); return (int)Tokens.NECESSARY; }
"readonly"			{ yylval.Token = new Token(Tokens.FALSE, yytext, yyline, yycol); return (int)Tokens.READONLY; }

/*
"-"{Integer}|"-"{Real}|"-"{Identifier}  { yyless(yyleng-1); yylval.Token = new Token(Tokens.UMINUS, yytext, yyline, yycol); return (int)Tokens.UMINUS;}
*/
{Integer}	 		{ yylval.Token = new Token(Tokens.LITERAL_INT, yytext, yyline, yycol); return (int)Tokens.LITERAL_INT; }
{Real}	 			{ yylval.Token = new Token(Tokens.LITERAL_REAL, yytext, yyline, yycol); return (int)Tokens.LITERAL_REAL; }
{String}			{ yylval.Token = new Token(Tokens.LITERAL_STRING, yytext, yyline, yycol); return (int)Tokens.LITERAL_STRING; }
{Identifier}		{ yylval.Token = new Token(Tokens.IDENTIFIER, yytext); return (int)Tokens.IDENTIFIER; }
{WhiteSpace}		{ /* Whitespace - Do Nothing */ }
{Comment}			{ /* Comment - Do Nothing */ }

%%

public override void yyerror (string format, params object[] args)
{
    Console.Write("Line {0}: ", yyline);
    Console.WriteLine(format, args);
}