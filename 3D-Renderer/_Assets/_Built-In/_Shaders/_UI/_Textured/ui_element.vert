#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 textureCoords;

uniform mat4 transformationMatrix;

out vec2 vTexCoords;

void main() {
    vec3 worldPosition = (transformationMatrix * vec4(position, 1)).xyz;
    gl_Position = vec4(worldPosition, 1);

    vTexCoords = textureCoords;
}