shader_type spatial;
uniform float height_scale = 0.5;
uniform sampler2D noise;
uniform sampler2D normalmap;

varying vec2 tex_position;

void vertex() {
	tex_position = VERTEX.xz / 2.0 + 0.5;
	float height = texture(noise, tex_position).x;
	VERTEX.y += height * height_scale;
}

void fragment() {
	NORMAL_MAP = texture(normalmap, tex_position).xyz;
	float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));
	RIM = 0.2;
	METALLIC = 0.1;
	ROUGHNESS = 0.01 * (1.0 - fresnel);
	ALBEDO = vec3(0.01, 0.03, 0.05) + (0.1 * fresnel);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
