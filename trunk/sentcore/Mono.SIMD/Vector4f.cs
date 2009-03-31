// Vector4f.cs
//
// Author:
//   Rodrigo Kumpera (rkumpera@novell.com)
//
// (C) 2008 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Runtime.InteropServices;

namespace Mono.Simd
{
	public enum ShuffleSel
	{
		XFromX,
		XFromY,
		XFromZ,
		XFromW,

		YFromX = 0x00,
		YFromY = 0x04,
		YFromZ = 0x08,
		YFromW = 0x0C,

		ZFromX = 0x00,
		ZFromY = 0x10,
		ZFromZ = 0x20,
		ZFromW = 0x30,

		WFromX = 0x00,
		WFromY = 0x40,
		WFromZ = 0x80,
		WFromW = 0xC0,

		/*Expand a single element into all elements*/
		ExpandX = XFromX | YFromX | ZFromX | WFromX,
		ExpandY = XFromY | YFromY | ZFromY | WFromY,
		ExpandZ = XFromZ | YFromZ | ZFromZ | WFromZ,
		ExpandW = XFromW | YFromW | ZFromW | WFromW,

		/*Expand a pair of elements (x,y,z,w) -> (x,x,y,y)*/
		ExpandXY = XFromX | YFromX | ZFromY | WFromY,
		ExpandZW = XFromZ | YFromZ | ZFromW | WFromW,

		/*Expand interleaving elements (x,y,z,w) -> (x,y,x,y)*/
		ExpandInterleavedXY = XFromX | YFromY | ZFromX | WFromY,
		ExpandInterleavedZW = XFromZ | YFromW | ZFromZ | WFromW,

		/*Rotate elements*/
		RotateRight = XFromY | YFromZ | ZFromW | WFromX,
		RotateLeft = XFromW | YFromX | ZFromY | WFromZ,

		/*Swap order*/
		Swap = XFromW | YFromZ | ZFromY | WFromX,
	};

/*
	TODO:
        Unary - (implemented as mulps [-1,-1,-1,-1])
        Abs (implemented as pand [7fffffff,...] )
        Comparison functions
        Mask extraction function
        Setters
        vector x float ops
        Single float constructor (expand it to the 4 positions)
		Replace Shuffle with less bug prone methods
*/

