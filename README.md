# Bump_Fingerprints

## Fall 2016
##

### Objective

Biometric fingerprinting scanners take several reference points on a person's finger for comparision with a known print. Therefore, the hold fingerprint itself is not used. There is a threshold the newly scanned print must meet to "match" the known print. With this said, it may be possible for two very similar fingerprints to "match." As long as the reference points in consideration to the part of the fingerprint aline, this will be considered a "match." Other locations of the fingerprint will not matter in such a scenario. This program seeks out different fingerprints that "match" within a database of several persons' fingerprints. One or more of these prints will be considered "bump" fingerprints. The hope is to have these prints match fingerprints from other databases.

### Prerequisities

C# <https://www.visualstudio.com/downloads/>
SourceAFIS 1.7.0 SDK <http://www.sourceafis.org/blog/>

### Setting Up

This code is in its developmental process. The easiest way to test the code thus far is to do the following:
Install Visual Studio 2015
Create a new project and install SourceAFIS version 1.7.0 via Nuget GUI or commandline
Use the Program.cs file to build and run the program

### Output
  
#### Output using dir_cmp 
General output format will include both benign and malicious results.
```
<path to first image>,<path to second image>,Score
<path to first image>,<path to second image>,Score
<path to first image>,<path to second image>,Score
...
<path to first image>,<path to second image>,Score
```
  
#### Output using person_cmp  
```
<first person ID>,<second person ID>,Score
<first person ID>,<second person ID>,Score
<first person ID>,<second person ID>,Score
...
<first person ID>,<second person ID>,Score
```
  
### References
SourceAFIS. N.p., n.d. Web. 28 Nov. 2016.  
<http://www.sourceafis.org/blog/>.
