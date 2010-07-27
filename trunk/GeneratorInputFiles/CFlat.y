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
%token<Token> TRUE FALSE READONLY NECESSARY PRIVATE PUBLIC AMP
%token<Token> LITERAL_INT LITERAL_REAL LITERAL_STRING IDENTIFIER

/* Precedence rules */
%right ASSIGN
%left OR AND INCREMENT DECREMENT EXP
%left SMALLER GREATER SMEQ GTEQ EQ NEQ
%left PLUS MINUS TIMES DIVIDE MOD AMP
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
%type<Expression> expression rvalue literal
%type<LValue> lvalue
%type<Type> type integralType
%type<ModifierList> modifierList
%type<Token> modifier formalModifier


%%

program			: classList			{ SyntaxTreeRoot = $$; $$.Location = CurrentLocationSpan; }
				;


classList		:								{ $$ = new ASTStatementList(); $$.Location = CurrentLocationSpan; }
				| classDeclaration classList	{ $$ = new ASTStatementList($1, $2); $$.Location = CurrentLocationSpan; }
				;


classDeclaration: CLASS IDENTIFIER LBRACE declList RBRACE				{ $$ = new ASTClassDefinition($2.Value, $4); $$.Location = CurrentLocationSpan; }
				| CLASS IDENTIFIER EXTENDS IDENTIFIER LBRACE declList RBRACE	{ $$ = new ASTSubClassDefinition($2.Value, $4.Value, $6); $$.Location = CurrentLocationSpan; }
				| error SEMI
				;


declList		:					{ $$ = new ASTDeclarationList(); $$.Location = CurrentLocationSpan; }
				| decl declList		{ $$ = new ASTDeclarationList($1, $2); $$.Location = CurrentLocationSpan; }
				;
	
	
decl			: modifierList type IDENTIFIER SEMI		{ $$ = new ASTDeclarationField($1, $2, $3.Value); $$.Location = CurrentLocationSpan; }
				| modifierList type IDENTIFIER LPAREN formals RPAREN LBRACE statementList RBRACE 
								{ $$ = new ASTDeclarationMethod($1, $2, $3.Value, $5, $8); $$.Location = CurrentLocationSpan; }
				| modifierList IDENTIFIER LPAREN formals RPAREN LBRACE statementList RBRACE 
								{ $$ = new ASTDeclarationCtor($1, $2.Value, $4, $7); $$.Location = CurrentLocationSpan; }
				;

modifierList	:						{ $$ = new ASTModifierList(); $$.Location = CurrentLocationSpan; }
				| modifier modifierList { $$ = new ASTModifierList($1.Value, $2); $$.Location = CurrentLocationSpan; }
				;

modifier		: PRIVATE		
				| PUBLIC		
				| NECESSARY		
				| READONLY		
				;
				

statementList	:							{ $$ = new ASTStatementList(); $$.Location = CurrentLocationSpan; }
				| statement statementList	{ $$ = new ASTStatementList($1, $2); $$.Location = CurrentLocationSpan; }
				;


