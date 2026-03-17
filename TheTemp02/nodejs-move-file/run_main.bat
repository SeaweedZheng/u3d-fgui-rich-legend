chcp 65001

set disk=%~d0 
set pth=%~dp0
:: cd %disk%
%disk%
cd %pth%

:: node rename_files.js  __slot_spin__bonus12208_0__spec     __slot_spin   ./test

::  node rename_files.js   [原名称]   [改为名称]   [路劲]
node njs-move-file.js


echo 结束main

pause