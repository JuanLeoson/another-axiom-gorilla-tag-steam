using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020007C9 RID: 1993
public static class UberShader
{
	// Token: 0x170004AF RID: 1199
	// (get) Token: 0x060031EA RID: 12778 RVA: 0x00103995 File Offset: 0x00101B95
	public static Material ReferenceMaterial
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterial;
		}
	}

	// Token: 0x170004B0 RID: 1200
	// (get) Token: 0x060031EB RID: 12779 RVA: 0x001039A1 File Offset: 0x00101BA1
	public static Shader ReferenceShader
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShader;
		}
	}

	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x060031EC RID: 12780 RVA: 0x001039AD File Offset: 0x00101BAD
	public static Material ReferenceMaterialNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterialNonSRP;
		}
	}

	// Token: 0x170004B2 RID: 1202
	// (get) Token: 0x060031ED RID: 12781 RVA: 0x001039B9 File Offset: 0x00101BB9
	public static Shader ReferenceShaderNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShaderNonSRP;
		}
	}

	// Token: 0x170004B3 RID: 1203
	// (get) Token: 0x060031EE RID: 12782 RVA: 0x001039C5 File Offset: 0x00101BC5
	public static UberShaderProperty[] AllProperties
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kProperties;
		}
	}

	// Token: 0x060031EF RID: 12783 RVA: 0x001039D4 File Offset: 0x00101BD4
	public static bool IsAnimated(Material m)
	{
		if (m == null)
		{
			return false;
		}
		if ((double)UberShader.UvShiftToggle.GetValue<float>(m) <= 0.5)
		{
			return false;
		}
		Vector2 value = UberShader.UvShiftRate.GetValue<Vector2>(m);
		return value.x > 0f || value.y > 0f;
	}

	// Token: 0x060031F0 RID: 12784 RVA: 0x00103A2F File Offset: 0x00101C2F
	private static UberShaderProperty GetProperty(int i)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x060031F1 RID: 12785 RVA: 0x00103A2F File Offset: 0x00101C2F
	private static UberShaderProperty GetProperty(int i, string expectedName)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x060031F2 RID: 12786 RVA: 0x00103A40 File Offset: 0x00101C40
	private static void InitDependencies()
	{
		if (UberShader.gInitialized)
		{
			return;
		}
		UberShader.kReferenceShader = Shader.Find("GorillaTag/UberShader");
		UberShader.kReferenceMaterial = new Material(UberShader.kReferenceShader);
		UberShader.kReferenceShaderNonSRP = Shader.Find("GorillaTag/UberShaderNonSRP");
		UberShader.kReferenceMaterialNonSRP = new Material(UberShader.kReferenceShaderNonSRP);
		UberShader.kProperties = UberShader.EnumerateAllProperties(UberShader.kReferenceShader);
		UberShader.gInitialized = true;
	}

	// Token: 0x060031F3 RID: 12787 RVA: 0x001039A1 File Offset: 0x00101BA1
	public static Shader GetShader()
	{
		UberShader.InitDependencies();
		return UberShader.kReferenceShader;
	}

	// Token: 0x060031F4 RID: 12788 RVA: 0x00103AA8 File Offset: 0x00101CA8
	private static UberShaderProperty[] EnumerateAllProperties(Shader uberShader)
	{
		int propertyCount = uberShader.GetPropertyCount();
		UberShaderProperty[] array = new UberShaderProperty[propertyCount];
		for (int i = 0; i < propertyCount; i++)
		{
			UberShaderProperty uberShaderProperty = new UberShaderProperty
			{
				index = i,
				flags = uberShader.GetPropertyFlags(i),
				type = uberShader.GetPropertyType(i),
				nameID = uberShader.GetPropertyNameId(i),
				name = uberShader.GetPropertyName(i),
				attributes = uberShader.GetPropertyAttributes(i)
			};
			if (uberShaderProperty.type == ShaderPropertyType.Range)
			{
				uberShaderProperty.rangeLimits = uberShader.GetPropertyRangeLimits(uberShaderProperty.index);
			}
			string[] attributes = uberShaderProperty.attributes;
			if (attributes != null && attributes.Length != 0)
			{
				foreach (string text in attributes)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						bool flag = text.StartsWith("Toggle(");
						uberShaderProperty.isKeywordToggle = flag;
						if (flag)
						{
							string keyword = text.Split('(', StringSplitOptions.RemoveEmptyEntries)[1].RemoveEnd(")", StringComparison.InvariantCulture);
							uberShaderProperty.keyword = keyword;
						}
					}
				}
			}
			array[i] = uberShaderProperty;
		}
		return array;
	}

	// Token: 0x04003DB4 RID: 15796
	private static Shader kReferenceShader;

	// Token: 0x04003DB5 RID: 15797
	private static Material kReferenceMaterial;

	// Token: 0x04003DB6 RID: 15798
	private static Shader kReferenceShaderNonSRP;

	// Token: 0x04003DB7 RID: 15799
	private static Material kReferenceMaterialNonSRP;

	// Token: 0x04003DB8 RID: 15800
	private static UberShaderProperty[] kProperties;

	// Token: 0x04003DB9 RID: 15801
	private static bool gInitialized = false;

	// Token: 0x04003DBA RID: 15802
	public static UberShaderProperty TransparencyMode = UberShader.GetProperty(0);

	// Token: 0x04003DBB RID: 15803
	public static UberShaderProperty Cutoff = UberShader.GetProperty(1);

	// Token: 0x04003DBC RID: 15804
	public static UberShaderProperty ColorSource = UberShader.GetProperty(2);

	// Token: 0x04003DBD RID: 15805
	public static UberShaderProperty BaseColor = UberShader.GetProperty(3);

	// Token: 0x04003DBE RID: 15806
	public static UberShaderProperty GChannelColor = UberShader.GetProperty(4);

	// Token: 0x04003DBF RID: 15807
	public static UberShaderProperty BChannelColor = UberShader.GetProperty(5);

	// Token: 0x04003DC0 RID: 15808
	public static UberShaderProperty AChannelColor = UberShader.GetProperty(6);

	// Token: 0x04003DC1 RID: 15809
	public static UberShaderProperty BaseMap = UberShader.GetProperty(7);

	// Token: 0x04003DC2 RID: 15810
	public static UberShaderProperty BaseMap_WH = UberShader.GetProperty(8);

	// Token: 0x04003DC3 RID: 15811
	public static UberShaderProperty TexelSnapToggle = UberShader.GetProperty(9);

	// Token: 0x04003DC4 RID: 15812
	public static UberShaderProperty TexelSnap_Factor = UberShader.GetProperty(10);

	// Token: 0x04003DC5 RID: 15813
	public static UberShaderProperty UVSource = UberShader.GetProperty(11);

	// Token: 0x04003DC6 RID: 15814
	public static UberShaderProperty AlphaDetailToggle = UberShader.GetProperty(12);

	// Token: 0x04003DC7 RID: 15815
	public static UberShaderProperty AlphaDetail_ST = UberShader.GetProperty(13);

	// Token: 0x04003DC8 RID: 15816
	public static UberShaderProperty AlphaDetail_Opacity = UberShader.GetProperty(14);

	// Token: 0x04003DC9 RID: 15817
	public static UberShaderProperty AlphaDetail_WorldSpace = UberShader.GetProperty(15);

	// Token: 0x04003DCA RID: 15818
	public static UberShaderProperty MaskMapToggle = UberShader.GetProperty(16);

	// Token: 0x04003DCB RID: 15819
	public static UberShaderProperty MaskMap = UberShader.GetProperty(17);

	// Token: 0x04003DCC RID: 15820
	public static UberShaderProperty MaskMap_WH = UberShader.GetProperty(18);

	// Token: 0x04003DCD RID: 15821
	public static UberShaderProperty LavaLampToggle = UberShader.GetProperty(19);

	// Token: 0x04003DCE RID: 15822
	public static UberShaderProperty GradientMapToggle = UberShader.GetProperty(20);

	// Token: 0x04003DCF RID: 15823
	public static UberShaderProperty GradientMap = UberShader.GetProperty(21);

	// Token: 0x04003DD0 RID: 15824
	public static UberShaderProperty DoTextureRotation = UberShader.GetProperty(22);

	// Token: 0x04003DD1 RID: 15825
	public static UberShaderProperty RotateAngle = UberShader.GetProperty(23);

	// Token: 0x04003DD2 RID: 15826
	public static UberShaderProperty RotateAnim = UberShader.GetProperty(24);

	// Token: 0x04003DD3 RID: 15827
	public static UberShaderProperty UseWaveWarp = UberShader.GetProperty(25);

	// Token: 0x04003DD4 RID: 15828
	public static UberShaderProperty WaveAmplitude = UberShader.GetProperty(26);

	// Token: 0x04003DD5 RID: 15829
	public static UberShaderProperty WaveFrequency = UberShader.GetProperty(27);

	// Token: 0x04003DD6 RID: 15830
	public static UberShaderProperty WaveScale = UberShader.GetProperty(28);

	// Token: 0x04003DD7 RID: 15831
	public static UberShaderProperty WaveTimeScale = UberShader.GetProperty(29);

	// Token: 0x04003DD8 RID: 15832
	public static UberShaderProperty UseWeatherMap = UberShader.GetProperty(30);

	// Token: 0x04003DD9 RID: 15833
	public static UberShaderProperty WeatherMap = UberShader.GetProperty(31);

	// Token: 0x04003DDA RID: 15834
	public static UberShaderProperty WeatherMapDissolveEdgeSize = UberShader.GetProperty(32);

	// Token: 0x04003DDB RID: 15835
	public static UberShaderProperty ReflectToggle = UberShader.GetProperty(33);

	// Token: 0x04003DDC RID: 15836
	public static UberShaderProperty ReflectBoxProjectToggle = UberShader.GetProperty(34);

	// Token: 0x04003DDD RID: 15837
	public static UberShaderProperty ReflectBoxCubePos = UberShader.GetProperty(35);

	// Token: 0x04003DDE RID: 15838
	public static UberShaderProperty ReflectBoxSize = UberShader.GetProperty(36);

	// Token: 0x04003DDF RID: 15839
	public static UberShaderProperty ReflectBoxRotation = UberShader.GetProperty(37);

	// Token: 0x04003DE0 RID: 15840
	public static UberShaderProperty ReflectMatcapToggle = UberShader.GetProperty(38);

	// Token: 0x04003DE1 RID: 15841
	public static UberShaderProperty ReflectMatcapPerspToggle = UberShader.GetProperty(39);

	// Token: 0x04003DE2 RID: 15842
	public static UberShaderProperty ReflectNormalToggle = UberShader.GetProperty(40);

	// Token: 0x04003DE3 RID: 15843
	public static UberShaderProperty ReflectTex = UberShader.GetProperty(41);

	// Token: 0x04003DE4 RID: 15844
	public static UberShaderProperty ReflectNormalTex = UberShader.GetProperty(42);

	// Token: 0x04003DE5 RID: 15845
	public static UberShaderProperty ReflectAlbedoTint = UberShader.GetProperty(43);

	// Token: 0x04003DE6 RID: 15846
	public static UberShaderProperty ReflectTint = UberShader.GetProperty(44);

	// Token: 0x04003DE7 RID: 15847
	public static UberShaderProperty ReflectOpacity = UberShader.GetProperty(45);

	// Token: 0x04003DE8 RID: 15848
	public static UberShaderProperty ReflectExposure = UberShader.GetProperty(46);

	// Token: 0x04003DE9 RID: 15849
	public static UberShaderProperty ReflectOffset = UberShader.GetProperty(47);

	// Token: 0x04003DEA RID: 15850
	public static UberShaderProperty ReflectScale = UberShader.GetProperty(48);

	// Token: 0x04003DEB RID: 15851
	public static UberShaderProperty ReflectRotate = UberShader.GetProperty(49);

	// Token: 0x04003DEC RID: 15852
	public static UberShaderProperty HalfLambertToggle = UberShader.GetProperty(50);

	// Token: 0x04003DED RID: 15853
	public static UberShaderProperty ZFightOffset = UberShader.GetProperty(51);

	// Token: 0x04003DEE RID: 15854
	public static UberShaderProperty ParallaxPlanarToggle = UberShader.GetProperty(52);

	// Token: 0x04003DEF RID: 15855
	public static UberShaderProperty ParallaxToggle = UberShader.GetProperty(53);

	// Token: 0x04003DF0 RID: 15856
	public static UberShaderProperty ParallaxAAToggle = UberShader.GetProperty(54);

	// Token: 0x04003DF1 RID: 15857
	public static UberShaderProperty ParallaxAABias = UberShader.GetProperty(55);

	// Token: 0x04003DF2 RID: 15858
	public static UberShaderProperty DepthMap = UberShader.GetProperty(56);

	// Token: 0x04003DF3 RID: 15859
	public static UberShaderProperty ParallaxAmplitude = UberShader.GetProperty(57);

	// Token: 0x04003DF4 RID: 15860
	public static UberShaderProperty ParallaxSamplesMinMax = UberShader.GetProperty(58);

	// Token: 0x04003DF5 RID: 15861
	public static UberShaderProperty UvShiftToggle = UberShader.GetProperty(59);

	// Token: 0x04003DF6 RID: 15862
	public static UberShaderProperty UvShiftSteps = UberShader.GetProperty(60);

	// Token: 0x04003DF7 RID: 15863
	public static UberShaderProperty UvShiftRate = UberShader.GetProperty(61);

	// Token: 0x04003DF8 RID: 15864
	public static UberShaderProperty UvShiftOffset = UberShader.GetProperty(62);

	// Token: 0x04003DF9 RID: 15865
	public static UberShaderProperty UseGridEffect = UberShader.GetProperty(63);

	// Token: 0x04003DFA RID: 15866
	public static UberShaderProperty UseCrystalEffect = UberShader.GetProperty(64);

	// Token: 0x04003DFB RID: 15867
	public static UberShaderProperty CrystalPower = UberShader.GetProperty(65);

	// Token: 0x04003DFC RID: 15868
	public static UberShaderProperty CrystalRimColor = UberShader.GetProperty(66);

	// Token: 0x04003DFD RID: 15869
	public static UberShaderProperty LiquidVolume = UberShader.GetProperty(67);

	// Token: 0x04003DFE RID: 15870
	public static UberShaderProperty LiquidFill = UberShader.GetProperty(68);

	// Token: 0x04003DFF RID: 15871
	public static UberShaderProperty LiquidFillNormal = UberShader.GetProperty(69);

	// Token: 0x04003E00 RID: 15872
	public static UberShaderProperty LiquidSurfaceColor = UberShader.GetProperty(70);

	// Token: 0x04003E01 RID: 15873
	public static UberShaderProperty LiquidSwayX = UberShader.GetProperty(71);

	// Token: 0x04003E02 RID: 15874
	public static UberShaderProperty LiquidSwayY = UberShader.GetProperty(72);

	// Token: 0x04003E03 RID: 15875
	public static UberShaderProperty LiquidContainer = UberShader.GetProperty(73);

	// Token: 0x04003E04 RID: 15876
	public static UberShaderProperty LiquidPlanePosition = UberShader.GetProperty(74);

	// Token: 0x04003E05 RID: 15877
	public static UberShaderProperty LiquidPlaneNormal = UberShader.GetProperty(75);

	// Token: 0x04003E06 RID: 15878
	public static UberShaderProperty VertexFlapToggle = UberShader.GetProperty(76);

	// Token: 0x04003E07 RID: 15879
	public static UberShaderProperty VertexFlapAxis = UberShader.GetProperty(77);

	// Token: 0x04003E08 RID: 15880
	public static UberShaderProperty VertexFlapDegreesMinMax = UberShader.GetProperty(78);

	// Token: 0x04003E09 RID: 15881
	public static UberShaderProperty VertexFlapSpeed = UberShader.GetProperty(79);

	// Token: 0x04003E0A RID: 15882
	public static UberShaderProperty VertexFlapPhaseOffset = UberShader.GetProperty(80);

	// Token: 0x04003E0B RID: 15883
	public static UberShaderProperty VertexWaveToggle = UberShader.GetProperty(81);

	// Token: 0x04003E0C RID: 15884
	public static UberShaderProperty VertexWaveDebug = UberShader.GetProperty(82);

	// Token: 0x04003E0D RID: 15885
	public static UberShaderProperty VertexWaveEnd = UberShader.GetProperty(83);

	// Token: 0x04003E0E RID: 15886
	public static UberShaderProperty VertexWaveParams = UberShader.GetProperty(84);

	// Token: 0x04003E0F RID: 15887
	public static UberShaderProperty VertexWaveFalloff = UberShader.GetProperty(85);

	// Token: 0x04003E10 RID: 15888
	public static UberShaderProperty VertexWaveSphereMask = UberShader.GetProperty(86);

	// Token: 0x04003E11 RID: 15889
	public static UberShaderProperty VertexWavePhaseOffset = UberShader.GetProperty(87);

	// Token: 0x04003E12 RID: 15890
	public static UberShaderProperty VertexWaveAxes = UberShader.GetProperty(88);

	// Token: 0x04003E13 RID: 15891
	public static UberShaderProperty VertexRotateToggle = UberShader.GetProperty(89);

	// Token: 0x04003E14 RID: 15892
	public static UberShaderProperty VertexRotateAngles = UberShader.GetProperty(90);

	// Token: 0x04003E15 RID: 15893
	public static UberShaderProperty VertexRotateAnim = UberShader.GetProperty(91);

	// Token: 0x04003E16 RID: 15894
	public static UberShaderProperty VertexLightToggle = UberShader.GetProperty(92);

	// Token: 0x04003E17 RID: 15895
	public static UberShaderProperty InnerGlowOn = UberShader.GetProperty(93);

	// Token: 0x04003E18 RID: 15896
	public static UberShaderProperty InnerGlowColor = UberShader.GetProperty(94);

	// Token: 0x04003E19 RID: 15897
	public static UberShaderProperty InnerGlowParams = UberShader.GetProperty(95);

	// Token: 0x04003E1A RID: 15898
	public static UberShaderProperty InnerGlowTap = UberShader.GetProperty(96);

	// Token: 0x04003E1B RID: 15899
	public static UberShaderProperty InnerGlowSine = UberShader.GetProperty(97);

	// Token: 0x04003E1C RID: 15900
	public static UberShaderProperty InnerGlowSinePeriod = UberShader.GetProperty(98);

	// Token: 0x04003E1D RID: 15901
	public static UberShaderProperty InnerGlowSinePhaseShift = UberShader.GetProperty(99);

	// Token: 0x04003E1E RID: 15902
	public static UberShaderProperty StealthEffectOn = UberShader.GetProperty(100);

	// Token: 0x04003E1F RID: 15903
	public static UberShaderProperty UseEyeTracking = UberShader.GetProperty(101);

	// Token: 0x04003E20 RID: 15904
	public static UberShaderProperty EyeTileOffsetUV = UberShader.GetProperty(102);

	// Token: 0x04003E21 RID: 15905
	public static UberShaderProperty EyeOverrideUV = UberShader.GetProperty(103);

	// Token: 0x04003E22 RID: 15906
	public static UberShaderProperty EyeOverrideUVTransform = UberShader.GetProperty(104);

	// Token: 0x04003E23 RID: 15907
	public static UberShaderProperty UseMouthFlap = UberShader.GetProperty(105);

	// Token: 0x04003E24 RID: 15908
	public static UberShaderProperty MouthMap = UberShader.GetProperty(106);

	// Token: 0x04003E25 RID: 15909
	public static UberShaderProperty MouthMap_Atlas = UberShader.GetProperty(107);

	// Token: 0x04003E26 RID: 15910
	public static UberShaderProperty MouthMap_AtlasSlice = UberShader.GetProperty(108);

	// Token: 0x04003E27 RID: 15911
	public static UberShaderProperty UseVertexColor = UberShader.GetProperty(109);

	// Token: 0x04003E28 RID: 15912
	public static UberShaderProperty WaterEffect = UberShader.GetProperty(110);

	// Token: 0x04003E29 RID: 15913
	public static UberShaderProperty HeightBasedWaterEffect = UberShader.GetProperty(111);

	// Token: 0x04003E2A RID: 15914
	public static UberShaderProperty UseDayNightLightmap = UberShader.GetProperty(112);

	// Token: 0x04003E2B RID: 15915
	public static UberShaderProperty UseSpecular = UberShader.GetProperty(113);

	// Token: 0x04003E2C RID: 15916
	public static UberShaderProperty UseSpecularAlphaChannel = UberShader.GetProperty(114);

	// Token: 0x04003E2D RID: 15917
	public static UberShaderProperty Smoothness = UberShader.GetProperty(115);

	// Token: 0x04003E2E RID: 15918
	public static UberShaderProperty UseSpecHighlight = UberShader.GetProperty(116);

	// Token: 0x04003E2F RID: 15919
	public static UberShaderProperty SpecularDir = UberShader.GetProperty(117);

	// Token: 0x04003E30 RID: 15920
	public static UberShaderProperty SpecularPowerIntensity = UberShader.GetProperty(118);

	// Token: 0x04003E31 RID: 15921
	public static UberShaderProperty SpecularColor = UberShader.GetProperty(119);

	// Token: 0x04003E32 RID: 15922
	public static UberShaderProperty SpecularUseDiffuseColor = UberShader.GetProperty(120);

	// Token: 0x04003E33 RID: 15923
	public static UberShaderProperty EmissionToggle = UberShader.GetProperty(121);

	// Token: 0x04003E34 RID: 15924
	public static UberShaderProperty EmissionColor = UberShader.GetProperty(122);

	// Token: 0x04003E35 RID: 15925
	public static UberShaderProperty EmissionMap = UberShader.GetProperty(123);

	// Token: 0x04003E36 RID: 15926
	public static UberShaderProperty EmissionMaskByBaseMapAlpha = UberShader.GetProperty(124);

	// Token: 0x04003E37 RID: 15927
	public static UberShaderProperty EmissionUVScrollSpeed = UberShader.GetProperty(125);

	// Token: 0x04003E38 RID: 15928
	public static UberShaderProperty EmissionDissolveProgress = UberShader.GetProperty(126);

	// Token: 0x04003E39 RID: 15929
	public static UberShaderProperty EmissionDissolveAnimation = UberShader.GetProperty(127);

	// Token: 0x04003E3A RID: 15930
	public static UberShaderProperty EmissionDissolveEdgeSize = UberShader.GetProperty(128);

	// Token: 0x04003E3B RID: 15931
	public static UberShaderProperty EmissionUseUVWaveWarp = UberShader.GetProperty(129);

	// Token: 0x04003E3C RID: 15932
	public static UberShaderProperty GreyZoneException = UberShader.GetProperty(130);

	// Token: 0x04003E3D RID: 15933
	public static UberShaderProperty Cull = UberShader.GetProperty(131);

	// Token: 0x04003E3E RID: 15934
	public static UberShaderProperty StencilReference = UberShader.GetProperty(132);

	// Token: 0x04003E3F RID: 15935
	public static UberShaderProperty StencilComparison = UberShader.GetProperty(133);

	// Token: 0x04003E40 RID: 15936
	public static UberShaderProperty StencilPassFront = UberShader.GetProperty(134);

	// Token: 0x04003E41 RID: 15937
	public static UberShaderProperty USE_DEFORM_MAP = UberShader.GetProperty(135);

	// Token: 0x04003E42 RID: 15938
	public static UberShaderProperty DeformMap = UberShader.GetProperty(136);

	// Token: 0x04003E43 RID: 15939
	public static UberShaderProperty DeformMapIntensity = UberShader.GetProperty(137);

	// Token: 0x04003E44 RID: 15940
	public static UberShaderProperty DeformMapMaskByVertColorRAmount = UberShader.GetProperty(138);

	// Token: 0x04003E45 RID: 15941
	public static UberShaderProperty DeformMapScrollSpeed = UberShader.GetProperty(139);

	// Token: 0x04003E46 RID: 15942
	public static UberShaderProperty DeformMapUV0Influence = UberShader.GetProperty(140);

	// Token: 0x04003E47 RID: 15943
	public static UberShaderProperty DeformMapObjectSpaceOffsetsU = UberShader.GetProperty(141);

	// Token: 0x04003E48 RID: 15944
	public static UberShaderProperty DeformMapObjectSpaceOffsetsV = UberShader.GetProperty(142);

	// Token: 0x04003E49 RID: 15945
	public static UberShaderProperty DeformMapWorldSpaceOffsetsU = UberShader.GetProperty(143);

	// Token: 0x04003E4A RID: 15946
	public static UberShaderProperty DeformMapWorldSpaceOffsetsV = UberShader.GetProperty(144);

	// Token: 0x04003E4B RID: 15947
	public static UberShaderProperty RotateOnYAxisBySinTime = UberShader.GetProperty(145);

	// Token: 0x04003E4C RID: 15948
	public static UberShaderProperty USE_TEX_ARRAY_ATLAS = UberShader.GetProperty(146);

	// Token: 0x04003E4D RID: 15949
	public static UberShaderProperty BaseMap_Atlas = UberShader.GetProperty(147);

	// Token: 0x04003E4E RID: 15950
	public static UberShaderProperty BaseMap_AtlasSlice = UberShader.GetProperty(148);

	// Token: 0x04003E4F RID: 15951
	public static UberShaderProperty EmissionMap_Atlas = UberShader.GetProperty(149);

	// Token: 0x04003E50 RID: 15952
	public static UberShaderProperty EmissionMap_AtlasSlice = UberShader.GetProperty(150);

	// Token: 0x04003E51 RID: 15953
	public static UberShaderProperty DeformMap_Atlas = UberShader.GetProperty(151);

	// Token: 0x04003E52 RID: 15954
	public static UberShaderProperty DeformMap_AtlasSlice = UberShader.GetProperty(152);

	// Token: 0x04003E53 RID: 15955
	public static UberShaderProperty DEBUG_PAWN_DATA = UberShader.GetProperty(153);

	// Token: 0x04003E54 RID: 15956
	public static UberShaderProperty SrcBlend = UberShader.GetProperty(154);

	// Token: 0x04003E55 RID: 15957
	public static UberShaderProperty DstBlend = UberShader.GetProperty(155);

	// Token: 0x04003E56 RID: 15958
	public static UberShaderProperty SrcBlendAlpha = UberShader.GetProperty(156);

	// Token: 0x04003E57 RID: 15959
	public static UberShaderProperty DstBlendAlpha = UberShader.GetProperty(157);

	// Token: 0x04003E58 RID: 15960
	public static UberShaderProperty ZWrite = UberShader.GetProperty(158);

	// Token: 0x04003E59 RID: 15961
	public static UberShaderProperty AlphaToMask = UberShader.GetProperty(159);

	// Token: 0x04003E5A RID: 15962
	public static UberShaderProperty Color = UberShader.GetProperty(160);

	// Token: 0x04003E5B RID: 15963
	public static UberShaderProperty Surface = UberShader.GetProperty(161);

	// Token: 0x04003E5C RID: 15964
	public static UberShaderProperty Metallic = UberShader.GetProperty(162);

	// Token: 0x04003E5D RID: 15965
	public static UberShaderProperty SpecColor = UberShader.GetProperty(163);

	// Token: 0x04003E5E RID: 15966
	public static UberShaderProperty DayNightLightmapArray = UberShader.GetProperty(164);

	// Token: 0x04003E5F RID: 15967
	public static UberShaderProperty DayNightLightmapArray_AtlasSlice = UberShader.GetProperty(165);

	// Token: 0x04003E60 RID: 15968
	public static UberShaderProperty SingleLightmap = UberShader.GetProperty(166);
}
