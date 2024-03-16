#version 330 core

layout (location = 0) in vec3 position;

uniform mat4 projectionMatrix;
uniform mat4 transformationMatrix;

void main() {
    vec3 worldPosition = (transformationMatrix * vec4(position, 1)).xyz;
    gl_Position = projectionMatrix * vec4(worldPosition, 1);
}