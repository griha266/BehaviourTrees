namespace Shipico.BehaviourTrees
{
    public class RepeatCount : DecoratorNode
    {
        public int Count;

        private int _counter;

        protected override void OnEnter()
        {
            _counter = 0;
        }

        protected override Status OnUpdate(float deltaTime)
        {
            var result = Child.UpdateNode(Blackboard, Tree, deltaTime);
            if (result == Status.Running)
            {
                return Status.Running;
            }
            
            _counter++;
            return _counter >= Count ? result : Status.Running;

        }

        public override TreeNode Clone()
        {
            var clone = Instantiate(this);
            clone.Count = Count;
            return clone;
        }
    }
}