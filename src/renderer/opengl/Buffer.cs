using OpenTK.Graphics.ES30;
namespace GameToBeNamed.src.renderer.opengl
{
    internal static class Buffer
    {
        //gen buffer
        public static int Gen() => 
            GL.GenBuffer();
        //bind buffer
        public static void VertexBind(int id) => 
            GL.BindBuffer(BufferTarget.ArrayBuffer,id);
        public static void ElementBind(int id) => 
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
        //data buffer
        public static void VertexData(float[] data, bool dynamic) =>
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
        public static void ElementData(uint[] data, bool dynamic) =>
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
        //sub data buffer
        public static void VertexSubData(nint offset,float[] data) =>
            GL.BufferSubData(BufferTarget.ArrayBuffer, offset, data.Length * sizeof(float), data);
        public static void ElementSubData(nint offset, uint[] data) =>
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, offset, data.Length * sizeof(uint), data);
        //delete buffer
        public static void Delete(int id) =>
            GL.DeleteBuffer(id);
    }
}
