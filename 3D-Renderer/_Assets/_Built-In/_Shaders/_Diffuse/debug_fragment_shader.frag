#version 330 core

in vec3 vWorldPosition;
in vec3 vCameraPosition;
in vec3 vNormal;
in vec2 vTexCoords;
in vec3 vLightDirection;

out vec4 fragmentColor;

uniform sampler2D textureSampler;
uniform sampler2D normalSampler;

void main(){
    float reflectivity = 1;
    float specularHighlightDamper = 15;

    vec3 toCamera = vCameraPosition - vWorldPosition;

    //mixing between normal and vertex normal:
    vec3 vertexNormal = normalize(vNormal);
    vec3 normalMap = normalize(texture(normalSampler, vTexCoords).xyz * 2.0 - 1.0);
    vec3 normal = mix(vertexNormal, normalMap, 0.0);

    //light and shadow settings:
    vec4 lightColor = vec4(1);
    vec4 shadowColor = vec4(19, 22, 46, 255) / 255.0 * 5;
    float minLight = 0.1;

    //Calculate brightness from directional light (and shadow):
    float lightStrength = max(-dot(vLightDirection, normal), 0);
    float lightStrengthClamped = max(lightStrength, minLight);
    float shadowColorFalloff = 0.55; //Must be higher than minLight
    float shadowColorStrength = shadowColorFalloff * pow(minLight / shadowColorFalloff, lightStrengthClamped/minLight);

    //Calculate specular highlights:
    vec3 reflectedDirection = reflect(normalize(toCamera), normal);
    float specularBrightness = max(dot(reflectedDirection, vLightDirection), 0);
    float specularHighlight = pow(specularBrightness, specularHighlightDamper) * reflectivity;

    //Combine texture and color
    vec4 sampledTexture = texture(textureSampler, vTexCoords);
    vec4 unlitColor = vec4(0.5, 0.4, 0.2, 1.0) 
        * sampledTexture;
    //Make color brightness based off light strength:
    fragmentColor = mix(unlitColor * lightStrengthClamped * lightColor, 
        shadowColor, shadowColorStrength) + specularHighlight * lightStrengthClamped * vec4(lightColor);
}