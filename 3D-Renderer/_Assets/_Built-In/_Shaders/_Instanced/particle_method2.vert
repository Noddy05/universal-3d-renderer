#version 420 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 textureCoords;

layout(std140, binding = 0) uniform Transformations { 
	mat4 transformationMatrix[1000];
};

uniform mat4 projectionMatrix;
uniform mat4 cameraMatrix;

out vec3 vNormal;
out vec2 vTexCoords;

void main() {
	mat4 transformationMat = transformationMatrix[gl_InstanceID];
    vec3 worldPosition = (transformationMat * vec4(position, 1)).xyz;
    gl_Position = projectionMatrix * cameraMatrix * vec4(worldPosition, 1);

	mat3 normalMatrix = mat3(transformationMat);
	normalMatrix = inverse(normalMatrix);
	normalMatrix = transpose(normalMatrix);
	vNormal = normalize(normalMatrix * normal).xyz;

    vTexCoords = textureCoords;
}