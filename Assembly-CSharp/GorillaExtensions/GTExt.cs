using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Cysharp.Text;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace GorillaExtensions
{
	// Token: 0x02000E42 RID: 3650
	public static class GTExt
	{
		// Token: 0x06005AAC RID: 23212 RVA: 0x001CA69C File Offset: 0x001C889C
		public static T GetComponentInHierarchy<T>(this Scene scene, bool includeInactive = true) where T : Component
		{
			if (!scene.IsValid())
			{
				return default(T);
			}
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				T component = gameObject.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					component = componentsInChildren[j].GetComponent<T>();
					if (component != null)
					{
						return component;
					}
				}
			}
			return default(T);
		}

		// Token: 0x06005AAD RID: 23213 RVA: 0x001CA734 File Offset: 0x001C8934
		public static List<T> GetComponentsInHierarchy<T>(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			List<T> list = new List<T>(capacity);
			if (!scene.IsValid())
			{
				return list;
			}
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				T[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren<T>(includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list;
		}

		// Token: 0x06005AAE RID: 23214 RVA: 0x001CA77C File Offset: 0x001C897C
		public static List<Object> GetComponentsInHierarchy(this Scene scene, Type type, bool includeInactive = true, int capacity = 64)
		{
			List<Object> list = new List<Object>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				Component[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren(type, includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list;
		}

		// Token: 0x06005AAF RID: 23215 RVA: 0x001CA7B9 File Offset: 0x001C89B9
		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			return scene.GetComponentsInHierarchy(includeInactive, capacity);
		}

		// Token: 0x06005AB0 RID: 23216 RVA: 0x001CA7C4 File Offset: 0x001C89C4
		public static List<T> GetComponentsInHierarchyUntil<T, TStop1>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		// Token: 0x06005AB1 RID: 23217 RVA: 0x001CA808 File Offset: 0x001C8A08
		public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		// Token: 0x06005AB2 RID: 23218 RVA: 0x001CA84C File Offset: 0x001C8A4C
		public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2, TStop3>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		// Token: 0x06005AB3 RID: 23219 RVA: 0x001CA890 File Offset: 0x001C8A90
		public static List<T> GetComponentsInChildrenUntil<T, TStop1>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component
		{
			GTExt.<>c__DisplayClass7_0<T, TStop1> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && root.GetComponent<TStop1>() != null)
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x06005AB4 RID: 23220 RVA: 0x001CA8F0 File Offset: 0x001C8AF0
		public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component
		{
			GTExt.<>c__DisplayClass8_0<T, TStop1, TStop2> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && (root.GetComponent<TStop1>() != null || root.GetComponent<TStop2>() != null))
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|8_0<T, TStop1, TStop2>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x06005AB5 RID: 23221 RVA: 0x001CA964 File Offset: 0x001C8B64
		public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			GTExt.<>c__DisplayClass9_0<T, TStop1, TStop2, TStop3> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && (root.GetComponent<TStop1>() != null || root.GetComponent<TStop2>() != null || root.GetComponent<TStop3>() != null))
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|9_0<T, TStop1, TStop2, TStop3>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x06005AB6 RID: 23222 RVA: 0x001CA9EA File Offset: 0x001C8BEA
		public static void GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(this Component root, out List<T> out_included, out HashSet<T> out_excluded, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			out_included = root.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
			out_excluded = new HashSet<T>(root.GetComponentsInChildren<T>(includeInactive));
			out_excluded.ExceptWith(new HashSet<T>(out_included));
		}

		// Token: 0x06005AB7 RID: 23223 RVA: 0x001CAA18 File Offset: 0x001C8C18
		private static void _GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(Transform currentTransform, List<T> included, List<Component> excluded, bool includeInactive) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if (includeInactive || transform.gameObject.activeSelf)
				{
					Component item;
					if (GTExt._HasAnyComponents<TStop1, TStop2, TStop3>(transform, out item))
					{
						excluded.Add(item);
					}
					else
					{
						T component = transform.GetComponent<T>();
						if (component != null)
						{
							included.Add(component);
						}
						GTExt._GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(transform, included, excluded, includeInactive);
					}
				}
			}
		}

		// Token: 0x06005AB8 RID: 23224 RVA: 0x001CAAB0 File Offset: 0x001C8CB0
		private static bool _HasAnyComponents<TStop1, TStop2, TStop3>(Component component, out Component stopComponent) where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			stopComponent = component.GetComponent<TStop1>();
			if (stopComponent != null)
			{
				return true;
			}
			stopComponent = component.GetComponent<TStop2>();
			if (stopComponent != null)
			{
				return true;
			}
			stopComponent = component.GetComponent<TStop3>();
			return stopComponent != null;
		}

		// Token: 0x06005AB9 RID: 23225 RVA: 0x001CAB0C File Offset: 0x001C8D0C
		public static T GetComponentWithRegex<T>(this Component root, string regexString) where T : Component
		{
			T[] componentsInChildren = root.GetComponentsInChildren<T>();
			Regex regex = new Regex(regexString);
			foreach (T t in componentsInChildren)
			{
				if (regex.IsMatch(t.name))
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x06005ABA RID: 23226 RVA: 0x001CAB5C File Offset: 0x001C8D5C
		private static List<T> GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, string regexString, bool includeInactive, int capacity = 64) where T : Component
		{
			List<T> result = new List<T>(capacity);
			Regex regex = new Regex(regexString);
			GTExt.GetComponentsWithRegex_Internal<T>(allComponents, regex, ref result);
			return result;
		}

		// Token: 0x06005ABB RID: 23227 RVA: 0x001CAB84 File Offset: 0x001C8D84
		private static void GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, Regex regex, ref List<T> foundComponents) where T : Component
		{
			foreach (T t in allComponents)
			{
				string name = t.name;
				if (regex.IsMatch(name))
				{
					foundComponents.Add(t);
				}
			}
		}

		// Token: 0x06005ABC RID: 23228 RVA: 0x001CABE4 File Offset: 0x001C8DE4
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(scene.GetComponentsInHierarchy(includeInactive, capacity), regexString, includeInactive, capacity);
		}

		// Token: 0x06005ABD RID: 23229 RVA: 0x001CABF6 File Offset: 0x001C8DF6
		public static List<T> GetComponentsWithRegex<T>(this Component root, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(root.GetComponentsInChildren<T>(includeInactive), regexString, includeInactive, capacity);
		}

		// Token: 0x06005ABE RID: 23230 RVA: 0x001CAC08 File Offset: 0x001C8E08
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string regexString, bool includeInactive = true, int capacity = 64)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexString, includeInactive, capacity);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x06005ABF RID: 23231 RVA: 0x001CAC70 File Offset: 0x001C8E70
		public static void GetComponentsWithRegex_Internal<T>(this List<T> allComponents, Regex[] regexes, int maxCount, ref List<T> foundComponents) where T : Component
		{
			if (maxCount == 0)
			{
				return;
			}
			int num = 0;
			foreach (T t in allComponents)
			{
				for (int i = 0; i < regexes.Length; i++)
				{
					if (regexes[i].IsMatch(t.name))
					{
						foundComponents.Add(t);
						num++;
						if (maxCount > 0 && num >= maxCount)
						{
							return;
						}
					}
				}
			}
		}

		// Token: 0x06005AC0 RID: 23232 RVA: 0x001CAD00 File Offset: 0x001C8F00
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1, int capacity = 64) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, capacity);
			List<T> result = new List<T>(componentsInHierarchy.Count);
			Regex[] array = new Regex[regexStrings.Length];
			for (int i = 0; i < regexStrings.Length; i++)
			{
				array[i] = new Regex(regexStrings[i]);
			}
			componentsInHierarchy.GetComponentsWithRegex_Internal(array, maxCount, ref result);
			return result;
		}

		// Token: 0x06005AC1 RID: 23233 RVA: 0x001CAD50 File Offset: 0x001C8F50
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, 64);
			List<T> list = new List<T>(componentsInHierarchy.Count);
			if (maxCount == 0)
			{
				return list;
			}
			int num = 0;
			foreach (T t in componentsInHierarchy)
			{
				bool flag = false;
				foreach (string pattern in regexStrings)
				{
					if (!flag && Regex.IsMatch(t.name, pattern))
					{
						foreach (string pattern2 in excludeRegexStrings)
						{
							if (!flag)
							{
								flag = Regex.IsMatch(t.name, pattern2);
							}
						}
						if (!flag)
						{
							list.Add(t);
							num++;
							if (maxCount > 0 && num >= maxCount)
							{
								return list;
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06005AC2 RID: 23234 RVA: 0x001CAE54 File Offset: 0x001C9054
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, includeInactive, maxCount, 64);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x06005AC3 RID: 23235 RVA: 0x001CAEC0 File Offset: 0x001C90C0
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, excludeRegexStrings, includeInactive, maxCount);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x06005AC4 RID: 23236 RVA: 0x001CAF2C File Offset: 0x001C912C
		public static List<T> GetComponentsByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
		{
			T[] componentsInChildren = xform.GetComponentsInChildren<T>(includeInactive);
			List<T> list = new List<T>(componentsInChildren.Length);
			foreach (T t in componentsInChildren)
			{
				if (t.name == name)
				{
					list.Add(t);
				}
			}
			return list;
		}

		// Token: 0x06005AC5 RID: 23237 RVA: 0x001CAF7C File Offset: 0x001C917C
		public static T GetComponentByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
		{
			foreach (T t in xform.GetComponentsInChildren<T>(includeInactive))
			{
				if (t.name == name)
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x06005AC6 RID: 23238 RVA: 0x001CAFC8 File Offset: 0x001C91C8
		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, string name, bool includeInactive = true)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				if (gameObject.name.Contains(name))
				{
					list.Add(gameObject);
				}
				foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(includeInactive))
				{
					if (transform.name.Contains(name))
					{
						list.Add(transform.gameObject);
					}
				}
			}
			return list;
		}

		// Token: 0x06005AC7 RID: 23239 RVA: 0x001CB04A File Offset: 0x001C924A
		public static T GetOrAddComponent<T>(this GameObject gameObject, ref T component) where T : Component
		{
			if (component == null)
			{
				component = gameObject.GetOrAddComponent<T>();
			}
			return component;
		}

		// Token: 0x06005AC8 RID: 23240 RVA: 0x001CB074 File Offset: 0x001C9274
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			T result;
			if (!gameObject.TryGetComponent<T>(out result))
			{
				result = gameObject.AddComponent<T>();
			}
			return result;
		}

		// Token: 0x06005AC9 RID: 23241 RVA: 0x001CB094 File Offset: 0x001C9294
		public static void SetLossyScale(this Transform transform, Vector3 scale)
		{
			scale = transform.InverseTransformVector(scale);
			Vector3 lossyScale = transform.lossyScale;
			transform.localScale = new Vector3(scale.x / lossyScale.x, scale.y / lossyScale.y, scale.z / lossyScale.z);
		}

		// Token: 0x06005ACA RID: 23242 RVA: 0x001CB0E3 File Offset: 0x001C92E3
		public static Quaternion TransformRotation(this Transform transform, Quaternion localRotation)
		{
			return transform.rotation * localRotation;
		}

		// Token: 0x06005ACB RID: 23243 RVA: 0x001CB0F1 File Offset: 0x001C92F1
		public static Quaternion InverseTransformRotation(this Transform transform, Quaternion localRotation)
		{
			return Quaternion.Inverse(transform.rotation) * localRotation;
		}

		// Token: 0x06005ACC RID: 23244 RVA: 0x001CB104 File Offset: 0x001C9304
		public static Vector3 ProjectOnPlane(this Vector3 point, Vector3 planeAnchorPosition, Vector3 planeNormal)
		{
			return planeAnchorPosition + Vector3.ProjectOnPlane(point - planeAnchorPosition, planeNormal);
		}

		// Token: 0x06005ACD RID: 23245 RVA: 0x001CB11C File Offset: 0x001C931C
		public static void ForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T obj = list[i];
				try
				{
					action(obj);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}

		// Token: 0x06005ACE RID: 23246 RVA: 0x001CB164 File Offset: 0x001C9364
		public static void AddSortedUnique<T>(this List<T> list, T item)
		{
			int num = list.BinarySearch(item);
			if (num < 0)
			{
				list.Insert(~num, item);
			}
		}

		// Token: 0x06005ACF RID: 23247 RVA: 0x001CB188 File Offset: 0x001C9388
		public static void SafeForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T obj = list[i];
				try
				{
					action(obj);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}

		// Token: 0x06005AD0 RID: 23248 RVA: 0x001CB1D0 File Offset: 0x001C93D0
		public static bool CompareAs255Unclamped(this Color a, Color b)
		{
			int num = (int)(a.r * 255f);
			int num2 = (int)(a.g * 255f);
			int num3 = (int)(a.b * 255f);
			int num4 = (int)(a.a * 255f);
			int num5 = (int)(b.r * 255f);
			int num6 = (int)(b.g * 255f);
			int num7 = (int)(b.b * 255f);
			int num8 = (int)(b.a * 255f);
			return num == num5 && num2 == num6 && num3 == num7 && num4 == num8;
		}

		// Token: 0x06005AD1 RID: 23249 RVA: 0x001CB264 File Offset: 0x001C9464
		public static Quaternion QuaternionFromToVec(Vector3 toVector, Vector3 fromVector)
		{
			Vector3 vector = Vector3.Cross(fromVector, toVector);
			Debug.Log(vector);
			Debug.Log(vector.magnitude);
			Debug.Log(Vector3.Dot(fromVector, toVector) + 1f);
			Quaternion quaternion = new Quaternion(vector.x, vector.y, vector.z, 1f + Vector3.Dot(toVector, fromVector));
			Debug.Log(quaternion);
			Debug.Log(quaternion.eulerAngles);
			Debug.Log(quaternion.normalized);
			return quaternion.normalized;
		}

		// Token: 0x06005AD2 RID: 23250 RVA: 0x001CB308 File Offset: 0x001C9508
		public static Vector3 Position(this Matrix4x4 matrix)
		{
			float m = matrix.m03;
			float m2 = matrix.m13;
			float m3 = matrix.m23;
			return new Vector3(m, m2, m3);
		}

		// Token: 0x06005AD3 RID: 23251 RVA: 0x001CB330 File Offset: 0x001C9530
		public static Vector3 Scale(this Matrix4x4 m)
		{
			Vector3 result = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
			if (Vector3.Cross(m.GetColumn(0), m.GetColumn(1)).normalized != m.GetColumn(2).normalized)
			{
				result.x *= -1f;
			}
			return result;
		}

		// Token: 0x06005AD4 RID: 23252 RVA: 0x000023F5 File Offset: 0x000005F5
		public static void SetLocalRelativeToParentMatrixWithParityAxis(this Matrix4x4 matrix, GTExt.ParityOptions parity = GTExt.ParityOptions.XFlip)
		{
		}

		// Token: 0x06005AD5 RID: 23253 RVA: 0x001CB3C8 File Offset: 0x001C95C8
		public static void MultiplyInPlaceWith(this Vector3 a, in Vector3 b)
		{
			a.x *= b.x;
			a.y *= b.y;
			a.z *= b.z;
		}

		// Token: 0x06005AD6 RID: 23254 RVA: 0x001CB3FC File Offset: 0x001C95FC
		public static void DecomposeWithXFlip(this Matrix4x4 matrix, out Vector3 transformation, out Quaternion rotation, out Vector3 scale)
		{
			Matrix4x4 matrix2 = matrix;
			bool flag = matrix2.ValidTRS();
			transformation = matrix2.Position();
			Quaternion quaternion;
			if (!flag)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				int num = 2;
				Vector3 forward = matrix2.GetColumnNoCopy(num);
				int num2 = 1;
				quaternion = Quaternion.LookRotation(forward, matrix2.GetColumnNoCopy(num2));
			}
			rotation = quaternion;
			Vector3 vector;
			if (!flag)
			{
				vector = Vector3.zero;
			}
			else
			{
				Matrix4x4 matrix4x = matrix;
				vector = matrix4x.lossyScale;
			}
			scale = vector;
		}

		// Token: 0x06005AD7 RID: 23255 RVA: 0x001CB478 File Offset: 0x001C9678
		public static void SetLocalMatrixRelativeToParentWithXParity(this Transform transform, in Matrix4x4 matrix4X4)
		{
			Vector3 localPosition;
			Quaternion localRotation;
			Vector3 localScale;
			matrix4X4.DecomposeWithXFlip(out localPosition, out localRotation, out localScale);
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
			transform.localScale = localScale;
		}

		// Token: 0x06005AD8 RID: 23256 RVA: 0x001CB4A8 File Offset: 0x001C96A8
		public static Matrix4x4 Matrix4x4Scale(in Vector3 vector)
		{
			Matrix4x4 result;
			result.m00 = vector.x;
			result.m01 = 0f;
			result.m02 = 0f;
			result.m03 = 0f;
			result.m10 = 0f;
			result.m11 = vector.y;
			result.m12 = 0f;
			result.m13 = 0f;
			result.m20 = 0f;
			result.m21 = 0f;
			result.m22 = vector.z;
			result.m23 = 0f;
			result.m30 = 0f;
			result.m31 = 0f;
			result.m32 = 0f;
			result.m33 = 1f;
			return result;
		}

		// Token: 0x06005AD9 RID: 23257 RVA: 0x001CB57C File Offset: 0x001C977C
		public static Vector4 GetColumnNoCopy(this Matrix4x4 matrix, in int index)
		{
			switch (index)
			{
			case 0:
				return new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30);
			case 1:
				return new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31);
			case 2:
				return new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32);
			case 3:
				return new Vector4(matrix.m03, matrix.m13, matrix.m23, matrix.m33);
			default:
				throw new IndexOutOfRangeException("Invalid column index!");
			}
		}

		// Token: 0x06005ADA RID: 23258 RVA: 0x001CB628 File Offset: 0x001C9828
		public static Quaternion RotationWithScaleContext(this Matrix4x4 m, in Vector3 scale)
		{
			Matrix4x4 matrix4x = m * GTExt.Matrix4x4Scale(scale);
			int num = 2;
			Vector3 forward = matrix4x.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(forward, matrix4x.GetColumnNoCopy(num2));
		}

		// Token: 0x06005ADB RID: 23259 RVA: 0x001CB66C File Offset: 0x001C986C
		public static Quaternion Rotation(this Matrix4x4 m)
		{
			int num = 2;
			Vector3 forward = m.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(forward, m.GetColumnNoCopy(num2));
		}

		// Token: 0x06005ADC RID: 23260 RVA: 0x001CB69C File Offset: 0x001C989C
		public static Vector3 x0y(this Vector2 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x06005ADD RID: 23261 RVA: 0x001CB6B4 File Offset: 0x001C98B4
		public static Vector3 x0y(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x06005ADE RID: 23262 RVA: 0x001CB6CC File Offset: 0x001C98CC
		public static Vector3 xy0(this Vector2 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x06005ADF RID: 23263 RVA: 0x001CB6E4 File Offset: 0x001C98E4
		public static Vector3 xy0(this Vector3 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x06005AE0 RID: 23264 RVA: 0x001CB6FC File Offset: 0x001C98FC
		public static Vector3 xz0(this Vector3 v)
		{
			return new Vector3(v.x, v.z, 0f);
		}

		// Token: 0x06005AE1 RID: 23265 RVA: 0x000408AA File Offset: 0x0003EAAA
		public static Vector3 x0z(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		// Token: 0x06005AE2 RID: 23266 RVA: 0x001CB714 File Offset: 0x001C9914
		public static Matrix4x4 LocalMatrixRelativeToParentNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one);
		}

		// Token: 0x06005AE3 RID: 23267 RVA: 0x001CB72C File Offset: 0x001C992C
		public static Matrix4x4 LocalMatrixRelativeToParentWithScale(this Transform transform)
		{
			if (transform.parent == null)
			{
				return transform.localToWorldMatrix;
			}
			return transform.parent.worldToLocalMatrix * transform.localToWorldMatrix;
		}

		// Token: 0x06005AE4 RID: 23268 RVA: 0x001CB759 File Offset: 0x001C9959
		public static void SetLocalMatrixRelativeToParent(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
			transform.localScale = matrix.Scale();
		}

		// Token: 0x06005AE5 RID: 23269 RVA: 0x001CB780 File Offset: 0x001C9980
		public static void SetLocalMatrixRelativeToParentNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
		}

		// Token: 0x06005AE6 RID: 23270 RVA: 0x001CB79B File Offset: 0x001C999B
		public static void SetLocalToWorldMatrixNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.Rotation();
		}

		// Token: 0x06005AE7 RID: 23271 RVA: 0x001CB7B6 File Offset: 0x001C99B6
		public static Matrix4x4 localToWorldNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		}

		// Token: 0x06005AE8 RID: 23272 RVA: 0x001CB7CE File Offset: 0x001C99CE
		public static void SetLocalToWorldMatrixWithScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.rotation;
			transform.SetLossyScale(matrix.lossyScale);
		}

		// Token: 0x06005AE9 RID: 23273 RVA: 0x001CB7F6 File Offset: 0x001C99F6
		public static Matrix4x4 Matrix4X4LerpNoScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.rotation, b.rotation, t), b.lossyScale);
		}

		// Token: 0x06005AEA RID: 23274 RVA: 0x001CB82A File Offset: 0x001C9A2A
		public static Matrix4x4 LerpTo(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpNoScale(a, b, t);
		}

		// Token: 0x06005AEB RID: 23275 RVA: 0x001CB834 File Offset: 0x001C9A34
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNaN(this Vector3 v)
		{
			return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
		}

		// Token: 0x06005AEC RID: 23276 RVA: 0x001CB85D File Offset: 0x001C9A5D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNan(this Quaternion q)
		{
			return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
		}

		// Token: 0x06005AED RID: 23277 RVA: 0x001CB893 File Offset: 0x001C9A93
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInfinity(this Vector3 v)
		{
			return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
		}

		// Token: 0x06005AEE RID: 23278 RVA: 0x001CB8BC File Offset: 0x001C9ABC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInfinity(this Quaternion q)
		{
			return float.IsInfinity(q.x) || float.IsInfinity(q.y) || float.IsInfinity(q.z) || float.IsInfinity(q.w);
		}

		// Token: 0x06005AEF RID: 23279 RVA: 0x001CB8F2 File Offset: 0x001C9AF2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ValuesInRange(this Vector3 v, in float maxVal)
		{
			return Mathf.Abs(v.x) < maxVal && Mathf.Abs(v.y) < maxVal && Mathf.Abs(v.z) < maxVal;
		}

		// Token: 0x06005AF0 RID: 23280 RVA: 0x001CB923 File Offset: 0x001C9B23
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(this Vector3 v, in float maxVal = 10000f)
		{
			return !v.IsNaN() && !v.IsInfinity() && v.ValuesInRange(maxVal);
		}

		// Token: 0x06005AF1 RID: 23281 RVA: 0x001CB940 File Offset: 0x001C9B40
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 GetValidWithFallback(this Vector3 v, in Vector3 safeVal)
		{
			float num = 10000f;
			if (!v.IsValid(num))
			{
				return safeVal;
			}
			return v;
		}

		// Token: 0x06005AF2 RID: 23282 RVA: 0x001CB96C File Offset: 0x001C9B6C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetValueSafe(this Vector3 v, in Vector3 newVal)
		{
			float num = 10000f;
			if (newVal.IsValid(num))
			{
				v = newVal;
			}
		}

		// Token: 0x06005AF3 RID: 23283 RVA: 0x001CB995 File Offset: 0x001C9B95
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsValid(this Quaternion q)
		{
			return !q.IsNan() && !q.IsInfinity();
		}

		// Token: 0x06005AF4 RID: 23284 RVA: 0x001CB9AA File Offset: 0x001C9BAA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion GetValidWithFallback(this Quaternion q, in Quaternion safeVal)
		{
			if (!q.IsValid())
			{
				return safeVal;
			}
			return q;
		}

		// Token: 0x06005AF5 RID: 23285 RVA: 0x001CB9C1 File Offset: 0x001C9BC1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetValueSafe(this Quaternion q, in Quaternion newVal)
		{
			if (newVal.IsValid())
			{
				q = newVal;
			}
		}

		// Token: 0x06005AF6 RID: 23286 RVA: 0x001CB9D8 File Offset: 0x001C9BD8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 ClampMagnitudeSafe(this Vector2 v2, float magnitude)
		{
			if (!float.IsFinite(v2.x))
			{
				v2.x = 0f;
			}
			if (!float.IsFinite(v2.y))
			{
				v2.y = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			return Vector2.ClampMagnitude(v2, magnitude);
		}

		// Token: 0x06005AF7 RID: 23287 RVA: 0x001CBA30 File Offset: 0x001C9C30
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClampThisMagnitudeSafe(this Vector2 v2, float magnitude)
		{
			if (!float.IsFinite(v2.x))
			{
				v2.x = 0f;
			}
			if (!float.IsFinite(v2.y))
			{
				v2.y = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			v2 = Vector2.ClampMagnitude(v2, magnitude);
		}

		// Token: 0x06005AF8 RID: 23288 RVA: 0x001CBA90 File Offset: 0x001C9C90
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ClampMagnitudeSafe(this Vector3 v3, float magnitude)
		{
			if (!float.IsFinite(v3.x))
			{
				v3.x = 0f;
			}
			if (!float.IsFinite(v3.y))
			{
				v3.y = 0f;
			}
			if (!float.IsFinite(v3.z))
			{
				v3.z = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			return Vector3.ClampMagnitude(v3, magnitude);
		}

		// Token: 0x06005AF9 RID: 23289 RVA: 0x001CBB00 File Offset: 0x001C9D00
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ClampThisMagnitudeSafe(this Vector3 v3, float magnitude)
		{
			if (!float.IsFinite(v3.x))
			{
				v3.x = 0f;
			}
			if (!float.IsFinite(v3.y))
			{
				v3.y = 0f;
			}
			if (!float.IsFinite(v3.z))
			{
				v3.z = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			v3 = Vector3.ClampMagnitude(v3, magnitude);
		}

		// Token: 0x06005AFA RID: 23290 RVA: 0x001CBB76 File Offset: 0x001C9D76
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MinSafe(this float value, float min)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			if (value >= min)
			{
				return min;
			}
			return value;
		}

		// Token: 0x06005AFB RID: 23291 RVA: 0x001CBB9D File Offset: 0x001C9D9D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMinSafe(this float value, float min)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			value = ((value < min) ? value : min);
		}

		// Token: 0x06005AFC RID: 23292 RVA: 0x001CBBCA File Offset: 0x001C9DCA
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MinSafe(this double value, float min)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)min))
			{
				min = 0f;
			}
			if (value >= (double)min)
			{
				return (double)min;
			}
			return value;
		}

		// Token: 0x06005AFD RID: 23293 RVA: 0x001CBBF8 File Offset: 0x001C9DF8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMinSafe(this double value, float min)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)min))
			{
				min = 0f;
			}
			value = ((value < (double)min) ? value : ((double)min));
		}

		// Token: 0x06005AFE RID: 23294 RVA: 0x001CBC2C File Offset: 0x001C9E2C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float MaxSafe(this float value, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			if (value <= max)
			{
				return max;
			}
			return value;
		}

		// Token: 0x06005AFF RID: 23295 RVA: 0x001CBC53 File Offset: 0x001C9E53
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMaxSafe(this float value, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			value = ((value > max) ? value : max);
		}

		// Token: 0x06005B00 RID: 23296 RVA: 0x001CBC80 File Offset: 0x001C9E80
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MaxSafe(this double value, float max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)max))
			{
				max = 0f;
			}
			if (value <= (double)max)
			{
				return (double)max;
			}
			return value;
		}

		// Token: 0x06005B01 RID: 23297 RVA: 0x001CBCAE File Offset: 0x001C9EAE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThisMaxSafe(this double value, float max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)max))
			{
				max = 0f;
			}
			value = ((value > (double)max) ? value : ((double)max));
		}

		// Token: 0x06005B02 RID: 23298 RVA: 0x001CBCE2 File Offset: 0x001C9EE2
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ClampSafe(this float value, float min, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		// Token: 0x06005B03 RID: 23299 RVA: 0x001CBD20 File Offset: 0x001C9F20
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ClampSafe(this double value, double min, double max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite(min))
			{
				min = 0.0;
			}
			if (!double.IsFinite(max))
			{
				max = 0.0;
			}
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		// Token: 0x06005B04 RID: 23300 RVA: 0x001CBD73 File Offset: 0x001C9F73
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetFinite(this float value)
		{
			if (!float.IsFinite(value))
			{
				return 0f;
			}
			return value;
		}

		// Token: 0x06005B05 RID: 23301 RVA: 0x001CBD84 File Offset: 0x001C9F84
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double GetFinite(this double value)
		{
			if (!double.IsFinite(value))
			{
				return 0.0;
			}
			return value;
		}

		// Token: 0x06005B06 RID: 23302 RVA: 0x001CBD99 File Offset: 0x001C9F99
		public static Matrix4x4 Matrix4X4LerpHandleNegativeScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.Rotation(), b.Rotation(), t), b.lossyScale);
		}

		// Token: 0x06005B07 RID: 23303 RVA: 0x001CBDCD File Offset: 0x001C9FCD
		public static Matrix4x4 LerpTo_HandleNegativeScale(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpHandleNegativeScale(a, b, t);
		}

		// Token: 0x06005B08 RID: 23304 RVA: 0x001CBDD8 File Offset: 0x001C9FD8
		public static Vector3 LerpToUnclamped(this Vector3 a, in Vector3 b, float t)
		{
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		// Token: 0x06005B09 RID: 23305 RVA: 0x001CBE2C File Offset: 0x001CA02C
		public static string ToLongString(this Vector3 self)
		{
			return string.Format("[{0}, {1}, {2}]", self.x, self.y, self.z);
		}

		// Token: 0x06005B0A RID: 23306 RVA: 0x001CBE59 File Offset: 0x001CA059
		public static int GetRandomIndex<T>(this IReadOnlyList<T> self)
		{
			return Random.Range(0, self.Count);
		}

		// Token: 0x06005B0B RID: 23307 RVA: 0x001CBE67 File Offset: 0x001CA067
		public static T GetRandomItem<T>(this IReadOnlyList<T> self)
		{
			return self[self.GetRandomIndex<T>()];
		}

		// Token: 0x06005B0C RID: 23308 RVA: 0x001CBE75 File Offset: 0x001CA075
		public static Vector2 xx(this float v)
		{
			return new Vector2(v, v);
		}

		// Token: 0x06005B0D RID: 23309 RVA: 0x001CBE7E File Offset: 0x001CA07E
		public static Vector2 xx(this Vector2 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x06005B0E RID: 23310 RVA: 0x001CBE91 File Offset: 0x001CA091
		public static Vector2 xy(this Vector2 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x06005B0F RID: 23311 RVA: 0x001CBEA4 File Offset: 0x001CA0A4
		public static Vector2 yy(this Vector2 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x06005B10 RID: 23312 RVA: 0x001CBEB7 File Offset: 0x001CA0B7
		public static Vector2 xx(this Vector3 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x06005B11 RID: 23313 RVA: 0x001CBECA File Offset: 0x001CA0CA
		public static Vector2 xy(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x06005B12 RID: 23314 RVA: 0x001CBEDD File Offset: 0x001CA0DD
		public static Vector2 xz(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x06005B13 RID: 23315 RVA: 0x001CBEF0 File Offset: 0x001CA0F0
		public static Vector2 yy(this Vector3 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x06005B14 RID: 23316 RVA: 0x001CBF03 File Offset: 0x001CA103
		public static Vector2 yz(this Vector3 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x06005B15 RID: 23317 RVA: 0x001CBF16 File Offset: 0x001CA116
		public static Vector2 zz(this Vector3 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x06005B16 RID: 23318 RVA: 0x001CBF29 File Offset: 0x001CA129
		public static Vector2 xx(this Vector4 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x06005B17 RID: 23319 RVA: 0x001CBF3C File Offset: 0x001CA13C
		public static Vector2 xy(this Vector4 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x06005B18 RID: 23320 RVA: 0x001CBF4F File Offset: 0x001CA14F
		public static Vector2 xz(this Vector4 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x06005B19 RID: 23321 RVA: 0x001CBF62 File Offset: 0x001CA162
		public static Vector2 xw(this Vector4 v)
		{
			return new Vector2(v.x, v.w);
		}

		// Token: 0x06005B1A RID: 23322 RVA: 0x001CBF75 File Offset: 0x001CA175
		public static Vector2 yy(this Vector4 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x06005B1B RID: 23323 RVA: 0x001CBF88 File Offset: 0x001CA188
		public static Vector2 yz(this Vector4 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x06005B1C RID: 23324 RVA: 0x001CBF9B File Offset: 0x001CA19B
		public static Vector2 yw(this Vector4 v)
		{
			return new Vector2(v.y, v.w);
		}

		// Token: 0x06005B1D RID: 23325 RVA: 0x001CBFAE File Offset: 0x001CA1AE
		public static Vector2 zz(this Vector4 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x06005B1E RID: 23326 RVA: 0x001CBFC1 File Offset: 0x001CA1C1
		public static Vector2 zw(this Vector4 v)
		{
			return new Vector2(v.z, v.w);
		}

		// Token: 0x06005B1F RID: 23327 RVA: 0x001CBFD4 File Offset: 0x001CA1D4
		public static Vector2 ww(this Vector4 v)
		{
			return new Vector2(v.w, v.w);
		}

		// Token: 0x06005B20 RID: 23328 RVA: 0x001CBFE7 File Offset: 0x001CA1E7
		public static Vector3 xxx(this float v)
		{
			return new Vector3(v, v, v);
		}

		// Token: 0x06005B21 RID: 23329 RVA: 0x001CBFF1 File Offset: 0x001CA1F1
		public static Vector3 xxx(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x06005B22 RID: 23330 RVA: 0x001CC00A File Offset: 0x001CA20A
		public static Vector3 xxy(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06005B23 RID: 23331 RVA: 0x001CC023 File Offset: 0x001CA223
		public static Vector3 xyy(this Vector2 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x06005B24 RID: 23332 RVA: 0x001CC03C File Offset: 0x001CA23C
		public static Vector3 yyy(this Vector2 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06005B25 RID: 23333 RVA: 0x001CC055 File Offset: 0x001CA255
		public static Vector3 xxx(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x06005B26 RID: 23334 RVA: 0x001CC06E File Offset: 0x001CA26E
		public static Vector3 xxy(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06005B27 RID: 23335 RVA: 0x001CC087 File Offset: 0x001CA287
		public static Vector3 xxz(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x06005B28 RID: 23336 RVA: 0x001CC0A0 File Offset: 0x001CA2A0
		public static Vector3 xyy(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x06005B29 RID: 23337 RVA: 0x001CC0B9 File Offset: 0x001CA2B9
		public static Vector3 xyz(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x06005B2A RID: 23338 RVA: 0x001CC0D2 File Offset: 0x001CA2D2
		public static Vector3 xzz(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x06005B2B RID: 23339 RVA: 0x001CC0EB File Offset: 0x001CA2EB
		public static Vector3 yyy(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06005B2C RID: 23340 RVA: 0x001CC104 File Offset: 0x001CA304
		public static Vector3 yyz(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x06005B2D RID: 23341 RVA: 0x001CC11D File Offset: 0x001CA31D
		public static Vector3 yzz(this Vector3 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x06005B2E RID: 23342 RVA: 0x001CC136 File Offset: 0x001CA336
		public static Vector3 zzz(this Vector3 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x06005B2F RID: 23343 RVA: 0x001CC14F File Offset: 0x001CA34F
		public static Vector3 xxx(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x06005B30 RID: 23344 RVA: 0x001CC168 File Offset: 0x001CA368
		public static Vector3 xxy(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06005B31 RID: 23345 RVA: 0x001CC181 File Offset: 0x001CA381
		public static Vector3 xxz(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x06005B32 RID: 23346 RVA: 0x001CC19A File Offset: 0x001CA39A
		public static Vector3 xxw(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.w);
		}

		// Token: 0x06005B33 RID: 23347 RVA: 0x001CC1B3 File Offset: 0x001CA3B3
		public static Vector3 xyy(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x06005B34 RID: 23348 RVA: 0x001CC1CC File Offset: 0x001CA3CC
		public static Vector3 xyz(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x06005B35 RID: 23349 RVA: 0x001CC1E5 File Offset: 0x001CA3E5
		public static Vector3 xyw(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.w);
		}

		// Token: 0x06005B36 RID: 23350 RVA: 0x001CC1FE File Offset: 0x001CA3FE
		public static Vector3 xzz(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x06005B37 RID: 23351 RVA: 0x001CC217 File Offset: 0x001CA417
		public static Vector3 xzw(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.w);
		}

		// Token: 0x06005B38 RID: 23352 RVA: 0x001CC230 File Offset: 0x001CA430
		public static Vector3 xww(this Vector4 v)
		{
			return new Vector3(v.x, v.w, v.w);
		}

		// Token: 0x06005B39 RID: 23353 RVA: 0x001CC249 File Offset: 0x001CA449
		public static Vector3 yyy(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06005B3A RID: 23354 RVA: 0x001CC262 File Offset: 0x001CA462
		public static Vector3 yyz(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x06005B3B RID: 23355 RVA: 0x001CC27B File Offset: 0x001CA47B
		public static Vector3 yyw(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.w);
		}

		// Token: 0x06005B3C RID: 23356 RVA: 0x001CC294 File Offset: 0x001CA494
		public static Vector3 yzz(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x06005B3D RID: 23357 RVA: 0x001CC2AD File Offset: 0x001CA4AD
		public static Vector3 yzw(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.w);
		}

		// Token: 0x06005B3E RID: 23358 RVA: 0x001CC2C6 File Offset: 0x001CA4C6
		public static Vector3 yww(this Vector4 v)
		{
			return new Vector3(v.y, v.w, v.w);
		}

		// Token: 0x06005B3F RID: 23359 RVA: 0x001CC2DF File Offset: 0x001CA4DF
		public static Vector3 zzz(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x06005B40 RID: 23360 RVA: 0x001CC2F8 File Offset: 0x001CA4F8
		public static Vector3 zzw(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.w);
		}

		// Token: 0x06005B41 RID: 23361 RVA: 0x001CC311 File Offset: 0x001CA511
		public static Vector3 zww(this Vector4 v)
		{
			return new Vector3(v.z, v.w, v.w);
		}

		// Token: 0x06005B42 RID: 23362 RVA: 0x001CC32A File Offset: 0x001CA52A
		public static Vector3 www(this Vector4 v)
		{
			return new Vector3(v.w, v.w, v.w);
		}

		// Token: 0x06005B43 RID: 23363 RVA: 0x001CC343 File Offset: 0x001CA543
		public static Vector4 xxxx(this float v)
		{
			return new Vector4(v, v, v, v);
		}

		// Token: 0x06005B44 RID: 23364 RVA: 0x001CC34E File Offset: 0x001CA54E
		public static Vector4 xxxx(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06005B45 RID: 23365 RVA: 0x001CC36D File Offset: 0x001CA56D
		public static Vector4 xxxy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06005B46 RID: 23366 RVA: 0x001CC38C File Offset: 0x001CA58C
		public static Vector4 xxyy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x06005B47 RID: 23367 RVA: 0x001CC3AB File Offset: 0x001CA5AB
		public static Vector4 xyyy(this Vector2 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x06005B48 RID: 23368 RVA: 0x001CC3CA File Offset: 0x001CA5CA
		public static Vector4 yyyy(this Vector2 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x06005B49 RID: 23369 RVA: 0x001CC3E9 File Offset: 0x001CA5E9
		public static Vector4 xxxx(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06005B4A RID: 23370 RVA: 0x001CC408 File Offset: 0x001CA608
		public static Vector4 xxxy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06005B4B RID: 23371 RVA: 0x001CC427 File Offset: 0x001CA627
		public static Vector4 xxxz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x06005B4C RID: 23372 RVA: 0x001CC446 File Offset: 0x001CA646
		public static Vector4 xxyy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x06005B4D RID: 23373 RVA: 0x001CC465 File Offset: 0x001CA665
		public static Vector4 xxyz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x06005B4E RID: 23374 RVA: 0x001CC484 File Offset: 0x001CA684
		public static Vector4 xxzz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x06005B4F RID: 23375 RVA: 0x001CC4A3 File Offset: 0x001CA6A3
		public static Vector4 xyyy(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x06005B50 RID: 23376 RVA: 0x001CC4C2 File Offset: 0x001CA6C2
		public static Vector4 xyyz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x06005B51 RID: 23377 RVA: 0x001CC4E1 File Offset: 0x001CA6E1
		public static Vector4 xyzz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x06005B52 RID: 23378 RVA: 0x001CC500 File Offset: 0x001CA700
		public static Vector4 xzzz(this Vector3 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x06005B53 RID: 23379 RVA: 0x001CC51F File Offset: 0x001CA71F
		public static Vector4 yyyy(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x06005B54 RID: 23380 RVA: 0x001CC53E File Offset: 0x001CA73E
		public static Vector4 yyyz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x06005B55 RID: 23381 RVA: 0x001CC55D File Offset: 0x001CA75D
		public static Vector4 yyzz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x06005B56 RID: 23382 RVA: 0x001CC57C File Offset: 0x001CA77C
		public static Vector4 yzzz(this Vector3 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x06005B57 RID: 23383 RVA: 0x001CC59B File Offset: 0x001CA79B
		public static Vector4 zzzz(this Vector3 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x06005B58 RID: 23384 RVA: 0x001CC5BA File Offset: 0x001CA7BA
		public static Vector4 xxxx(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06005B59 RID: 23385 RVA: 0x001CC5D9 File Offset: 0x001CA7D9
		public static Vector4 xxxy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06005B5A RID: 23386 RVA: 0x001CC5F8 File Offset: 0x001CA7F8
		public static Vector4 xxxz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x06005B5B RID: 23387 RVA: 0x001CC617 File Offset: 0x001CA817
		public static Vector4 xxxw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.w);
		}

		// Token: 0x06005B5C RID: 23388 RVA: 0x001CC636 File Offset: 0x001CA836
		public static Vector4 xxyy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x06005B5D RID: 23389 RVA: 0x001CC655 File Offset: 0x001CA855
		public static Vector4 xxyz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x06005B5E RID: 23390 RVA: 0x001CC674 File Offset: 0x001CA874
		public static Vector4 xxyw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.w);
		}

		// Token: 0x06005B5F RID: 23391 RVA: 0x001CC693 File Offset: 0x001CA893
		public static Vector4 xxzz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x06005B60 RID: 23392 RVA: 0x001CC6B2 File Offset: 0x001CA8B2
		public static Vector4 xxzw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.w);
		}

		// Token: 0x06005B61 RID: 23393 RVA: 0x001CC6D1 File Offset: 0x001CA8D1
		public static Vector4 xxww(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.w, v.w);
		}

		// Token: 0x06005B62 RID: 23394 RVA: 0x001CC6F0 File Offset: 0x001CA8F0
		public static Vector4 xyyy(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x06005B63 RID: 23395 RVA: 0x001CC70F File Offset: 0x001CA90F
		public static Vector4 xyyz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x06005B64 RID: 23396 RVA: 0x001CC72E File Offset: 0x001CA92E
		public static Vector4 xyyw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.w);
		}

		// Token: 0x06005B65 RID: 23397 RVA: 0x001CC74D File Offset: 0x001CA94D
		public static Vector4 xyzz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x06005B66 RID: 23398 RVA: 0x001CC76C File Offset: 0x001CA96C
		public static Vector4 xyzw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		// Token: 0x06005B67 RID: 23399 RVA: 0x001CC78B File Offset: 0x001CA98B
		public static Vector4 xyww(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.w, v.w);
		}

		// Token: 0x06005B68 RID: 23400 RVA: 0x001CC7AA File Offset: 0x001CA9AA
		public static Vector4 xzzz(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x06005B69 RID: 23401 RVA: 0x001CC7C9 File Offset: 0x001CA9C9
		public static Vector4 xzzw(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.w);
		}

		// Token: 0x06005B6A RID: 23402 RVA: 0x001CC7E8 File Offset: 0x001CA9E8
		public static Vector4 xzww(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.w, v.w);
		}

		// Token: 0x06005B6B RID: 23403 RVA: 0x001CC807 File Offset: 0x001CAA07
		public static Vector4 xwww(this Vector4 v)
		{
			return new Vector4(v.x, v.w, v.w, v.w);
		}

		// Token: 0x06005B6C RID: 23404 RVA: 0x001CC826 File Offset: 0x001CAA26
		public static Vector4 yyyy(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x06005B6D RID: 23405 RVA: 0x001CC845 File Offset: 0x001CAA45
		public static Vector4 yyyz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x06005B6E RID: 23406 RVA: 0x001CC864 File Offset: 0x001CAA64
		public static Vector4 yyyw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.w);
		}

		// Token: 0x06005B6F RID: 23407 RVA: 0x001CC883 File Offset: 0x001CAA83
		public static Vector4 yyzz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x06005B70 RID: 23408 RVA: 0x001CC8A2 File Offset: 0x001CAAA2
		public static Vector4 yyzw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.w);
		}

		// Token: 0x06005B71 RID: 23409 RVA: 0x001CC8C1 File Offset: 0x001CAAC1
		public static Vector4 yyww(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.w, v.w);
		}

		// Token: 0x06005B72 RID: 23410 RVA: 0x001CC8E0 File Offset: 0x001CAAE0
		public static Vector4 yzzz(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x06005B73 RID: 23411 RVA: 0x001CC8FF File Offset: 0x001CAAFF
		public static Vector4 yzzw(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.w);
		}

		// Token: 0x06005B74 RID: 23412 RVA: 0x001CC91E File Offset: 0x001CAB1E
		public static Vector4 yzww(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.w, v.w);
		}

		// Token: 0x06005B75 RID: 23413 RVA: 0x001CC93D File Offset: 0x001CAB3D
		public static Vector4 ywww(this Vector4 v)
		{
			return new Vector4(v.y, v.w, v.w, v.w);
		}

		// Token: 0x06005B76 RID: 23414 RVA: 0x001CC95C File Offset: 0x001CAB5C
		public static Vector4 zzzz(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x06005B77 RID: 23415 RVA: 0x001CC97B File Offset: 0x001CAB7B
		public static Vector4 zzzw(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.w);
		}

		// Token: 0x06005B78 RID: 23416 RVA: 0x001CC99A File Offset: 0x001CAB9A
		public static Vector4 zzww(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.w, v.w);
		}

		// Token: 0x06005B79 RID: 23417 RVA: 0x001CC9B9 File Offset: 0x001CABB9
		public static Vector4 zwww(this Vector4 v)
		{
			return new Vector4(v.z, v.w, v.w, v.w);
		}

		// Token: 0x06005B7A RID: 23418 RVA: 0x001CC9D8 File Offset: 0x001CABD8
		public static Vector4 wwww(this Vector4 v)
		{
			return new Vector4(v.w, v.w, v.w, v.w);
		}

		// Token: 0x06005B7B RID: 23419 RVA: 0x001CC9F7 File Offset: 0x001CABF7
		public static Vector4 WithX(this Vector4 v, float x)
		{
			return new Vector4(x, v.y, v.z, v.w);
		}

		// Token: 0x06005B7C RID: 23420 RVA: 0x001CCA11 File Offset: 0x001CAC11
		public static Vector4 WithY(this Vector4 v, float y)
		{
			return new Vector4(v.x, y, v.z, v.w);
		}

		// Token: 0x06005B7D RID: 23421 RVA: 0x001CCA2B File Offset: 0x001CAC2B
		public static Vector4 WithZ(this Vector4 v, float z)
		{
			return new Vector4(v.x, v.y, z, v.w);
		}

		// Token: 0x06005B7E RID: 23422 RVA: 0x001CCA45 File Offset: 0x001CAC45
		public static Vector4 WithW(this Vector4 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x06005B7F RID: 23423 RVA: 0x001CCA5F File Offset: 0x001CAC5F
		public static Vector3 WithX(this Vector3 v, float x)
		{
			return new Vector3(x, v.y, v.z);
		}

		// Token: 0x06005B80 RID: 23424 RVA: 0x001CCA73 File Offset: 0x001CAC73
		public static Vector3 WithY(this Vector3 v, float y)
		{
			return new Vector3(v.x, y, v.z);
		}

		// Token: 0x06005B81 RID: 23425 RVA: 0x001CCA87 File Offset: 0x001CAC87
		public static Vector3 WithZ(this Vector3 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x06005B82 RID: 23426 RVA: 0x001CCA9B File Offset: 0x001CAC9B
		public static Vector4 WithW(this Vector3 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x06005B83 RID: 23427 RVA: 0x001CCAB5 File Offset: 0x001CACB5
		public static Vector2 WithX(this Vector2 v, float x)
		{
			return new Vector2(x, v.y);
		}

		// Token: 0x06005B84 RID: 23428 RVA: 0x001CCAC3 File Offset: 0x001CACC3
		public static Vector2 WithY(this Vector2 v, float y)
		{
			return new Vector2(v.x, y);
		}

		// Token: 0x06005B85 RID: 23429 RVA: 0x001CCAD1 File Offset: 0x001CACD1
		public static Vector3 WithZ(this Vector2 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x06005B86 RID: 23430 RVA: 0x001CCAE5 File Offset: 0x001CACE5
		public static bool IsShorterThan(this Vector2 v, float len)
		{
			return v.sqrMagnitude < len * len;
		}

		// Token: 0x06005B87 RID: 23431 RVA: 0x001CCAF3 File Offset: 0x001CACF3
		public static bool IsShorterThan(this Vector2 v, Vector2 v2)
		{
			return v.sqrMagnitude < v2.sqrMagnitude;
		}

		// Token: 0x06005B88 RID: 23432 RVA: 0x001CCB05 File Offset: 0x001CAD05
		public static bool IsShorterThan(this Vector3 v, float len)
		{
			return v.sqrMagnitude < len * len;
		}

		// Token: 0x06005B89 RID: 23433 RVA: 0x001CCB13 File Offset: 0x001CAD13
		public static bool IsShorterThan(this Vector3 v, Vector3 v2)
		{
			return v.sqrMagnitude < v2.sqrMagnitude;
		}

		// Token: 0x06005B8A RID: 23434 RVA: 0x001CCB25 File Offset: 0x001CAD25
		public static bool IsLongerThan(this Vector2 v, float len)
		{
			return v.sqrMagnitude > len * len;
		}

		// Token: 0x06005B8B RID: 23435 RVA: 0x001CCB33 File Offset: 0x001CAD33
		public static bool IsLongerThan(this Vector2 v, Vector2 v2)
		{
			return v.sqrMagnitude > v2.sqrMagnitude;
		}

		// Token: 0x06005B8C RID: 23436 RVA: 0x001CCB45 File Offset: 0x001CAD45
		public static bool IsLongerThan(this Vector3 v, float len)
		{
			return v.sqrMagnitude > len * len;
		}

		// Token: 0x06005B8D RID: 23437 RVA: 0x001CCB53 File Offset: 0x001CAD53
		public static bool IsLongerThan(this Vector3 v, Vector3 v2)
		{
			return v.sqrMagnitude > v2.sqrMagnitude;
		}

		// Token: 0x06005B8E RID: 23438 RVA: 0x001CCB68 File Offset: 0x001CAD68
		public static Vector3 GetClosestPoint(this Ray ray, Vector3 target)
		{
			float d = Vector3.Dot(target - ray.origin, ray.direction);
			return ray.origin + ray.direction * d;
		}

		// Token: 0x06005B8F RID: 23439 RVA: 0x001CCBA8 File Offset: 0x001CADA8
		public static float GetClosestDistSqr(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).sqrMagnitude;
		}

		// Token: 0x06005B90 RID: 23440 RVA: 0x001CCBCC File Offset: 0x001CADCC
		public static float GetClosestDistance(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).magnitude;
		}

		// Token: 0x06005B91 RID: 23441 RVA: 0x001CCBF0 File Offset: 0x001CADF0
		public static Vector3 ProjectToPlane(this Ray ray, Vector3 planeOrigin, Vector3 planeNormalMustBeLength1)
		{
			Vector3 rhs = planeOrigin - ray.origin;
			float d = Vector3.Dot(planeNormalMustBeLength1, rhs);
			float d2 = Vector3.Dot(planeNormalMustBeLength1, ray.direction);
			return ray.origin + ray.direction * d / d2;
		}

		// Token: 0x06005B92 RID: 23442 RVA: 0x001CCC40 File Offset: 0x001CAE40
		public static Vector3 ProjectToLine(this Ray ray, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 normalized = (lineEnd - lineStart).normalized;
			Vector3 normalized2 = Vector3.Cross(Vector3.Cross(ray.direction, normalized), normalized).normalized;
			return ray.ProjectToPlane(lineStart, normalized2);
		}

		// Token: 0x06005B93 RID: 23443 RVA: 0x001CCC81 File Offset: 0x001CAE81
		public static bool IsNull(this Object mono)
		{
			return mono == null || !mono;
		}

		// Token: 0x06005B94 RID: 23444 RVA: 0x001CCC91 File Offset: 0x001CAE91
		public static bool IsNotNull(this Object mono)
		{
			return !mono.IsNull();
		}

		// Token: 0x06005B95 RID: 23445 RVA: 0x001CCC9C File Offset: 0x001CAE9C
		public static string GetPath(this Transform transform)
		{
			string text = transform.name;
			while (transform.parent)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return "/" + text;
		}

		// Token: 0x06005B96 RID: 23446 RVA: 0x001CCCE4 File Offset: 0x001CAEE4
		public static string GetPathQ(this Transform transform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				transform.GetPathQ(ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005B97 RID: 23447 RVA: 0x001CCD24 File Offset: 0x001CAF24
		public static void GetPathQ(this Transform transform, ref Utf16ValueStringBuilder sb)
		{
			sb.Append("\"");
			int length = sb.Length;
			do
			{
				if (sb.Length > length)
				{
					sb.Insert(length, "/");
				}
				sb.Insert(length, transform.name);
				transform = transform.parent;
			}
			while (transform != null);
			sb.Append("\"");
		}

		// Token: 0x06005B98 RID: 23448 RVA: 0x001CCD84 File Offset: 0x001CAF84
		public static string GetPath(this Transform transform, int maxDepth)
		{
			string text = transform.name;
			int num = 0;
			while (transform.parent && num < maxDepth)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
				num++;
			}
			return "/" + text;
		}

		// Token: 0x06005B99 RID: 23449 RVA: 0x001CCDD8 File Offset: 0x001CAFD8
		public static string GetPath(this Transform transform, Transform stopper)
		{
			string text = transform.name;
			while (transform.parent && transform.parent != stopper)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return "/" + text;
		}

		// Token: 0x06005B9A RID: 23450 RVA: 0x001CCE2E File Offset: 0x001CB02E
		public static string GetPath(this GameObject gameObject)
		{
			return gameObject.transform.GetPath();
		}

		// Token: 0x06005B9B RID: 23451 RVA: 0x001CCE3B File Offset: 0x001CB03B
		public static void GetPath(this GameObject gameObject, ref Utf16ValueStringBuilder sb)
		{
			gameObject.transform.GetPathQ(ref sb);
		}

		// Token: 0x06005B9C RID: 23452 RVA: 0x001CCE49 File Offset: 0x001CB049
		public static string GetPath(this GameObject gameObject, int limit)
		{
			return gameObject.transform.GetPath(limit);
		}

		// Token: 0x06005B9D RID: 23453 RVA: 0x001CCE58 File Offset: 0x001CB058
		public static string[] GetPaths(this GameObject[] gobj)
		{
			string[] array = new string[gobj.Length];
			for (int i = 0; i < gobj.Length; i++)
			{
				array[i] = gobj[i].GetPath();
			}
			return array;
		}

		// Token: 0x06005B9E RID: 23454 RVA: 0x001CCE88 File Offset: 0x001CB088
		public static string[] GetPaths(this Transform[] xform)
		{
			string[] array = new string[xform.Length];
			for (int i = 0; i < xform.Length; i++)
			{
				array[i] = xform[i].GetPath();
			}
			return array;
		}

		// Token: 0x06005B9F RID: 23455 RVA: 0x001CCEB8 File Offset: 0x001CB0B8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetRelativePath(string fromPath, string toPath, ref Utf16ValueStringBuilder ZStringBuilder)
		{
			if (string.IsNullOrEmpty(fromPath) || string.IsNullOrEmpty(toPath))
			{
				return;
			}
			int num = 0;
			while (num < fromPath.Length && fromPath[num] == '/')
			{
				num++;
			}
			int num2 = 0;
			while (num2 < toPath.Length && toPath[num2] == '/')
			{
				num2++;
			}
			int num3 = -1;
			int num4 = Mathf.Min(fromPath.Length - num, toPath.Length - num2);
			bool flag = true;
			for (int i = 0; i < num4; i++)
			{
				if (fromPath[num + i] != toPath[num2 + i])
				{
					flag = false;
					break;
				}
				if (fromPath[num + i] == '/')
				{
					num3 = i;
				}
			}
			num3 = (flag ? num4 : num3);
			int num5 = (num3 < fromPath.Length - num) ? (num3 + 1) : (fromPath.Length - num);
			int num6 = (num3 < toPath.Length - num2) ? (num3 + 1) : (toPath.Length - num2);
			if (num5 < fromPath.Length - num)
			{
				ZStringBuilder.Append("../");
				for (int j = num5; j < fromPath.Length - num; j++)
				{
					if (fromPath[num + j] == '/')
					{
						ZStringBuilder.Append("../");
					}
				}
			}
			else
			{
				ZStringBuilder.Append((toPath.Length - num2 - num6 > 0) ? "./" : ".");
			}
			ZStringBuilder.Append(toPath, num2 + num6, toPath.Length - (num2 + num6));
		}

		// Token: 0x06005BA0 RID: 23456 RVA: 0x001CD024 File Offset: 0x001CB224
		public static string GetRelativePath(string fromPath, string toPath)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				GTExt.GetRelativePath(fromPath, toPath, ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
				utf16ValueStringBuilder.Dispose();
			}
			return result;
		}

		// Token: 0x06005BA1 RID: 23457 RVA: 0x001CD06C File Offset: 0x001CB26C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetRelativePath(this Transform fromXform, Transform toXform, ref Utf16ValueStringBuilder ZStringBuilder)
		{
			GTExt.GetRelativePath(fromXform.GetPath(), toXform.GetPath(), ref ZStringBuilder);
		}

		// Token: 0x06005BA2 RID: 23458 RVA: 0x001CD080 File Offset: 0x001CB280
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetRelativePath(this Transform fromXform, Transform toXform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				fromXform.GetRelativePath(toXform, ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
				utf16ValueStringBuilder.Dispose();
			}
			return result;
		}

		// Token: 0x06005BA3 RID: 23459 RVA: 0x001CD0C8 File Offset: 0x001CB2C8
		public static void GetPathWithSiblingIndexes(this Transform transform, ref Utf16ValueStringBuilder strBuilder)
		{
			int length = strBuilder.Length;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x06005BA4 RID: 23460 RVA: 0x001CD130 File Offset: 0x001CB330
		public static string GetComponentPath(this Component component, int maxDepth = 2147483647)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				component.GetComponentPath(ref utf16ValueStringBuilder, maxDepth);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005BA5 RID: 23461 RVA: 0x001CD170 File Offset: 0x001CB370
		public static string GetComponentPath<T>(this T component, int maxDepth = 2147483647) where T : Component
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				component.GetComponentPath(ref utf16ValueStringBuilder, maxDepth);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005BA6 RID: 23462 RVA: 0x001CD1B0 File Offset: 0x001CB3B0
		public static void GetComponentPath<T>(this T component, ref Utf16ValueStringBuilder strBuilder, int maxDepth = 2147483647) where T : Component
		{
			Transform transform = component.transform;
			int length = strBuilder.Length;
			if (maxDepth > 0)
			{
				strBuilder.Append("/");
			}
			strBuilder.Append("->/");
			Type typeFromHandle = typeof(T);
			strBuilder.Append(typeFromHandle.Name);
			if (maxDepth <= 0)
			{
				return;
			}
			int num = 0;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				num++;
				if (maxDepth <= num)
				{
					break;
				}
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x06005BA7 RID: 23463 RVA: 0x001CD23C File Offset: 0x001CB43C
		public static void GetComponentPathWithSiblingIndexes<T>(this T component, ref Utf16ValueStringBuilder strBuilder) where T : Component
		{
			Transform transform = component.transform;
			int length = strBuilder.Length;
			strBuilder.Append("/->/");
			Type typeFromHandle = typeof(T);
			strBuilder.Append(typeFromHandle.Name);
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x06005BA8 RID: 23464 RVA: 0x001CD2D0 File Offset: 0x001CB4D0
		public static string GetComponentPathWithSiblingIndexes<T>(this T component) where T : Component
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				component.GetComponentPathWithSiblingIndexes(ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005BA9 RID: 23465 RVA: 0x001CD310 File Offset: 0x001CB510
		public static T GetComponentByPath<T>(this GameObject root, string path) where T : Component
		{
			string[] array = path.Split(new string[]
			{
				"/->/"
			}, StringSplitOptions.None);
			if (array.Length < 2)
			{
				return default(T);
			}
			string[] array2 = array[0].Split(new string[]
			{
				"/"
			}, StringSplitOptions.RemoveEmptyEntries);
			Transform transform = root.transform;
			for (int i = 1; i < array2.Length; i++)
			{
				string n = array2[i];
				transform = transform.Find(n);
				if (transform == null)
				{
					return default(T);
				}
			}
			Type type = Type.GetType(array[1].Split('#', StringSplitOptions.None)[0]);
			if (type == null)
			{
				return default(T);
			}
			Component component = transform.GetComponent(type);
			if (component == null)
			{
				return default(T);
			}
			return component as T;
		}

		// Token: 0x06005BAA RID: 23466 RVA: 0x001CD3EC File Offset: 0x001CB5EC
		public static int GetDepth(this Transform xform)
		{
			int num = 0;
			Transform parent = xform.parent;
			while (parent != null)
			{
				num++;
				parent = parent.parent;
			}
			return num;
		}

		// Token: 0x06005BAB RID: 23467 RVA: 0x001CD41C File Offset: 0x001CB61C
		public static string GetPathWithSiblingIndexes(this Transform transform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				transform.GetPathWithSiblingIndexes(ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06005BAC RID: 23468 RVA: 0x001CD45C File Offset: 0x001CB65C
		public static void GetPathWithSiblingIndexes(this GameObject gameObject, ref Utf16ValueStringBuilder stringBuilder)
		{
			gameObject.transform.GetPathWithSiblingIndexes(ref stringBuilder);
		}

		// Token: 0x06005BAD RID: 23469 RVA: 0x001CD46A File Offset: 0x001CB66A
		public static string GetPathWithSiblingIndexes(this GameObject gameObject)
		{
			return gameObject.transform.GetPathWithSiblingIndexes();
		}

		// Token: 0x06005BAE RID: 23470 RVA: 0x001CD477 File Offset: 0x001CB677
		public static void AddDictValue(Transform xForm, Dictionary<string, Transform> dict)
		{
			GTExt.caseSenseInner.Add(xForm, dict);
		}

		// Token: 0x06005BAF RID: 23471 RVA: 0x001CD485 File Offset: 0x001CB685
		public static void ClearDicts()
		{
			GTExt.caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
			GTExt.caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
		}

		// Token: 0x06005BB0 RID: 23472 RVA: 0x001CD49C File Offset: 0x001CB69C
		public static bool TryFindByExactPath([NotNull] string path, out Transform result, FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			if (findObjectsInactive != FindObjectsInactive.Exclude)
			{
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded && sceneAt.TryFindByExactPath(path, out result))
					{
						return true;
					}
				}
				result = null;
				return false;
			}
			if (path[0] != '/')
			{
				path = "/" + path;
			}
			GameObject gameObject = GameObject.Find(path);
			if (gameObject)
			{
				result = gameObject.transform;
				return true;
			}
			result = null;
			return false;
		}

		// Token: 0x06005BB1 RID: 23473 RVA: 0x001CD528 File Offset: 0x001CB728
		public static bool TryFindByExactPath(this Scene scene, string path, out Transform result)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			return scene.TryFindByExactPath(splitPath, out result);
		}

		// Token: 0x06005BB2 RID: 23474 RVA: 0x001CD55C File Offset: 0x001CB75C
		private static bool TryFindByExactPath(this Scene scene, IReadOnlyList<string> splitPath, out Transform result)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (GTExt.TryFindByExactPath_Internal(rootGameObjects[i].transform, splitPath, 0, out result))
				{
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06005BB3 RID: 23475 RVA: 0x001CD598 File Offset: 0x001CB798
		public static bool TryFindByExactPath(this Transform rootXform, string path, out Transform result)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			using (IEnumerator enumerator = rootXform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, 0, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06005BB4 RID: 23476 RVA: 0x001CD618 File Offset: 0x001CB818
		public static bool TryFindByExactPath(this Transform rootXform, IReadOnlyList<string> splitPath, out Transform result)
		{
			using (IEnumerator enumerator = rootXform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, 0, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06005BB5 RID: 23477 RVA: 0x001CD678 File Offset: 0x001CB878
		private static bool TryFindByExactPath_Internal(Transform current, IReadOnlyList<string> splitPath, int index, out Transform result)
		{
			if (current.name != splitPath[index])
			{
				result = null;
				return false;
			}
			if (index == splitPath.Count - 1)
			{
				result = current;
				return true;
			}
			using (IEnumerator enumerator = current.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, index + 1, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06005BB6 RID: 23478 RVA: 0x001CD704 File Offset: 0x001CB904
		public static bool TryFindByPath(string globPath, out Transform result, bool caseSensitive = false)
		{
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return GTExt._TryFindByPath(null, pathPartsRegex, -1, out result, caseSensitive, true, globPath);
		}

		// Token: 0x06005BB7 RID: 23479 RVA: 0x001CD724 File Offset: 0x001CB924
		public static bool TryFindByPath(this Scene scene, string globPath, out Transform result, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
			}
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return scene.TryFindByPath(pathPartsRegex, out result, globPath, caseSensitive);
		}

		// Token: 0x06005BB8 RID: 23480 RVA: 0x001CD758 File Offset: 0x001CB958
		private static bool TryFindByPath(this Scene scene, IReadOnlyList<string> pathPartsRegex, out Transform result, string globPath, bool caseSensitive = false)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (GTExt._TryFindByPath(rootGameObjects[i].transform, pathPartsRegex, 0, out result, caseSensitive, false, globPath))
				{
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06005BB9 RID: 23481 RVA: 0x001CD798 File Offset: 0x001CB998
		public static bool TryFindByPath(this Transform rootXform, string globPath, out Transform result, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
			}
			char c = globPath[0];
			if (c != ' ' && c != '\n' && c != '\t')
			{
				c = globPath[globPath.Length - 1];
				if (c != ' ' && c != '\n' && c != '\t')
				{
					string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
					return GTExt._TryFindByPath(rootXform, pathPartsRegex, -1, out result, caseSensitive, false, globPath);
				}
			}
			throw new Exception("TryFindByPath: Provided globPath cannot end or start with whitespace.\nProvided globPath=\"" + globPath + "\"");
		}

		// Token: 0x06005BBA RID: 23482 RVA: 0x001CD81B File Offset: 0x001CBA1B
		public static List<string> ShowAllStringsUsed()
		{
			return GTExt.allStringsUsed.Keys.ToList<string>();
		}

		// Token: 0x06005BBB RID: 23483 RVA: 0x001CD82C File Offset: 0x001CBA2C
		private static bool _TryFindByPath(Transform current, IReadOnlyList<string> pathPartsRegex, int index, out Transform result, bool caseSensitive, bool isAtSceneLevel, string joinedPath)
		{
			if (joinedPath != null && !GTExt.allStringsUsed.ContainsKey(joinedPath))
			{
				GTExt.allStringsUsed[joinedPath] = joinedPath;
			}
			if (caseSensitive)
			{
				if (GTExt.caseSenseInner.ContainsKey(current))
				{
					if (GTExt.caseSenseInner[current].ContainsKey(joinedPath))
					{
						result = GTExt.caseSenseInner[current][joinedPath];
						return true;
					}
				}
				else
				{
					GTExt.caseSenseInner[current] = new Dictionary<string, Transform>();
				}
			}
			else if (GTExt.caseInsenseInner.ContainsKey(current))
			{
				if (GTExt.caseInsenseInner[current].ContainsKey(joinedPath))
				{
					result = GTExt.caseInsenseInner[current][joinedPath];
					return true;
				}
			}
			else
			{
				GTExt.caseInsenseInner[current] = new Dictionary<string, Transform>();
			}
			string a;
			if (isAtSceneLevel)
			{
				index = ((index == -1) ? 0 : index);
				a = pathPartsRegex[index];
				if (a == ".." || a == "..**" || a == "**..")
				{
					result = null;
					return false;
				}
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
						for (int j = 0; j < rootGameObjects.Length; j++)
						{
							if (GTExt._TryFindByPath(rootGameObjects[j].transform, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
							{
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
						}
					}
				}
			}
			if (index != -1)
			{
				a = pathPartsRegex[index];
				if (!(a == "."))
				{
					if (!(a == ".."))
					{
						if (a == "**")
						{
							goto IL_50A;
						}
						if (!(a == "..**") && !(a == "**.."))
						{
							if (!Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
							{
								goto IL_8CB;
							}
							if (index == pathPartsRegex.Count - 1)
							{
								result = current;
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
							using (IEnumerator enumerator = current.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index + 1, out result, caseSensitive, false, joinedPath))
									{
										if (caseSensitive)
										{
											GTExt.caseSenseInner[current][joinedPath] = result;
										}
										else
										{
											GTExt.caseInsenseInner[current][joinedPath] = result;
										}
										return true;
									}
								}
							}
							goto IL_8CB;
						}
						else
						{
							string a2;
							do
							{
								index++;
								if (index >= pathPartsRegex.Count)
								{
									break;
								}
								a2 = pathPartsRegex[index];
							}
							while (a2 == "..**" || a2 == "**..");
							if (index == pathPartsRegex.Count)
							{
								result = current.root;
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
							Transform parent = current.parent;
							while (parent)
							{
								if (GTExt._TryFindByPath(parent, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
								{
									if (caseSensitive)
									{
										GTExt.caseSenseInner[current][joinedPath] = result;
									}
									else
									{
										GTExt.caseInsenseInner[current][joinedPath] = result;
									}
									return true;
								}
								using (IEnumerator enumerator = parent.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
										{
											if (caseSensitive)
											{
												GTExt.caseSenseInner[current][joinedPath] = result;
											}
											else
											{
												GTExt.caseInsenseInner[current][joinedPath] = result;
											}
											return true;
										}
									}
								}
								parent = parent.parent;
							}
							if (parent != null)
							{
								goto IL_8CB;
							}
							bool result2 = GTExt._TryFindByPath(current.root, pathPartsRegex, index, out result, caseSensitive, true, joinedPath);
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
								return result2;
							}
							GTExt.caseInsenseInner[current][joinedPath] = result;
							return result2;
						}
					}
				}
				else
				{
					while (pathPartsRegex[index] == ".")
					{
						if (index == pathPartsRegex.Count - 1)
						{
							result = current;
							return true;
						}
						index++;
					}
					if (GTExt._TryFindByPath(current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
					{
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
					using (IEnumerator enumerator = current.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
							{
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
						}
						goto IL_8CB;
					}
				}
				Transform transform = current;
				int num = index;
				while (pathPartsRegex[num] == "..")
				{
					if (num + 1 >= pathPartsRegex.Count)
					{
						result = transform.parent;
						return result != null;
					}
					if (transform.parent == null)
					{
						bool result3 = GTExt._TryFindByPath(transform, pathPartsRegex, num + 1, out result, caseSensitive, true, joinedPath);
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
							return result3;
						}
						GTExt.caseInsenseInner[current][joinedPath] = result;
						return result3;
					}
					else
					{
						transform = transform.parent;
						num++;
					}
				}
				using (IEnumerator enumerator = transform.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, num, out result, caseSensitive, false, joinedPath))
						{
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
							}
							else
							{
								GTExt.caseInsenseInner[current][joinedPath] = result;
							}
							return true;
						}
					}
					goto IL_8CB;
				}
				IL_50A:
				if (index == pathPartsRegex.Count - 1)
				{
					result = ((current.childCount > 0) ? current.GetChild(0) : null);
					return current.childCount > 0;
				}
				if (index <= pathPartsRegex.Count - 1 && Regex.IsMatch(current.name, pathPartsRegex[index + 1], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
				{
					if (index + 2 == pathPartsRegex.Count)
					{
						result = current;
						return true;
					}
					using (IEnumerator enumerator = current.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
							{
								return true;
							}
						}
					}
				}
				Transform transform2;
				if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out transform2, caseSensitive))
				{
					if (index + 2 == pathPartsRegex.Count)
					{
						result = transform2;
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
					if (GTExt._TryFindByPath(transform2, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
					{
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
				}
				IL_8CB:
				result = null;
				if (caseSensitive)
				{
					GTExt.caseSenseInner[current][joinedPath] = result;
				}
				else
				{
					GTExt.caseInsenseInner[current][joinedPath] = result;
				}
				return false;
			}
			if (pathPartsRegex.Count == 0)
			{
				result = null;
				return false;
			}
			a = pathPartsRegex[0];
			if (!(a == ".") && !(a == "..") && !(a == "..**") && !(a == "**.."))
			{
				using (IEnumerator enumerator = current.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath))
						{
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
							}
							else
							{
								GTExt.caseInsenseInner[current][joinedPath] = result;
							}
							return true;
						}
					}
				}
				result = null;
				if (caseSensitive)
				{
					GTExt.caseSenseInner[current][joinedPath] = result;
				}
				else
				{
					GTExt.caseInsenseInner[current][joinedPath] = result;
				}
				return false;
			}
			bool result4 = GTExt._TryFindByPath(current, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath);
			if (caseSensitive)
			{
				GTExt.caseSenseInner[current][joinedPath] = result;
				return result4;
			}
			GTExt.caseInsenseInner[current][joinedPath] = result;
			return result4;
		}

		// Token: 0x06005BBC RID: 23484 RVA: 0x001CE188 File Offset: 0x001CC388
		private static bool _TryBreadthFirstSearchNames(Transform root, string regexPattern, out Transform result, bool caseSensitive)
		{
			Queue<Transform> queue = new Queue<Transform>();
			using (IEnumerator enumerator = root.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform item = (Transform)obj;
					queue.Enqueue(item);
				}
				goto IL_9B;
			}
			IL_3D:
			Transform transform = queue.Dequeue();
			if (Regex.IsMatch(transform.name, regexPattern, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
			{
				result = transform;
				return true;
			}
			foreach (object obj2 in transform)
			{
				Transform item2 = (Transform)obj2;
				queue.Enqueue(item2);
			}
			IL_9B:
			if (queue.Count <= 0)
			{
				result = null;
				return false;
			}
			goto IL_3D;
		}

		// Token: 0x06005BBD RID: 23485 RVA: 0x001CE25C File Offset: 0x001CC45C
		public static T[] FindComponentsByExactPath<T>(string path) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						list.AddRange(sceneAt.FindComponentsByExactPath(path));
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06005BBE RID: 23486 RVA: 0x001CE2D0 File Offset: 0x001CC4D0
		public static T[] FindComponentsByExactPath<T>(this Scene scene, string path) where T : Component
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			return scene.FindComponentsByExactPath(splitPath);
		}

		// Token: 0x06005BBF RID: 23487 RVA: 0x001CE304 File Offset: 0x001CC504
		private static T[] FindComponentsByExactPath<T>(this Scene scene, string[] splitPath) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				GameObject[] rootGameObjects = scene.GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					GTExt._FindComponentsByExactPath<T>(rootGameObjects[i].transform, splitPath, 0, list);
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06005BC0 RID: 23488 RVA: 0x001CE374 File Offset: 0x001CC574
		public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string path) where T : Component
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				foreach (object obj in rootXform)
				{
					GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, 0, list);
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06005BC1 RID: 23489 RVA: 0x001CE420 File Offset: 0x001CC620
		public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string[] splitPath) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				foreach (object obj in rootXform)
				{
					GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, 0, list);
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06005BC2 RID: 23490 RVA: 0x001CE4AC File Offset: 0x001CC6AC
		private static void _FindComponentsByExactPath<T>(Transform current, string[] splitPath, int index, List<T> components) where T : Component
		{
			if (current.name != splitPath[index])
			{
				return;
			}
			if (index == splitPath.Length - 1)
			{
				T component = current.GetComponent<T>();
				if (component)
				{
					components.Add(component);
				}
				return;
			}
			foreach (object obj in current)
			{
				GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, index + 1, components);
			}
		}

		// Token: 0x06005BC3 RID: 23491 RVA: 0x001CE538 File Offset: 0x001CC738
		public static T[] FindComponentsByPathInLoadedScenes<T>(string wildcardPath, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(wildcardPath);
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
						for (int j = 0; j < rootGameObjects.Length; j++)
						{
							GTExt._FindComponentsByPath<T>(rootGameObjects[j].transform, pathPartsRegex, list, caseSensitive);
						}
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06005BC4 RID: 23492 RVA: 0x001CE5D8 File Offset: 0x001CC7D8
		public static T[] FindComponentsByPath<T>(this Scene scene, string globPath, bool caseSensitive = false) where T : Component
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
			}
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return scene.FindComponentsByPath(pathPartsRegex, caseSensitive);
		}

		// Token: 0x06005BC5 RID: 23493 RVA: 0x001CE608 File Offset: 0x001CC808
		private static T[] FindComponentsByPath<T>(this Scene scene, string[] pathPartsRegex, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				GameObject[] rootGameObjects = scene.GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					GTExt._FindComponentsByPath<T>(rootGameObjects[i].transform, pathPartsRegex, list, caseSensitive);
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06005BC6 RID: 23494 RVA: 0x001CE678 File Offset: 0x001CC878
		public static T[] FindComponentsByPath<T>(this Transform rootXform, string globPath, bool caseSensitive = false) where T : Component
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
			}
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return rootXform.FindComponentsByPath(pathPartsRegex, caseSensitive);
		}

		// Token: 0x06005BC7 RID: 23495 RVA: 0x001CE6A8 File Offset: 0x001CC8A8
		public static T[] FindComponentsByPath<T>(this Transform rootXform, string[] pathPartsRegex, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] result;
			using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
			{
				list.EnsureCapacity(64);
				GTExt._FindComponentsByPath<T>(rootXform, pathPartsRegex, list, caseSensitive);
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06005BC8 RID: 23496 RVA: 0x001CE6F8 File Offset: 0x001CC8F8
		public static void _FindComponentsByPath<T>(Transform current, string[] pathPartsRegex, List<T> components, bool caseSensitive) where T : Component
		{
			List<Transform> list;
			using (UnityEngine.Pool.CollectionPool<List<Transform>, Transform>.Get(out list))
			{
				list.EnsureCapacity(64);
				if (GTExt._TryFindAllByPath(current, pathPartsRegex, 0, list, caseSensitive, false))
				{
					for (int i = 0; i < list.Count; i++)
					{
						T[] components2 = list[i].GetComponents<T>();
						components.AddRange(components2);
					}
				}
			}
		}

		// Token: 0x06005BC9 RID: 23497 RVA: 0x001CE76C File Offset: 0x001CC96C
		private static bool _TryFindAllByPath(Transform current, IReadOnlyList<string> pathPartsRegex, int index, List<Transform> results, bool caseSensitive, bool isAtSceneLevel = false)
		{
			bool flag = false;
			string a;
			if (isAtSceneLevel)
			{
				a = pathPartsRegex[index];
				if (a == ".." || a == "..**" || a == "**..")
				{
					return false;
				}
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						foreach (GameObject gameObject in sceneAt.GetRootGameObjects())
						{
							flag |= GTExt._TryFindAllByPath(gameObject.transform, pathPartsRegex, index, results, caseSensitive, false);
						}
					}
				}
			}
			a = pathPartsRegex[index];
			if (!(a == "."))
			{
				if (!(a == ".."))
				{
					Transform transform2;
					if (!(a == "**"))
					{
						if (!(a == "..**") && !(a == "**.."))
						{
							if (Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
							{
								if (index == pathPartsRegex.Count - 1)
								{
									results.Add(current);
									return true;
								}
								foreach (object obj in current)
								{
									Transform current2 = (Transform)obj;
									flag |= GTExt._TryFindAllByPath(current2, pathPartsRegex, index + 1, results, caseSensitive, false);
								}
							}
						}
						else
						{
							int k;
							for (k = index + 1; k < pathPartsRegex.Count; k++)
							{
								string a2 = pathPartsRegex[k];
								if (!(a2 == "..**") && !(a2 == "**.."))
								{
									break;
								}
							}
							if (k == pathPartsRegex.Count)
							{
								results.Add(current.root);
								return true;
							}
							Transform transform = current;
							while (transform)
							{
								flag |= GTExt._TryFindAllByPath(transform, pathPartsRegex, index + 1, results, caseSensitive, false);
								transform = transform.parent;
							}
						}
					}
					else if (index == pathPartsRegex.Count - 1)
					{
						for (int l = 0; l < current.childCount; l++)
						{
							results.Add(current.GetChild(l));
							flag = true;
						}
					}
					else if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out transform2, caseSensitive))
					{
						if (index + 2 == pathPartsRegex.Count)
						{
							results.Add(transform2);
							return true;
						}
						flag |= GTExt._TryFindAllByPath(transform2, pathPartsRegex, index + 2, results, caseSensitive, false);
					}
				}
				else if (current.parent)
				{
					if (index == pathPartsRegex.Count - 1)
					{
						results.Add(current.parent);
						return true;
					}
					flag |= GTExt._TryFindAllByPath(current.parent, pathPartsRegex, index + 1, results, caseSensitive, false);
				}
			}
			else
			{
				if (index == pathPartsRegex.Count - 1)
				{
					results.Add(current);
					return true;
				}
				flag |= GTExt._TryFindAllByPath(current, pathPartsRegex, index + 1, results, caseSensitive, false);
			}
			return flag;
		}

		// Token: 0x06005BCA RID: 23498 RVA: 0x001CEA54 File Offset: 0x001CCC54
		public static string[] _GlobPathToPathPartsRegex(string path)
		{
			string[] array = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0)
				{
					string a = array[i];
					if (a == "**" || a == "..**" || a == "**..")
					{
						a = array[i - 1];
						if (a == "**" || a == "..**" || a == "**..")
						{
							num++;
						}
					}
				}
				array[i - num] = array[i];
			}
			if (num > 0)
			{
				Array.Resize<string>(ref array, array.Length - num);
			}
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = GTExt._GlobPathPartToRegex(array[j]);
			}
			return array;
		}

		// Token: 0x06005BCB RID: 23499 RVA: 0x001CEB14 File Offset: 0x001CCD14
		private static string _GlobPathPartToRegex(string pattern)
		{
			if (pattern == "." || pattern == ".." || pattern == "**" || pattern == "..**" || pattern == "**.." || pattern.StartsWith("^"))
			{
				return pattern;
			}
			return "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
		}

		// Token: 0x06005BCD RID: 23501 RVA: 0x001CEBB8 File Offset: 0x001CCDB8
		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass7_0<T, TStop1> A_2) where T : Component where TStop1 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(transform, ref components, ref A_2);
				}
			}
		}

		// Token: 0x06005BCE RID: 23502 RVA: 0x001CEC54 File Offset: 0x001CCE54
		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|8_0<T, TStop1, TStop2>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass8_0<T, TStop1, TStop2> A_2) where T : Component where TStop1 : Component where TStop2 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null) && !(transform.GetComponent<TStop2>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|8_0<T, TStop1, TStop2>(transform, ref components, ref A_2);
				}
			}
		}

		// Token: 0x06005BCF RID: 23503 RVA: 0x001CED04 File Offset: 0x001CCF04
		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|9_0<T, TStop1, TStop2, TStop3>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass9_0<T, TStop1, TStop2, TStop3> A_2) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null) && !(transform.GetComponent<TStop2>() != null) && !(transform.GetComponent<TStop3>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|9_0<T, TStop1, TStop2, TStop3>(transform, ref components, ref A_2);
				}
			}
		}

		// Token: 0x0400657F RID: 25983
		private static Dictionary<Transform, Dictionary<string, Transform>> caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();

		// Token: 0x04006580 RID: 25984
		private static Dictionary<Transform, Dictionary<string, Transform>> caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();

		// Token: 0x04006581 RID: 25985
		public static Dictionary<string, string> allStringsUsed = new Dictionary<string, string>();

		// Token: 0x02000E43 RID: 3651
		public enum ParityOptions
		{
			// Token: 0x04006583 RID: 25987
			XFlip,
			// Token: 0x04006584 RID: 25988
			YFlip,
			// Token: 0x04006585 RID: 25989
			ZFlip,
			// Token: 0x04006586 RID: 25990
			AllFlip
		}
	}
}
