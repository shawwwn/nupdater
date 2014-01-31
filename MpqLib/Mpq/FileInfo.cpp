//+-----------------------------------------------------------------------------
//|
//| Copyright (C) 2008, Magnus Ostberg, aka Magos
//| Contact: MagosX@GMail.com, http://www.magosx.com
//|
//| This file is part of MpqLib.
//| MpqLib is a library to manipulate (open, read, write) MoPaQ archives and
//| its contained files for the game WarCraft 3. It can (and is supposed to)
//| be freely used in tools and programs made by other developers.
//|
//| WarCraft is a trademark of Blizzard Entertainment, Inc.
//|
//| MpqLib is free software: you can redistribute it and/or modify
//| it under the terms of the GNU General Public License as published by
//| the Free Software Foundation, either version 3 of the License, or
//| (at your option) any later version.
//|
//| MpqLib is distributed in the hope that it will be useful,
//| but WITHOUT ANY WARRANTY; without even the implied warranty of
//| MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//| GNU General Public License for more details.
//|
//| You should have received a copy of the GNU General Public License
//| along with MpqLib. If not, see <http://www.gnu.org/licenses/>.
//|
//| This header must remain unaltered if changes are made to the file.
//| Additional information may be added as needed.
//|
//+-----------------------------------------------------------------------------
#include "FileInfo.h"

MpqLib::Mpq::CFileInfo::CFileInfo(System::String^ FileName, System::Int32 Size, System::Int32 CompressedSize)
{
	_FileName = FileName;
	_Size = Size;
	_CompressedSize = CompressedSize;
}

System::String^ MpqLib::Mpq::CFileInfo::FileName::get()
{
	return _FileName;
}

System::Int32 MpqLib::Mpq::CFileInfo::Size::get()
{
	return _Size;
}

System::Int32 MpqLib::Mpq::CFileInfo::CompressedSize::get()
{
	return _CompressedSize;
}