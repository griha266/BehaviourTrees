namespace Shipico.BehaviourTrees
{
    public static class NormalizedFloatExtensions
    {
        public static NormalizedFloat Normalize(this float value, float max, float min = 0) => NormalizedFloat.Create(value, min, max);
    }
}