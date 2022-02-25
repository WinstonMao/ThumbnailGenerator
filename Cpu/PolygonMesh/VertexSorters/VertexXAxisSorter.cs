﻿using MatterHackers.VectorMath;

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

//#define VALIDATE_SEARCH
using System;
using System.Collections.Generic;

namespace MatterHackers.PolygonMesh
{
	public class VertexXAxisSorter : VertexSorterBase
	{
		public VertexXAxisSorter()
		{
		}

		public override int Compare(Vector3 a, Vector3 b)
		{
			return a.X.CompareTo(b.X);
		}

		public override List<Vector3> FindVertices(List<Vector3> vertices, Vector3 position, double maxDistanceToConsiderVertexAsSame)
		{
			List<Vector3> foundVertices = new List<Vector3>();

			Vector3 testPos = position;
			int index = vertices.BinarySearch(testPos, this);
			if (index < 0)
			{
				index = ~index;
			}
			// we have the starting index now get all the vertices that are close enough starting from here
			double maxDistanceToConsiderVertexAsSameSquared = maxDistanceToConsiderVertexAsSame * maxDistanceToConsiderVertexAsSame;
			for (int i = index; i < vertices.Count; i++)
			{
				if (Math.Abs(vertices[i].X - position.X) > maxDistanceToConsiderVertexAsSame)
				{
					// we are too far away in x, we are done with this direction
					break;
				}
				AddToListIfSameEnough(vertices, position, foundVertices, maxDistanceToConsiderVertexAsSameSquared, i);
			}
			for (int i = index - 1; i >= 0; i--)
			{
				if (Math.Abs(vertices[i].X - position.X) > maxDistanceToConsiderVertexAsSame)
				{
					// we are too far away in x, we are done with this direction
					break;
				}
				AddToListIfSameEnough(vertices, position, foundVertices, maxDistanceToConsiderVertexAsSameSquared, i);
			}

			return foundVertices;
		}

		private void AddToListIfSameEnough(List<Vector3> vertices, Vector3 position, List<Vector3> findList, double maxDistanceToConsiderVertexAsSameSquared, int i)
		{
			if (vertices[i] == position)
			{
				findList.Add(vertices[i]);
			}
			else
			{
				double distanceSquared = (vertices[i] - position).LengthSquared;
				if (distanceSquared <= maxDistanceToConsiderVertexAsSameSquared)
				{
					findList.Add(vertices[i]);
				}
			}
		}
	}
}