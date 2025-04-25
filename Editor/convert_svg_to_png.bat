@echo off
setlocal enabledelayedexpansion

REM 配置路径
set "INKSCAPE_PATH=C:\Program Files\Inkscape\bin\inkscape.exe"
set "SVG_DIR=X:\work\u3d\package6\Packages\com.xuexue.simple-folder-icon\Arts\svg"
set "PNG_DIR=X:\work\u3d\package6\Packages\com.xuexue.simple-folder-icon\Arts\png"

REM 创建输出目录（如不存在）
if not exist "%PNG_DIR%" (
    mkdir "%PNG_DIR%"
)

REM 遍历 svg 文件并转换
for %%F in ("%SVG_DIR%\*.svg") do (
    set "FILENAME=%%~nF"
    echo Converting %%F to !FILENAME!.png
    "%INKSCAPE_PATH%" "%%F" --export-type=png --export-filename="%PNG_DIR%\!FILENAME!.png" --export-width=512 --export-height=512
)

echo All SVG files converted to PNG.
pause
