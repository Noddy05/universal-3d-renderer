//The std140 format seems to only allow for 4d vectors of data.


layout(std140, binding = 0) uniform uShadowInformation {
	vec3 shadowColor;
	float minLight;
} shadow_info;

//Allows for 16 directional lights
struct DirectionalLight {
	vec3 lightColor;
	float lightStrength;
	vec3 lightFromDirection;
	float _DUMMY_;
	mat4 lightMatrix;
};
layout(std140, binding = 1) uniform uDirectionalLight {
	vec3 DirectionalLight[16];
} directional_light;

//Allows for 512 point lights
layout(std140, binding = 2) uniform uPointLight {
	vec3  lightPosition[512];
	float lightStrength[512];
	bool  lightIsActive[512];
};