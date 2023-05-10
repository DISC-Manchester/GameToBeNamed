using OpenTK.Graphics.ES30;
using static OpenTK.Graphics.ES30.All;
namespace SquareSmash.renderer.opengl
{
    public enum DataType
    {
        FLOAT,
        INT,
        UNSIGNED_INT,
        FLOAT_TWO,
        INT_TWO,
        UNSIGNED_INT_TWO,
        FLOAT_THREE,
        INT_THREE,
        UNSIGNED_INT_THREE,
    }

    internal class VertexArray
    {
        private readonly int Id;
        public VertexArray() 
            => Id = GL.GenVertexArray();

        public void Bind()
            => GL.BindVertexArray(Id);

        public static void UnBind()
            => GL.BindVertexArray(0);

        ~VertexArray()
            => GL.DeleteVertexArray(Id);
    }
}
