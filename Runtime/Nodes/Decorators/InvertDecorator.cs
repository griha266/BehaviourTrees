using System;

namespace Shipico.BehaviourTrees
{
    public class Invert : DecoratorNode
    {
        protected override Status OnUpdate(float deltaTime)
        {
            var result = Child.UpdateNode(Blackboard, Tree, deltaTime);
            return result switch {
                Status.Running => Status.Running,
                Status.Success => Status.Failure,
                Status.Failure => Status.Success,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override TreeNode Clone()
        {
            return Instantiate(this);
        }
    }
}