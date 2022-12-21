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

let tryCreateFileWithOverwrite absoluteFilename =
    fun () -> File.Open(absoluteFilename, FileMode.Create)
    |> FsCombinators.ResultExtensions.tryAsResult
    |> IgnorableResult.ofResult
