# pragmabin
a scripting language that allows you to read/write/map binary files

# names for stuff
* reference is the name of a line in your script, you can set a reference by writing a unique name at the start of a line behind the colon (ie: `fileSize: readUInt64()` the reference would be `fileSize`)
* you can also leave lines unnamed, returned values will be assigned as the current line number
* sorry if you don't like camel case

# functions
```
  readBinary( <file location> )
  Example: readBinary(C:\pragmabin\myfile.bin)
  Usage: Opens a file for reading.
  Return: bool;true
  
  ---
  
  readByte()
  Example: FILE_INDEX: readByte()
  Usage: Reads a single unsigned byte from the currently opened file.
  Return: byte;read
  
  ---
  
  readSByte()
  Example: FILE_INDEX: readSByte()
  Usage: Reads a single signed byte from the currently opened file.
  Return: sbyte;read
  
  ---
  
  readUInt16()
  Example: TEXTURE_SIZE: readUInt16()
  Usage: Reads an unsigned 16-bit integer from the currently opened file.
  Return: uint16;read
  ---
  
  readInt16()
  Example: TEXTURE_SIZE: readUInt16()
  Usage: Reads an signed 16-bit integer from the currently opened file.
  Return: int16;read
  
  ---
  
  readUInt32()
  Example: USER_ID: readUInt32()
  Usage: Reads an unsigned 32-bit integer from the currently opened file.
  Return: uint32;read
  
  ---
  
  readInt32()
  Example: USER_ID: readUInt32()
  Usage: Reads an signed 32-bit integer from the currently opened file.
  Return: int32;read
  
  ---
  
  readUInt64()
  Example: BIG_NUMBER: readUInt64()
  Usage: Reads an unsigned 64-bit integer from the currently opened file.
  Return: uint64;read
  
  ---
  
  readInt64()
  Example: BIG_NUMBER: readUInt64()
  Usage: Reads an signed 64-bit integer from the currently opened file.
  Return: int64;read
  
  ---
  
  readChar()
  Example: AAAA: readChar()
  Usage: Reads a single byte and converts it to an ASCII string from the currently opened file.
  Return: string;read

  ---
  
  readString( <length > )
  Example: TEXTURE_NAME: readString(32)
  Usage: Reads a string from the currently opened file based on the length based into the function.
  Return: string;read
  
  ---
  
  execPrint( <reference > )
  Example: : execPrint(TEXTURE_NAME)
  Usage: Prints out the data held under the passed reference to the console.
  Return: -1
  
  ---
  
  execMessage( <string>, [<string>, <string>, ...] )
  Example: : execMessage(File has been parsed.)
  Usage: Displays a message box containing the string passed into the function.
  Return: DialogResult
  
  ---
  
  execSeek( <reference>, [base (BEGIN, CURRENT[default], END)] )
  Example: : execSeek(TEXTURE_SIZE)
  Usage: Seeks the current position of the binary base stream
  Return: stream_position
  
  ---
  
  execKill( <string> )
  Example: : execKill(Parsing fail)
  Usage: Ends the execution of the script and prints out a message.  
```
# example
Reading and listening all chunks in a GameMaker IFF-based data file
```
;;; Open Binary
: readBinary(D:\PragmaBin\data.win)

;;; File Constants
FILE_CONST: FORM

;;; Check if valid file
DATA_MAIN: readString(4)
DATA_SIZE: readUInt32()
: doCompare(DATA_MAIN == FILE_CONST, FAILURE)
  : doWhile(execTell < DATA_SIZE, SUCCESS)
  
    ;;; Read chunk name and size
    CHUNK_NAME: readString(4)
    CHUNK_SIZE: readUInt32()
    : execSeek(CHUNK_SIZE)
    
    ;;; Print out chunk info
    : execPrint(CHUNK_NAME)
    : execPrint(CHUNK_SIZE)
    
  SUCCESS: execKill(File has been parsed!)
FAILURE: execKill(File parsing has failed.)
```

# todo
 * add reference literals (pass information (strings, numbers) into a function that uses references 
 * add file exporting
 * add file modification
 * more stuff to make this actually useful
 
