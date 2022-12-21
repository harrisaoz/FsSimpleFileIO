module FsSimpleFileIO.Facade

open Export
open FileSystemExtensions

/// Write content from the given stream to file.
/// If the named destination file exists, then the action is ignored
let fsWriteToFile filename contentStream =
    writeContentToStream
        tryCreateFileWithoutOverwrite
        StandardDependencies.copyStream
        filename
        contentStream

/// Write content from the given stream to file, overwriting existing content.
let fsOverwriteFile filename contentStream =
    writeContentToStream
        tryCreateFileWithOverwrite
        StandardDependencies.copyStream
        filename
        contentStream

/// Write the given text to file, overwriting any existing content.
/// The text is treated as UTF8-encoded.
/// This is only suitable for bodies of text that fit in memory - given that
/// the input is, by virtue of being a 'string', in memory already, this should
/// generally be a reasonable constraint for the scenario.
let overwriteFileFromUtf8Text filename =
    Text.getUtf8Bytes
    >> Streams.inMemoryByteStream
    >> (fsOverwriteFile filename)
