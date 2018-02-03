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
2. Download the compiled binary [here]().

Run the console application by providing 

### How do I use the Cleaner class in my code?



### I am getting an error when I try to clean my file!

The quality of delimited text files varies greatly.  This application, while attempting to improve the accessibility of delimited text files to a wider community of consuming systems, still relies on some consistency in the structure of the delimited file.  In particular, the following are important for this application to work correctly:
* Delimiter characters should not appear in the data.
* If delimiter characters appear in the data, they must be escaped with text qualifiers.
* If a text qualifier appears in the data, it must be escaped by a preceding text qualifier character.  For example, if the text qualifier is a single quote and the data being saved is "The cow's pasture", then the data should be represented in the delimited text file as "'The cow''s pasture'".

Due to the carelessness with which some applications export data in a delimited format, these rules may not be followed and it will not be possible for CrLfCleaner to properly clean the file.
