using OpenTK.Mathematics;

namespace SquareSmash.client.Utils
{
    internal static class CollisionUtil
    {
        public static bool DoseIntersects(Vector2 position, Vector2 size, Vector2 position_2, Vector2 size_2)
        {
            float leftA = position.X;
            float rightA = position.X + size.X;
            float topA = position.Y;
            float bottomA = position.Y + size.Y;

            float leftB = position_2.X;
            float rightB = position_2.X + size_2.X;
            float topB = position_2.Y;
            float bottomB = position_2.Y + size_2.Y;

            return !(leftA > rightB || rightA < leftB || topA > bottomB || bottomA < topB);
        }
    }
}
