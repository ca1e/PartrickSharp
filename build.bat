@echo off

set projname="Decryptor"

rmdir /s /q publish 

cd %projname%
rmdir /s /q bin
rmdir /s /q obj

dotnet publish %projname%.csproj --nologo -c Release -f net6.0 --no-self-contained -o ..\publish

pause