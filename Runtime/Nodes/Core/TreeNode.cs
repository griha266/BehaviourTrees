using UnityEngine;

namespace Shipico.BehaviourTrees
{
    public abstract class TreeNode : ScriptableObject, ICloneable<TreeNode>
    {
        public string Id;
        public Vector2 GrahpPosition;
        
        public enum Status
        {
            Failure,
            Running,
            Success,
        }
        
        protected Blackboard Blackboard { get; private set; }
        protected BehaviourTree Tree { get; private set; }

        public void ResetValue()
        {
            CurrentStatus = Status.Failure;
        }

        public Status UpdateNode(Blackboard blackboard, BehaviourTree owner, float deltaTime)
        {
            if (CurrentStatus != Status.Running)
            {
                Blackboard = blackboard;
                Tree = owner;
                OnEnter();
                IsRunning = true;
                CurrentStatus = Status.Running;
            }
            else
            {
                CurrentStatus = OnUpdate(deltaTime);
                if (CurrentStatus != Status.Running)
                {
                    IsRunning = false;
                    OnExit(false);
                }
            }

            return CurrentStatus;
        }

        public bool IsRunning { get; private set; }

        public void Abort()
        {
            CurrentStatus = Status.Failure;
            IsRunning = false;
            OnExit(true);
        }

        public Status CurrentStatus { get; private set; }

        protected virtual void OnEnter() { }
        protected abstract void OnExit(bool cancelled);
        protected abstract Status OnUpdate(float deltaTime);

        public abstract TreeNode Clone();
    }
}