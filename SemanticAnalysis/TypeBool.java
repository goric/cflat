package sem;

/**
 * TypeBool is a subtype of TypeInt. 
 * 
 * @author Laurent
 *
 */
public class TypeBool extends TypeInt {
	protected boolean supertypeOf(TypeInt t)  { return false;}
	protected boolean supertypeOf(TypeBool t) { return true;}
	public String toString() { return "bool";}
	public int sizeOf() { return 4;}
	/**
	 * This function checks whether a TypeBool object is a subtype of another object
	 * 
	 * @param t: a Type object
	 * @return true if t is a TypeInt object  or a TypeBool object, and false otherwise
	 */
	public boolean subtype(Type t) { 
		return t.supertypeOf(this);
	}
}
