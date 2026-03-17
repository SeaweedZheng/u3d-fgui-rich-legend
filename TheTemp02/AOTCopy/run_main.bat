chcp 65001

set disk=%~d0 
set pth=%~dp0
:: cd %disk%
%disk%
cd %pth%

node aot_copy.js

echo 结束main

pause