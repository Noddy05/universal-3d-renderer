//https://www.youtube.com/watch?v=BZw2olDmZo4&list=PLPaoO-vpZnumdcb4tZc4x5Q-v7CkrQ6M-&index=21

#version 330 core

layout (triangles) in;
layout (triangle_strip, max_vertices = 3) out;

out vec2 vTexCoords;

in DATA {
	vec3 vNormal;
	vec2 vTexCoords;
	mat4 projectionMatrix;
	mat4 transformationMatrix;
} data_in[];

void main(){

	//Do nothing:
	for(int i = 0; i < 3; i++){
		vec4 finalPosition = data_in[i].projectionMatrix * gl_in[i].gl_Position;
		gl_Position = finalPosition;
		vTexCoords = data_in[i].vTexCoords;
		//Done with this vertex:
		EmitVertex();
	}
	EndPrimitive();
}