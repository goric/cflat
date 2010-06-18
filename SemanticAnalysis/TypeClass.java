package sem;

import java.util.Hashtable;
import java.util.Vector;

/**
 * It inherits from Typer. It stores information of 
 * a class.
 * 
 * @author Laurent Michel
 *
 */
public class TypeClass extends Type {
	/**
	 * The name of the Class
	 * @uml.property  name="_name"
	 */
	protected String    _name;
	/**
	 * The parent class (if any)
	 * @uml.property  name="_superClass"
	 * @uml.associationEnd  multiplicity="(1 1)"
	 */
	protected TypeClass _superClass;	
	/**
	 * The list of fields as pairs <Name,Type>
	 * @uml.property  name="_fields"
	 * @uml.associationEnd  qualifier="key:java.lang.String sem.Type"
	 */
	protected Hashtable<String,Type> _fields;
	/**
	 * The list of methods as pairs <Name,SignatureType>
	 * @uml.property  name="_methods"
	 * @uml.associationEnd  qualifier="mName:java.lang.String sem.TypeSignature"
	 */
	protected Hashtable<String,Type> _methods;
	/**
	 * The scope of the class that maps all fields and methods to their respective descriptors.
	 * @uml.property  name="_classScope"
	 * @uml.associationEnd  
	 */
	protected Scope  _classScope;
	/**
	 * The offset of the "next" field that could be added to the class (in bytes from the base address)
	 * This value changes over time as new fields are added in the class. 
	 * @uml.property  name="_fOfs"
	 */
	protected int    _fOfs;
	/**
	 * The number of methods  defined in this class. That number includes the methods defined in the
	 * parent class (& recursively up the hierarchy).
	 * @uml.property  name="_nbMethods"
	 */
	protected int    _nbMethods;
	/**
	 * Constructor. It creates a representation of the type of a class. It stores 
	 * the class name, the parent class (if any, it could be null) and creates 
	 * additional instance variables to keep track of the types of the fields and methods 
	 * defined within the class.
	 * 
	 * The constructor also initialize _fOfs, a field that tracks the amount of space (in bytes) 
	 * needed to store an instance of the class in memory. The start value is 4 bytes to account
	 * for the virtual table pointer. 
	 * 
	 * The constructor finally initializes _nbMethods to the number of methods of the class which
	 * is either 0 (if no parent class) or the number of methods in the parent class.
	 * 
	 * @param n	    the class name
	 * @param parent  the reference to the type describing the parent class.
	 */
	public TypeClass(String n,TypeClass parent) {
		_name = n;
		_superClass = parent;
		_fields = new Hashtable<String,Type>();
		_methods = new Hashtable<String,Type>();
		_fOfs    = 4; // reserve first 4 bytes for virtual table pointer.
		_nbMethods = _superClass==null ? 0 : _superClass.nbMethods();
	}
	/**
	 * Retrieve the names of all fields.
	 * @return an iterable over a set of strings.
	 */
	public Iterable<String> fieldNames() { return _fields.keySet();}
	/**
	 * Retrieve the type of field named n
	 * @param n The field name to retrive
	 * @return its type
	 */
	public Type fieldType(String n) { return _fields.get(n);}
	/**
	 * Retrieves the descriptor of field named n
	 * @param n The field name 
	 * @return its descriptor.
	 */
	public FieldDescriptor fieldDesc(String n) {
		Descriptor fd = _classScope.getSymbol(n);
		if (fd.isField())
			return (FieldDescriptor)fd;
		else return null;
	}
   public void fixNbMethods() {
      if (_superClass != null)
    	  	_nbMethods = _superClass._nbMethods;
      else _nbMethods = 0;
   }
	/**
	 * 
	 * @return A reference to the type descriptor of the parent class (null if no parent)
	 */
	public TypeClass getSuperType() { return _superClass;}
	/**
	 * The addField method records the presence of a new field into the class type.
	 * It records the field name and field type. It also updates the offset of the field
	 * within the class (to be used later on during code generation). 
	 * @param fName  the field name
	 * @param fType  the field type
	 * @return the offset of the field from the instance pointer (serving as a base)
	 */
	public int addField(String fName,Type fType) {
		_fields.put(fName,fType);
		int rv = _fOfs;
		_fOfs += fType.sizeOf();
		return rv;
	}
	/**
	 * The addMethod method records the presence of a method named mName with its type.
	 * If the method is already defined in the super class, this is a method overriding (virtual
	 * method). So we only have to record its presence in this class and return the offset of
	 * the method assigined within the parent class. 
	 * @param mName
	 * @param mType
	 * @return The offset of the method in the vtbl (we are numbering the methods)
	 */
	
