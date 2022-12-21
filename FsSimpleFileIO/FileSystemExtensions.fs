module FsSimpleFileIO.FileSystemExtensions

open System.IO
open FsCombinators.ExtraTypes

let asDirName (labels: string[]) =
    String.concat (string Path.DirectorySeparatorChar) labels

let assertFolder name =
    match Directory.Exists name with
    | true -> DirectoryInfo name
    | false -> Directory.CreateDirectory name

let assertSubfolder: DirectoryInfo -> string -> DirectoryInfo =
    fun parent child ->
        [| parent.FullName; child |] |> asDirName |> assertFolder

let absoluteName: DirectoryInfo -> string -> string =
    fun dir localName ->
        $"{dir.FullName}{Path.DirectorySeparatorChar}{localName}"

/// Try to create a file with the given absolute name and open a handle to the
/// new file.
/// Return:
/// - Ok fileHandle: the file was created successfully (a new file);
/// - Ignore: the file already exists;
/// - Error msg: any other failure occurred while trying to create the file.
let tryCreateFileWithoutOverwrite absoluteFilename =
    try
        IgnorableResult.Ok <| File.Open(absoluteFilename, FileMode.CreateNew)
    with
    | :? System.OutOfMemoryException -> reraise ()
    | :? IOException ->
        // When opening a file in CreateNew mode,
        // IOException is only thrown if the file already exists.
        IgnorableResult.Ignore
    | ex -> IgnorableResult.Error <| ex.Message

/// Try to create a file with the given absolute name and open a handle to the
/// (possibly new) file.
/// Return:
/// - Ok fileHandle: the file was created successfully (a new file) or a file
/// with the specified name already exists (and Open returned the file handle);
/// - Error msg: any other failure occurred while trying to create the file.
let tryCreateFileWithOverwrite absoluteFilename =
    fun () -> File.Open(absoluteFilename, FileMode.Create)
    |> FsCombinators.ResultExtensions.tryAsResult
    |> IgnorableResult.ofResult
