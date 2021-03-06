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

using System;
using MatterHackers.VectorMath;

namespace MatterHackers.Agg.UI
{
	public class NumberEdit : TextEditWidget
	{
		public double Value
		{
			get { return InternalNumberEdit.Value; }
			set { InternalNumberEdit.Value = value; }
		}

		public double MaxValue
		{
			get { return InternalNumberEdit.MaxValue; }
			set { InternalNumberEdit.MaxValue = value; }
		}

		public InternalNumberEdit InternalNumberEdit { get { return (InternalNumberEdit)InternalTextEditWidget; } }

		public NumberEdit(double startingValue,
			double x = 0,
			double y = 0,
			double pointSize = 12,
			double pixelWidth = 0,
			double pixelHeight = 0,
			bool allowNegatives = false,
			bool allowDecimals = false,
			double minValue = int.MinValue,
			double maxValue = int.MaxValue,
			double increment = 1,
			int tabIndex = 0)
		{
			if (!allowNegatives)
			{
				minValue = Math.Max(0, minValue);
			}

			InternalTextEditWidget = new InternalNumberEdit(startingValue,
				pointSize,
				allowNegatives,
				allowDecimals,
				minValue,
				maxValue,
				increment,
				tabIndex);

			HookUpToInternalWidget(pixelWidth, pixelHeight);
			OriginRelativeParent = new Vector2(x, y);
		}
	}
}