statement		: SEMI
				| expression SEMI								{ $$ = new ASTStatementExpr($1); $$.Location = CurrentLocationSpan; }
				| type IDENTIFIER SEMI							{ $$ = new ASTDeclarationLocal(CurrentLocationSpan, $1, $2.Value); $$.Location = CurrentLocationSpan; }
				| type IDENTIFIER ASSIGN expression SEMI		{ $$ = new ASTDeclarationLocal(CurrentLocationSpan, $1, $2.Value, $4); $$.Location = CurrentLocationSpan; }
				| lvalue ASSIGN expression SEMI					{ $$ = new ASTAssign($1, $3); $$.Location = CurrentLocationSpan; }
				| LBRACE statementList RBRACE					{ $$ = new ASTBlock($2); $$.Location = CurrentLocationSpan; }
				| WHILE LPAREN expression RPAREN statement		{ $$ = new ASTWhile($3, $5); $$.Location = CurrentLocationSpan; }
				| FOR LPAREN type IDENTIFIER ASSIGN expression SEMI expression SEMI expression RPAREN statement 
											{ $$ = new ASTFor(new ASTDeclarationLocal(CurrentLocationSpan, $3, $4.Value, $6), $8, $10, $12); $$.Location = CurrentLocationSpan; }
				| FOR LPAREN IDENTIFIER IN LBRACKET expression DOTDOT expression RBRACKET RPAREN statement
											{ $$ = new ASTForIn(new ASTIdentifier(CurrentLocationSpan, $3.Value), $6, $8, $11); $$.Location = CurrentLocationSpan; }
				| IF LPAREN expression RPAREN statement			{ $$ = new ASTIfThen($3, $5); $$.Location = CurrentLocationSpan; }
				| IF LPAREN expression RPAREN statement ELSE statement  { $$ = new ASTIfThenElse($3, $5, $7); $$.Location = CurrentLocationSpan; }
				| RETURN expression SEMI								{ $$ = new ASTReturn($2); $$.Location = CurrentLocationSpan; }
				| error SEMI									{ $$ = new ASTNoop(); $$.Location = CurrentLocationSpan; }
	  			;
          

formals			:					{ $$ = new ASTFormalList(); $$.Location = CurrentLocationSpan; }
				| onePlusFormals	{ $$ = $1; $$.Location = CurrentLocationSpan; }
				;
          

onePlusFormals	: formal							{ $$ = new ASTFormalList($1, new ASTFormalList()); $$.Location = CurrentLocationSpan; }
				| formal COMMA onePlusFormals		{ $$ = new ASTFormalList($1, $3); $$.Location = CurrentLocationSpan; }
				;
				
formalModifier	: READONLY	
				;
				
formal			: formalModifier type IDENTIFIER	{ $$ = new ASTFormal($1.Value, $2, $3.Value); $$.Location = CurrentLocationSpan; }
				| type IDENTIFIER					{ $$ = new ASTFormal($1, $2.Value); $$.Location = CurrentLocationSpan; }
				;          


