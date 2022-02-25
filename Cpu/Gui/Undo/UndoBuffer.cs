﻿//----------------------------------------------------------------------------
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
using System;
using System.Collections.Generic;

namespace MatterHackers.Agg.UI
{
	public class UndoBuffer
	{
		public event EventHandler Changed;

		private Stack<IUndoRedoCommand> redoBuffer = new Stack<IUndoRedoCommand>();

		private LimitStack<IUndoRedoCommand> undoBuffer = new LimitStack<IUndoRedoCommand>();

		public UndoBuffer()
		{
		}

		public int UndoCount => undoBuffer.Count;

		public int RedoCount => redoBuffer.Count;

		public int MaxUndos
		{
			get => undoBuffer.Limit;
			set => undoBuffer.Limit = value;
		}

		public void Add(IUndoRedoCommand command)
		{
			undoBuffer.Push(command);
			redoBuffer.Clear();
			Changed?.Invoke(this, null);
		}

		public void AddAndDo(IUndoRedoCommand command)
		{
			undoBuffer.Push(command);
			redoBuffer.Clear();
			Changed?.Invoke(this, null);

			command.Do();
		}

		public void Redo(int redoCount = 1)
		{
			for (int i = 1; i <= redoCount; i++)
			{
				if (redoBuffer.Count != 0)
				{
					IUndoRedoCommand command = redoBuffer.Pop();
					command.Do();
					undoBuffer.Push(command);
				}
			}
			Changed?.Invoke(this, null);
		}

		public void Undo(int undoCount = 1)
		{
			for (int i = 1; i <= undoCount; i++)
			{
				if (undoBuffer.Count != 0)
				{
					IUndoRedoCommand command = undoBuffer.Pop();
					command.Undo();
					redoBuffer.Push(command);
				}
			}
			Changed?.Invoke(this, null);
		}

		public void ClearHistory()
		{
			undoBuffer.Clear();
			redoBuffer.Clear();
			Changed?.Invoke(this, null);
		}
	}
}