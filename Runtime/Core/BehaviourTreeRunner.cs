using UnityEngine;

namespace Shipico.BehaviourTrees
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviourTree tree;
        [Tooltip("By default, tree asset will be cloned for each runner. In develop mode it will be using original asset, so you can modify it on the fly")]
        [SerializeField] private bool developMode;
        public BehaviourTree CurrentTree => tree;

        private void Awake()
        {
            if (!developMode)
            {
                tree = tree.Clone();
            }
        }

        public void SetTree(BehaviourTree newTree)
        {
            if (tree)
            {
                tree.Interrupt();
            }

            tree = newTree;
            tree.Init();
        }

        private void Update()
        {
            if (tree)
            {
                tree.UpdateTree(Time.deltaTime);
            }
        }

        private void OnDisable()
        {
            if (tree)
            {
                tree.Interrupt();
            }
        }
    }
}