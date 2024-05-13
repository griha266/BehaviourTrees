using UnityEngine;

namespace Shipico.BehaviourTrees
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviourTree tree;

        public BehaviourTree CurrentTree => tree;

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