	[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 16)]
	public struct Vector4f
	{
		private float x;
		private float y;
		private float z;
		private float w;

		public float X { get { return x; } set { x = value; } }
		public float Y { get { return y; } set { y = value; } }
		public float Z { get { return z; } set { z = value; } }
		public float W { get { return w; } set { w = value; } }

		public Vector4f (float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		[Acceleration (AccelMode.SSE1)]
		public static unsafe Vector4f operator & (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *a = (int*)&v1;
			int *b = (int*)&v2;
			int *c = (int*)&res;
			*c++ = *a++ & *b++;
			*c++ = *a++ & *b++;
			*c++ = *a++ & *b++;
			*c = *a & *b;
			return res;
		}

		[Acceleration (AccelMode.SSE1)]
		public static unsafe Vector4f operator | (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *a = (int*)&v1;
			int *b = (int*)&v2;
			int *c = (int*)&res;
			*c++ = *a++ | *b++;
			*c++ = *a++ | *b++;
			*c++ = *a++ | *b++;
			*c = *a | *b;
			return res;
		}

		[Acceleration (AccelMode.SSE1)]
		public static unsafe Vector4f operator ^ (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *a = (int*)&v1;
			int *b = (int*)&v2;
			int *c = (int*)&res;
			*c++ = *a++ ^ *b++;
			*c++ = *a++ ^ *b++;
			*c++ = *a++ ^ *b++;
			*c = *a ^ *b;
			return res;
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f operator + (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f operator - (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (v1.x - v2.x, v1.y - v2.y, v1.z - v2.z, v1.w - v2.w);
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f operator * (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (v1.x * v2.x, v1.y * v2.y, v1.z * v2.z, v1.w * v2.w);
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f operator / (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (v1.x / v2.x, v1.y / v2.y, v1.z / v2.z, v1.w / v2.w);
		}

		[Acceleration (AccelMode.SSE1)]
		public static unsafe Vector4f AndNot (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *a = (int*)&v1;
			int *b = (int*)&v2;
			int *c = (int*)&res;
			*c++ = ~*a++ & *b++;
			*c++ = ~*a++ & *b++;
			*c++ = ~*a++ & *b++;
			*c = ~*a & *b;
			return res;
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f Sqrt (Vector4f v1)
		{
			return new Vector4f ((float)System.Math.Sqrt ((float)v1.x),
								(float)System.Math.Sqrt ((float)v1.y),
								(float)System.Math.Sqrt ((float)v1.z),
								(float)System.Math.Sqrt ((float)v1.w));
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f InvSqrt (Vector4f v1)
		{
			return new Vector4f ((float)(1.0 / System.Math.Sqrt ((float)v1.x)),
								(float)(1.0 / System.Math.Sqrt ((float)v1.y)),
								(float)(1.0 / System.Math.Sqrt ((float)v1.z)),
								(float)(1.0 / System.Math.Sqrt ((float)v1.w)));
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f Reciprocal (Vector4f v1)
		{
			return new Vector4f (1.0f / v1.x, 1.0f / v1.y, 1.0f / v1.z, 1.0f / v1.w);
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f Max (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (System.Math.Max (v1.x, v2.x),
								System.Math.Max (v1.y, v2.y),
								System.Math.Max (v1.z, v2.z),
								System.Math.Max (v1.w, v2.w));
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f Min (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (System.Math.Min (v1.x, v2.x),
								System.Math.Min (v1.y, v2.y),
								System.Math.Min (v1.z, v2.z),
								System.Math.Min (v1.w, v2.w));
		}

		[Acceleration (AccelMode.SSE3)]
		public static Vector4f HorizontalAdd (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (v1.x + v1.y, v1.z + v1.w, v2.x + v2.y, v2.z + v2.w);
		}

		[Acceleration (AccelMode.SSE3)]
		public static Vector4f AddSub (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (v1.x - v2.x, v1.y + v2.y, v1.z - v2.z, v1.w + v2.w);
		}

		[Acceleration (AccelMode.SSE3)]
		public static Vector4f HorizontalSub (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (v1.x - v1.y, v1.z - v1.w, v2.x - v2.y, v2.z - v2.w);
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f InterleaveHigh (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (v1.z, v2.z, v1.w, v2.w);
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f InterleaveLow (Vector4f v1, Vector4f v2)
		{
			return new Vector4f (v1.x, v2.x, v1.y, v2.y);
		}

		/*Same as a == b. */
		[Acceleration (AccelMode.SSE1)]
		public unsafe static Vector4f CompareEqual (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *c = (int*)&res;
			*c++ = v1.x == v2.x ? -1 : 0;
			*c++ = v1.y == v2.y ? -1 : 0;
			*c++ = v1.z == v2.z ? -1 : 0;
			*c = v1.w == v2.w ? -1 : 0;
			return res;		}

		/*Same as a < b. */
		[Acceleration (AccelMode.SSE1)]
		public unsafe static Vector4f CompareLessThan (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *c = (int*)&res;
			*c++ = v1.x < v2.x ? -1 : 0;
			*c++ = v1.y < v2.y ? -1 : 0;
			*c++ = v1.z < v2.z ? -1 : 0;
			*c = v1.w < v2.w ? -1 : 0;
			return res;
		}

		/*Same as a <= b. */
		[Acceleration (AccelMode.SSE1)]
		public unsafe static Vector4f CompareLessEqual (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *c = (int*)&res;
			*c++ = v1.x <= v2.x ? -1 : 0;
			*c++ = v1.y <= v2.y ? -1 : 0;
			*c++ = v1.z <= v2.z ? -1 : 0;
			*c = v1.w <= v2.w ? -1 : 0;
			return res;		}

		/*Same float.IsNaN (a) || float.IsNaN (b). */
		[Acceleration (AccelMode.SSE1)]
		public unsafe static Vector4f CompareUnordered (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *c = (int*)&res;
			*c++ = float.IsNaN (v1.x) || float.IsNaN (v2.x) ? -1 : 0;
			*c++ = float.IsNaN (v1.y) || float.IsNaN (v2.y) ? -1 : 0;
			*c++ = float.IsNaN (v1.z) || float.IsNaN (v2.z) ? -1 : 0;
			*c = float.IsNaN (v1.w) || float.IsNaN (v2.w) ? -1 : 0;
			return res;		}

		/*Same as a != b. */
		[Acceleration (AccelMode.SSE1)]
		public unsafe static Vector4f CompareNotEqual (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *c = (int*)&res;
			*c++ = v1.x != v2.x ? -1 : 0;
			*c++ = v1.y != v2.y ? -1 : 0;
			*c++ = v1.z != v2.z ? -1 : 0;
			*c = v1.w != v2.w ? -1 : 0;
			return res;
		}

		/*Same as !(a < b). */
		[Acceleration (AccelMode.SSE1)]
		public unsafe static Vector4f CompareNotLessThan (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *c = (int*)&res;
			*c++ = v1.x < v2.x ? 0 : -1;
			*c++ = v1.y < v2.y ? 0 : -1;
			*c++ = v1.z < v2.z ? 0 : -1;
			*c = v1.w < v2.w ? 0 : -1;
			return res;
		}

		/*Same as !(a <= b). */
		[Acceleration (AccelMode.SSE1)]
		public unsafe static Vector4f CompareNotLessEqual (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *c = (int*)&res;
			*c++ = v1.x <= v2.x ? 0 : -1;
			*c++ = v1.y <= v2.y ? 0 : -1;
			*c++ = v1.z <= v2.z ? 0 : -1;
			*c = v1.w <= v2.w ? 0 : -1;
			return res;
		}

		/*Same !float.IsNaN (a) && !float.IsNaN (b). */
		[Acceleration (AccelMode.SSE1)]
		public unsafe static Vector4f CompareOrdered (Vector4f v1, Vector4f v2)
		{
			Vector4f res = new Vector4f ();
			int *c = (int*)&res;
			*c++ = !float.IsNaN (v1.x) && !float.IsNaN (v2.x) ? -1 : 0;
			*c++ = !float.IsNaN (v1.y) && !float.IsNaN (v2.y) ? -1 : 0;
			*c++ = !float.IsNaN (v1.z) && !float.IsNaN (v2.z) ? -1 : 0;
			*c = !float.IsNaN (v1.w) && !float.IsNaN (v2.w) ? -1 : 0;
			return res;		}

		[Acceleration (AccelMode.SSE3)]
		public static Vector4f DuplicateLow (Vector4f v1)
		{
			return new Vector4f (v1.x, v1.x, v1.z, v1.z);
		}

		[Acceleration (AccelMode.SSE3)]
		public static Vector4f DuplicateHigh (Vector4f v1)
		{
			return new Vector4f (v1.y, v1.y, v1.w, v1.w);
		}

		/*
		The sel argument must be a value combination of ShuffleSel flags.
		*/
		[Acceleration (AccelMode.SSE2)]
		public static unsafe Vector4f Shuffle (Vector4f v1, ShuffleSel sel)
		{
			float *ptr = (float*)&v1;
			int idx = (int)sel;
			return new Vector4f (*(ptr + ((idx >> 0) & 0x3)),*(ptr + ((idx >> 2) & 0x3)),*(ptr + ((idx >> 4) & 0x3)),*(ptr + ((idx >> 6) & 0x3)));
		}

		[Acceleration (AccelMode.SSE1)]
		public static unsafe explicit operator Vector2d (Vector4f v)
		{
			Vector2d* p = (Vector2d*)&v;
			return *p;
		}

		[Acceleration (AccelMode.SSE1)]
		public static unsafe explicit operator Vector2l (Vector4f v)
		{
			Vector2l* p = (Vector2l*)&v;
			return *p;
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static unsafe explicit operator Vector2ul (Vector4f v)
		{
			Vector2ul* p = (Vector2ul*)&v;
			return *p;
		}

		[Acceleration (AccelMode.SSE1)]
		public static unsafe explicit operator Vector4i (Vector4f v)
		{
			Vector4i* p = (Vector4i*)&v;
			return *p;
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static unsafe explicit operator Vector4ui (Vector4f v)
		{
			Vector4ui* p = (Vector4ui*)&v;
			return *p;
		}

		[Acceleration (AccelMode.SSE1)]
		public static unsafe explicit operator Vector8s (Vector4f v)
		{
			Vector8s* p = (Vector8s*)&v;
			return *p;
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static unsafe explicit operator Vector8us (Vector4f v)
		{
			Vector8us* p = (Vector8us*)&v;
			return *p;
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static unsafe explicit operator Vector16sb (Vector4f v)
		{
			Vector16sb* p = (Vector16sb*)&v;
			return *p;
		}

		[Acceleration (AccelMode.SSE1)]
		public static unsafe explicit operator Vector16b (Vector4f v)
		{
			Vector16b* p = (Vector16b*)&v;
			return *p;
		}

		[Acceleration (AccelMode.SSE1)]
		public static Vector4f LoadAligned (ref Vector4f v)
		{
			return v;
		}

		[Acceleration (AccelMode.SSE1)]
		public static void StoreAligned (ref Vector4f res, Vector4f val)
		{
			res = val;
		}

		[CLSCompliant(false)]
		[Acceleration (AccelMode.SSE1)]
		public static unsafe Vector4f LoadAligned (Vector4f *v)
		{
			return *v;
		}

		[CLSCompliant(false)]
		[Acceleration (AccelMode.SSE1)]
		public static unsafe void StoreAligned (Vector4f *res, Vector4f val)
		{
			*res = val;
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static void PrefetchTemporalAllCacheLevels (ref Vector4f res)
		{
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static void PrefetchTemporal1stLevelCache (ref Vector4f res)
		{
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static void PrefetchTemporal2ndLevelCache (ref Vector4f res)
		{
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static void PrefetchNonTemporal (ref Vector4f res)
		{
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static unsafe void PrefetchTemporalAllCacheLevels (Vector4f *res)
		{
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static unsafe void PrefetchTemporal1stLevelCache (Vector4f *res)
		{
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static unsafe void PrefetchTemporal2ndLevelCache (Vector4f *res)
		{
		}

		[Acceleration (AccelMode.SSE1)]
		[CLSCompliant(false)]
		public static unsafe void PrefetchNonTemporal (Vector4f *res)
		{
		}
	}
}