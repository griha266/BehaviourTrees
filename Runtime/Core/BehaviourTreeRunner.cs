using UnityEngine;

namespace Shipico.BehaviourTrees
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviourTree tree;
        [Header("Used for runtime debugging and editing")]
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