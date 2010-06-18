package sem;

/**
 * TypeInt is a child class of Type. 
 * 
 * @author Laurent
 *
 */
public class TypeInt extends Type {
	/**
	 * int <: int
	 * @return true always
	 */
	protected boolean supertypeOf(TypeInt t)  { return true;} // reflexivity
	/**
	 * bool <: int
	 * @return true always
	 */
	protected boolean supertypeOf(TypeBool t) { return true;}
	public String toString() { return "int";}
	public int sizeOf() { return 4;}
	/**
	 * int <: t 
	 * 
	 * @param t: a Type object
	 * @return true if t is a supertype of int.
	 */
	public boolean subtype(Type t) { 
		return t.supertypeOf(this);
	}
}
