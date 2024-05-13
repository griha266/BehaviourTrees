namespace Shipico.BehaviourTrees
{
    public class SelectorNode : ComposeNode
    {
        private int _currentNodeIndex;

        protected override void OnEnter()
        {
            _currentNodeIndex = 0;
        }

        protected override void OnExit(bool cancelled)
        {
            if (cancelled && _currentNodeIndex >= Children.Count)
            {
                Children[_currentNodeIndex].Abort();
            }
        }

        protected override Status OnUpdate(float deltaTime)
        {
            var currentNode = Children[_currentNodeIndex];
            var result = currentNode.UpdateNode(Blackboard, Tree, deltaTime);
            switch (result)
            {
                case Status.Success:
                    return Status.Success;
                case Status.Failure:
                {
                    _currentNodeIndex++;
                    if (_currentNodeIndex >= Children.Count)
                    {
                        return Status.Failure;
                    }

                    return Status.Running;
                }
                default:
                    return Status.Running;
            }
        }
    }
}