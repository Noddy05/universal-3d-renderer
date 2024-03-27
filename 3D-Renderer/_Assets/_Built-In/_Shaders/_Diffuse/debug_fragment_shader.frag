#version 420 core

in vec4 position;
in vec3 vPosition;
in vec3 cameraPosition;

in vec3 vNormal;
in vec2 vTexCoords;
in vec3 lightDirections[16];

out vec4 fragmentColor;

uniform sampler2D textureSampler;
uniform samplerCube reflectionSampler;
uniform sampler2D normalSampler;
uniform sampler2D shadowSampler;

uniform float cubemapReflectivity = 0.5;
uniform float cubemapRefractivity = 0.5;
uniform float reflectivity = 1;
uniform float specularHighlightDamper = 15;
uniform vec4 color;
in float vUseNormalMap;
in vec4 lightSpacePositions[16];


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
	DirectionalLight[16] lights;
} directional_light;


vec3 normal;

vec4 DirectLight(int index){
    DirectionalLight light = directional_light.lights[index];
    if(light.lightStrength <= 0)
        return vec4(0);

    vec3 toCamera = cameraPosition - vPosition;

    vec4 lightColor = vec4(light.lightColor, 1);
    vec3 lightFromDirection = lightDirections[index];
    float lightStrength = min(max(dot(lightFromDirection, normal), 0), 1)
        * light.lightStrength;
    float lightStrengthClamped = max(lightStrength, 0);
    
    vec3 reflectedlightFromDirection = reflect(-lightFromDirection, normal);
    float specularBrightness = max(dot(reflectedlightFromDirection, -normalize(toCamera)), 0);
    float specularHighlight = pow(specularBrightness, specularHighlightDamper) * reflectivity;
    
    return vec4(lightColor * lightStrengthClamped + 
        lightColor * specularHighlight * lightStrengthClamped);
}


void main(){
    vec3 toCamera = cameraPosition - vPosition;

    //mixing between normal and vertex normal:
    vec3 normalMap = normalize(texture(normalSampler, vTexCoords).xyz * 2.0 - 1.0);
    normal = mix(vNormal, normalMap, vUseNormalMap);

    //light and shadow settings:
    vec4 shadowColor = vec4(shadow_info.shadowColor, 1);
    float minLight = shadow_info.minLight;

    //Combine texture and color
    vec4 sampledTexture = texture(textureSampler, vTexCoords);
    vec4 unlitColor = color 
        * sampledTexture;

    //Make color brightness based off light strength:
    vec4 sumLights = DirectLight(0);
    float shadowColorFalloff = 0.55; //Must be higher than minLight
    float lightSummedStrength = length(sumLights);
    float shadowColorStrength = shadowColorFalloff * pow(minLight / shadowColorFalloff, 
        lightSummedStrength/minLight);

    float distanceToLight = texture(shadowSampler, (lightSpacePositions[0].xy + 1) / 2).r;
    if(lightSpacePositions[0].z > distanceToLight){
        unlitColor = vec4(0);
    }

    fragmentColor = unlitColor * mix(sumLights, shadowColor, shadowColorStrength);
    //fragmentColor = vec4(lightSpacePositions[0].w, 0, 0, 0);
    //fragmentColor = vec4(1-distanceToLight, 0, 0, 0);
    //fragmentColor = vec4((lightSpacePositions[0].xy + 1) / 2, 0, 0);

    //Calculate cubemap reflectionColor 
    /*
    vec3 refractionDirection = refract(-normalize(toCamera), normalize(normal), 1/1.33);
    vec3 reflectedDirection = reflect(-normalize(toCamera), normalize(normal));
    vec4 refractedColor = texture(reflectionSampler, refractionDirection);
    vec4 reflectedColor = texture(reflectionSampler, reflectedDirection);
    */
    //fragmentColor = mix(fragmentColor, refractedColor, cubemapRefractivity * lightStrengthClamped);
    //fragmentColor = mix(fragmentColor, reflectedColor, cubemapReflectivity * lightStrengthClamped);
}
