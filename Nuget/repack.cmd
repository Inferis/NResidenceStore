rem @ECHO OFF
SETLOCAL
SET VERSION=%1
 
FOR /f "" %%G IN ('dir /b/a:d') DO (
  nuget pack %%G\%%G.nuspec -Version %VERSION% -Symbols -o Packages
)