using System;
using UnityEngine;

// Token: 0x02000B4D RID: 2893
public struct GTUberShader_MaterialKeywordStates
{
	// Token: 0x06004566 RID: 17766 RVA: 0x00159E60 File Offset: 0x00158060
	public GTUberShader_MaterialKeywordStates(Material mat)
	{
		this.material = mat;
		this.STEREO_INSTANCING_ON = mat.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.UNITY_SINGLE_PASS_STEREO = mat.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.STEREO_MULTIVIEW_ON = mat.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.STEREO_CUBEMAP_RENDER_ON = mat.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = mat.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._ZONE_LIQUID_SHAPE__CYLINDER = mat.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = mat.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._USE_TEXTURE = mat.IsKeywordEnabled("_USE_TEXTURE");
		this.USE_TEXTURE__AS_MASK = mat.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
		this._UV_SOURCE__UV0 = mat.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = mat.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._USE_VERTEX_COLOR = mat.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = mat.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._ALPHA_DETAIL_MAP = mat.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._HALF_LAMBERT_TERM = mat.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._WATER_EFFECT = mat.IsKeywordEnabled("_WATER_EFFECT");
		this._HEIGHT_BASED_WATER_EFFECT = mat.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._WATER_CAUSTICS = mat.IsKeywordEnabled("_WATER_CAUSTICS");
		this._ALPHATEST_ON = mat.IsKeywordEnabled("_ALPHATEST_ON");
		this._MAINTEX_ROTATE = mat.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._UV_WAVE_WARP = mat.IsKeywordEnabled("_UV_WAVE_WARP");
		this._LIQUID_VOLUME = mat.IsKeywordEnabled("_LIQUID_VOLUME");
		this._LIQUID_CONTAINER = mat.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._GT_RIM_LIGHT = mat.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = mat.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = mat.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._SPECULAR_HIGHLIGHT = mat.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._EMISSION = mat.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = mat.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._USE_DEFORM_MAP = mat.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_DAY_NIGHT_LIGHTMAP = mat.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_TEX_ARRAY_ATLAS = mat.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = mat.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._CRYSTAL_EFFECT = mat.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._EYECOMP = mat.IsKeywordEnabled("_EYECOMP");
		this._MOUTHCOMP = mat.IsKeywordEnabled("_MOUTHCOMP");
		this._ALPHA_BLUE_LIVE_ON = mat.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._GRID_EFFECT = mat.IsKeywordEnabled("_GRID_EFFECT");
		this._REFLECTIONS = mat.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_BOX_PROJECT = mat.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = mat.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_ALBEDO_TINT = mat.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_USE_NORMAL_TEX = mat.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._VERTEX_ROTATE = mat.IsKeywordEnabled("_VERTEX_ROTATE");
		this._VERTEX_ANIM_FLAP = mat.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = mat.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._GRADIENT_MAP_ON = mat.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._PARALLAX = mat.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = mat.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = mat.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._MASK_MAP_ON = mat.IsKeywordEnabled("_MASK_MAP_ON");
		this._FX_LAVA_LAMP = mat.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._INNER_GLOW = mat.IsKeywordEnabled("_INNER_GLOW");
		this._STEALTH_EFFECT = mat.IsKeywordEnabled("_STEALTH_EFFECT");
		this._UV_SHIFT = mat.IsKeywordEnabled("_UV_SHIFT");
		this._TEXEL_SNAP_UVS = mat.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = mat.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._GT_EDITOR_TIME = mat.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._DEBUG_PAWN_DATA = mat.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._COLOR_GRADE_PROTANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_DEUTERANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_TRITANOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = mat.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._COLOR_GRADE_ACHROMATOMALY = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = mat.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this.LIGHTMAP_ON = mat.IsKeywordEnabled("LIGHTMAP_ON");
		this.DIRLIGHTMAP_COMBINED = mat.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = mat.IsKeywordEnabled("INSTANCING_ON");
	}

