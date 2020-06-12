echo off
cls

cd ".\Translate Generator\Origin"
del *.txt>NUL
cd ..\..\

cd ".\Translate Generator\Output"
del *.txt>NUL
cd ..\..\

cd ".\Whale To KiriKiri\Output"
del *.txt>NUL
cd ..\..\


MacroConverter.exe -nostop ".\Whale To KiriKiri"
move /Y ".\Whale To KiriKiri\Output\*" ".\Translate Generator\Origin"
MacroConverter.exe  -nostop -nocomment ".\Translate Generator"
cd ".\Translate Generator"
call Rename.bat
cd ..\
del ".\Output\*.txt"
del ".\Output\*.scn"
mkdir Output
move  /Y ".\Translate Generator\Origin\*.txt" ".\Output"
"EntryPoint Generator.exe" ".\Output"
move  /Y ".\Translate Generator\Output\*.txt" ".\Output"