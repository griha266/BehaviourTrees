using UnityEngine.UIElements;

namespace Shipico.BehaviourTrees.Editor
{
    // Just to be able to use it in UI Builder, should be extracted to common library
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
    }
}