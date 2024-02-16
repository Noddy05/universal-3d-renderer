#version 330 core

in vec2 vTexCoords;

uniform vec4 color;
uniform sampler2D textureSampler;

out vec4 fragmentColor;

void main(){
    //Combine texture and color
    vec4 sampledTexture = texture(textureSampler, vTexCoords);
    vec4 unlitColor = color * sampledTexture;
    fragmentColor = unlitColor;
}