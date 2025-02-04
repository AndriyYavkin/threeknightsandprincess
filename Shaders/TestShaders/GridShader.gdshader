shader_type spatial;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float fresnel = sqrt(1.0 - dot(NORMAL, VIEW));
	ALBEDO = vec3(0.01, 0.03, 0.05) + (0.1 * fresnel);
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
