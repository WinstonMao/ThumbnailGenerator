using MatterHackers.Agg.Image;

//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software
// is granted provided this copyright notice appears in all copies.
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// scanline_u8 class
//
//----------------------------------------------------------------------------
using System;

namespace MatterHackers.Agg
{
	public interface IAlphaMask
	{
		byte pixel(int x, int y);

		byte combine_pixel(int x, int y, byte val);

		void fill_hspan(int x, int y, byte[] dst, int dstIndex, int num_pix);

		void fill_vspan(int x, int y, byte[] dst, int dstIndex, int num_pix);

		void combine_hspanFullCover(int x, int y, byte[] dst, int dstIndex, int num_pix);

		void combine_hspan(int x, int y, byte[] dst, int dstIndex, int num_pix);

		void combine_vspan(int x, int y, byte[] dst, int dstIndex, int num_pix);
	};

	public sealed class AlphaMaskByteUnclipped : IAlphaMask
	{
		private IImageByte m_rbuf;
		private uint m_Step;
		private uint m_Offset;

		public static readonly int cover_shift = 8;
		public static readonly int cover_none = 0;
		public static readonly int cover_full = 255;

		public AlphaMaskByteUnclipped(IImageByte rbuf, uint Step, uint Offset)
		{
			m_Step = Step;
			m_Offset = Offset;
			m_rbuf = rbuf;
		}

		public void attach(IImageByte rbuf)
		{
			m_rbuf = rbuf;
		}

		//--------------------------------------------------------------------
		public byte pixel(int x, int y)
		{
			int bufferIndex = m_rbuf.GetBufferOffsetXY(x, y);
			byte[] buffer = m_rbuf.GetBuffer();
			return buffer[bufferIndex];
		}

		//--------------------------------------------------------------------
		public byte combine_pixel(int x, int y, byte val)
		{
			unchecked
			{
				int bufferIndex = m_rbuf.GetBufferOffsetXY(x, y);
				byte[] buffer = m_rbuf.GetBuffer();
				return (byte)((255 + val * buffer[bufferIndex]) >> 8);
			}
		}

		public void fill_hspan(int x, int y, byte[] dst, int dstIndex, int num_pix)
		{
			throw new NotImplementedException();
#if false
            byte[] mask = m_rbuf.GetPixelPointerY(y) + x * m_Step + m_Offset;
            do
            {
                *dst++ = *mask;
                mask += m_Step;
            }
            while (--num_pix != 0);
#endif
		}

		public void combine_hspanFullCover(int x, int y, byte[] covers, int coversIndex, int count)
		{
			int maskIndex = m_rbuf.GetBufferOffsetXY(x, y);
			byte[] mask = m_rbuf.GetBuffer();
			do
			{
				covers[coversIndex++] = mask[maskIndex++];
			}
			while (--count != 0);
		}

		public void combine_hspan(int x, int y, byte[] covers, int coversIndex, int count)
		{
			int maskIndex = m_rbuf.GetBufferOffsetXY(x, y);
			byte[] mask = m_rbuf.GetBuffer();
			do
			{
				covers[coversIndex] = (byte)((255 + (covers[coversIndex]) * mask[maskIndex]) >> 8);
				coversIndex++;
				maskIndex++;
			}
			while (--count != 0);
		}

		public void fill_vspan(int x, int y, byte[] buffer, int bufferIndex, int num_pix)
		{
			throw new NotImplementedException();
#if false
            byte[] mask = m_rbuf.GetPixelPointerY(y) + x * m_Step + m_Offset;
            do
            {
                *dst++ = *mask;
                mask += m_rbuf.StrideInBytes();
            }
            while (--num_pix != 0);
#endif
		}

		public void combine_vspan(int x, int y, byte[] dst, int dstIndex, int num_pix)
		{
			throw new NotImplementedException();
#if false
            byte[] mask = m_rbuf.GetPixelPointerY(y) + x * m_Step + m_Offset;
            do
            {
                *dst = (byte)((cover_full + (*dst) * (*mask)) >> cover_shift);
                ++dst;
                mask += m_rbuf.StrideInBytes();
            }
            while (--num_pix != 0);
#endif
		}
	};

