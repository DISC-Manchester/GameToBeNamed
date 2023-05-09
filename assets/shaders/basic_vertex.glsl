#version 330 core
// input
layout(location = 0) in vec2 position;
layout(location = 1) in vec4 colour;
// output
out vec4 vertex_colour;
void main()
{
    gl_Position = vec4(position, 0.0, 1.0);
    vertex_colour = colour;
}