@echo off&chcp 936>nul&setlocal enabledelayedexpansion
title 【删除bin和obj文件夹】
echo;
if [%1] equ [] echo 【未指定文件夹】 &goto end
set fp=%1
set fp="%fp%" & set fp=%fp:"=%
if "%fp%" neq "" if "%fp:~-1%" neq "\" set fp=%fp%\
set nowPath="%fp%"

set /a findFileCount=0
set /a findFolderCount=0
set /a findAllCount=0
set /a delFileCount=0
set /a delFolderCount=0
set /a fileSize=0
set delFileOrFolder=*.vshost.*,bin,obj


echo; &echo 待清理目录：%nowPath%
echo; &echo 删除指定目录下的：
set delType=%delFileOrFolder%
:loop
for /f "tokens=1* delims=," %%a in ("%delType%") do echo %%a &set delType=%%b
if defined delType goto :loop
echo; &echo 按任意键将继续，退出请关闭窗口。 &pause>nul
echo; &echo --------------------------------------------------
echo 待删除列表：

rem search delete specify file or folder(such as:*.pdb,*.vshost.*)
::for /r %nowPath% %%i in (*.pdb,*.vshost.*) do (echo del "%%i")
for /r %nowPath% %%i in (%delFileOrFolder%) do (IF EXIST "%%i" (echo "%%i"&IF EXIST "%%i\.\" (set /a findFolderCount+=1) else set /a findFileCount+=1
    set fs=0&(for /f "tokens=3* delims= " %%a in ('dir "%%i" /a-d /s /-c ^| find /i "个文件"') do set fs=%%a)&call :bigNumAdd !fileSize! !fs! fileSize
))
set /a findAllCount=%findFolderCount%+%findFileCount%
echo;&if %findAllCount% equ 0 echo [file or folder not found] &goto end
call :numToThousand %fileSize% fileSize
echo 共找到%findAllCount%个待删除项【文件：%findFileCount%个，文件夹：%findFolderCount%个，总共大小：%fileSize% bytes】
echo --------------------------------------------------
set /p var=是否确定删除以上文件或文件夹？[Y/N]:
if /i "%var%"=="Y" (goto DelFileAndFolder) else goto cancelDel

:DelFileAndFolder
rem delete specify file or folder(such as:obj,bin,*.pdb,*.vshost.*)
::for /r %nowPath% %%i in (%delFileOrFolder%) do (IF EXIST "%%i" IF EXIST "%%i\.\" (echo "%%i\.\") ELSE echo "%%i")
for /r %nowPath% %%i in (%delFileOrFolder%) do (IF EXIST "%%i" (echo Deleting file "%%i"&IF EXIST "%%i\.\" (RD /s /q "%%i" &set /a delFolderCount+=1) else del "%%i" &set /a delFileCount+=1))
echo; &echo [Delete completed] delete %delFileCount% files, delete %delFolderCount% folders, total size %fileSize% bytes &echo;
goto end

:cancelDel
echo; &echo [Cancel deletion] &echo;

:end
Pause
goto :eof


::浮点加法 call _ADD  <被加数> <加数> [返回变量]
::分别支持十进制的64位整数部分和64位小数部分,一次计算仅用0.01秒
:bigNumAdd <被加数> <加数> [返回变量]   // by jack on 2021-09-03
(setlocal enabledelayedexpansion
set L=&for /l %%a in (1,1,8) do set L=!L!00000000
for /f "tokens=1-3 delims=." %%a in ("!L!%1.!L!") do set at=%%a&set aw=%%b%%c
for /f "tokens=1-3 delims=." %%a in ("!L!%2.!L!") do set bt=%%a&set bw=%%b%%c
set a=!at:~-64!!aw:~,64!&set b=!bt:~-64!!bw:~,64!&set e=&set v=200000000
for /l %%a in (8,8,128)do set/a v=1!b:~-%%a,8!+1!a:~-%%a,8!+!v:~-9,-8!-2&set e=!v:~-8!!e!
set e=!e:0= !&for /f "tokens=*" %%a in ("!e:~,-64!_.!e:~64!") do set e=%%~nxa
set e=!e:_=!&for %%a in ("!e: =0!") do endlocal&(if %3.==. (echo %%~a) else set %3=%%~a)
exit/b)

::整数字符转成千分符表示
:numToThousand  <数字参数> [返回变量]
(setlocal enabledelayedexpansion&set n=%~1
set len=0&for /l %%i in (0,1,1000) do if "!n:~%%i,1!."=="." set len=%%i&goto :breakFor
:breakFor
set nv=&for /l %%a in (1,1,%len%)do (set nv=!n:~-%%a,1!!nv!&set/a m=%%a%%3&if !m!==0 if not %%a==%len% set nv=,!nv!)
endlocal&(if %2.==. (echo %nv%) else set %2=%nv%)
exit/b)