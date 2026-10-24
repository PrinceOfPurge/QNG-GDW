


public abstract class Command
{

        public abstract void Execute();
    public abstract void Undo(); //char custimazation
    public virtual string Description => GetType().Name; // this will be used to change

    public virtual bool TryMerge(Command next) => false;
    
    
    
}
