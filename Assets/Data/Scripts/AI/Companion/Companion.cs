public class Companion : StateBehaviour<CompanionState>
{
    private void Start()
    {
        ChangeState(CompanionState.Idle);
    }
}
