#version 330 core

#include "C:/Users/noah0/source/repos/3D-Renderer/3D-Renderer/_Assets/_Built-In/_Shaders/test_library.glsl"

layout (location = 0) in vec3 position;
layout (location = 2) in vec2 textureCoords;

uniform mat4 projectionMatrix;
uniform mat4 transformationMatrix;
uniform mat4 cameraMatrix;

out vec2 vTexCoords;

void main() {
    vec3 worldPosition = (transformationMatrix * vec4(position, 1)).xyz;
    gl_Position = projectionMatrix * cameraMatrix * vec4(worldPosition, 1);

    vTexCoords = textureCoords;
}