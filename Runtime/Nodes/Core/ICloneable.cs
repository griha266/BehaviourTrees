namespace Shipico.BehaviourTrees
{
    public interface ICloneable<out TCloneType>
    {
        TCloneType Clone();
    }
}