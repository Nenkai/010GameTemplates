@echo off
setlocal enableDelayedExpansion

for %%i in (%*) do (
     if not exist "%~dp0MMat_ModelMaterial.fbs" (
	  @echo Missing 'MMat_ModelMaterial.fbs', if you don't have it please download it from the GBFRDataTools github. && @pause && @exit /b 0
     )

     if exist "flatcPath=%~dp0flatc.exe" (
          set "flatc=%~dp0flatc.exe"
     ) else if exist "%~dp0..\flatc.exe" (
          set "flatc=%~dp0..\flatc.exe"
     ) else (
          @echo Can't find flatc in bat file's directory or parent directory. && @pause && @exit /b 0
     )

     if "%%~xi" == ".mmat" (
          "!flatc!" --json --strict-json "%~dp0MMat_ModelMaterial.fbs" -- "%%~i" --raw-binary && @exit /b -1

     ) else if "%%~xi" == ".json" (
          findstr \^"^"^"magic\^"^": 20230727\^"^" "%%~i"
          if errorlevel 1 (
               @echo Incorrect magic number in json, cancelling conversion. && @pause && @exit /b 0
          )
          "!flatc!" --binary "%~dp0MMat_ModelMaterial.fbs" "%%~i"
          if exist "%%~ni.mmat" (
               del "%%~ni.mmat"
          )
               ren "*%%~ni.bin" "*%%~ni.mmat" && @exit /b -1

     ) else ( 
          @echo Can't convert %%~xi files, please only use .mmat or .json && @pause && @exit /b 0
     )

)

@exit
