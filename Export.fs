module FsSimpleFileIO

open System
open System.IO
open Combinators.TernaryResult
open Combinators.String

let illegalCharacters = Path.GetInvalidFileNameChars()

let clean (replacement: string) =
    split illegalCharacters
    >> join replacement

let asDirName (labels: string[]) =
    join (string Path.DirectorySeparatorChar) labels

let sanitise =
    flatten Path.PathSeparator "__" >> clean "_"

let assertFolder name =
    match Directory.Exists name with
    | true -> DirectoryInfo name
    | false -> Directory.CreateDirectory name

let assertSubfolder: DirectoryInfo -> string -> DirectoryInfo =
    fun parent child ->
        [|
            parent.FullName
            child
        |]
        |> asDirName
        |> assertFolder

let absoluteName: DirectoryInfo -> string -> string =
    fun dir localName ->
        $"{dir.FullName}{Path.DirectorySeparatorChar}{localName}"
    
let tryCreateFile absoluteFilename =
    try
        Ok <| File.Open(absoluteFilename, FileMode.CreateNew)
    with
        | :? OutOfMemoryException ->
            reraise()
        | :? IOException ->
            // When opening a file in CreateNew mode,
            // IOException is only thrown if the file already exists.
            Ignore
        | ex ->
            Error <| ex.Message

let tryStreamCopy streamCopy (destination: #Stream) source =
    try
        streamCopy source destination
        Ok <| destination.Length
    with
        | :? OutOfMemoryException ->
            reraise()
        | ex -> Error <| ex.Message

let writeContentToStream createStream streamCopy destStreamName source =
    createStream destStreamName
    |> bind (
        fun stream ->
            use fStream = stream
            tryStreamCopy streamCopy fStream source
        )

let fsWriteToFile streamCopy = writeContentToStream tryCreateFile streamCopy
