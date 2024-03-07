//https://www.youtube.com/watch?v=BZw2olDmZo4&list=PLPaoO-vpZnumdcb4tZc4x5Q-v7CkrQ6M-&index=21

#version 420 core

layout (triangles) in;
layout (triangle_strip, max_vertices = 3) out;


out vec3 vPosition;
out vec3 lightCastFromDirection;
out vec3 cameraPosition;

out vec2 vTexCoords;
out vec3 vNormal;
out mat3 TBNMatrix;

in DATA {
	vec2 vTexCoords;
	mat4 projectionMatrix;
	mat4 transformationMatrix;
	mat4 cameraMatrix;

	vec3 lightCastFromDirection;
	vec3 cameraPosition;

	vec3 vNormal;
} data_in[];



void main(){

	//TBN calculation https://youtu.be/JNj1A1bl7gg?si=Gj05jp66z7PEQT1f&t=225
	vec3 edge0 = gl_in[1].gl_Position.xyz - gl_in[0].gl_Position.xyz;
	vec3 edge1 = gl_in[2].gl_Position.xyz - gl_in[0].gl_Position.xyz;
	vec2 deltaUV0 = data_in[1].vTexCoords - data_in[0].vTexCoords;
	vec2 deltaUV1 = data_in[2].vTexCoords - data_in[0].vTexCoords;

	float inverseDeterminant = 1.0 / (deltaUV0.x * deltaUV1.y - deltaUV1.x * deltaUV0.y);
	vec3 tangent = vec3(inverseDeterminant * (deltaUV1.y * edge0 - deltaUV0.y * edge1));
	vec3 bitangent = vec3(inverseDeterminant * (-deltaUV1.x * edge0 + deltaUV0.x * edge1));

	vec3 T = normalize(vec3(data_in[0].transformationMatrix * vec4(tangent, 0)));
	vec3 B = normalize(vec3(data_in[0].transformationMatrix * vec4(bitangent, 0)));
	vec3 N = normalize(vec3(data_in[0].transformationMatrix * vec4(cross(edge1, edge0), 0)));
	
	TBNMatrix = mat3(T, B, N);


	//Do nothing:
	for(int i = 0; i < 3; i++){
		vec4 finalPosition = data_in[i].projectionMatrix * 
			data_in[i].cameraMatrix * gl_in[i].gl_Position;
		gl_Position = finalPosition;
		vTexCoords = data_in[i].vTexCoords;
		vPosition = gl_in[i].gl_Position.xyz;
		lightCastFromDirection = data_in[i].lightCastFromDirection;
		cameraPosition = data_in[i].cameraPosition;
		vNormal = data_in[i].vNormal;

		//Done with this vertex: 
		EmitVertex();
	}
	EndPrimitive();
}