
namespace Shipico.BehaviourTrees
{
    public class DelaySeconds : DecoratorNode
    {
        public float Seconds;

        private float _timer;

        protected override void OnEnter()
        {
            _timer = 0f;
        }

        protected override Status OnUpdate(float deltaTime)
        {
            if (_timer <= Seconds)
            {
                _timer += deltaTime;
                return Status.Running;
            }

            return Child.UpdateNode(Blackboard, Tree, deltaTime);
        }

        public override TreeNode Clone()
        {
            var clone = Instantiate(this);
            clone.Seconds = Seconds;
            return clone;
        }
    }
}