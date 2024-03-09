#version 330 core

in vec2 vTexCoords;

uniform vec4 color;

out vec4 fragmentColor;

void main(){
    //Combine texture and color
    vec4 unlitColor = color;

    fragmentColor = unlitColor;
}