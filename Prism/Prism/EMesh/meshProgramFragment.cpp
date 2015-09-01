#version 330

struct hsl {
	float h;
	float s;
	float l;
};

hsl RGBtoHSL(vec3);
vec3 HSLtoRGB(hsl);
float Max(vec3);
float Min(vec3);
float Mod(float, float, float);
float Clamp(float, float, float);
vec3 Clamp(vec3, float, float);

uniform bool enableTexture = true;
uniform vec4 uniColor = vec4(1, 1, 1, 1);

uniform sampler2D texture;
in vec2 _uv;
in vec3 _hsl;

out vec4 fragment;

void main() {
	if (enableTexture) {
		fragment = texture(texture, _uv) * uniColor;

		if (fragment.a <= 0.0)
			discard;

		if (_hsl != vec3(0.0, 0.0, 0.0)) {
			hsl buff = RGBtoHSL(fragment.rgb);

			buff.h = Mod(buff.h + _hsl.x, 0.0, 360.0);
			buff.s = Clamp(buff.s * _hsl.y, 0.0, 1.0);
			buff.l = Clamp(buff.l * _hsl.z, 0.0, 1.0);

			fragment.rgb = HSLtoRGB(buff);
		}
	}
	else {
		fragment = uniColor;
	}
}

hsl RGBtoHSL(vec3 rgb) {
	float M = Max(rgb), m = Min(rgb);

	float C = M - m;
	float H = 0;

	if (C == 0)
		H = 0;
	else if (M == rgb.r)
		H = (rgb.g - rgb.b) / C;
	else if (M == rgb.g)
		H = (rgb.b - rgb.r) / C + 2;
	else if (M == rgb.b)
		H = (rgb.r - rgb.g) / C + 4;

	H = Mod(H, 0f, 6f) * 60;

	float L = (m + M) / 2;
	float S = C != 0 ?
		C / (1 - abs(L * 2 - 1)) :
		0;

	return hsl(H, S, L);
}

vec3 HSLtoRGB(hsl clr) {
	float C = clr.s * (1 - abs(clr.l * 2 - 1));
	float H = clr.h / 60;

	float N = (1 - abs(Mod(H, 0, 2) - 1)) * C;

	vec3 rgb = vec3(0.0, 0.0, 0.0);

	if (0 <= H && H < 1)
		rgb = vec3(C, N, 0);
	else if (1 <= H && H < 2)
		rgb = vec3(N, C, 0);
	else if (2 <= H && H < 3)
		rgb = vec3(0, C, N);
	else if (3 <= H && H < 4)
		rgb = vec3(0, N, C);
	else if (4 <= H && H < 5)
		rgb = vec3(N, 0, C);
	else if (5 <= H && H < 6)
		rgb = vec3(C, 0, N);

	float m = clr.l - C * 0.5;
	return vec3(rgb.r + m, rgb.g + m, rgb.b + m);
}

float Max(vec3 vec) {
	if (vec.r > vec.g && vec.r > vec.b)
		return vec.r;

	else {
		if (vec.g > vec.b)
			return vec.g;

		return vec.b;
	}
}

float Min(vec3 vec) {
	if (vec.r < vec.g && vec.r < vec.b)
		return vec.r;

	else {
		if (vec.g < vec.b)
			return vec.g;

		return vec.b;
	}
}

float Mod(float v, float min, float max) {
	float diff = max - min;

	while (v >= max) v -= diff;
	while (v < min) v += diff;

	return v;
}

float Clamp(float v, float min, float max) {
	if (v < min) return min;
	if (v > max) return max;

	return v;
}

vec3 Clamp(vec3 v, float min, float max) {
	return vec3(
		Clamp(v.x, min, max),
		Clamp(v.y, min, max),
		Clamp(v.z, min, max)
		);
}