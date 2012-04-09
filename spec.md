Conventions in this document
============================

The key words "MUST", "MUST NOT", "REQUIRED", "SHALL", "SHALL NOT", "SHOULD",
"SHOULD NOT", "RECOMMENDED",  "MAY", and "OPTIONAL" in this document are to be
interpreted as described in RFC 2119.

Definitions
===========

 * A **file** is a specific stream of bytes. A file is not the same as a file
   name. Rather, a file is a snapshot of the contents of a file.
 * A **Folder** is a collection of files and other folders. In the same way
   that a file is a snapshot, a folder is a snapshot of files and other folders.
 * A **Commit** is a shapshot of the root folder of a repository along with
   a collection of pointers to parent commits.
 * **Text** is any stream of unicode code points.
 * **UTF-8N** is the UTF-8 encoding with the caveat that it MUST NOT be preceded
   by a byte-order-mark and must otherwise validate as UTF-8 text.

The Portable VCS Format
=======================

General Format
--------------

A repository in the Portable VCS Format is stored on the host file system in
some location chosen by the user. For the purposes of defining relative paths,
this location will be refered to as "/" for the remainder of this document.

In general, all objects are stored as separate files on the host file system.
The files SHOULD be named as a hash of their content, so that files with the
same content share the same location on disk. Files with different contents MUST
be named with distinct names. In addition, all objects MUST be compressed with
the gzip compression scheme on disk, given the extension `.gz`, and stored in
the folder named "/objects".

The file name on the host operation system (without the `.gz` extension and
without the `/object` folder) will be refered to as the "object name" and the
path of the object on the host (including the extension and relative path) will
be refered to as the "object path"

It is RECOMMENDED that the object name of an object be the SHA-1 hash of the
object.

### File object format ###

All files in a repository are considered binary files and are stored as-is
according to the general format listed above. That is, the files are stored with
a filename that is unique to the file's contents and gzip compressed.

### Folder object format ###

Folders are special files that are nevertheless stored according to the general
format listed above, in the same way as a file object.

The content of a folder object is the concatenation of all of the file and
folder descriptors for the elements contained by the file. The descriptors
SHOULD be sorted in some deterministic fashion, so that two folders with
logically equivalent contents may have exactly the same contents, and therefore
share a file on disk.  It is RECOMMENDED that the descriptors be sorted first
lexicographically, first by the 

#### Descriptor format ####

A descriptor follows the following format:

    <ObjectNameLength><\0><ObjectName><\0><Type><\0><NameLength><\0><Name><\0>

The fields above (represented by text surrounded by angle brackets) are
generated as follows:

 * `<\0>` represents a single byte with a zero value (a null byte).
 * `<ObjectNameLength>` is the length in bytes, of the `<ObjectName>` field. The
   value is represented in ASCII hexadecimal digits. The hexadecimal digits 'a'
   through 'f' SHOULD be represented in lower case.
 * `<ObjectName>` is object name of the object referenced. The name is text and
   therefore MUST be encoded using the UTF-8N encoding. The value MUST be valid
   UTF-8N.
 * `<Type>` is a single byte with either the ASCII value 't' (for "tree"), if
   the descriptor represents a folder; or the ASCII value 'b' (for "blob"), if
   the descriptor represents a file.
 * `<NameLength>` is the length in bytes, of the `<Name>` field. The value is
   represented in ASCII hexadecimal digits. The hexadecimal digits 'a' through
   'f' SHOULD be represented in lower case.
 * `<Name>` is name of the object referenced. The name is text and therefore
   MUST be encoded using the UTF-8N encoding. The value MUST be valid UTF-8N.

Be aware that no guarantee is made that either the `<ObjectName>` field or the
`<Name>` field will be free of null characters.  This means that an
implementation SHOULD NOT simply use the null characters as a delimeter, unless
care is taken to handle the cases where the field contains a null character.