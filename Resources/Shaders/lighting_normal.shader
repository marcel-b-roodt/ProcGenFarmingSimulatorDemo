shader_type canvas_item;

uniform float light_strength = 1;
uniform float shadow_strength = 5;

void fragment()
{
	if (AT_LIGHT_PASS)
	{
		vec4 col = texture(TEXTURE, UV);
	    COLOR = col;
	}
	else
	{
		COLOR = vec4(0);
	}
	vec4 white = vec4(1,1,1,1);
	vec4 col_black = vec4(0,0,0,0);
	

    col = vec4(LIGHT.x, LIGHT.y, 1, 1);


}