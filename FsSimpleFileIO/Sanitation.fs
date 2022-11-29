module FsSimpleFileIO.Sanitation

open System.IO
module StringExt = FsCombinators.StringExtensions

let illegalCharacters =
    Path.GetInvalidFileNameChars()

let clean (replacement: string) =
    StringExt.split illegalCharacters
    >> String.concat replacement

let sanitise pathSeparatorReplacement illegalCharReplacement =
    StringExt.flatten Path.PathSeparator pathSeparatorReplacement
    >> clean illegalCharReplacement

let sanitiseStandard = sanitise "__" "_"
