//The std140 format seems to only allow for 4d vectors of data.


layout(std140, binding = 0) uniform uShadowInformation {
	vec3 shadowColor;
	float minLight;
} shadow_info;

//Allows for 16 directional lights
struct DirectionalLight {
	vec3 directionalLightColor;
	float directionalLightStrength;
	vec3 directionalLightDirection;
	float _DUMMY_;
};
layout(std140, binding = 1) uniform uDirectionalLight {
	vec3 DirectionalLight[16];
} directional_light;

//Allows for 512 point lights
layout(std140, binding = 2) uniform uPointLight {
	vec3  pointLightPosition[512];
	float pointLightStrength[512];
	bool  pointLightIsActive[512];
};