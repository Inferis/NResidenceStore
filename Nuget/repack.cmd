@ECHO OFF
SETLOCAL
 
FOR /f "" %%G IN ('dir /b/a:d') DO (
  for /f "" %%V in (%%G\VERSION) do (
	  nuget pack %%G\%%G.nuspec -Version %%V -Symbols -o Packages
  )
)