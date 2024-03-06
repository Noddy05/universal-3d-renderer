#version 420 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 textureCoords;
layout (location = 3) in mat4 instanceTransformation;

uniform mat4 projectionMatrix;
uniform mat4 cameraMatrix;

out vec3 vNormal;
out vec2 vTexCoords;

void main() {
    vec3 worldPosition = (instanceTransformation * vec4(position, 1)).xyz;
    gl_Position = projectionMatrix * cameraMatrix * vec4(worldPosition, 1);

	mat3 normalMatrix = mat3(instanceTransformation);
	normalMatrix = inverse(normalMatrix);
	normalMatrix = transpose(normalMatrix);
	vNormal = normalize(normalMatrix * normal).xyz;

    vTexCoords = textureCoords;
}