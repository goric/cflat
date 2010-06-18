%namespace SyntaxAnalysis
%visibility internal

%using LexicalAnalysis;
%using AbstractSyntaxTree;
%using CFlat;

%start program
%YYSTYPE SemanticValue

/* Terminals */
%token<Token> SEMI RPAREN LBRACE RBRACE RBRACKET PBRACKET DOT DOTDOT COMMA IN BASE
%token<Token> TINT TREAL TSTRING TBOOL TVOID WHILE FOR IF ELSE SELF CLASS EXTENDS NEW RETURN 
%token<Token> TRUE FALSE
%token<Token> LITERAL_INT LITERAL_REAL LITERAL_STRING IDENTIFIER

/* Precedence rules */
%right ASSIGN
%left OR AND INCREMENT DECREMENT EXP
%left SMALLER GREATER SMEQ GTEQ EQ NEQ
%left PLUS MINUS TIMES DIVIDE MOD
%left UMINUS LPAREN NOT LBRACKET
%nonassoc ELSE

/* Non-terminal types */
%type<StatementList> program classList statementList
%type<Statement> classDeclaration statement 
%type<DeclarationList> declList 
%type<Declaration> decl 
%type<FormalList> formals onePlusFormals 
%type<Formal> formal
%type<ExpressionList> onePlusActuals actuals
%type<Expression> expression lvalue literal
%type<Type> type integralType


%%

program			: classList			{ SyntaxTreeRoot = $$; }
				;


classList		:								{ $$ = new ASTStatementList(); }
				| classDeclaration classList	{ $$ = new ASTStatementList($1, $2); }
				;


classDeclaration: CLASS IDENTIFIER LBRACE declList RBRACE				{ $$ = new ASTClassDefinition($2.Value, $4); }
				| CLASS IDENTIFIER EXTENDS IDENTIFIER LBRACE declList RBRACE	{ $$ = new ASTSubClassDefinition($2.Value, $4.Value, $6); }
				| error SEMI
				;


declList		:					{ $$ = new ASTDeclarationList(); }
				| decl declList		{ $$ = new ASTDeclarationList($1, $2); }
				;
	
	
decl			: type IDENTIFIER SEMI	{ $$ = new ASTDeclarationField($1, $2.Value); }
				| type IDENTIFIER LPAREN formals RPAREN LBRACE statementList RBRACE 
								{ $$ = new ASTDeclarationMethod($1, $2.Value, $4, $7); }
				| IDENTIFIER LPAREN formals RPAREN LBRACE statementList RBRACE 
								{ $$ = new ASTDeclarationCtor($1.Value, $3, $6); }
				;


statementList	:							{ $$ = new ASTStatementList(); }
				| statement statementList	{ $$ = new ASTStatementList($1, $2); }
				;


statement		: SEMI
				| expression SEMI								{ $$ = new ASTStatementExpr($1); }
				| type IDENTIFIER SEMI							{ $$ = new ASTDeclarationLocal(Location(@$.StartLine, @1.StartColumn), $1, $2.Value); }
				| type IDENTIFIER ASSIGN expression SEMI		{ $$ = new ASTDeclarationLocal(Location(@$.StartLine, @1.StartColumn), $1, $2.Value, $4); }
				| lvalue ASSIGN expression SEMI					{ $$ = new ASTAssign($1, $3); }
				| LBRACE statementList RBRACE					{ $$ = new ASTBlock($2); }
				| WHILE LPAREN expression RPAREN statement		{ $$ = new ASTWhile($3, $5); }
				| FOR LPAREN statement expression SEMI expression RPAREN statement 
											{ $$ = new ASTFor($3, $4, $6, $8); }
				| FOR LPAREN IDENTIFIER IN LBRACKET expression DOTDOT expression RBRACKET RPAREN statement
											{ $$ = new ASTForIn(new ASTIdentifier(Location(@$.StartLine, @3.StartColumn), $3.Value), $6, $8, $11); }
				| IF LPAREN expression RPAREN statement			{ $$ = new ASTIfThen($3, $5); }
				| IF LPAREN expression RPAREN statement ELSE statement  { $$ = new ASTIfThenElse($3, $5, $7); }
				| RETURN expression SEMI								{ $$ = new ASTReturn($2); }
				| error SEMI									{ $$ = new ASTNoop(); }
	  			;
          

formals			:					{ $$ = new ASTFormalList(); }
				| onePlusFormals	{ $$ = $1; }
				;
          

onePlusFormals	: formal							{ $$ = new ASTFormalList($1, new ASTFormalList()); }
				| formal COMMA onePlusFormals		{ $$ = new ASTFormalList($1, $3); }
				;
          