	// Token: 0x06004567 RID: 17767 RVA: 0x0015A360 File Offset: 0x00158560
	public void Refresh()
	{
		Material material = this.material;
		this.STEREO_INSTANCING_ON = material.IsKeywordEnabled("STEREO_INSTANCING_ON");
		this.UNITY_SINGLE_PASS_STEREO = material.IsKeywordEnabled("UNITY_SINGLE_PASS_STEREO");
		this.STEREO_MULTIVIEW_ON = material.IsKeywordEnabled("STEREO_MULTIVIEW_ON");
		this.STEREO_CUBEMAP_RENDER_ON = material.IsKeywordEnabled("STEREO_CUBEMAP_RENDER_ON");
		this._GLOBAL_ZONE_LIQUID_TYPE__WATER = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
		this._GLOBAL_ZONE_LIQUID_TYPE__LAVA = material.IsKeywordEnabled("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		this._ZONE_LIQUID_SHAPE__CYLINDER = material.IsKeywordEnabled("_ZONE_LIQUID_SHAPE__CYLINDER");
		this._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX = material.IsKeywordEnabled("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		this._USE_TEXTURE = material.IsKeywordEnabled("_USE_TEXTURE");
		this.USE_TEXTURE__AS_MASK = material.IsKeywordEnabled("USE_TEXTURE__AS_MASK");
		this._UV_SOURCE__UV0 = material.IsKeywordEnabled("_UV_SOURCE__UV0");
		this._UV_SOURCE__WORLD_PLANAR_Y = material.IsKeywordEnabled("_UV_SOURCE__WORLD_PLANAR_Y");
		this._USE_VERTEX_COLOR = material.IsKeywordEnabled("_USE_VERTEX_COLOR");
		this._USE_WEATHER_MAP = material.IsKeywordEnabled("_USE_WEATHER_MAP");
		this._ALPHA_DETAIL_MAP = material.IsKeywordEnabled("_ALPHA_DETAIL_MAP");
		this._HALF_LAMBERT_TERM = material.IsKeywordEnabled("_HALF_LAMBERT_TERM");
		this._WATER_EFFECT = material.IsKeywordEnabled("_WATER_EFFECT");
		this._HEIGHT_BASED_WATER_EFFECT = material.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
		this._WATER_CAUSTICS = material.IsKeywordEnabled("_WATER_CAUSTICS");
		this._ALPHATEST_ON = material.IsKeywordEnabled("_ALPHATEST_ON");
		this._MAINTEX_ROTATE = material.IsKeywordEnabled("_MAINTEX_ROTATE");
		this._UV_WAVE_WARP = material.IsKeywordEnabled("_UV_WAVE_WARP");
		this._LIQUID_VOLUME = material.IsKeywordEnabled("_LIQUID_VOLUME");
		this._LIQUID_CONTAINER = material.IsKeywordEnabled("_LIQUID_CONTAINER");
		this._GT_RIM_LIGHT = material.IsKeywordEnabled("_GT_RIM_LIGHT");
		this._GT_RIM_LIGHT_FLAT = material.IsKeywordEnabled("_GT_RIM_LIGHT_FLAT");
		this._GT_RIM_LIGHT_USE_ALPHA = material.IsKeywordEnabled("_GT_RIM_LIGHT_USE_ALPHA");
		this._SPECULAR_HIGHLIGHT = material.IsKeywordEnabled("_SPECULAR_HIGHLIGHT");
		this._EMISSION = material.IsKeywordEnabled("_EMISSION");
		this._EMISSION_USE_UV_WAVE_WARP = material.IsKeywordEnabled("_EMISSION_USE_UV_WAVE_WARP");
		this._USE_DEFORM_MAP = material.IsKeywordEnabled("_USE_DEFORM_MAP");
		this._USE_DAY_NIGHT_LIGHTMAP = material.IsKeywordEnabled("_USE_DAY_NIGHT_LIGHTMAP");
		this._USE_TEX_ARRAY_ATLAS = material.IsKeywordEnabled("_USE_TEX_ARRAY_ATLAS");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY");
		this._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = material.IsKeywordEnabled("_GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z");
		this._CRYSTAL_EFFECT = material.IsKeywordEnabled("_CRYSTAL_EFFECT");
		this._EYECOMP = material.IsKeywordEnabled("_EYECOMP");
		this._MOUTHCOMP = material.IsKeywordEnabled("_MOUTHCOMP");
		this._ALPHA_BLUE_LIVE_ON = material.IsKeywordEnabled("_ALPHA_BLUE_LIVE_ON");
		this._GRID_EFFECT = material.IsKeywordEnabled("_GRID_EFFECT");
		this._REFLECTIONS = material.IsKeywordEnabled("_REFLECTIONS");
		this._REFLECTIONS_BOX_PROJECT = material.IsKeywordEnabled("_REFLECTIONS_BOX_PROJECT");
		this._REFLECTIONS_MATCAP = material.IsKeywordEnabled("_REFLECTIONS_MATCAP");
		this._REFLECTIONS_MATCAP_PERSP_AWARE = material.IsKeywordEnabled("_REFLECTIONS_MATCAP_PERSP_AWARE");
		this._REFLECTIONS_ALBEDO_TINT = material.IsKeywordEnabled("_REFLECTIONS_ALBEDO_TINT");
		this._REFLECTIONS_USE_NORMAL_TEX = material.IsKeywordEnabled("_REFLECTIONS_USE_NORMAL_TEX");
		this._VERTEX_ROTATE = material.IsKeywordEnabled("_VERTEX_ROTATE");
		this._VERTEX_ANIM_FLAP = material.IsKeywordEnabled("_VERTEX_ANIM_FLAP");
		this._VERTEX_ANIM_WAVE = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE");
		this._VERTEX_ANIM_WAVE_DEBUG = material.IsKeywordEnabled("_VERTEX_ANIM_WAVE_DEBUG");
		this._GRADIENT_MAP_ON = material.IsKeywordEnabled("_GRADIENT_MAP_ON");
		this._PARALLAX = material.IsKeywordEnabled("_PARALLAX");
		this._PARALLAX_AA = material.IsKeywordEnabled("_PARALLAX_AA");
		this._PARALLAX_PLANAR = material.IsKeywordEnabled("_PARALLAX_PLANAR");
		this._MASK_MAP_ON = material.IsKeywordEnabled("_MASK_MAP_ON");
		this._FX_LAVA_LAMP = material.IsKeywordEnabled("_FX_LAVA_LAMP");
		this._INNER_GLOW = material.IsKeywordEnabled("_INNER_GLOW");
		this._STEALTH_EFFECT = material.IsKeywordEnabled("_STEALTH_EFFECT");
		this._UV_SHIFT = material.IsKeywordEnabled("_UV_SHIFT");
		this._TEXEL_SNAP_UVS = material.IsKeywordEnabled("_TEXEL_SNAP_UVS");
		this._UNITY_EDIT_MODE = material.IsKeywordEnabled("_UNITY_EDIT_MODE");
		this._GT_EDITOR_TIME = material.IsKeywordEnabled("_GT_EDITOR_TIME");
		this._DEBUG_PAWN_DATA = material.IsKeywordEnabled("_DEBUG_PAWN_DATA");
		this._COLOR_GRADE_PROTANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOMALY");
		this._COLOR_GRADE_PROTANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_PROTANOPIA");
		this._COLOR_GRADE_DEUTERANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOMALY");
		this._COLOR_GRADE_DEUTERANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_DEUTERANOPIA");
		this._COLOR_GRADE_TRITANOMALY = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOMALY");
		this._COLOR_GRADE_TRITANOPIA = material.IsKeywordEnabled("_COLOR_GRADE_TRITANOPIA");
		this._COLOR_GRADE_ACHROMATOMALY = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOMALY");
		this._COLOR_GRADE_ACHROMATOPSIA = material.IsKeywordEnabled("_COLOR_GRADE_ACHROMATOPSIA");
		this.LIGHTMAP_ON = material.IsKeywordEnabled("LIGHTMAP_ON");
		this.DIRLIGHTMAP_COMBINED = material.IsKeywordEnabled("DIRLIGHTMAP_COMBINED");
		this.INSTANCING_ON = material.IsKeywordEnabled("INSTANCING_ON");
	}

	// Token: 0x04004FF2 RID: 20466
	public Material material;

	// Token: 0x04004FF3 RID: 20467
	public bool STEREO_INSTANCING_ON;

	// Token: 0x04004FF4 RID: 20468
	public bool UNITY_SINGLE_PASS_STEREO;

	// Token: 0x04004FF5 RID: 20469
	public bool STEREO_MULTIVIEW_ON;

	// Token: 0x04004FF6 RID: 20470
	public bool STEREO_CUBEMAP_RENDER_ON;

	// Token: 0x04004FF7 RID: 20471
	public bool _GLOBAL_ZONE_LIQUID_TYPE__WATER;

	// Token: 0x04004FF8 RID: 20472
	public bool _GLOBAL_ZONE_LIQUID_TYPE__LAVA;

	// Token: 0x04004FF9 RID: 20473
	public bool _ZONE_LIQUID_SHAPE__CYLINDER;

	// Token: 0x04004FFA RID: 20474
	public bool _ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;

	// Token: 0x04004FFB RID: 20475
	public bool _USE_TEXTURE;

	// Token: 0x04004FFC RID: 20476
	public bool USE_TEXTURE__AS_MASK;

	// Token: 0x04004FFD RID: 20477
	public bool _UV_SOURCE__UV0;

	// Token: 0x04004FFE RID: 20478
	public bool _UV_SOURCE__WORLD_PLANAR_Y;

	// Token: 0x04004FFF RID: 20479
	public bool _USE_VERTEX_COLOR;

	// Token: 0x04005000 RID: 20480
	public bool _USE_WEATHER_MAP;

	// Token: 0x04005001 RID: 20481
	public bool _ALPHA_DETAIL_MAP;

	// Token: 0x04005002 RID: 20482
	public bool _HALF_LAMBERT_TERM;

	// Token: 0x04005003 RID: 20483
	public bool _WATER_EFFECT;

	// Token: 0x04005004 RID: 20484
	public bool _HEIGHT_BASED_WATER_EFFECT;

	// Token: 0x04005005 RID: 20485
	public bool _WATER_CAUSTICS;

	// Token: 0x04005006 RID: 20486
	public bool _ALPHATEST_ON;

	// Token: 0x04005007 RID: 20487
	public bool _MAINTEX_ROTATE;

	// Token: 0x04005008 RID: 20488
	public bool _UV_WAVE_WARP;

	// Token: 0x04005009 RID: 20489
	public bool _LIQUID_VOLUME;

	// Token: 0x0400500A RID: 20490
	public bool _LIQUID_CONTAINER;

	// Token: 0x0400500B RID: 20491
	public bool _GT_RIM_LIGHT;

	// Token: 0x0400500C RID: 20492
	public bool _GT_RIM_LIGHT_FLAT;

	// Token: 0x0400500D RID: 20493
	public bool _GT_RIM_LIGHT_USE_ALPHA;

	// Token: 0x0400500E RID: 20494
	public bool _SPECULAR_HIGHLIGHT;

	// Token: 0x0400500F RID: 20495
	public bool _EMISSION;

	// Token: 0x04005010 RID: 20496
	public bool _EMISSION_USE_UV_WAVE_WARP;

	// Token: 0x04005011 RID: 20497
	public bool _USE_DEFORM_MAP;

	// Token: 0x04005012 RID: 20498
	public bool _USE_DAY_NIGHT_LIGHTMAP;

	// Token: 0x04005013 RID: 20499
	public bool _USE_TEX_ARRAY_ATLAS;

	// Token: 0x04005014 RID: 20500
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__PROPERTY;

	// Token: 0x04005015 RID: 20501
	public bool _GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;

	// Token: 0x04005016 RID: 20502
	public bool _CRYSTAL_EFFECT;

	// Token: 0x04005017 RID: 20503
	public bool _EYECOMP;

	// Token: 0x04005018 RID: 20504
	public bool _MOUTHCOMP;

	// Token: 0x04005019 RID: 20505
	public bool _ALPHA_BLUE_LIVE_ON;

	// Token: 0x0400501A RID: 20506
	public bool _GRID_EFFECT;

	// Token: 0x0400501B RID: 20507
	public bool _REFLECTIONS;

	// Token: 0x0400501C RID: 20508
	public bool _REFLECTIONS_BOX_PROJECT;

	// Token: 0x0400501D RID: 20509
	public bool _REFLECTIONS_MATCAP;

	// Token: 0x0400501E RID: 20510
	public bool _REFLECTIONS_MATCAP_PERSP_AWARE;

	// Token: 0x0400501F RID: 20511
	public bool _REFLECTIONS_ALBEDO_TINT;

	// Token: 0x04005020 RID: 20512
	public bool _REFLECTIONS_USE_NORMAL_TEX;

	// Token: 0x04005021 RID: 20513
	public bool _VERTEX_ROTATE;

	// Token: 0x04005022 RID: 20514
	public bool _VERTEX_ANIM_FLAP;

	// Token: 0x04005023 RID: 20515
	public bool _VERTEX_ANIM_WAVE;

	// Token: 0x04005024 RID: 20516
	public bool _VERTEX_ANIM_WAVE_DEBUG;

	// Token: 0x04005025 RID: 20517
	public bool _GRADIENT_MAP_ON;

	// Token: 0x04005026 RID: 20518
	public bool _PARALLAX;

	// Token: 0x04005027 RID: 20519
	public bool _PARALLAX_AA;

	// Token: 0x04005028 RID: 20520
	public bool _PARALLAX_PLANAR;

	// Token: 0x04005029 RID: 20521
	public bool _MASK_MAP_ON;

	// Token: 0x0400502A RID: 20522
	public bool _FX_LAVA_LAMP;

	// Token: 0x0400502B RID: 20523
	public bool _INNER_GLOW;

	// Token: 0x0400502C RID: 20524
	public bool _STEALTH_EFFECT;

	// Token: 0x0400502D RID: 20525
	public bool _UV_SHIFT;

	// Token: 0x0400502E RID: 20526
	public bool _TEXEL_SNAP_UVS;

	// Token: 0x0400502F RID: 20527
	public bool _UNITY_EDIT_MODE;

	// Token: 0x04005030 RID: 20528
	public bool _GT_EDITOR_TIME;

	// Token: 0x04005031 RID: 20529
	public bool _DEBUG_PAWN_DATA;

	// Token: 0x04005032 RID: 20530
	public bool _COLOR_GRADE_PROTANOMALY;

	// Token: 0x04005033 RID: 20531
	public bool _COLOR_GRADE_PROTANOPIA;

	// Token: 0x04005034 RID: 20532
	public bool _COLOR_GRADE_DEUTERANOMALY;

	// Token: 0x04005035 RID: 20533
	public bool _COLOR_GRADE_DEUTERANOPIA;

	// Token: 0x04005036 RID: 20534
	public bool _COLOR_GRADE_TRITANOMALY;

	// Token: 0x04005037 RID: 20535
	public bool _COLOR_GRADE_TRITANOPIA;

	// Token: 0x04005038 RID: 20536
	public bool _COLOR_GRADE_ACHROMATOMALY;

	// Token: 0x04005039 RID: 20537
	public bool _COLOR_GRADE_ACHROMATOPSIA;

	// Token: 0x0400503A RID: 20538
	public bool LIGHTMAP_ON;

	// Token: 0x0400503B RID: 20539
	public bool DIRLIGHTMAP_COMBINED;

	// Token: 0x0400503C RID: 20540
	public bool INSTANCING_ON;
}
