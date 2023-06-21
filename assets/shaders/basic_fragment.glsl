#version 330 core

// input
in vec3 vertex_colour;
in vec2 texture_coord; //mask texture_coord
uniform sampler2D mask;
// output
out vec4 frag_colour;

void main()
{
        precision highp float;

        vec4 texColor = texture2D(mask, texture_coord);

        if (texColor.a < 0.9)
            discard;

        float grayscale = dot(texColor.rgb, vec3(0.2126, 0.7152, 0.0722));
        vec3 grayscaleColor = vec3(grayscale, grayscale, grayscale);
        
        frag_colour = vec4(vertex_colour - grayscaleColor,texColor.a);
}