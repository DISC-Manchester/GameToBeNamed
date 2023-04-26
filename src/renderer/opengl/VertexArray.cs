using OpenTK.Graphics.ES30;
namespace GameToBeNamed.src.renderer.opengl
{
    internal static class VertexArray
    {
        //gen vertex array
        public static int Gen() =>
            GL.GenVertexArray();
        //config bond buffer vertex array
        public static void EnableAttrib(int id) =>
            GL.EnableVertexAttribArray(id);
        public static void DisableAttrib(int id) =>
            GL.DisableVertexAttribArray(id);
        public static void AttribPointer(int id, int size, VertexAttribPointerType type, int stride, nint offset) =>
            GL.VertexAttribPointer(id, size, type, false, stride, offset);
        //bind vertex array
        public static void Bind(int id) =>
           GL.BindVertexArray(id);
        //draw
        public static void DrawVertex(int count) =>
            GL.DrawArrays(PrimitiveType.Triangles, 0, count);
        public static void DrawVertexInstance(int count,int instance_count) =>
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, count, instance_count);
        public static void DrawElements(int count) =>
            GL.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, 0);
        //delete vertex array
        public static void Delete(int id) =>
            GL.DeleteVertexArray(id);
    }
}
