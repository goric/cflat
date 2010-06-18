package sem;

/**
 * TypeVoid is a type to represent the absence of type. Typically used to type statements
 * or methods that return nothing. 
 * 
 * @author Laurent
 *
 */
public class TypeVoid extends Type {
	protected boolean supertypeOf(TypeVoid t) { return true;} // reflexivity
	public String toString() { return "void";}
	public boolean subtype(Type t) { 
		return t.supertypeOf(this);
	}
}
