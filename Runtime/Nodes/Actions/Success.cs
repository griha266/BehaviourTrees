namespace Shipico.BehaviourTrees
{
    public class Success : ActionNode
    {
        protected override void OnExit(bool cancelled)
        {
        }

        protected override Status OnUpdate(float deltaTime)
        {
            return Status.Success;
        }

        public override TreeNode Clone()
        {
            return Instantiate(this);
        }
    }
}