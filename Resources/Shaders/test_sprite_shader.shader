shader_type canvas_item;

uniform float timeFactor = 1;
uniform vec2 amplitude = vec2(10, 5);

void vertex() 
{
	VERTEX.x += cos(TIME * timeFactor + VERTEX.x + VERTEX.y) * amplitude.x;	
	VERTEX.y += sin(TIME * timeFactor + VERTEX.x + VERTEX.y) * amplitude.y;	
}

void frag()
{
	
}