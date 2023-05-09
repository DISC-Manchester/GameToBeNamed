#version 330 core
// input
in vec4 vertex_colour;
// output
out vec4 frag_colour;
void main()
{
    frag_colour = vertex_colour;
}