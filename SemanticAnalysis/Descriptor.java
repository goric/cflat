package sem;

import mc.MLocation;
import ir.Location;

/**
 * Descriptor is an abstract class. It is meant to encapsulate the type of the object it 
 * describes together with extra attributes used during code generation. The class is abstract and 
 * several sub-classes are defined to match the needs of each specific objects (classes, fields, 
 * methods, functions and object instances).
 *   
 * @author Laurent
 *
 */
public abstract class Descriptor {
	/**
	 * The type stored in the descriptor. From a semantic checking point of view, this is the only
	 * part that matters. 
	 * @uml.property  name="_type"
	 * @uml.associationEnd  multiplicity="(1 1)"
	 */
	protected Type _type;
	public Descriptor(Type t) { _type = t;}
	public Type getType() { return _type;}
	/**
	 * 
	 * @return true if and only if this descriptor refers to a class. (concrete type is therefore ClassDescriptor)
	 */
	public boolean isType() { return false;}
	/**
	 * 
	 * @return true if and only if this descriptor refers to an object (Concrete type is ObjectDescriptor)
	 */
	public boolean isObject() { return false;}
	/**
	 * 
	 * @return true if and only if this descriptor refers to a field (Concrete type is FieldDescriptor)
	 */
	public boolean isField() { return false;}
	/**
	 * 
	 * @return true if and only if this descriptor refers to a method (Concrete type is MethodDescriptor)
	 */
	public boolean isMethod() { return false;}
	/**
	 * Used for code generation *only*. It retrieve the Location (Memory/Register/Stack) that a name n is in.
	 * @param n name to search for
	 * @return a physical location.
	 */
	public Location getSymbolic(String n) { return null;}
	public abstract MLocation getLocation();
}
