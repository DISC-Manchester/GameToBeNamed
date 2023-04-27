using OpenTK.Graphics.ES30;
namespace SquareSmash.client.renderer.opengl
{
    internal static class Shader
    {
        public sealed class ShaderHandle : IDisposable
        {
            public Dictionary<string, int> Uniforms { get; }
            public int Id { get; private set; }
            public ShaderHandle(int id)
            {
                Id = id;
                Uniforms = new();
            }

            public int GetUniformLocation(string name)
            {
                if (!Uniforms.ContainsKey(name))
                    Uniforms.Add(name, GL.GetUniformLocation(Id, name));
                return Uniforms[name];
            }

            public void Dispose()
            {
                Uniforms.Clear();
            }
        }
        public static ShaderHandle Create(string vertex_source, string fragment_source)
        {
            int id = GL.CreateProgram();
            int vid = GL.CreateShader(ShaderType.VertexShader);
            GL.CompileShader(vid);
            GL.ShaderSource(vid, vertex_source);
            int fid = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fid, fragment_source);
            GL.CompileShader(fid);
            GL.AttachShader(id, vid);
            GL.AttachShader(id, fid);
            GL.LinkProgram(id);
            GL.DeleteShader(vid);
            GL.DeleteShader(fid);
            return new(id);
        }

        public static void Bind(ShaderHandle id) =>
            GL.UseProgram(id.Id);

        public static void Delete(ShaderHandle id)
        {
            GL.DeleteProgram(id.Id);
        }
    }
}
