#version 330 core
// input
layout(location = 0) in vec2 position;
layout(location = 1) in float colour;
layout (location = 2) in vec2 uv;
// output
out vec3 vertex_colour;
out vec2 texture_coord;

//colour look up
vec3 colours[] = vec3[](vec3(255,255,255),vec3(157, 6, 241),vec3(255, 127, 80),vec3(251, 0, 250),vec3(0, 192, 237),vec3(249, 185, 0),vec3(0, 238, 0));

void main()
{
    gl_Position = vec4(position, 0.0, 1.0);
    vertex_colour = normalize(colours[int(colour)]);
    texture_coord = uv;
}