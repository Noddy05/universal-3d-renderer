#version 330 core

layout (location = 0) in vec3 position;
layout (location = 3) in mat4 instanceTransformation;

uniform mat4 projectionMatrix;
uniform mat4 transformationMatrix;

void main() {
    vec3 worldPosition = (instanceTransformation * transformationMatrix * vec4(position, 1)).xyz;
    gl_Position = projectionMatrix * vec4(worldPosition, 1);
}