#version 330 core

// input
in vec3 vertex_colour;
in vec2 texture_coord;
uniform sampler2D mask;
// output
out vec4 frag_colour;

void main()
{
    precision highp float;

    vec4 texColor = texture2D(mask, texture_coord);

    if (texColor.a < 0.9)
        discard;
    frag_colour = vec4(vertex_colour,texColor.a);
}