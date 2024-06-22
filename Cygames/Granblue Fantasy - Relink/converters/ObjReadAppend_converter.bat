@echo off
setlocal enableDelayedExpansion

for %%i in (%*) do (
     if not exist "%~dp0ObjReadAppend.fbs" (
	  @echo Missing 'ObjReadAppend.fbs', if you don't have it please download it from the GBFRDataTools github. && @pause && @exit /b 0
     )

     if exist "flatcPath=%~dp0flatc.exe" (
          set "flatc=%~dp0flatc.exe"
     ) else if exist "%~dp0..\flatc.exe" (
          set "flatc=%~dp0..\flatc.exe"
     ) else (
          @echo Can't find flatc in bat file's directory or parent directory. && @pause && @exit /b 0
     )

     echo %%~ni|findstr /i /L "append">nul
     if not errorlevel 1 (
          if "%%~xi" == ".objread" (
               "!flatc!" --raw-binary -t "%~dp0ObjReadAppend.fbs" -- "%%~i" --strict-json && @exit /b -1

          ) else if "%%~xi" == ".json" (
               "!flatc!" --binary "%~dp0ObjReadAppend.fbs" "%%~i"
               if exist "%%~ni.objread" (
                    del "%%~ni.objread"
               )
               ren "*%%~ni.bin" "*%%~ni.objread" && @exit /b -1

          ) else (
               @echo Can't convert %%~xi files, please only use .objread or .json && @pause && @exit /b 0
          )

     ) else (
          @echo Can't convert filenames starting with "%%~ni", please only use filenames that start with "append" && @pause && @exit /b 0
     )

)

@exit
