module FsSimpleFileIO.Export

open System.IO

open FsCombinators.ExtraTypes

open FileSystemExtensions

type CreateStream<'StreamName, 'Stream> when 'Stream :> Stream = 'StreamName -> IgnorableResult<'Stream, string>

type StreamCopy<'Src, 'Dst> when 'Src :> Stream and 'Dst :> Stream = 'Src -> 'Dst -> unit

type WriteToStream<'Name, 'Src, 'Dst> when 'Src :> Stream and 'Dst :> Stream =
    StreamCopy<'Src, 'Dst> -> 'Name -> 'Src -> IgnorableResult<int64, string>

type ExportToStream<'Name, 'Src, 'Dst> when 'Src :> Stream and 'Dst :> Stream =
    CreateStream<'Name, 'Dst> -> WriteToStream<'Name, 'Src, 'Dst>

let tryStreamCopy: StreamCopy<'Src, 'Dst> -> 'Dst -> 'Src -> IgnorableResult<int64, string> =
    fun streamCopy (destination: 'Dst) source ->
        FsCombinators.ResultExtensions.tryAsResult
        <| fun () ->
            streamCopy source destination
            destination.Length
        |> IgnorableResult.ofResult

let writeContentToStream: ExportToStream<'Name, 'Src, 'Dst> =
    fun createStream streamCopy destStreamName source ->
        createStream destStreamName
        |> IgnorableResult.bind (fun stream ->
            use fStream = stream
            tryStreamCopy streamCopy fStream source)

let fsWriteToFile streamCopy =
    writeContentToStream tryCreateFileWithoutOverwrite streamCopy

let fsOverwriteFile streamCopy =
    streamCopy
    |> writeContentToStream (
        tryCreateFileWithOverwrite
        >> IgnorableResult.ofResult
    )
