#version 330 core

in vec3 vNormal;
in vec2 vTexCoords;

out vec4 fragmentColor;

uniform vec4 color;

void main(){
    //Combine texture and color
    vec4 unlitColor = color;

    fragmentColor = unlitColor;
}