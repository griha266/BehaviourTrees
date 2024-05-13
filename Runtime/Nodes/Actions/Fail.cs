namespace Shipico.BehaviourTrees
{
    public class Fail : ActionNode
    {
        protected override void OnExit(bool cancelled)
        {
            
        }

        protected override Status OnUpdate(float deltaTime)
        {
            return Status.Failure;
        }
    }
}