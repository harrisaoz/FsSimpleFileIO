module FsSimpleFileIO.Text

let getUtf8Bytes (text: string) =
    System.Text.Encoding.UTF8.GetBytes(text)
