#version 330 core


layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;

uniform mat4 projectionMatrix;
uniform mat4 transformationMatrix;
uniform mat4 cameraMatrix;
uniform float outlineThickness;

void main() {
    vec3 worldPosition = (transformationMatrix * vec4(position + normal * outlineThickness, 1)).xyz;
    gl_Position = projectionMatrix * cameraMatrix * vec4(worldPosition, 1);
}