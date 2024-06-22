@echo off
setlocal enabledelayedexpansion

:Start
@echo 1. MInfo
@echo 2. MMat
@echo 3. Info.Objread
@echo 4. Append.Objread
@echo 5. ETI Effect Texture Implementation
@echo 6. Index File
@echo 7. Exit
choice /c 1234567 /m "Which file type to convert from or to?"

if %errorlevel% == 7 exit
if %errorlevel% == 6 call "%~dp0\converters\IndexFile_converter.bat" %* && goto End
if %errorlevel% == 5 call "%~dp0\converters\ETI_converter.bat" %* && goto End
if %errorlevel% == 4 call "%~dp0\converters\ObjReadAppend_converter.bat" %* && goto End
if %errorlevel% == 3 call "%~dp0\converters\ObjReadInfo_converter.bat" %* && goto End
if %errorlevel% == 2 call "%~dp0\converters\MMat_converter.bat" %* && goto End
if %errorlevel% == 1 call "%~dp0\converters\MInfo_converter.bat" %* && goto End

:End
if %errorlevel% == 0 (
     choice /c yn /m "Try another option?"
     if !errorlevel! == 1 goto Start

) else if %errorlevel% == -1 (
     @echo Finished converting "%~nx1" && @pause
)

@exit