expression		: expression AND  expression		{ $$ = new ASTAnd($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression OR expression			{ $$ = new ASTOr($1, $3); $$.Location = CurrentLocationSpan; } 
		        | expression SMALLER expression		{ $$ = new ASTSmaller($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression GREATER expression		{ $$ = new ASTGreater($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression SMEQ expression		{ $$ = new ASTSmallerEqual($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression GTEQ expression		{ $$ = new ASTGreaterEqual($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression EQ expression			{ $$ = new ASTEqual($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression NEQ expression			{ $$ = new ASTNotEqual($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression PLUS expression		{ $$ = new ASTAdd($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression MINUS expression		{ $$ = new ASTSubtract($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression TIMES expression		{ $$ = new ASTMultiply($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression DIVIDE expression		{ $$ = new ASTDivide($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression MOD expression			{ $$ = new ASTModulo($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression EXP expression			{ $$ = new ASTExponent($1, $3); $$.Location = CurrentLocationSpan; } 
				| expression AMP expression			{ $$ = new ASTConcatenate($1, $3); $$.Location = CurrentLocationSpan; } 
				| literal							{ $$ = $1; $$.Location = CurrentLocationSpan; }
				| MINUS expression	%prec UMINUS	{ $$ = new ASTNegative($2); $$.Location = CurrentLocationSpan; } 
				| NOT expression					{ $$ = new ASTNot($2); $$.Location = CurrentLocationSpan; } 
				| expression INCREMENT				{ $$ = new ASTIncrement($1); $$.Location = CurrentLocationSpan; }
				| expression DECREMENT				{ $$ = new ASTDecrement($1); $$.Location = CurrentLocationSpan; }
				| LPAREN expression RPAREN			{ $$ = $2; $$.Location = CurrentLocationSpan; } 
				| lvalue							{ $$ = $1; $$.Location = CurrentLocationSpan; }
				| rvalue							{ $$ = $1; $$.Location = CurrentLocationSpan; }
				| NEW IDENTIFIER LPAREN actuals RPAREN		{ $$ = new ASTInstantiateClass($2.Value, $4); $$.Location = CurrentLocationSpan; }
				| NEW type LBRACKET expression DOTDOT expression RBRACKET 
						{ $$ = new ASTInstantiateArray($2, $4, $6); $$.Location = CurrentLocationSpan; }
				| NEW type LBRACKET expression RBRACKET { $$ = new ASTInstantiateArray($2, $4, new ASTInteger(0)); $$.Location = CurrentLocationSpan; }
				;
	     

lvalue			: IDENTIFIER						{ $$ = new ASTIdentifier(CurrentLocationSpan, $1.Value); $$.Location = CurrentLocationSpan; }
				| lvalue LBRACKET expression RBRACKET			{ $$ = new ASTDereferenceArray($1, $3); $$.Location = CurrentLocationSpan; }
				| lvalue DOT IDENTIFIER							{ $$ = new ASTDereferenceField($1, $3.Value); $$.Location = CurrentLocationSpan; }
				| SELF								{ $$ = new ASTSelf(); $$.Location = CurrentLocationSpan; }
				| BASE								{ $$ = new ASTBase(); $$.Location = CurrentLocationSpan; }
				;

rvalue			: IDENTIFIER LPAREN actuals RPAREN 
								{ $$ = new ASTInvoke(new ASTSelf(), $1.Value, $3); $$.Location = CurrentLocationSpan; }
				| lvalue DOT IDENTIFIER LPAREN actuals RPAREN	{ $$ = new ASTInvoke($1, $3.Value, $5); $$.Location = CurrentLocationSpan; }
				;

actuals			:					{ $$ = new ASTExpressionList(); $$.Location = CurrentLocationSpan; }
				| onePlusActuals	{ $$ = $1; $$.Location = CurrentLocationSpan; }
				;
          

onePlusActuals	: expression						{ $$ = new ASTExpressionList($1, new ASTExpressionList()); $$.Location = CurrentLocationSpan; }
				| expression COMMA onePlusActuals	{ $$ = new ASTExpressionList($1, $3); $$.Location = CurrentLocationSpan; }
				;


type			: integralType				{ $$ = $1; $$.Location = CurrentLocationSpan; }
				| integralType PBRACKET		{ $$ = new ASTTypeArray($1); $$.Location = CurrentLocationSpan; }
				;
				
					     
integralType	: TINT			{ $$ = new ASTTypeInt(); $$.Location = CurrentLocationSpan; }
				| TBOOL			{ $$ = new ASTTypeBool(); $$.Location = CurrentLocationSpan; }
		        | TVOID			{ $$ = new ASTTypeVoid(); $$.Location = CurrentLocationSpan; }
				| TSTRING		{ $$ = new ASTTypeString(); $$.Location = CurrentLocationSpan; }
				| TREAL			{ $$ = new ASTTypeReal(); $$.Location = CurrentLocationSpan; }
				| IDENTIFIER	{ $$ = new ASTTypeClass($1.Value); $$.Location = CurrentLocationSpan; }
				;


literal			: LITERAL_INT		{ $$ = new ASTInteger(Int32.Parse($1.Value)); $$.Location = CurrentLocationSpan; } 
				| LITERAL_REAL		{ $$ = new ASTReal(Double.Parse($1.Value)); $$.Location = CurrentLocationSpan; } 
				| LITERAL_STRING	{ $$ = new ASTString($1.Value); $$.Location = CurrentLocationSpan; } 
				| TRUE				{ $$ = new ASTBoolean(true); $$.Location = CurrentLocationSpan; }
				| FALSE				{ $$ = new ASTBoolean(false); $$.Location = CurrentLocationSpan; }
				;
	
%%

public ASTStatementList SyntaxTreeRoot { get; set; }

public Parser(Scanner scan) : base(scan)
{
}
