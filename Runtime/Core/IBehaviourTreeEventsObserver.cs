namespace Shipico.BehaviourTrees
{
    public interface IBehaviourTreeEventsObserver<in T>
    {
        void OnNextEvent(T newEvent);
    }
}