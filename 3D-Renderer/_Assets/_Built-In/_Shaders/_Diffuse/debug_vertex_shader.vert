#version 330 core

#include "C:/Users/noah0/source/repos/3D-Renderer/3D-Renderer/_Assets/_Built-In/_Shaders/test_library.glsl"

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 textureCoords;

uniform mat4 projectionMatrix;
uniform mat4 transformationMatrix;
uniform mat4 cameraMatrix;

uniform vec4 clippingPlane;

out vec3 vWorldPosition;
out vec3 vCameraPosition;
out vec3 vLightDirection;
out vec3 vNormal;
out vec2 vTexCoords;

void main() {
    vec3 worldPosition = (transformationMatrix * vec4(position, 1)).xyz;
    gl_Position = projectionMatrix * cameraMatrix * vec4(worldPosition, 1);

    //gl_ClipDistance[0] = dot(worldPosition, clippingPlane.xyz) + clippingPlane.w;

    //The direction the sun is pointing:
    vec3 lightDirection = normalize(vec3(0, 0, -1));
    vLightDirection = lightDirection;

    vCameraPosition = (inverse(cameraMatrix) * vec4(vec3(0), 1.0)).xyz;
    vWorldPosition = worldPosition;
    
	mat3 normalMatrix = mat3(transformationMatrix);
	normalMatrix = inverse(normalMatrix);
	normalMatrix = transpose(normalMatrix);
	vNormal = normalize(normalMatrix * normal).xyz;

    vTexCoords = textureCoords;
}