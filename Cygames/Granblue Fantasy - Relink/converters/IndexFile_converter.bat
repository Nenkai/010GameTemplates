@echo off
setlocal enableDelayedExpansion

for %%i in (%*) do (
     if not exist "%~dp0IndexFile.fbs" (
	  @echo Missing 'IndexFile.fbs', if you don't have it please download it from the GBFRDataTools github. && @pause && @exit /b 0
     )

     if exist "flatcPath=%~dp0flatc.exe" (
          set "flatc=%~dp0flatc.exe"
     ) else if exist "%~dp0..\flatc.exe" (
          set "flatc=%~dp0..\flatc.exe"
     ) else (
          @echo Can't find flatc in bat file's directory or parent directory. && @pause && @exit /b 0
     )

     if "%%~xi" == ".i" (
          "!flatc!" --raw-binary -t "%~dp0IndexFile.fbs" -- "%%~i" --strict-json --no-warnings  && @exit /b -1

     ) else if "%%~xi" == ".json" (
          "!flatc!" --binary "%~dp0IndexFile.fbs" "%%~i" --no-warnings
          if exist "%%~ni.i" (
               del "%%~ni.i"
          )
          ren "*%%~ni.bin" "*%%~ni.i" && @exit /b -1

     ) else ( 
          @echo Can't convert %%~xi files, please only use .i or .json && @pause && @exit /b 0
     )

)

@exit
