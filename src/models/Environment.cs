using System.Collections.Generic;


// TODO handle types and other here?
class Environment {

    private Dictionary<string, Variable> environment = new Dictionary<string, Variable>();

    public bool Initialize(string ident, string type, object value) {
        return environment.TryAdd(ident, new Variable(type, value));
    }

    public bool Declare(string ident, string type) {
        return environment.TryAdd(ident, new Variable(type, null));
    }

    public bool Assign(string ident, object value) {
        if (!environment.ContainsKey(ident)) {
            return false;
        }
        environment[ident].Value = value;
        return true;
    }

    public object Get(string ident) {
        return environment[ident].Value;
    }

    public string GetType(string ident) {
        return environment[ident].Type;
    }

    public bool Contains(string ident) {
        return environment.ContainsKey(ident);
    }

    public void setLock(string ident, bool state) {
        environment[ident].Locked = state;
    }

    public bool isLocked(string ident) {
        return environment[ident].Locked;
    } 
}