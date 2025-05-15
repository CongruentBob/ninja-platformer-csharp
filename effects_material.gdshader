shader_type canvas_item;

uniform bool flash_enabled = false;
uniform vec4 flash_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec4 texture_color = texture(TEXTURE, UV);
	vec4 output_color = texture_color;
	if (flash_enabled) {
		output_color = mix(texture_color, flash_color, abs(texture_color.a));
	}
	COLOR = output_color;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
