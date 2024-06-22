@echo off
setlocal enableDelayedExpansion

for %%i in (%*) do (
     if not exist "%~dp0EffectTextureImplementationHeader.fbs" (
	  @echo Missing 'EffectTextureImplementationHeader.fbs', if you don't have it please download it from the GBFRDataTools github. && @pause && @exit /b 0
     )

     if exist "flatcPath=%~dp0flatc.exe" (
          set "flatc=%~dp0flatc.exe"
     ) else if exist "%~dp0..\flatc.exe" (
          set "flatc=%~dp0..\flatc.exe"
     ) else (
          @echo Can't find flatc in bat file's directory or parent directory. && @pause && @exit /b 0
     )

     if "%%~xi" == ".eti" (
          "!flatc!" --raw-binary -t "%~dp0EffectTextureImplementationHeader.fbs" -- "%%~i" --strict-json && @exit /b -1

     ) else if "%%~xi" == ".json" (
          "!flatc!" --binary "%~dp0EffectTextureImplementationHeader.fbs" "%%~i"
          if exist "%%~ni.eti" (
               del "%%~ni.eti"
          )
          ren "*%%~ni.bin" "*%%~ni.eti" && @exit /b -1

     ) else ( 
          @echo Can't convert %%~xi files, please only use .eti or .json && @pause && @exit /b 0
     )

)

@exit
