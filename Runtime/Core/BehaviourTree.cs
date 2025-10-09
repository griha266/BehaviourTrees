using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shipico.BehaviourTrees
{
    [CreateAssetMenu(menuName = "BehaviourTrees/Create", fileName = "BehaviourTree", order = 0)]
    public class BehaviourTree : ScriptableObject
    {
        private readonly ObserversCollection _observers = new();
        
        public TreeNode rootNode;
        public List<TreeNode> nodes;
        
        public bool IsInitialized => Blackboard != null;
        public Blackboard Blackboard { get; private set; }
        public BehaviourTree ParentTree { get; private set; }

        public event Action OnTreeTick;
        public event Action OnTreeStopped;
        
        public void Init(BehaviourTree parentTree = null, Blackboard parentBlackboard = null)
        {
            Blackboard = new Blackboard(parentBlackboard);
            ParentTree = parentTree;
            foreach (var node in nodes)
            {
                node.ResetValue();
            }
        }
        
        public TreeNode.Status UpdateTree(float deltaTime)
        {
            var result = rootNode.UpdateNode(Blackboard, this, deltaTime);
            OnTreeTick?.Invoke();
            return result;
        }

        public void Interrupt()
        {
            rootNode.Abort();
            OnTreeStopped?.Invoke();
            foreach (var node in nodes)
            {
                node.ResetValue();
            }
        }

        public IDisposable Subscribe<TEvent>(IBehaviourTreeEventsObserver<TEvent> observer)
        {
            return _observers.AddObserver(observer);
        }

        public void PushEvent<TEvent>(TEvent eventData)
        {
            _observers.PushEvent(eventData);
            if (ParentTree)
            {
                ParentTree.PushEvent(eventData);
            }
        }

        // TODO separate asset from tree model
        public BehaviourTree Clone()
        {
            var clone = Instantiate(this);
            var rootClone = rootNode.Clone();
            clone.rootNode = rootClone;
            return clone;
        }

    }
}