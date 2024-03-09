#version 420 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 textureCoords;
layout (location = 3) in mat4 instanceTransformation;

uniform mat4 projectionMatrix;
uniform mat4 transformationMatrix;
uniform mat4 cameraMatrix;

uniform vec4 clippingPlane;

out DATA {
	vec2 vTexCoords;
	mat4 projectionMatrix;
	mat4 transformationMatrix;
	mat4 cameraMatrix;

	vec3 cameraPosition;

	vec3 vNormal;
} data_out;


//Allows for 16 directional lights
struct DirectionalLight {
	vec3 lightColor;
	float lightStrength;
	vec3 lightFromDirection;
	float _DUMMY_;
};

void main() {
	vec3 worldPosition;
	worldPosition = (instanceTransformation * vec4(position, 1)).xyz;
    gl_Position = vec4(worldPosition, 1);

    //gl_ClipDistance[0] = dot(worldPosition, clippingPlane.xyz) + clippingPlane.w;

	data_out.cameraMatrix = cameraMatrix;
    data_out.cameraPosition = (inverse(cameraMatrix) * vec4(vec3(0), 1.0)).xyz;
    data_out.vTexCoords = textureCoords;
	data_out.projectionMatrix = projectionMatrix;
	data_out.transformationMatrix = instanceTransformation;

	mat3 normalMatrix = mat3(instanceTransformation);
	normalMatrix = inverse(normalMatrix);
	normalMatrix = transpose(normalMatrix);
	data_out.vNormal = normalize(normalMatrix * normal).xyz;
}