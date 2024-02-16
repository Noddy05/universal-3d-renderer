#version 330 core

layout (location = 0) in vec3 position;

uniform mat4 projectionMatrix;
uniform mat4 cameraMatrix;

out vec3 vCoordinates;

void main() {
    vCoordinates = position;
    gl_Position = projectionMatrix * cameraMatrix * (vec4(position, 1.0));
}