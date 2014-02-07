using NunitUtils.Constraints;

namespace NunitUtils
{
    public static class Looks
    {
        public static LikeConstraint<T> Like<T>(T expected)
        {
            return new LikeConstraint<T>(expected);
        }
    }
}
