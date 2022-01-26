shader_type canvas_item;

uniform float light_strength = 1;
uniform float shadow_strength = 5;

const vec4 col_white = vec4(1,1,1,1);
const vec4 col_black = vec4(0,0,0,1);
const vec4 col_red = vec4(1,0,0,1);
const vec4 col_green = vec4(0,1,0,1);
const vec4 col_blue = vec4(0,0,1,1);
const vec4 col_gray = vec4(0.5f,0.5f,0.5f,1);
const vec4 col_brown = vec4(0.5f,0.5f,0,1);
const vec4 col_purple = vec4(0.5f,0,0.5f,1);

void fragment()
{
	if (AT_LIGHT_PASS)
	{
		//vec4 light_strength_vec = LIGHT_COLOR;
		//vec4 light_strength_vec = LIGHT_COLOR;
		vec4 col = texture(TEXTURE, UV);
	    COLOR = col;
	}
	else
	{
		vec4 col = texture(TEXTURE, UV);
		col.xyz *= 0.1f;
		COLOR = col;
	}
}

void light()
{
	//vec3 light_normal = NORMAL;
	
	vec4 col = vec4(0.5f,0.5f,0.5f,1f);
	LIGHT_COLOR = col;	
	//SHADOW_COLOR = col;
}