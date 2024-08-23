shader_type canvas_item;

uniform vec2 direction = vec2(1.0, 0.0);
uniform float speed_scale = 0.1;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 move = direction * TIME * speed_scale;
	COLOR = texture(TEXTURE, UV + move);
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