	public sealed class AlphaMaskByteClipped : IAlphaMask
	{
		private IImageByte m_rbuf;
		private uint m_Step;
		private uint m_Offset;

		public static readonly int cover_shift = 8;
		public static readonly int cover_none = 0;
		public static readonly int cover_full = 255;

		public AlphaMaskByteClipped(IImageByte rbuf, uint Step, uint Offset)
		{
			m_Step = Step;
			m_Offset = Offset;
			m_rbuf = rbuf;
		}

		public void attach(IImageByte rbuf)
		{
			m_rbuf = rbuf;
		}

		//--------------------------------------------------------------------
		public byte pixel(int x, int y)
		{
			unchecked
			{
				if ((uint)x < (uint)m_rbuf.Width
					&& (uint)y < (uint)m_rbuf.Height)
				{
					int bufferIndex = m_rbuf.GetBufferOffsetXY(x, y);
					byte[] buffer = m_rbuf.GetBuffer();
					return buffer[bufferIndex];
				}
			}

			return 0;
		}

		public byte combine_pixel(int x, int y, byte val)
		{
			unchecked
			{
				if ((uint)x < (uint)m_rbuf.Width
					&& (uint)y < (uint)m_rbuf.Height)
				{
					int bufferIndex = m_rbuf.GetBufferOffsetXY(x, y);
					byte[] buffer = m_rbuf.GetBuffer();
					return (byte)((val * buffer[bufferIndex] + 255) >> 8);
				}
			}
			return 0;
		}

		public void fill_hspan(int x, int y, byte[] dst, int dstIndex, int num_pix)
		{
			throw new NotImplementedException();
#if false
            int xmax = (int)m_rbuf.Width() - 1;
            int ymax = (int)m_rbuf.Height() - 1;

            int count = num_pix;
            byte[] covers = dst;

            if (y < 0 || y > ymax)
            {
                agg_basics.MemClear(dst, num_pix);
                return;
            }

            if (x < 0)
            {
                count += x;
                if (count <= 0)
                {
                    agg_basics.MemClear(dst, num_pix);
                    return;
                }
                agg_basics.MemClear(covers, -x);
                covers -= x;
                x = 0;
            }

            if (x + count > xmax)
            {
                int rest = x + count - xmax - 1;
                count -= rest;
                if (count <= 0)
                {
                    agg_basics.MemClear(dst, num_pix);
                    return;
                }
                agg_basics.MemClear(covers + count, rest);
            }

            byte[] mask = m_rbuf.GetPixelPointerY(y) + x * m_Step + m_Offset;
            do
            {
                *covers++ = *(mask);
                mask += m_Step;
            }
            while (--count != 0);
#endif
		}

		public void combine_hspanFullCover(int x, int y, byte[] covers, int coversIndex, int num_pix)
		{
			int xmax = (int)m_rbuf.Width - 1;
			int ymax = (int)m_rbuf.Height - 1;

			int count = num_pix;

			if (y < 0 || y > ymax)
			{
				agg_basics.MemClear(covers, coversIndex, num_pix);
				return;
			}

			if (x < 0)
			{
				count += x;
				if (count <= 0)
				{
					agg_basics.MemClear(covers, coversIndex, num_pix);
					return;
				}
				agg_basics.MemClear(covers, coversIndex, -x);
				coversIndex -= x;
				x = 0;
			}

			if (x + count > xmax)
			{
				int rest = x + count - xmax - 1;
				count -= rest;
				if (count <= 0)
				{
					agg_basics.MemClear(covers, coversIndex, num_pix);
					return;
				}
				agg_basics.MemClear(covers, coversIndex + count, rest);
			}

			int maskIndex = m_rbuf.GetBufferOffsetXY(x, y);
			byte[] mask = m_rbuf.GetBuffer();
			do
			{
				covers[coversIndex++] = mask[maskIndex++];
			}
			while (--count != 0);
		}

