module FsSimpleFileIO.Export

open System.IO

open FsCombinators.ExtraTypes

open FileSystemExtensions

type CreateStream<'StreamName, 'NewStream> when 'NewStream :> Stream =
    'StreamName -> IgnorableResult<'NewStream, string>

type StreamCopy<'Src, 'Dst> when 'Src :> Stream and 'Dst :> Stream =
    'Src -> 'Dst -> unit

type WriteToStream<'Name, 'Src, 'Dst> when 'Src :> Stream and 'Dst :> Stream =
    StreamCopy<'Src, 'Dst> -> 'Name -> 'Src -> IgnorableResult<int64, string>

type ExportToStream<'Name, 'Src, 'Dst> when 'Src :> Stream and 'Dst :> Stream =
    CreateStream<'Name, 'Dst> -> WriteToStream<'Name, 'Src, 'Dst>

let tryStreamCopy: StreamCopy<'Src, 'Dst>
    -> 'Dst
    -> 'Src
    -> IgnorableResult<int64, string> =
    fun streamCopy (destination: 'Dst) source ->
        fun () ->
            streamCopy source destination
            destination.Length
        |> FsCombinators.ResultExtensions.tryAsResult
        |> IgnorableResult.ofResult

let writeContentToStream: ExportToStream<'Name, 'Src, 'Dst> =
    fun createStream streamCopy destStreamName sourceStream ->
        createStream destStreamName
        |> IgnorableResult.bind (fun stream ->
            use fStream = stream

let fsWriteToFile () =
    writeContentToStream tryCreateFileWithoutOverwrite
            tryStreamCopy streamCopy fStream sourceStream)

let fsOverwriteFile () =
    writeContentToStream tryCreateFileWithOverwrite
module StandardDependencies =
    let copyStream (inStream: #Stream) (outStream: #Stream) =
        inStream.CopyTo(outStream)
