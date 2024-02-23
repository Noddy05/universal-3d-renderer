#version 330 core

in vec3 vWorldPosition;
in vec3 vCameraPosition;
in vec3 vNormal;
in vec2 vTexCoords;
in vec3 vLightDirection;

out vec4 fragmentColor;

uniform sampler2D textureSampler;
uniform samplerCube reflectionSampler;
uniform float cubemapReflectivity = 0.5;
uniform float cubemapRefractivity = 0.5;
uniform float reflectivity = 1;
uniform float specularHighlightDamper = 15;
uniform vec4 color;

void main(){
    vec3 toCamera = vCameraPosition - vWorldPosition;

    //mixing between normal and vertex normal:
    vec3 vertexNormal = normalize(vNormal);
    //vec3 normalMap = normalize(texture(normalSampler, vTexCoords).xyz * 2.0 - 1.0);
    //vec3 normal = mix(vertexNormal, normalMap, 0.0);
    vec3 normal = vertexNormal;

    //light and shadow settings:
    vec4 lightColor = vec4(1);
    vec4 shadowColor = vec4(19, 22, 46, 255) / 255.0 * 5;
    float minLight = 0.1;

    //Calculate brightness from directional light (and shadow):
    float lightStrength = max(-dot(vLightDirection, normal), 0);
    float lightStrengthClamped = max(lightStrength, minLight);
    float shadowColorFalloff = 0.55; //Must be higher than minLight
    float shadowColorStrength = shadowColorFalloff * pow(minLight / shadowColorFalloff, 
        lightStrengthClamped/minLight);

    //Calculate specular highlights:
    vec3 refractionDirection = refract(-normalize(toCamera), normalize(normal), 1/1.33);
    vec3 reflectedDirection = reflect(-normalize(toCamera), normalize(normal));
    float specularBrightness = max(dot(reflectedDirection, -vLightDirection), 0);
    float specularHighlight = pow(specularBrightness, specularHighlightDamper) * reflectivity;

    //Combine texture and color
    vec4 sampledTexture = texture(textureSampler, vTexCoords);
    vec4 unlitColor = color 
        * sampledTexture;
    //Make color brightness based off light strength:
    fragmentColor = mix(unlitColor * lightStrengthClamped * lightColor, 
        shadowColor, shadowColorStrength) + specularHighlight * lightStrengthClamped * vec4(lightColor);

        
    //Calculate cubemap reflectionColor
    vec4 refractedColor = texture(reflectionSampler, refractionDirection);
    vec4 reflectedColor = texture(reflectionSampler, reflectedDirection);
    fragmentColor = mix(fragmentColor, refractedColor, cubemapRefractivity * lightStrengthClamped);
    fragmentColor = mix(fragmentColor, reflectedColor, cubemapReflectivity * lightStrengthClamped);
}