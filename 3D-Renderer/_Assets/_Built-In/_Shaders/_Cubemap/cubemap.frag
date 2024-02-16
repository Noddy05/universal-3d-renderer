#version 330 core

in vec3 vCoordinates;

uniform samplerCube textureSampler;

out vec4 fragmentColor;

void main(){
    vec4 skyboxSampled = texture(textureSampler, vCoordinates);
    fragmentColor = skyboxSampled;
}