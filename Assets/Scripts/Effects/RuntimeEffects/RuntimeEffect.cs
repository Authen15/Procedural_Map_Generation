public abstract class RuntimeEffect
{
    public abstract void Apply(Creature self);

    // return true if the effect is still going
    public abstract bool Tick(Creature self);
}