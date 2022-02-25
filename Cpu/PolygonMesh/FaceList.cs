﻿/*
Copyright (c) 2014, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using System;
using System.Collections.Generic;
using MatterHackers.VectorMath;

namespace MatterHackers.PolygonMesh
{
	public class FaceList : List<Face>
	{
		public FaceList()
		{
		}

		public FaceList(IEnumerable<int> f, List<Vector3Float> vertices)
		{
			AddFromIntArray(f, vertices);
		}

		public FaceList(FaceList f)
		{
			for (int i = 0; i < f.Count; i++)
			{
				Add(new Face(f[i].v0, f[i].v1, f[i].v2, f[i].normal));
			}
		}

		public FaceList(IEnumerable<Face> faces)
		{
			foreach (var face in faces)
			{
				Add(new Face(face.v0, face.v1, face.v2, face.normal));
			}
		}

		/// <summary>
		/// Add a face from vertex indexes and calculate the normal (from vertex positions).
		/// </summary>
		/// <param name="v0">Index of vertex 0.</param>
		/// <param name="v1">Index of vertex 1.</param>
		/// <param name="v2">Index of vertex 2.</param>
		/// <param name="vertices">The list the vertex position data is in.</param>
		public void Add(int v0, int v1, int v2, List<Vector3Float> vertices)
		{
			this.Add(new Face(v0, v1, v2, vertices));
		}

		public void Add(int v0, int v1, int v2, Vector3Float normal)
		{
			this.Add(new Face(v0, v1, v2, normal));
		}

		public void AddFromIntArray(IEnumerable<int> f, List<Vector3Float> vertices)
		{
			this.Clear();

			var enumeratior = f.GetEnumerator();
			while (enumeratior.MoveNext())
			{
				var v0 = enumeratior.Current;
				enumeratior.MoveNext();
				var v1 = enumeratior.Current;
				enumeratior.MoveNext();
				var v2 = enumeratior.Current;

				Add(item: new Face(v0, v1, v2, vertices));
			}
		}

		public int[] ToIntArray()
		{
			var fa = new int[Count * 3];
			int i = 0;
			foreach (var face in this)
			{
				fa[i++] = face.v0;
				fa[i++] = face.v1;
				fa[i++] = face.v2;
			}

			return fa;
		}
	}
}