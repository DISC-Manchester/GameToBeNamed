using OpenTK.Mathematics;

namespace SquareSmash.utils
{
    internal static class CollisionUtil
    {

        public static void ToWorldSpace(ref Vector2 position, ref Vector2 size)
        {
            position.X = (position.X - Client.Instance.Size.X / 2f) / (Client.Instance.Size.X / 2f);
            position.Y = (Client.Instance.Size.Y / 2f - position.Y) / (Client.Instance.Size.Y / 2f);

            size.X /= Client.Instance.Size.X;
            size.Y /= Client.Instance.Size.Y;
        }

        public static bool DoseFullIntersects(Vector2 position, Vector2 size, Vector2 position_2, Vector2 size_2)
        {
            ToWorldSpace(ref position, ref size);
            ToWorldSpace(ref position_2, ref size_2);
            // collision x-axis?
            bool collisionX = position.X + size.X >= position_2.X &&
                position_2.X + size_2.X >= position.X;
            // collision y-axis?
            bool collisionY = position.Y + size.Y >= position_2.Y &&
                position_2.Y + size_2.Y >= position.Y;
            return collisionX && collisionY;
        }
    }
}
