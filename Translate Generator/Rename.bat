cd Output
for /f "" %%f in ('dir /b *.txt') do (
    copy %%f %%~nf_en.txt
    copy %%f %%~nf_cn.txt
    ren %%f %%~nf_tw.txt
)
cd ..\