formal			: type IDENTIFIER	{ $$ = new ASTFormal($1, $2.Value); }
				;          


expression		: expression AND  expression		{ $$ = new ASTAnd($1, $3); } 
				| expression OR expression			{ $$ = new ASTOr($1, $3); } 
		        | expression SMALLER expression		{ $$ = new ASTSmaller($1, $3); } 
				| expression GREATER expression		{ $$ = new ASTGreater($1, $3); } 
				| expression SMEQ expression		{ $$ = new ASTSmallerEqual($1, $3); } 
				| expression GTEQ expression		{ $$ = new ASTGreaterEqual($1, $3); } 
				| expression EQ expression			{ $$ = new ASTEqual($1, $3); } 
				| expression NEQ expression			{ $$ = new ASTNotEqual($1, $3); } 
				| expression PLUS expression		{ $$ = new ASTAdd($1, $3); } 
				| expression MINUS expression		{ $$ = new ASTSubtract($1, $3); } 
				| expression TIMES expression		{ $$ = new ASTMultiply($1, $3); } 
				| expression DIVIDE expression		{ $$ = new ASTDivide($1, $3); } 
				| expression MOD expression			{ $$ = new ASTModulo($1, $3); } 
				| expression EXP expression			{ $$ = new ASTExponent($1, $3); } 
				| literal							{ $$ = $1; }
				| MINUS expression	%prec UMINUS	{ $$ = new ASTNegative($2); } 
				| NOT expression					{ $$ = new ASTNot($2); } 
				| expression INCREMENT				{ $$ = new ASTIncrement($1); }
				| expression DECREMENT				{ $$ = new ASTDecrement($1); }
				| LPAREN expression RPAREN			{ $$ = $2; } 
				| lvalue							{ $$ = $1; }
				| NEW IDENTIFIER LPAREN actuals RPAREN		{ $$ = new ASTInstantiateClass($2.Value, $4); }
				| NEW type LBRACKET expression DOTDOT expression RBRACKET 
						{ $$ = new ASTInstantiateArray($2, $4, $6); }
				| NEW type LBRACKET expression RBRACKET { $$ = new ASTInstantiateArray($2, $4, new ASTInteger(0)); }
				;
	     

lvalue			: IDENTIFIER						{ $$ = new ASTIdentifier(Location(@$.StartLine, @1.StartColumn), $1.Value); }
				| IDENTIFIER LPAREN actuals RPAREN 
								{ $$ = new ASTInvoke(new ASTIdentifier(Location(@$.StartLine, @1.StartColumn), "self"), $1.Value, $3); }
				| lvalue LBRACKET expression RBRACKET			{ $$ = new ASTDereferenceArray($1, $3); }
				| lvalue DOT IDENTIFIER LPAREN actuals RPAREN	{ $$ = new ASTInvoke($1, $3.Value, $5); }
				| lvalue DOT IDENTIFIER							{ $$ = new ASTDereferenceField($1, $3.Value); }
				| SELF								{ $$ = new ASTSelf(); }
				| BASE								{ $$ = new ASTBase(); }
				;
		  

actuals			:					{ $$ = new ASTExpressionList(); }
				| onePlusActuals	{ $$ = $1; }
				;
          

onePlusActuals	: expression						{ $$ = new ASTExpressionList($1, new ASTExpressionList()); }
				| expression COMMA onePlusActuals	{ $$ = new ASTExpressionList($1, $3); }
				;


type			: integralType				{ $$ = $1; }
				| integralType PBRACKET		{ $$ = new ASTTypeArray($1); }
				;
				
					     
integralType	: TINT			{ $$ = new ASTTypeInt(); }
				| TBOOL			{ $$ = new ASTTypeBool(); }
		        | TVOID			{ $$ = new ASTTypeVoid(); }
				| TSTRING		{ $$ = new ASTTypeString(); }
				| TREAL			{ $$ = new ASTTypeReal(); }
				| IDENTIFIER	{ $$ = new ASTTypeName($1.Value); }
				;


literal			: LITERAL_INT		{ $$ = new ASTInteger(Int32.Parse($1.Value)); } 
				| LITERAL_REAL		{ $$ = new ASTReal(Double.Parse($1.Value)); } 
				| LITERAL_STRING	{ $$ = new ASTString($1.Value); } 
				| TRUE				{ $$ = new ASTBoolean(true); }
				| FALSE				{ $$ = new ASTBoolean(false); }
				;

	
%%

public ASTStatementList SyntaxTreeRoot { get; set; }

public Parser(Scanner scan) : base(scan)
{
}

public SourceLocation Location(int line, int column) 
{ 
	return new SourceLocation(line, column, string.Empty);
}
