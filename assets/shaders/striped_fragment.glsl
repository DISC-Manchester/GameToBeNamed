#version 330 core
// input
in vec3 vertex_colour;
in vec2 texture_coord;
// output
out vec4 frag_colour;

void main()
{
        frag_colour = vec4(vertex_colour,1.0);
}