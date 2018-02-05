# CrLfCleaner

Cleans newline characters from the data fields of delimited files.

### What is CrLfCleaner?

Delimited text files are a common, and commonly malformed, data transfer medium.  Applications of all types read and write delimited data with varying degrees of quality and success.  A common issue when working with delimited data files arises when text data written to a delimited format contains carriage returns and/or linefeed characters.  Very few applications correctly read the newline sequences as part of the data, even when such sequences are text qualified.

CrLfCleaner attempts to solve this issue by reading delimited text files and removing extra linefeed characters which occur in a field.  The output is a file where one record is contained on a single line rather than across multiple lines.

### How does it work?

CrLfCleaner knows, either by reading the header row or by being explicitly told, how many delimiters should occur in a single row of data.  It then scans the text data and removes newline characters that occur between delimiters that would be anywhere in the middle of a row of data.

### How do I use the Console application?

You can obtain the Console application in one of two ways:
1. Clone the repository and compile CrLfCleaner yourself.
2. Download the latest release on the [releases page](https://github.com/tlarsen7572/CrLfCleaner/releases).

Run the console application to clean a file by providing the following parameters:
* file={fileName}: Specifies the name or path of the file to clean.  Can contain spaces.
* delimiter={char} OR delimiter=C{###}: A single character describing the delimiter.  Alternatively, an ASCII character code can be specified for the delimiter.  The character code must be a three digit number, including leading zeroes if necessary, preceded by a capital 'C'.  This is especially useful if the delimiter is a control character.
* hasHeaders={Y|N}: Identifies whether the first row of data contains headers.
* delimitersPerRow={#}: This is required if hasHeaders=N.  When a data file has headers the application auto-calculates the number of delimiters per row.  If the file has no headers then this value must be supplied manually.

The followin parameter is optional:
* qualifier={string}: Identifies the text qualifier.  Delimiters inside text qualifiers are not counted as delimiters.  This value must be a single character or one of the following paired character sets: (), [], {}.

Wrap each parameter in double quotes, including the name of the parameter and the equal sign.  If the vaue of the parameter contains a double quote, use two double quotes as an escape mechanism.

###### Example:
A file called 'MyData.csv' uses comma delimiters and double-quote text qualifiers.  It also contains a header row.  To clean the file from the cmd command line, use:

> CfLfCleaner "file=MyData.csv" "delimiter=," "hasHeaders=Y" "qualifier="""

###### Example:
"C:\Users\Me\Documents\My Data\Data.txt" uses field separator delimiters (ASCII character code 28) and no text qualifiers.  It does not contain a header row but should have 15 delimiters in each row of data.  To clean this file from the cmd command line, use:

> CrLfCleaner "file=C:\Users\Me\Documents\My Data\Data.txt" "delimiter=C028" "hasHeaders=N" "delimitersPerRow=15"

### How do I use the Cleaner class in my code?

Create an instance of the Cleaner class using one of the provided factory constructors.  Call the Clean method when ready to clean.  The Clean method reads the specified text file, cleans it, and writes a cleaned version of the file to the source file directory.  The new file has the same name as the source file, with 'clean' added as a suffix to the extension.

### How fast is it?  What do I need to run this application?

CrLfCleaner is a .NET application intended to run on Windows desktop or server devices.  The cleaning operation is fast: a 20gb text file can be cleaned in as little as 5 minutes, depending on the supporting hardware.  The primary limiter of speed is the disk.  The application cleans and writes the new output as it reads from the disk, so its memory footprint is low.

### I am getting an error when I try to clean my file!

The quality of delimited text files varies greatly.  This application, while attempting to improve the accessibility of delimited text files to a wider community of consuming systems, still relies on some consistency in the structure of the delimited file.  In particular, the following are important for this application to work correctly:
* Delimiter characters should not appear in the data.
* If delimiter characters appear in the data, they must be escaped with text qualifiers.
* If a text qualifier appears in the data, it must be escaped by a preceding text qualifier character.  For example, if the text qualifier is a single quote and the data being saved is "The cow's pasture", then the data should be represented in the delimited text file as "'The cow''s pasture'".

Due to the carelessness with which some applications export data in a delimited format, these rules may not be followed and it will not be possible for CrLfCleaner to properly clean the file.
