
public abstract class HeroState    
{
    protected HeroStateMachine heroStateMachine;
    public HeroState(HeroStateMachine HeroStateMachine)
    {
        this.heroStateMachine = HeroStateMachine;
    }
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void FrameFixedUpdate() { }
}