		public void combine_hspan(int x, int y, byte[] buffer, int bufferIndex, int num_pix)
		{
			int xmax = (int)m_rbuf.Width - 1;
			int ymax = (int)m_rbuf.Height - 1;

			int count = num_pix;
			byte[] covers = buffer;
			int coversIndex = bufferIndex;

			if (y < 0 || y > ymax)
			{
				agg_basics.MemClear(buffer, bufferIndex, num_pix);
				return;
			}

			if (x < 0)
			{
				count += x;
				if (count <= 0)
				{
					agg_basics.MemClear(buffer, bufferIndex, num_pix);
					return;
				}
				agg_basics.MemClear(covers, coversIndex, -x);
				coversIndex -= x;
				x = 0;
			}

			if (x + count > xmax)
			{
				int rest = x + count - xmax - 1;
				count -= rest;
				if (count <= 0)
				{
					agg_basics.MemClear(buffer, bufferIndex, num_pix);
					return;
				}
				agg_basics.MemClear(covers, coversIndex + count, rest);
			}

			int maskIndex = m_rbuf.GetBufferOffsetXY(x, y);
			byte[] mask = m_rbuf.GetBuffer();
			do
			{
				covers[coversIndex] = (byte)(((covers[coversIndex]) * mask[maskIndex] + 255) >> 8);
				coversIndex++;
				maskIndex++;
			}
			while (--count != 0);
		}

		public void fill_vspan(int x, int y, byte[] buffer, int bufferIndex, int num_pix)
		{
			throw new NotImplementedException();
#if false
            int xmax = (int)m_rbuf.Width() - 1;
            int ymax = (int)m_rbuf.Height() - 1;

            int count = num_pix;
            byte[] covers = dst;

            if (x < 0 || x > xmax)
            {
                agg_basics.MemClear(dst, num_pix);
                return;
            }

            if (y < 0)
            {
                count += y;
                if (count <= 0)
                {
                    agg_basics.MemClear(dst, num_pix);
                    return;
                }
                agg_basics.MemClear(covers, -y);
                covers -= y;
                y = 0;
            }

            if (y + count > ymax)
            {
                int rest = y + count - ymax - 1;
                count -= rest;
                if (count <= 0)
                {
                    agg_basics.MemClear(dst, num_pix);
                    return;
                }
                agg_basics.MemClear(covers + count, rest);
            }

            byte[] mask = m_rbuf.GetPixelPointerY(y) + x * m_Step + m_Offset;
            do
            {
                *covers++ = *mask;
                mask += m_rbuf.StrideInBytes();
            }
            while (--count != 0);
#endif
		}

		public void combine_vspan(int x, int y, byte[] buffer, int bufferIndex, int num_pix)
		{
			throw new NotImplementedException();
#if false
            int xmax = (int)m_rbuf.Width() - 1;
            int ymax = (int)m_rbuf.Height() - 1;

            int count = num_pix;
            byte[] covers = dst;

            if (x < 0 || x > xmax)
            {
                agg_basics.MemClear(dst, num_pix);
                return;
            }

            if (y < 0)
            {
                count += y;
                if (count <= 0)
                {
                    agg_basics.MemClear(dst, num_pix);
                    return;
                }
                agg_basics.MemClear(covers, -y);
                covers -= y;
                y = 0;
            }

            if (y + count > ymax)
            {
                int rest = y + count - ymax - 1;
                count -= rest;
                if (count <= 0)
                {
                    agg_basics.MemClear(dst, num_pix);
                    return;
                }
                agg_basics.MemClear(covers + count, rest);
            }

            byte[] mask = m_rbuf.GetPixelPointerY(y) + x * m_Step + m_Offset;
            do
            {
                *covers = (byte)((cover_full + (*covers) * (*mask)) >> cover_shift);
                ++covers;
                mask += m_rbuf.StrideInBytes();
            }
            while (--count != 0);
#endif
		}
	};
}