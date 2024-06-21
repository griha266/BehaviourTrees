namespace Shipico.BehaviourTrees
{
    public class RepeatForever : DecoratorNode
    {
        protected override Status OnUpdate(float deltaTime)
        {
            Child.UpdateNode(Blackboard, Tree, deltaTime);
            return Status.Running;
        }

        public override TreeNode Clone()
        {
            return Instantiate(this);
        }
    }
}