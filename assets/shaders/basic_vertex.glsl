#version 330 core
// input
layout(location = 0) in vec2 position;
layout(location = 1) in vec3 colour;
layout (location = 2) in vec2 uv;
// output
out vec3 vertex_colour;
out vec2 texture_coord;

void main()
{
    gl_Position = vec4(position, 0.0, 1.0);
    vertex_colour = colour;
    texture_coord = uv;
}