package sem;

import java.util.Map;
import java.util.TreeMap;
import java.util.Vector;

/**
 * It inherits from Type. It stores the signature information 
 * of a method or constructor, e.g., return type and scope 
 * information. A TypeFunction is the type of a Method (or constructor)
 * 
 * Example:
 * A method  int foo(int n,Bar x) { ... }
 *  
 * where Bar is another class, would have a type signature
 * 
 * foo : TypeFunction(n : TypeInt, x : TypeInstance(Bar) -> TypeInt
 * 
 * i.e., foo is of type "a function FROM Int x Instance(Bar)  TO Int" 
 * 
 * @author Laurent Michel
 *
 */
public class TypeFunction extends Type {
	protected Vector<Type> _fieldTypes;
	protected Vector<String> _fieldNames;
	/**
	 * @uml.property  name="_fMap"
	 * @uml.associationEnd  qualifier="name:java.lang.String java.lang.Integer"
	 */
	protected Map<String,Integer> _fMap;
	/**
	 * @uml.property  name="_retType"
	 * @uml.associationEnd  
	 */
	protected Type _retType;
	/**
	 * @uml.property  name="_theScope"
	 * @uml.associationEnd  
	 */
	protected Scope  _theScope;
	/**
	 * Constructor. Creates an empty type function for a method/constructor/function
	 */
	public TypeFunction() {
		_fieldTypes = new Vector<Type>();
		_fieldNames = new Vector<String>();	
		_fMap = new TreeMap<String,Integer>();
	}
	/**
	 * Initializes the return type of the method/function ... -> t
	 * @param t the return type of the method/function.
	 */
	public void setReturnType(Type t) { _retType = t;}
	/**
	 * Add a new formal at the end of the list of formals in the method/function to obtain
	 * a type equal to (N0:F0,N1:F1,....,Nk:Fk,Name:Type) -> _retType 
	 * where Ni is the name of the ith formal and Fi is the type of the ith formal.  
	 * @param name name of new formal
	 * @param type type of new formal.
	 */
	public void addFormal(String name,Type type) {
		_fieldTypes.add(type);
		_fieldNames.add(name);
		int idx = _fieldTypes.size() - 1;
		_fMap.put(name,idx);
	}
	/**
	 * always return true, since this denotes a method.
	 */
	public boolean isFunction() { return true;}
	/**
	 * Arity
	 * @return returns the number of FORMALS in the signature.
	 */
	public int getArity() { return _fieldTypes.size();}
	/**
	 * Returns the name of the idx-th formal
	 * @param idx  number of formal to query
	 * @return its name
	 */
	public String getNameOf(int idx) { return _fieldNames.get(idx);}
	/**
	 * Returns the type of the idx-th formal
	 * 
	 * @param idx number of formal to query
	 * @return its name
	 */
	public Type getTypeOf(int idx) { return _fieldTypes.get(idx);}
	/**
	 * Retrieves the type of a formal _by name_. 
	 * @param n name of formal to retrieve
	 * @return type of retrieved formal.
	 */
	public Type getTypeFor(String n) { return _fieldTypes.get(_fMap.get(n));}
	/**
	 * Accessor
	 * @return retrieves the return type of the signature. 
	 */
	public Type getReturnType() { return _retType;}
	/** 
	 * Records the scope produces during semantic analysis for this signature.
	 * @param s
	 */
	public void setScope(Scope s) { _theScope = s;}
	/**
	 * Accessor
	 * @return returns the scope of the signature.
	 */
	public Scope getScope() { return _theScope;}
	/**
	 * Produces a human friendly printout of the signature.
	 */
	public String toString() {
		StringBuffer sb = new StringBuffer(32);
		boolean first = true;
		sb.append("(");
		for(int k=0;k<_fieldNames.size();k++) {
			sb.append(first ? "" : ",");
			sb.append(_fieldNames.elementAt(k));
			sb.append(" : ");
			sb.append(_fieldTypes.elementAt(k).toString());
			//sb.append(" [desc:" + _theScope.getSymbol(_fieldNames.elementAt(k)).toString());
			//sb.append("]");
			first = false;
		}
		sb.append(") -> ");
		sb.append(_retType.toString());
		//sb.append("scope " + _theScope.toStringFlat());		
		return sb.toString();
	}
	/**
	 * Equality test. This method determines whether two function types are identical. i.e., same
	 * arity, same type for all the formals. (The return type is not used in the equality test)
	 * @param s
	 * @return true if and only if the two function types expect the same formals (type wise)
	 */
	public boolean equals(TypeFunction s) {
		int a0 = getArity();
		int a1 = s.getArity();
		if (a0 != a1) 
			return false;
		else {
			for(int k=0;k<a0;k++) {
				if (!getTypeOf(k).equals(s.getTypeOf(k)))
					return false;
			}
			return true;
		}
	}
	/**
	 * Subtyping test. The method returns true if and only if 
	 * - The two function types have the same number of formals
	 * - The formals of this are subtypes of the formals of t. Note that this applies 
	 * contra-variance. 
	 * 
	 * Note that the return type is not used in the test either.
	 * @param t the type function we are testing against
	 */
	protected boolean supertypeOf(TypeFunction t) {
		int a0 = getArity();
		int a1 = t.getArity();
		if (a0 != a1) 
			return false;
		else {
			for(int k=0;k<a0;k++) {
				// contra-variance in argument position.
				if (!getTypeOf(k).subtype(t.getTypeOf(k)))
					return false;
			}		
			return true;
		}
	}
	/**
	 * The 'usual' subtyping test. expressed in terms  of superTypeOf. 
	 */
	public boolean subtype(Type t) {
		return t.supertypeOf(this);
	}
	/**
	 * This method determines whether the type function can accept a call with arguments whose
	 * types are given in the input argument at. The call is acceptable if the type of each argument 
	 * is a subtype of the corresponding formal's type.
	 * @param at a vector with the types of the arguments
	 * @return true if the call is acceptable.
	 */
	public boolean acceptCall(Vector<Type> at) {
		boolean ok = true;
		if (at.size() == getArity()) {
			for(int k=0;ok && k<getArity();k++) 
				ok = at.elementAt(k).subtype(getTypeOf(k));			
		} else ok = false;		
		return ok;
	}
	/**
	 * Equality test. The method determines whether the type function received in argument is the same 
	 * type, i.e., a signature with the same arity, same return type and same argument types.
	 * @return true if and only if both types are equal.
	 */
	public boolean equals(Object o) {
		TypeFunction t = (TypeFunction)o;
		if (t!=null) {
			if (t == this) return true;
			if (getArity() != t.getArity()) return false;
			boolean ok = _retType.equals(t._retType);
			for(int k=0;ok && k < getArity();k++) {
				ok = getTypeOf(k).equals(t.getTypeOf(k));
				ok = ok && getNameOf(k).equals(t.getNameOf(k));
			}
			return ok;
		} else return false;
	}
}
