using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FDE RID: 4062
	public class BoingReactorField : BoingBase
	{
		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x06006582 RID: 25986 RVA: 0x0020200E File Offset: 0x0020020E
		public static BoingReactorField.ShaderPropertyIdSet ShaderPropertyId
		{
			get
			{
				if (BoingReactorField.s_shaderPropertyId == null)
				{
					BoingReactorField.s_shaderPropertyId = new BoingReactorField.ShaderPropertyIdSet();
				}
				return BoingReactorField.s_shaderPropertyId;
			}
		}

		// Token: 0x06006583 RID: 25987 RVA: 0x00202028 File Offset: 0x00200228
		public bool UpdateShaderConstants(MaterialPropertyBlock props, float positionSampleMultiplier = 1f, float rotationSampleMultiplier = 1f)
		{
			if (this.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return false;
			}
			if (this.m_fieldParamsBuffer == null || this.m_cellsBuffer == null)
			{
				return false;
			}
			props.SetFloat(BoingReactorField.ShaderPropertyId.PositionSampleMultiplier, positionSampleMultiplier);
			props.SetFloat(BoingReactorField.ShaderPropertyId.RotationSampleMultiplier, rotationSampleMultiplier);
			props.SetBuffer(BoingReactorField.ShaderPropertyId.RenderFieldParams, this.m_fieldParamsBuffer);
			props.SetBuffer(BoingReactorField.ShaderPropertyId.RenderCells, this.m_cellsBuffer);
			return true;
		}

		// Token: 0x06006584 RID: 25988 RVA: 0x002020A4 File Offset: 0x002002A4
		public bool UpdateShaderConstants(Material material, float positionSampleMultiplier = 1f, float rotationSampleMultiplier = 1f)
		{
			if (this.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return false;
			}
			if (this.m_fieldParamsBuffer == null || this.m_cellsBuffer == null)
			{
				return false;
			}
			material.SetFloat(BoingReactorField.ShaderPropertyId.PositionSampleMultiplier, positionSampleMultiplier);
			material.SetFloat(BoingReactorField.ShaderPropertyId.RotationSampleMultiplier, rotationSampleMultiplier);
			material.SetBuffer(BoingReactorField.ShaderPropertyId.RenderFieldParams, this.m_fieldParamsBuffer);
			material.SetBuffer(BoingReactorField.ShaderPropertyId.RenderCells, this.m_cellsBuffer);
			return true;
		}

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x06006585 RID: 25989 RVA: 0x0020211D File Offset: 0x0020031D
		public int GpuResourceSetId
		{
			get
			{
				return this.m_gpuResourceSetId;
			}
		}

		// Token: 0x06006586 RID: 25990 RVA: 0x00202128 File Offset: 0x00200328
		public BoingReactorField()
		{
			this.Params.Init();
			this.m_bounds = Aabb.Empty;
			this.m_init = false;
		}

		// Token: 0x06006587 RID: 25991 RVA: 0x00202208 File Offset: 0x00200408
		public void Reboot()
		{
			this.m_gridCenter = base.transform.position;
			Vector3 vector = this.QuantizeNorm(this.m_gridCenter);
			this.m_qPrevGridCenterNorm = vector;
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			if (cellMoveMode == BoingReactorField.CellMoveModeEnum.Follow)
			{
				this.m_gridCenter = base.transform.position;
				this.m_iCellBaseX = 0;
				this.m_iCellBaseY = 0;
				this.m_iCellBaseZ = 0;
				this.m_iCellBaseZ = 0;
				this.m_iCellBaseZ = 0;
				return;
			}
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.WrapAround)
			{
				return;
			}
			this.m_gridCenter = vector * this.CellSize;
			this.m_iCellBaseX = MathUtil.Modulo((int)this.m_qPrevGridCenterNorm.x, this.CellsX);
			this.m_iCellBaseY = MathUtil.Modulo((int)this.m_qPrevGridCenterNorm.y, this.CellsY);
			this.m_iCellBaseZ = MathUtil.Modulo((int)this.m_qPrevGridCenterNorm.z, this.CellsZ);
		}

		// Token: 0x06006588 RID: 25992 RVA: 0x002022E7 File Offset: 0x002004E7
		public void OnEnable()
		{
			this.Reboot();
			BoingManager.Register(this);
		}

		// Token: 0x06006589 RID: 25993 RVA: 0x002022F5 File Offset: 0x002004F5
		public void Start()
		{
			this.Reboot();
			this.m_cellMoveMode = this.CellMoveMode;
		}

		// Token: 0x0600658A RID: 25994 RVA: 0x00202309 File Offset: 0x00200509
		public void OnDisable()
		{
			BoingManager.Unregister(this);
			this.DisposeCpuResources();
			this.DisposeGpuResources();
		}

		// Token: 0x0600658B RID: 25995 RVA: 0x0020231D File Offset: 0x0020051D
		public void DisposeCpuResources()
		{
			this.m_aCpuCell = null;
		}

		// Token: 0x0600658C RID: 25996 RVA: 0x00202328 File Offset: 0x00200528
		public void DisposeGpuResources()
		{
			if (this.m_effectorIndexBuffer != null)
			{
				this.m_effectorIndexBuffer.Dispose();
				this.m_effectorIndexBuffer = null;
			}
			if (this.m_reactorParamsBuffer != null)
			{
				this.m_reactorParamsBuffer.Dispose();
				this.m_reactorParamsBuffer = null;
			}
			if (this.m_fieldParamsBuffer != null)
			{
				this.m_fieldParamsBuffer.Dispose();
				this.m_fieldParamsBuffer = null;
			}
			if (this.m_cellsBuffer != null)
			{
				this.m_cellsBuffer.Dispose();
				this.m_cellsBuffer = null;
			}
			if (this.m_cellsBuffer != null)
			{
				this.m_cellsBuffer.Dispose();
				this.m_cellsBuffer = null;
			}
		}

		// Token: 0x0600658D RID: 25997 RVA: 0x002023B8 File Offset: 0x002005B8
		public bool SampleCpuGrid(Vector3 p, out Vector3 positionOffset, out Vector4 rotationOffset)
		{
			bool flag = false;
			switch (this.FalloffDimensions)
			{
			case BoingReactorField.FalloffDimensionsEnum.XYZ:
				flag = this.m_bounds.Contains(p);
				break;
			case BoingReactorField.FalloffDimensionsEnum.XY:
				flag = (this.m_bounds.ContainsX(p) && this.m_bounds.ContainsY(p));
				break;
			case BoingReactorField.FalloffDimensionsEnum.XZ:
				flag = (this.m_bounds.ContainsX(p) && this.m_bounds.ContainsZ(p));
				break;
			case BoingReactorField.FalloffDimensionsEnum.YZ:
				flag = (this.m_bounds.ContainsY(p) && this.m_bounds.ContainsZ(p));
				break;
			}
			if (!flag)
			{
				positionOffset = Vector3.zero;
				rotationOffset = QuaternionUtil.ToVector4(Quaternion.identity);
				return false;
			}
			float num = 0.5f * this.CellSize;
			Vector3 a = p - (this.m_gridCenter + this.GetCellCenterOffset(0, 0, 0));
			Vector3 vector = this.QuantizeNorm(a + new Vector3(-num, -num, -num));
			Vector3 b = vector * this.CellSize;
			int num2 = Mathf.Clamp((int)vector.x, 0, this.CellsX - 1);
			int num3 = Mathf.Clamp((int)vector.y, 0, this.CellsY - 1);
			int num4 = Mathf.Clamp((int)vector.z, 0, this.CellsZ - 1);
			int x = Mathf.Min(num2 + 1, this.CellsX - 1);
			int y = Mathf.Min(num3 + 1, this.CellsY - 1);
			int z = Mathf.Min(num4 + 1, this.CellsZ - 1);
			int num5;
			int num6;
			int num7;
			this.ResolveCellIndex(num2, num3, num4, 1, out num5, out num6, out num7);
			int num8;
			int num9;
			int num10;
			this.ResolveCellIndex(x, y, z, 1, out num8, out num9, out num10);
			bool lerpX = num5 != num8;
			bool lerpY = num6 != num9;
			bool lerpZ = num7 != num10;
			Vector3 vector2 = (a - b) / this.CellSize;
			Vector3 vector3 = p - base.transform.position;
			switch (this.FalloffDimensions)
			{
			case BoingReactorField.FalloffDimensionsEnum.XY:
				vector3.z = 0f;
				break;
			case BoingReactorField.FalloffDimensionsEnum.XZ:
				vector3.y = 0f;
				break;
			case BoingReactorField.FalloffDimensionsEnum.YZ:
				vector3.x = 0f;
				break;
			}
			int num11 = Mathf.Max(this.CellsX, Mathf.Max(this.CellsY, this.CellsZ));
			float num12 = 1f;
			BoingReactorField.FalloffModeEnum falloffMode = this.FalloffMode;
			if (falloffMode != BoingReactorField.FalloffModeEnum.Circle)
			{
				if (falloffMode == BoingReactorField.FalloffModeEnum.Square)
				{
					Vector3 a2 = num * new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ);
					Vector3 vector4 = this.FalloffRatio * a2 - num * Vector3.one;
					vector4.x = Mathf.Max(0f, vector4.x);
					vector4.y = Mathf.Max(0f, vector4.y);
					vector4.z = Mathf.Max(0f, vector4.z);
					Vector3 vector5 = (1f - this.FalloffRatio) * a2 - num * Vector3.one;
					vector5.x = Mathf.Max(MathUtil.Epsilon, vector5.x);
					vector5.y = Mathf.Max(MathUtil.Epsilon, vector5.y);
					vector5.z = Mathf.Max(MathUtil.Epsilon, vector5.z);
					Vector3 vector6 = new Vector3(1f - Mathf.Clamp01((Mathf.Abs(vector3.x) - vector4.x) / vector5.x), 1f - Mathf.Clamp01((Mathf.Abs(vector3.y) - vector4.y) / vector5.y), 1f - Mathf.Clamp01((Mathf.Abs(vector3.z) - vector4.z) / vector5.z));
					switch (this.FalloffDimensions)
					{
					case BoingReactorField.FalloffDimensionsEnum.XY:
						vector6.x = 1f;
						break;
					case BoingReactorField.FalloffDimensionsEnum.XZ:
						vector6.y = 1f;
						break;
					case BoingReactorField.FalloffDimensionsEnum.YZ:
						vector6.z = 1f;
						break;
					}
					num12 = Mathf.Min(vector6.x, Mathf.Min(vector6.y, vector6.z));
				}
			}
			else
			{
				float num13 = num * (float)num11;
				Vector3 vector7 = new Vector3((float)num11 / (float)this.CellsX, (float)num11 / (float)this.CellsY, (float)num11 / (float)this.CellsZ);
				vector3.x *= vector7.x;
				vector3.y *= vector7.y;
				vector3.z *= vector7.z;
				float magnitude = vector3.magnitude;
				float num14 = Mathf.Max(0f, this.FalloffRatio * num13 - num);
				float num15 = Mathf.Max(MathUtil.Epsilon, (1f - this.FalloffRatio) * num13 - num);
				num12 = 1f - Mathf.Clamp01((magnitude - num14) / num15);
			}
			BoingReactorField.s_aCellOffset[0] = this.m_aCpuCell[num7, num6, num5].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, num3, num4);
			BoingReactorField.s_aCellOffset[1] = this.m_aCpuCell[num7, num6, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(x, num3, num4);
			BoingReactorField.s_aCellOffset[2] = this.m_aCpuCell[num7, num9, num5].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, y, num4);
			BoingReactorField.s_aCellOffset[3] = this.m_aCpuCell[num7, num9, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(x, y, num4);
			BoingReactorField.s_aCellOffset[4] = this.m_aCpuCell[num10, num6, num5].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, num3, z);
			BoingReactorField.s_aCellOffset[5] = this.m_aCpuCell[num10, num6, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(x, num3, z);
			BoingReactorField.s_aCellOffset[6] = this.m_aCpuCell[num10, num9, num5].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(num2, y, z);
			BoingReactorField.s_aCellOffset[7] = this.m_aCpuCell[num10, num9, num8].PositionSpring.Value - this.m_gridCenter - this.GetCellCenterOffset(x, y, z);
			positionOffset = VectorUtil.TriLerp(ref BoingReactorField.s_aCellOffset[0], ref BoingReactorField.s_aCellOffset[1], ref BoingReactorField.s_aCellOffset[2], ref BoingReactorField.s_aCellOffset[3], ref BoingReactorField.s_aCellOffset[4], ref BoingReactorField.s_aCellOffset[5], ref BoingReactorField.s_aCellOffset[6], ref BoingReactorField.s_aCellOffset[7], lerpX, lerpY, lerpZ, vector2.x, vector2.y, vector2.z);
			rotationOffset = VectorUtil.TriLerp(ref this.m_aCpuCell[num7, num6, num5].RotationSpring.ValueVec, ref this.m_aCpuCell[num7, num6, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num7, num9, num5].RotationSpring.ValueVec, ref this.m_aCpuCell[num7, num9, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num6, num5].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num6, num8].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num9, num5].RotationSpring.ValueVec, ref this.m_aCpuCell[num10, num9, num8].RotationSpring.ValueVec, lerpX, lerpY, lerpZ, vector2.x, vector2.y, vector2.z);
			positionOffset *= num12;
			rotationOffset = QuaternionUtil.ToVector4(QuaternionUtil.Pow(QuaternionUtil.FromVector4(rotationOffset, true), num12));
			return true;
		}

		// Token: 0x0600658E RID: 25998 RVA: 0x00202C88 File Offset: 0x00200E88
		private void UpdateFieldParamsGpu()
		{
			this.m_fieldParams.CellsX = this.CellsX;
			this.m_fieldParams.CellsY = this.CellsY;
			this.m_fieldParams.CellsZ = this.CellsZ;
			this.m_fieldParams.NumEffectors = 0;
			if (this.Effectors != null)
			{
				foreach (BoingEffector boingEffector in this.Effectors)
				{
					if (!(boingEffector == null))
					{
						BoingEffector component = boingEffector.GetComponent<BoingEffector>();
						if (!(component == null) && component.isActiveAndEnabled)
						{
							this.m_fieldParams.NumEffectors = this.m_fieldParams.NumEffectors + 1;
						}
					}
				}
			}
			this.m_fieldParams.iCellBaseX = this.m_iCellBaseX;
			this.m_fieldParams.iCellBaseY = this.m_iCellBaseY;
			this.m_fieldParams.iCellBaseZ = this.m_iCellBaseZ;
			this.m_fieldParams.FalloffMode = (int)this.FalloffMode;
			this.m_fieldParams.FalloffDimensions = (int)this.FalloffDimensions;
			this.m_fieldParams.PropagationDepth = this.PropagationDepth;
			this.m_fieldParams.GridCenter = this.m_gridCenter;
			this.m_fieldParams.UpWs = (this.Params.Bits.IsBitSet(6) ? this.Params.RotationReactionUp : (base.transform.rotation * VectorUtil.NormalizeSafe(this.Params.RotationReactionUp, Vector3.up)));
			this.m_fieldParams.FieldPosition = base.transform.position;
			this.m_fieldParams.FalloffRatio = this.FalloffRatio;
			this.m_fieldParams.CellSize = this.CellSize;
			this.m_fieldParams.DeltaTime = Time.deltaTime;
			if (this.m_fieldParamsBuffer != null)
			{
				this.m_fieldParamsBuffer.SetData(new BoingReactorField.FieldParams[]
				{
					this.m_fieldParams
				});
			}
		}

		// Token: 0x0600658F RID: 25999 RVA: 0x00202E6C File Offset: 0x0020106C
		private void UpdateFlags()
		{
			this.Params.Bits.SetBit(0, this.TwoDDistanceCheck);
			this.Params.Bits.SetBit(1, this.TwoDPositionInfluence);
			this.Params.Bits.SetBit(2, this.TwoDRotationInfluence);
			this.Params.Bits.SetBit(3, this.EnablePositionEffect);
			this.Params.Bits.SetBit(4, this.EnableRotationEffect);
			this.Params.Bits.SetBit(6, this.GlobalReactionUpVector);
			this.Params.Bits.SetBit(7, this.EnablePropagation);
			this.Params.Bits.SetBit(8, this.AnchorPropagationAtBorder);
		}

		// Token: 0x06006590 RID: 26000 RVA: 0x00202F34 File Offset: 0x00201134
		public void UpdateBounds()
		{
			this.m_bounds = new Aabb(this.m_gridCenter + this.GetCellCenterOffset(0, 0, 0), this.m_gridCenter + this.GetCellCenterOffset(this.CellsX - 1, this.CellsY - 1, this.CellsZ - 1));
			this.m_bounds.Expand(this.CellSize);
		}

		// Token: 0x06006591 RID: 26001 RVA: 0x00202F9C File Offset: 0x0020119C
		public void PrepareExecute()
		{
			this.Init();
			if (this.SharedParams != null)
			{
				BoingWork.Params.Copy(ref this.SharedParams.Params, ref this.Params);
			}
			this.UpdateFlags();
			this.UpdateBounds();
			BoingReactorField.HardwareModeEnum hardwareMode;
			if (this.m_hardwareMode != this.HardwareMode)
			{
				hardwareMode = this.m_hardwareMode;
				if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
				{
					if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
					{
						this.DisposeGpuResources();
					}
				}
				else
				{
					this.DisposeCpuResources();
				}
				this.m_hardwareMode = this.HardwareMode;
			}
			hardwareMode = this.m_hardwareMode;
			if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					this.ValidateGpuResources();
				}
			}
			else
			{
				this.ValidateCpuResources();
			}
			this.HandleCellMove();
			hardwareMode = this.m_hardwareMode;
			if (hardwareMode == BoingReactorField.HardwareModeEnum.CPU)
			{
				this.FinishPrepareExecuteCpu();
				return;
			}
			if (hardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return;
			}
			this.FinishPrepareExecuteGpu();
		}

		// Token: 0x06006592 RID: 26002 RVA: 0x00203058 File Offset: 0x00201258
		private void ValidateCpuResources()
		{
			this.CellsX = Mathf.Max(1, this.CellsX);
			this.CellsY = Mathf.Max(1, this.CellsY);
			this.CellsZ = Mathf.Max(1, this.CellsZ);
			if (this.m_aCpuCell == null || this.m_cellsX != this.CellsX || this.m_cellsY != this.CellsY || this.m_cellsZ != this.CellsZ)
			{
				this.m_aCpuCell = new BoingWork.Params.InstanceData[this.CellsZ, this.CellsY, this.CellsX];
				for (int i = 0; i < this.CellsZ; i++)
				{
					for (int j = 0; j < this.CellsY; j++)
					{
						for (int k = 0; k < this.CellsX; k++)
						{
							int x;
							int y;
							int z;
							this.ResolveCellIndex(k, j, i, -1, out x, out y, out z);
							this.m_aCpuCell[i, j, k].Reset(this.m_gridCenter + this.GetCellCenterOffset(x, y, z), false);
						}
					}
				}
				this.m_cellsX = this.CellsX;
				this.m_cellsY = this.CellsY;
				this.m_cellsZ = this.CellsZ;
			}
		}

		// Token: 0x06006593 RID: 26003 RVA: 0x00203188 File Offset: 0x00201388
		private void ValidateGpuResources()
		{
			bool flag = false;
			bool flag2 = this.m_shader == null || BoingReactorField.s_computeKernelId == null;
			if (flag2)
			{
				this.m_shader = Resources.Load<ComputeShader>("Boing Kit/BoingReactorFieldCompute");
				flag = true;
				if (BoingReactorField.s_computeKernelId == null)
				{
					BoingReactorField.s_computeKernelId = new BoingReactorField.ComputeKernelId();
					BoingReactorField.s_computeKernelId.InitKernel = this.m_shader.FindKernel("Init");
					BoingReactorField.s_computeKernelId.MoveKernel = this.m_shader.FindKernel("Move");
					BoingReactorField.s_computeKernelId.WrapXKernel = this.m_shader.FindKernel("WrapX");
					BoingReactorField.s_computeKernelId.WrapYKernel = this.m_shader.FindKernel("WrapY");
					BoingReactorField.s_computeKernelId.WrapZKernel = this.m_shader.FindKernel("WrapZ");
					BoingReactorField.s_computeKernelId.ExecuteKernel = this.m_shader.FindKernel("Execute");
				}
			}
			bool flag3 = this.m_effectorIndexBuffer == null || (this.Effectors != null && this.m_numEffectors != this.Effectors.Length);
			if (flag3 && this.Effectors != null)
			{
				if (this.m_effectorIndexBuffer != null)
				{
					this.m_effectorIndexBuffer.Dispose();
				}
				this.m_effectorIndexBuffer = new ComputeBuffer(this.Effectors.Length, 4);
				flag = true;
				this.m_numEffectors = this.Effectors.Length;
			}
			if (flag2 || flag3)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.EffectorIndices, this.m_effectorIndexBuffer);
			}
			bool flag4 = this.m_reactorParamsBuffer == null;
			if (flag4)
			{
				this.m_reactorParamsBuffer = new ComputeBuffer(1, BoingWork.Params.Stride);
				flag = true;
			}
			if (flag2 || flag4)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.ReactorParams, this.m_reactorParamsBuffer);
			}
			bool flag5 = this.m_fieldParamsBuffer == null;
			if (flag5)
			{
				this.m_fieldParamsBuffer = new ComputeBuffer(1, BoingReactorField.FieldParams.Stride);
				flag = true;
			}
			if (flag2 || flag5)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.InitKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.MoveKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapXKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapYKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapZKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.ComputeFieldParams, this.m_fieldParamsBuffer);
			}
			this.m_cellBufferNeedsReset = (this.m_cellsBuffer == null || this.m_cellsX != this.CellsX || this.m_cellsY != this.CellsY || this.m_cellsZ != this.CellsZ);
			if (this.m_cellBufferNeedsReset)
			{
				if (this.m_cellsBuffer != null)
				{
					this.m_cellsBuffer.Dispose();
				}
				int num = this.CellsX * this.CellsY * this.CellsZ;
				this.m_cellsBuffer = new ComputeBuffer(num, BoingWork.Params.InstanceData.Stride);
				BoingWork.Params.InstanceData[] array = new BoingWork.Params.InstanceData[num];
				for (int i = 0; i < num; i++)
				{
					array[i].PositionSpring.Reset();
					array[i].RotationSpring.Reset();
				}
				this.m_cellsBuffer.SetData(array);
				flag = true;
				this.m_cellsX = this.CellsX;
				this.m_cellsY = this.CellsY;
				this.m_cellsZ = this.CellsZ;
			}
			if (flag2 || this.m_cellBufferNeedsReset)
			{
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.InitKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.MoveKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapXKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapYKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.WrapZKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
				this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.ComputeCells, this.m_cellsBuffer);
			}
			if (flag)
			{
				this.m_gpuResourceSetId++;
				if (this.m_gpuResourceSetId < 0)
				{
					this.m_gpuResourceSetId = -1;
				}
			}
		}

		// Token: 0x06006594 RID: 26004 RVA: 0x00203668 File Offset: 0x00201868
		private void FinishPrepareExecuteCpu()
		{
			Quaternion rotation = base.transform.rotation;
			for (int i = 0; i < this.CellsZ; i++)
			{
				for (int j = 0; j < this.CellsY; j++)
				{
					for (int k = 0; k < this.CellsX; k++)
					{
						int x;
						int y;
						int z;
						this.ResolveCellIndex(k, j, i, -1, out x, out y, out z);
						this.m_aCpuCell[i, j, k].PrepareExecute(ref this.Params, this.m_gridCenter, rotation, this.GetCellCenterOffset(x, y, z));
					}
				}
			}
		}

		// Token: 0x06006595 RID: 26005 RVA: 0x002036F0 File Offset: 0x002018F0
		private void FinishPrepareExecuteGpu()
		{
			if (this.m_cellBufferNeedsReset)
			{
				this.UpdateFieldParamsGpu();
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.InitKernel, this.CellsX, this.CellsY, this.CellsZ);
			}
		}

		// Token: 0x06006596 RID: 26006 RVA: 0x00203727 File Offset: 0x00201927
		public void Init()
		{
			if (this.m_init)
			{
				return;
			}
			this.m_hardwareMode = this.HardwareMode;
			this.m_init = true;
		}

		// Token: 0x06006597 RID: 26007 RVA: 0x00203745 File Offset: 0x00201945
		public void Sanitize()
		{
			if (this.PropagationDepth < 0)
			{
				Debug.LogWarning("Propagation iterations must be a positive number.");
			}
			else if (this.PropagationDepth > 3)
			{
				Debug.LogWarning("For performance reasons, propagation is limited to 3 iterations.");
			}
			this.PropagationDepth = Mathf.Clamp(this.PropagationDepth, 1, 3);
		}

		// Token: 0x06006598 RID: 26008 RVA: 0x00203784 File Offset: 0x00201984
		public void HandleCellMove()
		{
			if (this.m_cellMoveMode != this.CellMoveMode)
			{
				this.Reboot();
				this.m_cellMoveMode = this.CellMoveMode;
			}
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			BoingReactorField.HardwareModeEnum hardwareMode;
			if (cellMoveMode == BoingReactorField.CellMoveModeEnum.Follow)
			{
				Vector3 vector = base.transform.position - this.m_gridCenter;
				hardwareMode = this.HardwareMode;
				if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
				{
					if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
					{
						this.UpdateFieldParamsGpu();
						this.m_shader.SetVector(BoingReactorField.ShaderPropertyId.MoveParams, vector);
						this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.MoveKernel, this.CellsX, this.CellsY, this.CellsZ);
					}
				}
				else
				{
					for (int i = 0; i < this.CellsZ; i++)
					{
						for (int j = 0; j < this.CellsY; j++)
						{
							for (int k = 0; k < this.CellsX; k++)
							{
								ref BoingWork.Params.InstanceData ptr = ref this.m_aCpuCell[i, j, k];
								ptr.PositionSpring.Value = ptr.PositionSpring.Value + vector;
							}
						}
					}
				}
				this.m_gridCenter = base.transform.position;
				this.m_qPrevGridCenterNorm = this.QuantizeNorm(this.m_gridCenter);
				return;
			}
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.WrapAround)
			{
				return;
			}
			this.m_gridCenter = base.transform.position;
			Vector3 vector2 = this.QuantizeNorm(this.m_gridCenter);
			this.m_gridCenter = vector2 * this.CellSize;
			int num = (int)(vector2.x - this.m_qPrevGridCenterNorm.x);
			int num2 = (int)(vector2.y - this.m_qPrevGridCenterNorm.y);
			int num3 = (int)(vector2.z - this.m_qPrevGridCenterNorm.z);
			this.m_qPrevGridCenterNorm = vector2;
			if (num == 0 && num2 == 0 && num3 == 0)
			{
				return;
			}
			hardwareMode = this.m_hardwareMode;
			if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					this.WrapGpu(num, num2, num3);
				}
			}
			else
			{
				this.WrapCpu(num, num2, num3);
			}
			this.m_iCellBaseX = MathUtil.Modulo(this.m_iCellBaseX + num, this.CellsX);
			this.m_iCellBaseY = MathUtil.Modulo(this.m_iCellBaseY + num2, this.CellsY);
			this.m_iCellBaseZ = MathUtil.Modulo(this.m_iCellBaseZ + num3, this.CellsZ);
		}

		// Token: 0x06006599 RID: 26009 RVA: 0x002039C2 File Offset: 0x00201BC2
		private void InitPropagationCpu(ref BoingWork.Params.InstanceData data)
		{
			data.PositionPropagationWorkData = Vector3.zero;
			data.RotationPropagationWorkData = Vector3.zero;
		}

		// Token: 0x0600659A RID: 26010 RVA: 0x002039E0 File Offset: 0x00201BE0
		private void PropagateSpringCpu(ref BoingWork.Params.InstanceData data, float dt)
		{
			data.PositionSpring.Velocity = data.PositionSpring.Velocity + BoingReactorField.kPropagationFactor * this.PositionPropagation * data.PositionPropagationWorkData * dt;
			data.RotationSpring.VelocityVec = data.RotationSpring.VelocityVec + BoingReactorField.kPropagationFactor * this.RotationPropagation * data.RotationPropagationWorkData * dt;
		}

		// Token: 0x0600659B RID: 26011 RVA: 0x00203A60 File Offset: 0x00201C60
		private void ExtendPropagationBorder(ref BoingWork.Params.InstanceData data, float weight, int adjDeltaX, int adjDeltaY, int adjDeltaZ)
		{
			data.PositionPropagationWorkData += weight * (data.PositionOrigin + new Vector3((float)adjDeltaX, (float)adjDeltaY, (float)adjDeltaZ) * this.CellSize);
			data.RotationPropagationWorkData += weight * data.RotationOrigin;
		}

		// Token: 0x0600659C RID: 26012 RVA: 0x00203AD0 File Offset: 0x00201CD0
		private void AccumulatePropagationWeightedNeighbor(ref BoingWork.Params.InstanceData data, ref BoingWork.Params.InstanceData neighbor, float weight)
		{
			data.PositionPropagationWorkData += weight * (neighbor.PositionSpring.Value - neighbor.PositionOrigin);
			data.RotationPropagationWorkData += weight * (neighbor.RotationSpring.ValueVec - neighbor.RotationOrigin);
		}

		// Token: 0x0600659D RID: 26013 RVA: 0x00203B44 File Offset: 0x00201D44
		private void GatherPropagation(ref BoingWork.Params.InstanceData data, float weightSum)
		{
			data.PositionPropagationWorkData = data.PositionPropagationWorkData / weightSum - (data.PositionSpring.Value - data.PositionOrigin);
			data.RotationPropagationWorkData = data.RotationPropagationWorkData / weightSum - (data.RotationSpring.ValueVec - data.RotationOrigin);
		}

		// Token: 0x0600659E RID: 26014 RVA: 0x002039C2 File Offset: 0x00201BC2
		private void AnchorPropagationBorder(ref BoingWork.Params.InstanceData data)
		{
			data.PositionPropagationWorkData = Vector3.zero;
			data.RotationPropagationWorkData = Vector3.zero;
		}

		// Token: 0x0600659F RID: 26015 RVA: 0x00203BAC File Offset: 0x00201DAC
		private void PropagateCpu(float dt)
		{
			int[] array = new int[this.PropagationDepth * 2 + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i - this.PropagationDepth;
			}
			for (int j = 0; j < this.CellsZ; j++)
			{
				for (int k = 0; k < this.CellsY; k++)
				{
					for (int l = 0; l < this.CellsX; l++)
					{
						this.InitPropagationCpu(ref this.m_aCpuCell[j, k, l]);
					}
				}
			}
			for (int m = 0; m < this.CellsZ; m++)
			{
				for (int n = 0; n < this.CellsY; n++)
				{
					for (int num = 0; num < this.CellsX; num++)
					{
						int num2;
						int num3;
						int num4;
						this.ResolveCellIndex(num, n, m, -1, out num2, out num3, out num4);
						float num5 = 0f;
						foreach (int num7 in array)
						{
							foreach (int num9 in array)
							{
								foreach (int num11 in array)
								{
									if (num11 != 0 || num9 != 0 || num7 != 0)
									{
										int num12 = num11 * num11 + num9 * num9 + num7 * num7;
										float num13 = BoingReactorField.s_aSqrtInv[num12];
										num5 += num13;
										if ((this.CellsX <= 2 || ((num2 != 0 || num11 >= 0) && (num2 != this.CellsX - 1 || num11 <= 0))) && (this.CellsY <= 2 || ((num3 != 0 || num9 >= 0) && (num3 != this.CellsY - 1 || num9 <= 0))) && (this.CellsZ <= 2 || ((num4 != 0 || num7 >= 0) && (num4 != this.CellsZ - 1 || num7 <= 0))))
										{
											int num14 = MathUtil.Modulo(num + num11, this.CellsX);
											int num15 = MathUtil.Modulo(n + num9, this.CellsY);
											int num16 = MathUtil.Modulo(m + num7, this.CellsZ);
											this.AccumulatePropagationWeightedNeighbor(ref this.m_aCpuCell[m, n, num], ref this.m_aCpuCell[num16, num15, num14], num13);
										}
									}
								}
							}
						}
						if (num5 > 0f)
						{
							this.GatherPropagation(ref this.m_aCpuCell[m, n, num], num5);
						}
					}
				}
			}
			if (this.AnchorPropagationAtBorder)
			{
				for (int num17 = 0; num17 < this.CellsZ; num17++)
				{
					for (int num18 = 0; num18 < this.CellsY; num18++)
					{
						for (int num19 = 0; num19 < this.CellsX; num19++)
						{
							int num20;
							int num21;
							int num22;
							this.ResolveCellIndex(num19, num18, num17, -1, out num20, out num21, out num22);
							if (((num20 == 0 || num20 == this.CellsX - 1) && this.CellsX > 2) || ((num21 == 0 || num21 == this.CellsY - 1) && this.CellsY > 2) || ((num22 == 0 || num22 == this.CellsZ - 1) && this.CellsZ > 2))
							{
								this.AnchorPropagationBorder(ref this.m_aCpuCell[num17, num18, num19]);
							}
						}
					}
				}
			}
			for (int num23 = 0; num23 < this.CellsZ; num23++)
			{
				for (int num24 = 0; num24 < this.CellsY; num24++)
				{
					for (int num25 = 0; num25 < this.CellsX; num25++)
					{
						this.PropagateSpringCpu(ref this.m_aCpuCell[num23, num24, num25], dt);
					}
				}
			}
		}

		// Token: 0x060065A0 RID: 26016 RVA: 0x00203F5C File Offset: 0x0020215C
		private void WrapCpu(int deltaX, int deltaY, int deltaZ)
		{
			if (deltaX != 0)
			{
				int num = (deltaX > 0) ? -1 : 1;
				for (int i = 0; i < this.CellsZ; i++)
				{
					for (int j = 0; j < this.CellsY; j++)
					{
						int num2 = (deltaX > 0) ? (deltaX - 1) : (this.CellsX + deltaX);
						while (num2 >= 0 && num2 < this.CellsX)
						{
							int num3;
							int num4;
							int num5;
							this.ResolveCellIndex(num2, j, i, 1, out num3, out num4, out num5);
							int x;
							int y;
							int z;
							this.ResolveCellIndex(num3 - deltaX, num4 - deltaY, num5 - deltaZ, -1, out x, out y, out z);
							this.m_aCpuCell[num5, num4, num3].Reset(this.m_gridCenter + this.GetCellCenterOffset(x, y, z), true);
							num2 += num;
						}
					}
				}
			}
			if (deltaY != 0)
			{
				int num6 = (deltaY > 0) ? -1 : 1;
				for (int k = 0; k < this.CellsZ; k++)
				{
					int num7 = (deltaY > 0) ? (deltaY - 1) : (this.CellsY + deltaY);
					while (num7 >= 0 && num7 < this.CellsY)
					{
						for (int l = 0; l < this.CellsX; l++)
						{
							int num8;
							int num9;
							int num10;
							this.ResolveCellIndex(l, num7, k, 1, out num8, out num9, out num10);
							int x2;
							int y2;
							int z2;
							this.ResolveCellIndex(num8 - deltaX, num9 - deltaY, num10 - deltaZ, -1, out x2, out y2, out z2);
							this.m_aCpuCell[num10, num9, num8].Reset(this.m_gridCenter + this.GetCellCenterOffset(x2, y2, z2), true);
						}
						num7 += num6;
					}
				}
			}
			if (deltaZ != 0)
			{
				int num11 = (deltaZ > 0) ? -1 : 1;
				int num12 = (deltaZ > 0) ? (deltaZ - 1) : (this.CellsZ + deltaZ);
				while (num12 >= 0 && num12 < this.CellsZ)
				{
					for (int m = 0; m < this.CellsY; m++)
					{
						for (int n = 0; n < this.CellsX; n++)
						{
							int num13;
							int num14;
							int num15;
							this.ResolveCellIndex(n, m, num12, 1, out num13, out num14, out num15);
							int x3;
							int y3;
							int z3;
							this.ResolveCellIndex(num13 - deltaX, num14 - deltaY, num15 - deltaZ, -1, out x3, out y3, out z3);
							this.m_aCpuCell[num15, num14, num13].Reset(this.m_gridCenter + this.GetCellCenterOffset(x3, y3, z3), true);
						}
					}
					num12 += num11;
				}
			}
		}

		// Token: 0x060065A1 RID: 26017 RVA: 0x002041B0 File Offset: 0x002023B0
		private void WrapGpu(int deltaX, int deltaY, int deltaZ)
		{
			this.UpdateFieldParamsGpu();
			this.m_shader.SetInts(BoingReactorField.ShaderPropertyId.WrapParams, new int[]
			{
				deltaX,
				deltaY,
				deltaZ
			});
			if (deltaX != 0)
			{
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.WrapXKernel, 1, this.CellsY, this.CellsZ);
			}
			if (deltaY != 0)
			{
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.WrapYKernel, this.CellsX, 1, this.CellsZ);
			}
			if (deltaZ != 0)
			{
				this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.WrapZKernel, this.CellsX, this.CellsY, 1);
			}
		}

		// Token: 0x060065A2 RID: 26018 RVA: 0x0020425C File Offset: 0x0020245C
		public void ExecuteCpu(float dt)
		{
			this.PrepareExecute();
			if (this.Effectors == null || this.Effectors.Length == 0)
			{
				return;
			}
			if (this.EnablePropagation)
			{
				this.PropagateCpu(dt);
			}
			foreach (BoingEffector boingEffector in this.Effectors)
			{
				if (!(boingEffector == null))
				{
					BoingEffector.Params @params = default(BoingEffector.Params);
					@params.Fill(boingEffector);
					if (this.m_bounds.Intersects(ref @params))
					{
						for (int j = 0; j < this.CellsZ; j++)
						{
							for (int k = 0; k < this.CellsY; k++)
							{
								for (int l = 0; l < this.CellsX; l++)
								{
									this.m_aCpuCell[j, k, l].AccumulateTarget(ref this.Params, ref @params, dt);
								}
							}
						}
					}
				}
			}
			for (int m = 0; m < this.CellsZ; m++)
			{
				for (int n = 0; n < this.CellsY; n++)
				{
					for (int num = 0; num < this.CellsX; num++)
					{
						this.m_aCpuCell[m, n, num].EndAccumulateTargets(ref this.Params);
						this.m_aCpuCell[m, n, num].Execute(ref this.Params, dt);
					}
				}
			}
		}

		// Token: 0x060065A3 RID: 26019 RVA: 0x002043B0 File Offset: 0x002025B0
		public void ExecuteGpu(float dt, ComputeBuffer effectorParamsBuffer, Dictionary<int, int> effectorParamsIndexMap)
		{
			this.PrepareExecute();
			this.UpdateFieldParamsGpu();
			this.m_shader.SetBuffer(BoingReactorField.s_computeKernelId.ExecuteKernel, BoingReactorField.ShaderPropertyId.Effectors, effectorParamsBuffer);
			if (this.m_fieldParams.NumEffectors > 0)
			{
				int[] array = new int[this.m_fieldParams.NumEffectors];
				int num = 0;
				foreach (BoingEffector boingEffector in this.Effectors)
				{
					if (!(boingEffector == null))
					{
						BoingEffector component = boingEffector.GetComponent<BoingEffector>();
						int num2;
						if (!(component == null) && component.isActiveAndEnabled && effectorParamsIndexMap.TryGetValue(component.GetInstanceID(), out num2))
						{
							array[num++] = num2;
						}
					}
				}
				this.m_effectorIndexBuffer.SetData(array);
			}
			this.s_aReactorParams[0] = this.Params;
			this.m_reactorParamsBuffer.SetData(this.s_aReactorParams);
			this.m_shader.SetVector(BoingReactorField.ShaderPropertyId.PropagationParams, new Vector4(this.PositionPropagation, this.RotationPropagation, BoingReactorField.kPropagationFactor, 0f));
			this.m_shader.Dispatch(BoingReactorField.s_computeKernelId.ExecuteKernel, this.CellsX, this.CellsY, this.CellsZ);
		}

		// Token: 0x060065A4 RID: 26020 RVA: 0x002044EA File Offset: 0x002026EA
		public void OnDrawGizmosSelected()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			this.DrawGizmos(true);
		}

		// Token: 0x060065A5 RID: 26021 RVA: 0x002044FC File Offset: 0x002026FC
		private void DrawGizmos(bool drawEffectors)
		{
			Vector3 vector = this.GetGridCenter();
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.Follow)
			{
				if (cellMoveMode == BoingReactorField.CellMoveModeEnum.WrapAround)
				{
					vector = new Vector3(Mathf.Round(base.transform.position.x / this.CellSize), Mathf.Round(base.transform.position.y / this.CellSize), Mathf.Round(base.transform.position.z / this.CellSize)) * this.CellSize;
				}
			}
			else
			{
				vector = base.transform.position;
			}
			BoingWork.Params.InstanceData[,,] array = null;
			BoingReactorField.HardwareModeEnum hardwareMode = this.HardwareMode;
			if (hardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				if (hardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					if (this.m_cellsBuffer != null)
					{
						array = new BoingWork.Params.InstanceData[this.CellsZ, this.CellsY, this.CellsX];
						this.m_cellsBuffer.GetData(array);
					}
				}
			}
			else
			{
				array = this.m_aCpuCell;
			}
			int num = 1;
			if (this.CellsX * this.CellsY * this.CellsZ > 1024)
			{
				num = 2;
			}
			if (this.CellsX * this.CellsY * this.CellsZ > 4096)
			{
				num = 3;
			}
			if (this.CellsX * this.CellsY * this.CellsZ > 8192)
			{
				num = 4;
			}
			for (int i = 0; i < this.CellsZ; i++)
			{
				for (int j = 0; j < this.CellsY; j++)
				{
					for (int k = 0; k < this.CellsX; k++)
					{
						int x;
						int y;
						int z;
						this.ResolveCellIndex(k, j, i, -1, out x, out y, out z);
						Vector3 center = vector + this.GetCellCenterOffset(x, y, z);
						if (array != null && k % num == 0 && j % num == 0 && i % num == 0)
						{
							BoingWork.Params.InstanceData instanceData = array[i, j, k];
							Gizmos.color = new Color(1f, 1f, 1f, 1f);
							Gizmos.matrix = Matrix4x4.TRS(instanceData.PositionSpring.Value, instanceData.RotationSpring.ValueQuat, Vector3.one);
							Gizmos.DrawCube(Vector3.zero, Mathf.Min(0.1f, 0.5f * this.CellSize) * Vector3.one);
							Gizmos.matrix = Matrix4x4.identity;
						}
						Gizmos.color = new Color(1f, 0.5f, 0.2f, 1f);
						Gizmos.DrawWireCube(center, this.CellSize * Vector3.one);
					}
				}
			}
			BoingReactorField.FalloffModeEnum falloffMode = this.FalloffMode;
			if (falloffMode != BoingReactorField.FalloffModeEnum.Circle)
			{
				if (falloffMode == BoingReactorField.FalloffModeEnum.Square)
				{
					Vector3 size = this.CellSize * this.FalloffRatio * new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ);
					Gizmos.color = new Color(1f, 1f, 0.2f, 0.5f);
					Gizmos.DrawWireCube(vector, size);
				}
			}
			else
			{
				float num2 = (float)Mathf.Max(this.CellsX, Mathf.Max(this.CellsY, this.CellsZ));
				Gizmos.color = new Color(1f, 1f, 0.2f, 0.5f);
				Gizmos.matrix = Matrix4x4.Translate(vector) * Matrix4x4.Scale(new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ) / num2);
				Gizmos.DrawWireSphere(Vector3.zero, 0.5f * this.CellSize * num2 * this.FalloffRatio);
				Gizmos.matrix = Matrix4x4.identity;
			}
			if (drawEffectors && this.Effectors != null)
			{
				foreach (BoingEffector boingEffector in this.Effectors)
				{
					if (!(boingEffector == null))
					{
						boingEffector.OnDrawGizmosSelected();
					}
				}
			}
		}

		// Token: 0x060065A6 RID: 26022 RVA: 0x002048D4 File Offset: 0x00202AD4
		private Vector3 GetGridCenter()
		{
			BoingReactorField.CellMoveModeEnum cellMoveMode = this.CellMoveMode;
			if (cellMoveMode == BoingReactorField.CellMoveModeEnum.Follow)
			{
				return base.transform.position;
			}
			if (cellMoveMode != BoingReactorField.CellMoveModeEnum.WrapAround)
			{
				return base.transform.position;
			}
			return this.QuantizeNorm(base.transform.position) * this.CellSize;
		}

		// Token: 0x060065A7 RID: 26023 RVA: 0x00204925 File Offset: 0x00202B25
		private Vector3 QuantizeNorm(Vector3 p)
		{
			return new Vector3(Mathf.Round(p.x / this.CellSize), Mathf.Round(p.y / this.CellSize), Mathf.Round(p.z / this.CellSize));
		}

		// Token: 0x060065A8 RID: 26024 RVA: 0x00204964 File Offset: 0x00202B64
		private Vector3 GetCellCenterOffset(int x, int y, int z)
		{
			return this.CellSize * (-0.5f * (new Vector3((float)this.CellsX, (float)this.CellsY, (float)this.CellsZ) - Vector3.one) + new Vector3((float)x, (float)y, (float)z));
		}

		// Token: 0x060065A9 RID: 26025 RVA: 0x002049BC File Offset: 0x00202BBC
		private void ResolveCellIndex(int x, int y, int z, int baseMult, out int resX, out int resY, out int resZ)
		{
			resX = MathUtil.Modulo(x + baseMult * this.m_iCellBaseX, this.CellsX);
			resY = MathUtil.Modulo(y + baseMult * this.m_iCellBaseY, this.CellsY);
			resZ = MathUtil.Modulo(z + baseMult * this.m_iCellBaseZ, this.CellsZ);
		}

		// Token: 0x04007031 RID: 28721
		private static BoingReactorField.ShaderPropertyIdSet s_shaderPropertyId;

		// Token: 0x04007032 RID: 28722
		private BoingReactorField.FieldParams m_fieldParams;

		// Token: 0x04007033 RID: 28723
		public BoingReactorField.HardwareModeEnum HardwareMode = BoingReactorField.HardwareModeEnum.GPU;

		// Token: 0x04007034 RID: 28724
		private BoingReactorField.HardwareModeEnum m_hardwareMode;

		// Token: 0x04007035 RID: 28725
		public BoingReactorField.CellMoveModeEnum CellMoveMode = BoingReactorField.CellMoveModeEnum.WrapAround;

		// Token: 0x04007036 RID: 28726
		private BoingReactorField.CellMoveModeEnum m_cellMoveMode;

		// Token: 0x04007037 RID: 28727
		[Range(0.1f, 10f)]
		public float CellSize = 1f;

		// Token: 0x04007038 RID: 28728
		public int CellsX = 8;

		// Token: 0x04007039 RID: 28729
		public int CellsY = 1;

		// Token: 0x0400703A RID: 28730
		public int CellsZ = 8;

		// Token: 0x0400703B RID: 28731
		private int m_cellsX = -1;

		// Token: 0x0400703C RID: 28732
		private int m_cellsY = -1;

		// Token: 0x0400703D RID: 28733
		private int m_cellsZ = -1;

		// Token: 0x0400703E RID: 28734
		private int m_iCellBaseX;

		// Token: 0x0400703F RID: 28735
		private int m_iCellBaseY;

		// Token: 0x04007040 RID: 28736
		private int m_iCellBaseZ;

		// Token: 0x04007041 RID: 28737
		public BoingReactorField.FalloffModeEnum FalloffMode = BoingReactorField.FalloffModeEnum.Square;

		// Token: 0x04007042 RID: 28738
		[Range(0f, 1f)]
		public float FalloffRatio = 0.7f;

		// Token: 0x04007043 RID: 28739
		public BoingReactorField.FalloffDimensionsEnum FalloffDimensions = BoingReactorField.FalloffDimensionsEnum.XZ;

		// Token: 0x04007044 RID: 28740
		public BoingEffector[] Effectors = new BoingEffector[1];

		// Token: 0x04007045 RID: 28741
		private int m_numEffectors = -1;

		// Token: 0x04007046 RID: 28742
		private Aabb m_bounds;

		// Token: 0x04007047 RID: 28743
		public bool TwoDDistanceCheck;

		// Token: 0x04007048 RID: 28744
		public bool TwoDPositionInfluence;

		// Token: 0x04007049 RID: 28745
		public bool TwoDRotationInfluence;

		// Token: 0x0400704A RID: 28746
		public bool EnablePositionEffect = true;

		// Token: 0x0400704B RID: 28747
		public bool EnableRotationEffect = true;

		// Token: 0x0400704C RID: 28748
		public bool GlobalReactionUpVector;

		// Token: 0x0400704D RID: 28749
		public BoingWork.Params Params;

		// Token: 0x0400704E RID: 28750
		public SharedBoingParams SharedParams;

		// Token: 0x0400704F RID: 28751
		public bool EnablePropagation;

		// Token: 0x04007050 RID: 28752
		[Range(0f, 1f)]
		public float PositionPropagation = 1f;

		// Token: 0x04007051 RID: 28753
		[Range(0f, 1f)]
		public float RotationPropagation = 1f;

		// Token: 0x04007052 RID: 28754
		[Range(1f, 3f)]
		public int PropagationDepth = 1;

		// Token: 0x04007053 RID: 28755
		public bool AnchorPropagationAtBorder;

		// Token: 0x04007054 RID: 28756
		private static readonly float kPropagationFactor = 600f;

		// Token: 0x04007055 RID: 28757
		private BoingWork.Params.InstanceData[,,] m_aCpuCell;

		// Token: 0x04007056 RID: 28758
		private ComputeShader m_shader;

		// Token: 0x04007057 RID: 28759
		private ComputeBuffer m_effectorIndexBuffer;

		// Token: 0x04007058 RID: 28760
		private ComputeBuffer m_reactorParamsBuffer;

		// Token: 0x04007059 RID: 28761
		private ComputeBuffer m_fieldParamsBuffer;

		// Token: 0x0400705A RID: 28762
		private ComputeBuffer m_cellsBuffer;

		// Token: 0x0400705B RID: 28763
		private int m_gpuResourceSetId = -1;

		// Token: 0x0400705C RID: 28764
		private static BoingReactorField.ComputeKernelId s_computeKernelId;

		// Token: 0x0400705D RID: 28765
		private bool m_init;

		// Token: 0x0400705E RID: 28766
		private Vector3 m_gridCenter;

		// Token: 0x0400705F RID: 28767
		private Vector3 m_qPrevGridCenterNorm;

		// Token: 0x04007060 RID: 28768
		private static Vector3[] s_aCellOffset = new Vector3[8];

		// Token: 0x04007061 RID: 28769
		private bool m_cellBufferNeedsReset;

		// Token: 0x04007062 RID: 28770
		private static float[] s_aSqrtInv = new float[]
		{
			0f,
			1f,
			0.70711f,
			0.57735f,
			0.5f,
			0.44721f,
			0.40825f,
			0.37796f,
			0.35355f,
			0.33333f,
			0.31623f,
			0.30151f,
			0.28868f,
			0.27735f,
			0.26726f,
			0.2582f,
			0.25f,
			0.24254f,
			0.2357f,
			0.22942f,
			0.22361f,
			0.21822f,
			0.2132f,
			0.20851f,
			0.20412f,
			0.2f,
			0.19612f,
			0.19245f
		};

		// Token: 0x04007063 RID: 28771
		private BoingWork.Params[] s_aReactorParams = new BoingWork.Params[1];

		// Token: 0x02000FDF RID: 4063
		public enum HardwareModeEnum
		{
			// Token: 0x04007065 RID: 28773
			CPU,
			// Token: 0x04007066 RID: 28774
			GPU
		}

		// Token: 0x02000FE0 RID: 4064
		public enum CellMoveModeEnum
		{
			// Token: 0x04007068 RID: 28776
			Follow,
			// Token: 0x04007069 RID: 28777
			WrapAround
		}

		// Token: 0x02000FE1 RID: 4065
		public enum FalloffModeEnum
		{
			// Token: 0x0400706B RID: 28779
			None,
			// Token: 0x0400706C RID: 28780
			Circle,
			// Token: 0x0400706D RID: 28781
			Square
		}

		// Token: 0x02000FE2 RID: 4066
		public enum FalloffDimensionsEnum
		{
			// Token: 0x0400706F RID: 28783
			XYZ,
			// Token: 0x04007070 RID: 28784
			XY,
			// Token: 0x04007071 RID: 28785
			XZ,
			// Token: 0x04007072 RID: 28786
			YZ
		}

		// Token: 0x02000FE3 RID: 4067
		public class ShaderPropertyIdSet
		{
			// Token: 0x060065AB RID: 26027 RVA: 0x00204A44 File Offset: 0x00202C44
			public ShaderPropertyIdSet()
			{
				this.MoveParams = Shader.PropertyToID("moveParams");
				this.WrapParams = Shader.PropertyToID("wrapParams");
				this.Effectors = Shader.PropertyToID("aEffector");
				this.EffectorIndices = Shader.PropertyToID("aEffectorIndex");
				this.ReactorParams = Shader.PropertyToID("reactorParams");
				this.ComputeFieldParams = Shader.PropertyToID("fieldParams");
				this.ComputeCells = Shader.PropertyToID("aCell");
				this.RenderFieldParams = Shader.PropertyToID("aBoingFieldParams");
				this.RenderCells = Shader.PropertyToID("aBoingFieldCell");
				this.PositionSampleMultiplier = Shader.PropertyToID("positionSampleMultiplier");
				this.RotationSampleMultiplier = Shader.PropertyToID("rotationSampleMultiplier");
				this.PropagationParams = Shader.PropertyToID("propagationParams");
			}

			// Token: 0x04007073 RID: 28787
			public int MoveParams;

			// Token: 0x04007074 RID: 28788
			public int WrapParams;

			// Token: 0x04007075 RID: 28789
			public int Effectors;

			// Token: 0x04007076 RID: 28790
			public int EffectorIndices;

			// Token: 0x04007077 RID: 28791
			public int ReactorParams;

			// Token: 0x04007078 RID: 28792
			public int ComputeFieldParams;

			// Token: 0x04007079 RID: 28793
			public int ComputeCells;

			// Token: 0x0400707A RID: 28794
			public int RenderFieldParams;

			// Token: 0x0400707B RID: 28795
			public int RenderCells;

			// Token: 0x0400707C RID: 28796
			public int PositionSampleMultiplier;

			// Token: 0x0400707D RID: 28797
			public int RotationSampleMultiplier;

			// Token: 0x0400707E RID: 28798
			public int PropagationParams;
		}

		// Token: 0x02000FE4 RID: 4068
		private struct FieldParams
		{
			// Token: 0x060065AC RID: 26028 RVA: 0x00204B18 File Offset: 0x00202D18
			private void SuppressWarnings()
			{
				this.m_padding0 = 0;
				this.m_padding1 = 0;
				this.m_padding2 = 0f;
				this.m_padding4 = 0f;
				this.m_padding5 = 0f;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = (int)this.m_padding2;
				this.m_padding2 = this.m_padding3;
				this.m_padding3 = this.m_padding4;
				this.m_padding4 = this.m_padding5;
			}

			// Token: 0x0400707F RID: 28799
			public static readonly int Stride = 112;

			// Token: 0x04007080 RID: 28800
			public int CellsX;

			// Token: 0x04007081 RID: 28801
			public int CellsY;

			// Token: 0x04007082 RID: 28802
			public int CellsZ;

			// Token: 0x04007083 RID: 28803
			public int NumEffectors;

			// Token: 0x04007084 RID: 28804
			public int iCellBaseX;

			// Token: 0x04007085 RID: 28805
			public int iCellBaseY;

			// Token: 0x04007086 RID: 28806
			public int iCellBaseZ;

			// Token: 0x04007087 RID: 28807
			public int m_padding0;

			// Token: 0x04007088 RID: 28808
			public int FalloffMode;

			// Token: 0x04007089 RID: 28809
			public int FalloffDimensions;

			// Token: 0x0400708A RID: 28810
			public int PropagationDepth;

			// Token: 0x0400708B RID: 28811
			public int m_padding1;

			// Token: 0x0400708C RID: 28812
			public Vector3 GridCenter;

			// Token: 0x0400708D RID: 28813
			private float m_padding3;

			// Token: 0x0400708E RID: 28814
			public Vector3 UpWs;

			// Token: 0x0400708F RID: 28815
			private float m_padding2;

			// Token: 0x04007090 RID: 28816
			public Vector3 FieldPosition;

			// Token: 0x04007091 RID: 28817
			public float m_padding4;

			// Token: 0x04007092 RID: 28818
			public float FalloffRatio;

			// Token: 0x04007093 RID: 28819
			public float CellSize;

			// Token: 0x04007094 RID: 28820
			public float DeltaTime;

			// Token: 0x04007095 RID: 28821
			private float m_padding5;
		}

		// Token: 0x02000FE5 RID: 4069
		private class ComputeKernelId
		{
			// Token: 0x04007096 RID: 28822
			public int InitKernel;

			// Token: 0x04007097 RID: 28823
			public int MoveKernel;

			// Token: 0x04007098 RID: 28824
			public int WrapXKernel;

			// Token: 0x04007099 RID: 28825
			public int WrapYKernel;

			// Token: 0x0400709A RID: 28826
			public int WrapZKernel;

			// Token: 0x0400709B RID: 28827
			public int ExecuteKernel;
		}
	}
}
