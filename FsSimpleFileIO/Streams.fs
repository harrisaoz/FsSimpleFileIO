module FsSimpleFileIO.Streams

let inMemoryByteStream (data: byte[]) = new System.IO.MemoryStream(data)
