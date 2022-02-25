/*
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

using MatterHackers.VectorMath;
using MIConvexHull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MatterHackers.PolygonMesh
{
	public static class MeshConvexHull
	{
		public static string ConvexHullMesh => nameof(ConvexHullMesh);

		public static string CreatingConvexHullMesh => nameof(CreatingConvexHullMesh);

		public static Mesh GetConvexHull(this Mesh mesh, bool generateAsync)
		{
			// currently disabling async as it has some threading issues
			generateAsync = false;
			if (mesh.Faces.Count < 4)
			{
				return null;
			}

			// build the convex hull for faster bounding calculations
			// we have a mesh so don't recurse into children
			object meshData;
			mesh.PropertyBag.TryGetValue(ConvexHullMesh, out meshData);
			if (meshData is Mesh convexHullMesh)
			{
				return convexHullMesh;
			}
			else
			{
				object creatingHullData;
				mesh.PropertyBag.TryGetValue(CreatingConvexHullMesh, out creatingHullData);
				bool currentlyCreatingHull = creatingHullData is CreatingHullFlag;
				if (!currentlyCreatingHull)
				{
					// set the marker that we are creating the data
					if (generateAsync)
					{
						mesh.PropertyBag.Add(CreatingConvexHullMesh, new CreatingHullFlag());

						Task.Run(() =>
						{
							CreateHullMesh(mesh);
							if (mesh.PropertyBag.ContainsKey(CreatingConvexHullMesh))
							{
								mesh.PropertyBag.Remove(CreatingConvexHullMesh);
							}
						});
					}
					else
					{
						return CreateHullMesh(mesh);
					}
				}
				else if (!generateAsync)
				{
					var count = 0;
					// we need to wait for the data to be ready and return it
					while (currentlyCreatingHull && count < 1000)
					{
						Thread.Sleep(1);
						mesh.PropertyBag.TryGetValue(CreatingConvexHullMesh, out creatingHullData);
						currentlyCreatingHull = creatingHullData is CreatingHullFlag;
						count++;
					}

					return CreateHullMesh(mesh);
				}
			}

			return null;
		}

		private static Mesh CreateHullMesh(Mesh mesh)
		{
			var bounds = AxisAlignedBoundingBox.Empty();
			// Get the convex hull for the mesh
			var cHVertexList = new List<CHVertex>();
			foreach (var position in mesh.Vertices.Distinct().ToArray())
			{
				cHVertexList.Add(new CHVertex(position));
				bounds.ExpandToInclude(position);
			}

			var tollerance = .01;

			if (cHVertexList.Count == 0
				|| bounds.XSize <= tollerance
				|| bounds.YSize <= tollerance
				|| bounds.ZSize <= tollerance
				|| double.IsNaN(cHVertexList.First().Position[0]))
			{
				return mesh;
			}

			var convexHull = ConvexHull.Create<CHVertex, CHFace>(cHVertexList, tollerance);
			if (convexHull != null)
			{
				// create the mesh from the hull data
				Mesh hullMesh = new Mesh();
				foreach (var face in convexHull.Result.Faces)
				{
					int vertexCount = hullMesh.Vertices.Count;

					foreach (var vertex in face.Vertices)
					{
						hullMesh.Vertices.Add(new Vector3(vertex.Position[0], vertex.Position[1], vertex.Position[2]));
					}

					hullMesh.Faces.Add(vertexCount, vertexCount + 1, vertexCount + 2, hullMesh.Vertices);
				}

				try
				{
					// make sure there is not currently a convex hull on this object
					if (mesh.PropertyBag.ContainsKey(ConvexHullMesh))
					{
						mesh.PropertyBag.Remove(ConvexHullMesh);
					}

					// add the new hull
					mesh.PropertyBag.Add(ConvexHullMesh, hullMesh);
					// make sure we remove this hull if the mesh changes
					mesh.Changed += MeshChanged_RemoveConvexHull;

					// remove the marker that says we are building the hull
					if (mesh.PropertyBag.ContainsKey(CreatingConvexHullMesh))
					{
						mesh.PropertyBag.Remove(CreatingConvexHullMesh);
					}

					return hullMesh;
				}
				catch
				{
					mesh.PropertyBag.Remove(CreatingConvexHullMesh);
				}
			}

			return null;
		}

		private static void MeshChanged_RemoveConvexHull(object sender, EventArgs e)
		{
			if (sender is Mesh mesh)
			{
				mesh.Changed -= MeshChanged_RemoveConvexHull;

				// remove any cached hull as it is no longer valid (the mesh changed)
				if (mesh.PropertyBag.ContainsKey(ConvexHullMesh))
				{
					mesh.PropertyBag.Remove(ConvexHullMesh);
				}

				// remove the marker that says we are building the hull
				if (mesh.PropertyBag.ContainsKey(CreatingConvexHullMesh))
				{
					mesh.PropertyBag.Remove(CreatingConvexHullMesh);
				}
			}
		}

		internal class CHFace : ConvexFace<CHVertex, CHFace>
		{
		}

		internal class CHVertex : MIConvexHull.IVertex
		{
			private double[] position;

			internal CHVertex(Vector3 position)
			{
				this.position = position.ToArray();
			}

			internal CHVertex(Vector3Float position)
			{
				this.position = new double[] { position.X, position.Y, position.Z };
			}

			public double[] Position => position;
		}

		internal class CreatingHullFlag
		{
		}
	}
}