	public int addMethod(String mName,Type mType) {
		if (hasMethod(mName,mType)) {
			int ofs = getMethodId(mName,mType);
			_methods.put(mName,mType);			
			return ofs;
		} else {
			_methods.put(mName,mType);
			return _nbMethods++;
		}
	}
	/**
	 * getMethodId retrieves the method number for a method whose name is mName (with type mType).
	 * If the current class contains an entry for this method, simply return its offset found in 
	 * the method descriptor that is available from the classScope. If the method is not present
	 * in this class, we must search for it in the parent class recursively. If the method cannot
	 * be found, we return -1 to indicate a failure (this should not happen)
	 * @param mName
	 * @param mType
	 * @return The method number as assigned by addMethod when it was recorded.
	 */
	protected int getMethodId(String mName,Type mType) {
		if (_methods.containsKey(mName)) {
			Type mt = _methods.get(mName);
			if (mt.equals(mType)) 
				return ((MethodDescriptor)_classScope.getSymbol(mName)).getOffset();
		}
		if (_superClass != null) 
			return _superClass.getMethodId(mName,mType);
		else {
			assert false;
			return -1;
		}
	}
	/**
	 * It simply retrieves the number of the 'special method' that is used as the constructor. 
	 * Same logic as above except that the class must contain a constructor (so we don't search
	 * for it recursively.
	 * @return
	 */
	public int getConstructorId() {
		if (_methods.containsKey(_name)) {
			assert _classScope.getSymbol(_name) != null;
			return ((MethodDescriptor)_classScope.getSymbol(_name)).getOffset();			
		} else return -1;
	}
	/**
	 * Searches for a method of name mName that can 'accept' a call with arguments of types found in 
	 * actuals. A method accepts a call if the types of the actuals are subtypes of the types of the
	 * formals.  If an accepting method is not found in this class, we must search the super class
	 * recursively.
	 * @param mName
	 * @param actuals
	 * @return The method Descriptor of the accepting method.
	 */
	public MethodDescriptor getAcceptingMethod(String mName,Vector<Type> actuals) {
		if (_methods.containsKey(mName)) {
			TypeFunction mt = (TypeFunction)_methods.get(mName);
			if (mt.acceptCall(actuals)) {
				return ((MethodDescriptor)_classScope.getSymbol(mName));				
			} 
		} 
		if (_superClass != null)
			return _superClass.getAcceptingMethod(mName,actuals);
		else return null;			
	}
	/**
	 * This method determines whether a method of name mName and type mType exist for this class.
	 * It must search this class and the parent classes recursively as appropriate.
	 * @param mName
	 * @param mType
	 * @return a boolean.
	 */
	public boolean hasMethod(String mName,Type mType) {
		if (_methods.containsKey(mName)) {
			Type mt = _methods.get(mName);
			return mt.equals(mType);
		} else {
			if (_superClass != null)
				return _superClass.hasMethod(mName,mType);
			else return false;
		}
	}
	/**
	 * Stores the classScope produced during the semantic analysis for this class direcly within
	 * the type for later access.
	 * @param sc
	 */
	public void setScope(Scope sc) { 
		_classScope = sc;
		if (_superClass != null)
			_classScope.setParent(_superClass.getScope());
	}
	/** 
	 * Retrieves the class scope.
	 */
	public Scope getScope() { return _classScope;}
	/** 
	 * @return true if and only if this is a class type.
	 */
	public boolean isClass() { return true;}
	/**
	 * 
	 * @return the number of methods defined in this class.
	 */
	public int nbMethods() { return _nbMethods;}
	/**
	 * @return the space occupied by any instance of this class. This is the sum of the sizes
	 * of all the fields in this class and its super classes (recursively)
	 */
	public int sizeOf() {
		int ttl;
		if (_superClass != null)
			ttl = _superClass.sizeOf();
		else ttl = 4; // to account for the VTBL pointer
		for(String key : _fields.keySet()) {
			ttl += _fields.get(key).sizeOf();
		}			
		return ttl;
	}
	/**
	 * @return A human readable printout of the class type.
	 */
	public String toString() { 
		StringBuffer s = new StringBuffer("class(" + _name + ")");
		return s.toString();		
	}
	/**
	 * @return A boolean that is true if and only the current class is a super-type of class t.
	 * In other words, it returns true if and only if t <: this (t is a subtype of this)
	 */
	protected boolean supertypeOf(TypeClass t) { 
		TypeClass low = t;
		while (low != null) {
			if (this.equals(low))
				return true;
			low = low._superClass;
		}
		return false;
	}
	/**
	 * @return A boolean that is true if and only if this <: t. The method is implemented in terms
	 * of the supertype method above. Note that it only traverse the type hierarchy of the classes.
	 * 
	 */
	public boolean subtype(Type t) {
		return t.supertypeOf(this);
	}
	/**
	 * Equality test.
	 * @return A boolean that is true if and only if the two types are identical, i.e., they 
	 * have the same super class and the same fields.  (structural equality)
	 */
	public boolean equals(Object o) {		
		TypeClass t = (TypeClass)o;
		if (t != null) {
			if (t==this) return true;
			boolean ok = _superClass.equals(t._superClass);
			ok = ok && _name.equals(t._name);
			ok = ok && _fields.equals(t._fields);
			return ok;
		} else return false;		
	}
}
