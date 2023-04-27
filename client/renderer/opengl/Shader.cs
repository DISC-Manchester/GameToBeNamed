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
            int vid = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vid, vertex_source);
            GL.CompileShader(vid);
            string vertexShaderInfoLog = GL.GetShaderInfoLog(vid);

            if (!string.IsNullOrWhiteSpace(vertexShaderInfoLog))
                throw new ApplicationException($"Vertex shader compilation failed: {vertexShaderInfoLog}");

            int fid = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fid, fragment_source);
            GL.CompileShader(fid);
            string fragmentShaderInfoLog = GL.GetShaderInfoLog(fid);

            if (!string.IsNullOrWhiteSpace(fragmentShaderInfoLog))
                throw new ApplicationException($"Fragment shader compilation failed: {fragmentShaderInfoLog}");

            int id = GL.CreateProgram();
            GL.AttachShader(id, vid);
            GL.AttachShader(id, fid);
            GL.LinkProgram(id);
            string programLinkInfoLog = GL.GetProgramInfoLog(id);

            if (!string.IsNullOrWhiteSpace(programLinkInfoLog))
                throw new ApplicationException($"Shader program linking failed: {programLinkInfoLog}");

            GL.DetachShader(id, vid);
            GL.DetachShader(id, fid);
            GL.DeleteShader(vid);
            GL.DeleteShader(fid);
            GL.ReleaseShaderCompiler();

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
